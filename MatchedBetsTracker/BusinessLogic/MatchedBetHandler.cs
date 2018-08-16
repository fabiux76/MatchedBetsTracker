using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MatchedBetsTracker.BusinessLogic
{
    public class MatchedBetHandler
    {
        public static double layBrokerNetGain = 0.95;

        public static MatchedBet CreateMatchedBet(SimpleMatchedBetFormViewModel simpleMatchedBet)
        {
            return new MatchedBet
            {
                EventDescription = simpleMatchedBet.EventDescription,
                Status = MatchedBetStatus.Open
            };
        }

        public static Bet CreateBackBet(SimpleMatchedBetFormViewModel simpleMatchedBet, MatchedBet matchedBet)
        {
            return new Bet
            {
                BrokerAccountId = simpleMatchedBet.BackBrokerAccountId,
                BetAmount = simpleMatchedBet.BackAmount,                                                           
                Quote = simpleMatchedBet.BackQuote,
                Responsability = simpleMatchedBet.BackAmount,
                IsLay = false,
            }.Initialize(simpleMatchedBet, matchedBet);
        }

        public static Bet CreateLayBet(SimpleMatchedBetFormViewModel simpleMatchedBet, MatchedBet matchedBet)
        {
            return new Bet
            {
                BrokerAccountId = simpleMatchedBet.LayBrokerAccountId,
                BetAmount = simpleMatchedBet.LayAmount,
                Quote = simpleMatchedBet.LayQuote,
                Responsability = ComputeLayResponsability(simpleMatchedBet.LayQuote, simpleMatchedBet.LayAmount),
                IsLay = true,
            }.Initialize(simpleMatchedBet, matchedBet);
        }

        public static Transaction CreateOpenBetTransaction(Bet bet)
        {
            return new Transaction
            {
                Date = bet.BetDate,                
                Amount = GetOpenBetAmount(bet),
                Bet = bet,
                BrokerAccountId = bet.BrokerAccountId,
                TransactionTypeId =  TransactionType.OpenBet,
                Validated = false
            };
        }

        public static Transaction CreateCloseBetTransaction(Bet bet)
        {
            if (bet.IsWon())
            {
                return new Transaction
                {
                    Date = bet.EventDate,
                    Amount = bet.ProfitLoss - GetOpenBetAmount(bet),
                    Bet = bet,
                    BrokerAccountId = bet.BrokerAccountId,
                    TransactionTypeId = TransactionType.CreditBet,
                    Validated = false
                };
            }
            return null;
        }

        public static void RecomputeBetResponsabilityAndProfit(Bet bet)
        {
            if (bet.IsLay)
            {
                bet.Responsability = ComputeLayResponsability(bet.Quote, bet.BetAmount);
            }
            else
            {
                bet.Responsability = bet.BetAmount;
            }

            if (bet.IsLay)
            {
                if (bet.IsWon())
                {
                    bet.ProfitLoss = bet.BetAmount * layBrokerNetGain;
                }
                else if (bet.IsLost())
                {
                    bet.ProfitLoss = -bet.Responsability;
                }
                else
                {
                    bet.ProfitLoss = 0;
                }
            }
            else
            {
                if (bet.IsWon())
                {
                    bet.ProfitLoss = bet.BetAmount * (bet.Quote - 1);
                }
                else if (bet.IsLost())
                {
                    bet.ProfitLoss = -bet.BetAmount;
                }
                else
                {
                    bet.ProfitLoss = 0;
                }
            }
        }

        public static double ComputeLayResponsability(double layQuote, double layAmount)
        {
            return (layQuote - 1) * layAmount;
        }

        public static double GetOpenBetAmount(Bet bet)
        {
            if (bet.IsLay)
            {
                return -bet.Responsability;
            }
            else
            {
                return -bet.BetAmount;
            }
        }
    }

    public static class InizializationHelper
    {
        public static Bet Initialize(this Bet bet, SimpleMatchedBetFormViewModel simpleMatchedBet, MatchedBet matchedBet)
        {
            bet.BetStatusId = BetStatus.Open;
            bet.BetDate = simpleMatchedBet.BetDate;
            bet.BetDescription = simpleMatchedBet.BetDescription;
            bet.EventDate = simpleMatchedBet.EventDate;
            bet.EventDescription = simpleMatchedBet.EventDescription;
            bet.MatchedBet = matchedBet;
            bet.ProfitLoss = 0;

            return bet;
        }

        public static bool IsWon(this Bet bet)
        {
            return bet.BetStatusId == BetStatus.Won;
        }

        public static bool IsLost(this Bet bet)
        {
            return bet.BetStatusId == BetStatus.Loss;
        }

        public static bool IsOpen(this Bet bet)
        {
            return bet.BetStatusId == BetStatus.Open;
        }

        public static bool IsWinning(this Bet bet, MatchedBetStatus status)
        {
            return (bet.IsLay && status == MatchedBetStatus.LayWon) ||
                   (!bet.IsLay && status == MatchedBetStatus.BackWon);
        }
    }
    
}