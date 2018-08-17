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

        public static BrokerAccountWithSummariesViewModel CreateAccountWithSummeries(BrokerAccount brokerAccount)
        {
            return new BrokerAccountWithSummariesViewModel
            {
                BrokerAccount = brokerAccount,
                AmountValidated = ComputeAvailableAmount(brokerAccount, false),
                AmountTotal = ComputeAvailableAmount(brokerAccount, true),
                OpenBetsResponsabilityValidated = ComputeOpenResponsabilities(brokerAccount, false),
                OpenBetsResponsabilityTotal = ComputeOpenResponsabilities(brokerAccount, true),
            };
        }

        //C'E' QUALCOSA CE NON VA. NON CONTROLLO Validated della bet corrispondente
        public static double ComputeOpenResponsabilities(BrokerAccount brokerAccount,
            bool includeNotValidatedTransactions)
        {
            return brokerAccount.Bets
                .Where(b => b.BetStatusId == BetStatus.Open)
                .Sum(b => b.Responsability);
        }

        public static double ComputeAvailableAmount(BrokerAccount brokerAccount, bool includeNotValidatedTransactions)
        {
            return brokerAccount.IntialAmount +
                   brokerAccount.Transactions.Where(t => includeNotValidatedTransactions || t.Validated)
                    .Sum(t => t.Amount);
        }

        public static BrokerAccountsSummaryViewModel CreateAccountsSummary(IEnumerable<BrokerAccountWithSummariesViewModel> brokers)
        {
            BrokerAccountsSummaryViewModel res = new BrokerAccountsSummaryViewModel
            {
                BrokerAccounts = brokers,
                TotalDeposit = ComputeTotalTransactionAmount(brokers, TransactionType.MoneyCredit, true),
                TotalDepositValidated = ComputeTotalTransactionAmount(brokers, TransactionType.MoneyCredit, false),
                TotalWithdrawn = ComputeTotalTransactionAmount(brokers, TransactionType.MoneyDebit, true),
                TotalWithdrawnValidated = ComputeTotalTransactionAmount(brokers, TransactionType.MoneyDebit, false),
                TotalBonusCredit = ComputeTotalTransactionAmount(brokers, TransactionType.CreditBonus, true),
                TotalBonusCreditValidated = ComputeTotalTransactionAmount(brokers, TransactionType.CreditBonus, false),
                TotalOpenResponsabilities = ComputeTotalOpenResponsabilities(brokers, true),
                TotalOpenResponsabilitiesValidated = ComputeTotalOpenResponsabilities(brokers, false),
                TotalAvailability = ComputeTotalAvailability(brokers, true),
                TotalAvailabilityValidated = ComputeTotalAvailability(brokers, false),
            };
            ComputeNetProfit(res);
            return res;
        }

        private static void ComputeNetProfit(BrokerAccountsSummaryViewModel summary)
        {
            double totalInitialAmounts = ComputeTotalInitialAmounts(summary.BrokerAccounts);

            summary.NetProfit = -summary.TotalWithdrawn
                                + summary.TotalAvailability
                                + summary.TotalOpenResponsabilities
                                - summary.TotalDeposit
                                - totalInitialAmounts;

            summary.NetProfitValidated = -summary.TotalWithdrawnValidated
                                + summary.TotalAvailabilityValidated
                                + summary.TotalOpenResponsabilitiesValidated
                                - summary.TotalDepositValidated
                                - totalInitialAmounts;
        }

        private static double ComputeTotalInitialAmounts(IEnumerable<BrokerAccountWithSummariesViewModel> brokers)
        {
            return brokers.Select(b => b.BrokerAccount)
                          .Sum(b => b.IntialAmount);
        }

        private static double ComputeTotalTransactionAmount(IEnumerable<BrokerAccountWithSummariesViewModel> brokers, byte transactionType, bool includeNotValidatedTransactions)
        {
            return brokers.Select(b => b.BrokerAccount)
                          .SelectMany(b => b.Transactions)
                          .Where(t => t.TransactionTypeId == transactionType)
                          .Where(t => includeNotValidatedTransactions || t.Validated)
                          .Sum(t => t.Amount);
        }

        private static double ComputeTotalAvailability(IEnumerable<BrokerAccountWithSummariesViewModel> brokers, bool includeNotValidatedTransactions)
        {
            return includeNotValidatedTransactions ? brokers.Sum(b => b.AmountTotal) : brokers.Sum(b => b.AmountValidated);
        }

        private static double ComputeTotalOpenResponsabilities(IEnumerable<BrokerAccountWithSummariesViewModel> brokers, bool includeNotValidatedTransactions)
        {
            return includeNotValidatedTransactions ? brokers.Sum(b => b.OpenBetsResponsabilityTotal) : brokers.Sum(b => b.OpenBetsResponsabilityValidated);
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