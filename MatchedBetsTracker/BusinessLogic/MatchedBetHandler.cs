using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace MatchedBetsTracker.BusinessLogic
{
    public static class MatchedBetHandler
    {
        public static double layBrokerNetGain = 0.95;

        //NEW STUFFF
        public static MatchedBetCreatedObjects CreateObjectsForSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel, int userId)
        {
            //Creo lo SportEvent (unico)
            var sportEvent = CreateSportEvent(matchedBetViewModel.EventDate, matchedBetViewModel.EventDescription);

            //Creo la brokerAccount
            var matchedBet = CreateMatchedBet(matchedBetViewModel.EventDescription, userId);

            //Creo la Bet per la prima scommessa
            var firstBet = CreateBet(matchedBetViewModel.BetDescription, 
                                     matchedBetViewModel.BetDate,
                                     matchedBetViewModel.BackQuote, 
                                     matchedBetViewModel.BackAmount, 
                                     BetType.SingleBack)
                           .ForBrokerAccount(matchedBetViewModel.BackBrokerAccountId)
                           .ForUserAccount(userId)
                           .ForMatchedBet(matchedBet);

            //Creo la BetEvent per la prima scommessa
            var firstBetEvent = CreateBetEvent(matchedBetViewModel.BackQuote, false)
                .ForSportEvent(sportEvent)
                .ForBet(firstBet);

            //Cre la transaction per la prima scommessa
            var firstBetTransaction = CreateOpenBetTransaction(firstBet);            

            //Creo la Bet per la seconda scommessa

            var secondBet = CreateBet(matchedBetViewModel.BetDescription,
                    matchedBetViewModel.BetDate,
                    matchedBetViewModel.LayQuote,
                    matchedBetViewModel.LayAmount,
                    matchedBetViewModel.IsBackBack ? BetType.SingleBack : BetType.SingleLay)
                .ForBrokerAccount(matchedBetViewModel.LayBrokerAccountId)
                .ForUserAccount(userId)
                .ForMatchedBet(matchedBet);

            //Creo la BetEvent per la seconda scommessa

            var secondBetEvent = CreateBetEvent(matchedBetViewModel.LayQuote, !matchedBetViewModel.IsBackBack)
                .ForSportEvent(sportEvent)
                .ForBet(secondBet);

            //Creo la transaction per la seconda scommessa
            var secondBetTransaction = CreateOpenBetTransaction(secondBet);

            if (matchedBetViewModel.ValidateTransactions)
            {
                firstBetTransaction.Validated = true;
                secondBetTransaction.Validated = true;
            }

            return new MatchedBetCreatedObjects
            {
                MatchedBet = matchedBet,
                Bets = new List<Bet> { firstBet, secondBet },
                BetEvents = new List<BetEvent> { firstBetEvent, secondBetEvent },
                Transactions = new List<Transaction> { firstBetTransaction, secondBetTransaction },
                SportEvents = new List<SportEvent> { sportEvent}
            };
        }

        private static MatchedBet CreateMatchedBet(string eventDescription, int userId)
        {
            return new MatchedBet
            {
                EventDescription = eventDescription,
                Status = MatchedBetStatus.Open,
                UserAccountId = userId
            };
        }

        private static SportEvent CreateSportEvent(DateTime eventDate, string eventDescription)
        {
            return new SportEvent
            {
                EventDate = eventDate,
                EventDescription = eventDescription
            };
        }

        private static BetEvent CreateBetEvent(double quote, bool isLay)
        { 
            return new BetEvent
            {
                BetStatusId = BetStatus.Open,
                Quote = quote,
                IsLay = isLay
            };
        }

        private static BetEvent ForSportEvent(this BetEvent betEvent, SportEvent sportEvent)
        {
            betEvent.SportEvent = sportEvent;
            return betEvent;
        }

        private static BetEvent ForBet(this BetEvent betEvent, Bet bet)
        {
            betEvent.Bet = bet;
            return betEvent;
        }

        private static Bet CreateBet(string betDescription, DateTime betDate, double quote, double betAmount, BetType betType)
        {
            return new Bet
            {
                BetDescription = betDescription,
                BetDate = betDate,
                Quote = quote,
                BetAmount = betAmount,
                Responsability = ComputeResponsability(betType, betAmount, quote),
                BetStatusId = BetStatus.Open,
                ProfitLoss = 0,
                BetType = betType
            };
        }

        private static Bet ForMatchedBet(this Bet bet, MatchedBet matchedBet)
        {
            bet.MatchedBet = matchedBet;
            return bet;
        }

        private static Bet ForBrokerAccount(this Bet bet, BrokerAccount brokerAccount)
        {
            bet.BrokerAccount = brokerAccount;
            return bet;
        }

        private static Bet ForBrokerAccount(this Bet bet, int brokerAccountId)
        {
            bet.BrokerAccountId = brokerAccountId;
            return bet;
        }

        private static Bet ForUserAccount(this Bet bet, UserAccount userAccount)
        {
            bet.UserAccount = userAccount;
            return bet;
        }

        private static Bet ForUserAccount(this Bet bet, int userAccountId)
        {
            bet.UserAccountId = userAccountId;
            return bet;
        }

        private static double ComputeResponsability(BetType betType, double betAmount, double betQuote)
        {
            return betType == BetType.SingleLay ? ComputeLayResponsability(betQuote, betAmount) : betAmount;
        }

        //\NEW STUFF

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
                    Date = bet.LastBetEvent().SportEvent.EventDate,
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
            if (bet.IsLay())
            {
                bet.Responsability = ComputeLayResponsability(bet.Quote, bet.BetAmount);
            }
            else
            {
                bet.Responsability = bet.BetAmount;
            }

            if (bet.IsLay())
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
            if (bet.IsLay())
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
                TotalBonusCredit = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, true) +
                                   ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.ExpireBonus, true),
                TotalBonusCreditValidated = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, false) +
                                            ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.ExpireBonus, false),
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
                            TotalBonus = ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.CreditBonus, true, user) +
                                         ComputeTotalTransactionAmount(brokerAccountWithSummariesViewModels, TransactionType.ExpireBonus, true, user),
                            TotalBetGain = ComputeTotalGainOnClosedBets(user, brokerAccountWithSummariesViewModels.Select(ba => ba.BrokerAccount)),
                            TotalOpenResponsabilities = ComputeOpenResponsabilities(user, brokerAccountWithSummariesViewModels.Select(ba => ba.BrokerAccount))
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
                userAccountSummary.NetProfit = userAccountSummary.TotalBonus + userAccountSummary.TotalBetGain;
                userAccountSummary.Exposure = userAccountSummary.TotalDeposit + userAccountSummary.TotalWithdrawn;
            });

            /*
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
            */
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

        private static double ComputeTotalGainOnClosedBets(UserAccount userAccount, IEnumerable<BrokerAccount> brokerAccounts)
        {
            return brokerAccounts.SelectMany(ba => ba.Bets)
                                .Where(bet => bet.UserAccount == userAccount)
                                .Where(bet => bet.BetStatusId != BetStatus.Open)
                                .Sum(bet => bet.ProfitLoss);
        }

        private static double ComputeOpenResponsabilities(UserAccount userAccount, IEnumerable<BrokerAccount> brokerAccounts)
        {
            return brokerAccounts.SelectMany(ba => ba.Bets)
                                              .Where(bet => bet.UserAccount == userAccount)
                                              .Where(bet => bet.BetStatusId == BetStatus.Open)
                                              .Sum(bet => bet.Responsability);
        }

        public static SimpleMatchedBetFormViewModel Parse(String text)
        {
            var lines = text.Replace("\t", "").Split(new [] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            var isPuntaBanca = !lines.GetValueForHeaderNextLine("PuntaBanca").IsNullOrWhiteSpace();

            var date = lines.GetValueForHeaderNextLine("Ora"); //2018-10-04 15:00:00
            var match = lines.GetValueForHeaderNextLine("Partita");
            var evento = isPuntaBanca ? lines.GetValueForHeaderNextLine("PuntaBanca") : lines.GetValueForHeaderNextLineAtOccurence("PuntaPunta", 1, 2, 1);
            var firstBet = lines.GetValueForHeaderNextLine("VAI SU"); //PUNTA €100 A QUOTA 4.70
            var secondBet = isPuntaBanca ? lines.GetValueForHeaderNextLineAtOccurence("VAI SU", 2, 1, 3) :
                                           lines.GetValueForHeaderNextLineAtOccurence("VAI SU", 2, 1, 1); //BANCA copia€ 108.05   A QUOTA 4.40

            var firstBetParsed = ParseQuoteAmount(firstBet, isPuntaBanca);
            var secondBetParsed = ParseQuoteAmount(secondBet, isPuntaBanca);

            return new SimpleMatchedBetFormViewModel
            {
                BetDescription = evento,
                EventDate = ParseDateFromNinjabet(date),
                EventDescription = match,
                BackAmount = firstBetParsed.Amount,
                BackQuote = firstBetParsed.Quote,
                LayAmount = secondBetParsed.Amount,
                LayQuote = secondBetParsed.Quote,
                IsBackBack = !isPuntaBanca
            };
        }

        private static DateTime ParseDateFromNinjabet(string ninjabetDate)
        {
            return DateTime.ParseExact(ninjabetDate.Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static ParsedQuoteAmount ParseQuoteAmount(string ninjabetQuoteAmount, bool isPuntaBanca)
        {
            var rx = new Regex(@"[0-9]+(\.[0-9][0-9]?)?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Find matches.
            var matches = rx.Matches(ninjabetQuoteAmount);

            return new ParsedQuoteAmount
            {
                Amount = double.Parse(matches[isPuntaBanca ? 0 : 1].Value, CultureInfo.InvariantCulture),
                Quote = double.Parse(matches[isPuntaBanca? 1 : 2].Value, CultureInfo.InvariantCulture)
            };
        }

    }

    public class ParsedQuoteAmount
    {
        public double Quote { get; set; }
        public double Amount { get; set; }
    }

    public static class ParserHelper
    {
        public static string GetValueForHeaderNextLine(this string[] lines, string headerPart)
        {
            return lines.GetValueForHeaderNextLineAtOccurence(headerPart, 1, 1, 1);
        }

        public static string GetValueForHeaderNextLineAtOccurence(this string[] lines, string headerPart, int occurrence, int skipLines, int numLines)
        {
            var numOccurrences = 0;
            return lines.GetValueForHeader(line =>
                {
                    if (line.Contains(headerPart))
                    {
                        numOccurrences++;
                        if (numOccurrences == occurrence) return true;
                    }
                    return false;
                },
                index =>
                {
                    var res = "";
                    for (var i = 0; i < numLines; i++)
                        res += lines[index + skipLines + i] + " ";
                    return res;
                });
        } 

        public static string GetValueForHeader(this string[] lines, 
                                               Func<string, bool> headerMatchingFunc, 
                                               Func<int, string> extractInfoFunc)
        {
            for (var i = 0; i < lines.Count(); i++)
            {
                if (headerMatchingFunc(lines[i]))
                {
                    return extractInfoFunc(i);
                }
            }
            return string.Empty;            
        }
    }

    public static class InizializationHelper
    {
        public static Bet Initialize(this Bet bet, SimpleMatchedBetFormViewModel simpleMatchedBet, MatchedBet matchedBet)
        {
            

            bet.BetStatusId = BetStatus.Open;
            bet.BetDate = simpleMatchedBet.BetDate;
            bet.BetDescription = simpleMatchedBet.BetDescription;
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

        public static bool IsLay(this Bet bet)
        {
            return bet.BetType == BetType.SingleLay;
        }

        public static bool IsWinning(this Bet bet, MatchedBetStatus status)
        {
            return (bet.IsLay() && status == MatchedBetStatus.LayWon) ||
                   (!bet.IsLay() && status == MatchedBetStatus.BackWon);
        }

        public static BetEvent LastBetEvent(this Bet bet)
        {
            return bet.BetEvents.OrderBy(betEvent => betEvent.SportEvent.EventDate).FirstOrDefault();
        }
    }
    

    public class MatchedBetCreatedObjects {
        public MatchedBet MatchedBet { get; set; }
        public List<Bet> Bets { get; set; }
        public List<BetEvent> BetEvents { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<SportEvent> SportEvents { get; set; }
    }
}