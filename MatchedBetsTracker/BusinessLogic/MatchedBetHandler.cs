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
                TransactionTypeId =  (byte)Constants.TransactionType.OpenBet,
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
                    Amount = bet.ProfitLoss,
                    Bet = bet,
                    BrokerAccountId = bet.BrokerAccountId,
                    TransactionTypeId = (byte)Constants.TransactionType.CreditBet,
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

            if (bet.IsWon())
            {
                if (bet.IsLay)
                {
                    bet.ProfitLoss = bet.BetAmount;
                }
                else
                {
                    bet.ProfitLoss = bet.Responsability * layBrokerNetGain;
                }
            }
            else if (bet.IsLost())
            {
                if (bet.IsLay)
                {
                    bet.ProfitLoss = -bet.Responsability;
                }
                else
                {
                    bet.ProfitLoss = -bet.BetAmount;
                }
            }
            else
            {
                bet.ProfitLoss = 0;
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
            bet.BetStatusId = (byte)Constants.BetStatus.Open;
            bet.BetDate = simpleMatchedBet.BetDate;
            bet.BetDescription = simpleMatchedBet.BetDescription;
            bet.EventDate = simpleMatchedBet.EventDate;
            bet.EventDescription = simpleMatchedBet.EventDescription;
            bet.MatchedBet = matchedBet;
            bet.ProfitLoss = 0;
            bet.Validated = false;

            return bet;
        }

        public static bool IsWon(this Bet bet)
        {
            return bet.BetStatusId == (byte)Constants.BetStatus.Won;
        }

        public static bool IsLost(this Bet bet)
        {
            return bet.BetStatusId == (byte)Constants.BetStatus.Loss;
        }

        public static bool IsOpen(this Bet bet)
        {
            return bet.BetStatusId == (byte)Constants.BetStatus.Open;
        }

        static Constants.BetStatus GetStatus(this Bet bet)
        {
            return (Constants.BetStatus)bet.BetStatusId;
        }
    }
    
}