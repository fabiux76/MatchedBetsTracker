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
                Status = MatchedBetStatus.Open,
                UserAccountId = simpleMatchedBet.BackBrokerAccountId
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

        public static Bet CreateSecondBackBet(SimpleMatchedBetFormViewModel simpleMatchedBet, MatchedBet matchedBet)
        {
            return new Bet
            {
                BrokerAccountId = simpleMatchedBet.LayBrokerAccountId,
                BetAmount = simpleMatchedBet.LayAmount,
                Quote = simpleMatchedBet.LayQuote,
                Responsability = simpleMatchedBet.LayAmount,
                IsLay = false,
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
                Validated = false,
                UserAccountId = bet.UserAccountId
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
                    Validated = false,
                    UserAccountId = bet.UserAccountId
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

        public static BrokerAccountsSummaryViewModel CreateAccountsSummary(IEnumerable<BrokerAccountWithSummariesViewModel> brokers, IEnumerable<UserAccount> userAccounts, bool showInactiveAccounts)
        {
            var brokerAccountWithSummariesViewModels = brokers as IList<BrokerAccountWithSummariesViewModel> ?? brokers.ToList();
            var res = new BrokerAccountsSummaryViewModel
            {
                BrokerAccounts = brokerAccountWithSummariesViewModels,
                TotalDeposit = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyCredit, true),
                TotalDepositValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyCredit, false),
                TotalWithdrawn = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyDebit, true),
                TotalWithdrawnValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyDebit, false),
                TotalBonusCredit = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, true),
                TotalBonusCreditValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, false),
                TotalOpenResponsabilities = ComputeTotalOpenResponsabilities(brokerAccountWithSummariesViewModels, true),
                TotalOpenResponsabilitiesValidated = ComputeTotalOpenResponsabilities(brokerAccountWithSummariesViewModels, false),
                TotalAvailability = ComputeTotalAvailability(brokerAccountWithSummariesViewModels, true),
                TotalAvailabilityValidated = ComputeTotalAvailability(brokerAccountWithSummariesViewModels, false),
                ShowInactiveAccounts = showInactiveAccounts,
                UserAccountSummaries = userAccounts
                    .ToDictionary(user => user.Name,
                        user => new UserAccountSummary
                        {
                            TotalDeposit = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyCredit, true, user),
                            TotalDepositValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyCredit, false, user),
                            TotalWithdrawn = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyDebit, true, user),
                            TotalWithdrawnValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.MoneyDebit, false, user),
                            TotalBonusCredit = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, true, user),
                            TotalBonusCreditValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, false, user),
                            TotalBonusExpired = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.ExpireBonus, true, user),
                            TotalBonusExpiredValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.ExpireBonus, false, user),
                            TotalCreditBetAmountOnClosedBets = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBet, true, user),
                            TotalCreditBetAmountOnClosedBetsValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBet, false, user),
                            TotalOpenBetAmountOnClosedBets = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBet, true, user, t => t.Bet != null && t.Bet.BetStatusId != BetStatus.Open),
                            TotalOpenBetAmountOnOpenBets = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBet, true, user, t => t.Bet != null && t.Bet.BetStatusId == BetStatus.Open),
                        })
            };
            ComputeNetProfits(res);

            return res;
        }

        private static void ComputeNetProfits(BrokerAccountsSummaryViewModel summary)
        {
            double totalInitialAmounts = ComputeTotalInitialAmounts(summary.BrokerAccounts);
            double totalCorrections = ComputeTotalTransactionAmount(summary.BrokerAccounts, TransactionType.Correction, true);
            summary.NetProfit = -summary.TotalWithdrawn
                                + summary.TotalAvailability
                                + summary.TotalOpenResponsabilities
                                - summary.TotalDeposit
                                - totalInitialAmounts
                                - totalCorrections;

            summary.NetProfitValidated = -summary.TotalWithdrawnValidated
                                + summary.TotalAvailabilityValidated
                                + summary.TotalOpenResponsabilitiesValidated
                                - summary.TotalDepositValidated
                                - totalInitialAmounts
                                - totalCorrections;

            summary.UserAccountSummaries.Values.ForEach(userAccountSummary =>
            {
                userAccountSummary.NetExposition = userAccountSummary.TotalDeposit + userAccountSummary.TotalWithdrawn;

                userAccountSummary.NetProfit = userAccountSummary.TotalBonusCredit
                                               + userAccountSummary.TotalBonusExpired
                                               + userAccountSummary.TotalCreditBetAmountOnClosedBets
                                               + userAccountSummary.TotalOpenBetAmountOnClosedBets;

                userAccountSummary.NetExpositionValidated = userAccountSummary.TotalDepositValidated + userAccountSummary.TotalWithdrawnValidated;

                userAccountSummary.NetProfit = userAccountSummary.TotalBonusCreditValidated
                                               + userAccountSummary.TotalBonusExpiredValidated
                                               + userAccountSummary.TotalCreditBetAmountOnClosedBetsValidated
                                               + userAccountSummary.TotalOpenBetAmountOnClosedBetsValidated;

            });

        }

        private static double ComputeTotalInitialAmounts(IEnumerable<BrokerAccountWithSummariesViewModel> brokers)
        {
            return brokers.Select(b => b.BrokerAccount)
                          .Sum(b => b.IntialAmount);
        }

        private static double ComputeTotalTransactionAmount(IEnumerable<BrokerAccountWithSummariesViewModel> brokers, byte transactionType, bool includeNotValidatedTransactions, UserAccount userAccount = null, Func<Transaction, bool> condition = null)
        {
            return brokers.Select(b => b.BrokerAccount)
                          .SelectMany(b => b.Transactions)
                          .Where(t => userAccount == null || t.UserAccountId == userAccount.Id)
                          .Where(t => condition == null || condition(t))
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
            bet.UserAccountId = matchedBet.UserAccountId;
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