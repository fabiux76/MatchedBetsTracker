using System;
using System.Collections.Generic;
using System.Linq;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;

namespace MatchedBetsTracker.BusinessLogic
{
    public interface IMatchedBetModelController
    {
        MatchedBet CreateNewSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel, int brokerAccountForUsedId);

        MatchedBet CreateNewMultipleMatchedBet(MultipleMatchedBetFormViewModel matchedBetViewModel, int brokerAccountForUsedId);

        MatchedBet AddSingleToMultipleMatchedBet(MultipleMatchedBetAddSingleFormViewModel multipleMatchedBetAddSingleFormViewModel);

        void DeleteMatchedBet(int matchedBetId);

        SportEvent SetHappenStatusOnEvent(int sportEventId, bool? newHappenStatus);

        SportEvent GetNextUnmatchedEventInMultiple(int matchedBetId);

        MatchedBet DeleteLastMatchedSingleInMultiple(int matchedBetId);
    }

    public class MatchedBetModelController : IMatchedBetModelController
    {
        private readonly ApplicationDbContext context;
        private readonly IMatchedBetsRepository _matchedBetsRepository;

        public MatchedBetModelController(ApplicationDbContext context, IMatchedBetsRepository matchedBetsRepository)
        {
            this.context = context;
            _matchedBetsRepository = matchedBetsRepository;
        }

        public MatchedBet CreateNewSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel,
            int brokerAccountForUsedId)
        {
            var brockerAccout = context.BrokerAccounts
                .Where(ba => ba.Id == matchedBetViewModel.BackBrokerAccountId)
                .Single();

            var userId = brockerAccout.OwnerId;

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
            var firstBetEvent = CreateBetEvent(matchedBetViewModel.BackQuote, BetEventType.BackHappen)
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

            var secondBetEvent = CreateBetEvent(matchedBetViewModel.LayQuote, matchedBetViewModel.IsBackBack ? BetEventType.BackNotHappen : BetEventType.Lay)
                .ForSportEvent(sportEvent)
                .ForBet(secondBet);

            //Creo la transaction per la seconda scommessa
            var secondBetTransaction = CreateOpenBetTransaction(secondBet);

            if (matchedBetViewModel.ValidateTransactions)
            {
                firstBetTransaction.Validated = true;
                secondBetTransaction.Validated = true;
            }

            context.MatchedBets.Add(matchedBet);
            context.SaveChanges();
            return matchedBet;
        }

        public MatchedBet CreateNewMultipleMatchedBet(MultipleMatchedBetFormViewModel matchedBetViewModel,
            int brokerAccountForUserId)
        {
            var brockerAccout = context.BrokerAccounts.Find(matchedBetViewModel.MultipleBrokerAccountId);
            var userId = brockerAccout.OwnerId;

            //Creo la matchedBet
            var matchedBet = CreateMatchedBet(matchedBetViewModel.BetDescription, userId);

            //creo la bet per la multipla
            var multipleBet = CreateBet(matchedBetViewModel.BetDescription,
                    matchedBetViewModel.MultipleBetDate,
                    matchedBetViewModel.MultipleQuoteTotal,
                    matchedBetViewModel.MultipleAmount,
                    BetType.MultipleBack) 
                .ForBrokerAccount(matchedBetViewModel.MultipleBrokerAccountId)
                .ForUserAccount(userId)
                .ForMatchedBet(matchedBet);

            //Cre la transaction per la prima scommessa
            var multiBetTransaction = CreateOpenBetTransaction(multipleBet);

            var sportEvents = new List<SportEvent>();
            var multipleBetEvents = new List<BetEvent>();

            //Ciclo sugli eventi
            matchedBetViewModel.Singles.ForEach(single =>
            {
                //Creo lo SportEvent (unico)
                var sportEvent = CreateSportEvent(single.EventDate, single.EventDescription);
                sportEvents.Add(sportEvent);

                //Creo la BetEvent per la prima scommessa
                var multipleBetEvent = CreateBetEvent(single.QuoteInMultiple, BetEventType.BackHappen)
                    .ForSportEvent(sportEvent)
                    .ForBet(multipleBet);
                multipleBetEvents.Add(multipleBetEvent);

                //Questa è la logica che invece devo fare quando aggiungo la bet singola
                /*
                //Creo la Bet per la scommessa
                var singleBet = CreateBet(matchedBetViewModel.BetDescription,
                        single.SingleBetDate,
                        single.QuoteInSingle,
                        single.SingleAmount,
                        single.IsSingleLay ? BetType.SingleLay : BetType.SingleBack)
                    .ForBrokerAccount(single.SingleBrokerAccountId)
                    .ForUserAccount(userId)
                    .ForMatchedBet(matchedBet);

                //Creo la BetEvent per la seconda scommessa
                var singleBetEvent = CreateBetEvent(single.QuoteInSingle, single.IsSingleLay ? BetEventType.Lay : BetEventType.BackNotHappen)
                    .ForSportEvent(sportEvent)
                    .ForBet(singleBet);

                //Creo la transaction per la seconda scommessa
                var secondBetTransaction = CreateOpenBetTransaction(singleBet);

                if (matchedBetViewModel.ValidateTransactions)
                {
                    secondBetTransaction.Validated = true;
                }
                */
            });
                      
            if (matchedBetViewModel.ValidateTransactions)
            {
                multiBetTransaction.Validated = true;
            }

            context.MatchedBets.Add(matchedBet);
            context.SaveChanges();
            return matchedBet;
        }

        public MatchedBet AddSingleToMultipleMatchedBet(MultipleMatchedBetAddSingleFormViewModel multipleMatchedBetAddSingleFormViewModel)
        {
            var matchedBet = _matchedBetsRepository.LoadMatchedBet(multipleMatchedBetAddSingleFormViewModel.MatchedBetId);
            var userId = matchedBet.Bets.Single(b => b.BetType == BetType.MultipleBack).UserAccountId;
            /*
            .Include(mb => mb.Bets)
            .Include(mb => mb.Bets.Select(b => b.BetEvents));
            */

            var sportEvent =
                context.SportEvents.Single(sp => sp.Id == multipleMatchedBetAddSingleFormViewModel.SportEventId);

            //Creo la Bet per la scommessa
            var singleBet = CreateBet(multipleMatchedBetAddSingleFormViewModel.SportEvent.EventDescription,
                    multipleMatchedBetAddSingleFormViewModel.BetDate,
                    multipleMatchedBetAddSingleFormViewModel.Quote,
                    multipleMatchedBetAddSingleFormViewModel.Amount,
                    multipleMatchedBetAddSingleFormViewModel.IsLay ? BetType.SingleLay : BetType.SingleBack)
                .ForBrokerAccount(multipleMatchedBetAddSingleFormViewModel.BrokerAccountId)
                .ForUserAccount(userId)
                .ForMatchedBet(matchedBet);

            //Creo la BetEvent per la seconda scommessa
            var singleBetEvent = CreateBetEvent(multipleMatchedBetAddSingleFormViewModel.Quote, multipleMatchedBetAddSingleFormViewModel.IsLay ? BetEventType.Lay : BetEventType.BackNotHappen)
                .ForSportEvent(sportEvent)
                .ForBet(singleBet);

            //Creo la transaction per la seconda scommessa
            var secondBetTransaction = CreateOpenBetTransaction(singleBet);

            if (multipleMatchedBetAddSingleFormViewModel.ValidateTransactions)
            {
                secondBetTransaction.Validated = true;
            }

            context.SaveChanges();
            return matchedBet;
        }

        public void DeleteMatchedBet(int matchedBetId)
        {
            _matchedBetsRepository.DeleteMatchedBet(matchedBetId);
        }

        //E' giusto che non venga ritornato nulla?!?! In questo caso dovrebbe essere lui ad occuparsi del salvataggio su DB...

        public SportEvent SetHappenStatusOnEvent(int sportEventId, bool? newHappenStatus)
        {
            //Verifico qual'è lo stato precedente
            var sportEvent = _matchedBetsRepository.LoadSportEvent(sportEventId);

            bool? previousState = sportEvent.Happened;

            //Verifico intanto se lo stato esistente è diverso dallo stato attuale. Se no, ritorno senza fare nulla
            if (previousState == newHappenStatus) return sportEvent;

            //A questo punto aggiorno gli stati ed aggiungo le transazioni
            sportEvent.Happened = newHappenStatus;

            //Aggiorno lo stato della BetEvent
            sportEvent.BetEvents.ForEach(be => be.UpdateBetStatus(newHappenStatus));

            //Aggiorno lo status delle bet
            sportEvent.BetEvents.Select(be => be.Bet).ForEach(b => b.UpdateBetStatusAndAmount());

            //Aggiorno lo status delle MatchedBet
            sportEvent.BetEvents.Select(be => be.Bet).FirstOrDefault().MatchedBet.UpdateMatchedBetStatus();

            //Se è !=null, devo fare undo dei stati e delle transazioni
            if (previousState != null)
            {
                //Rimuovo le winning transactions
                //Devo eliminare la transazione di CreditBet sulla scommessa vincente
                var creditBetTransactions = sportEvent.BetEvents
                                                      .Select(be => be.Bet)                            
                                                      .SelectMany(bet => bet.Transactions)
                    .Where(t => t.TransactionTypeId == TransactionType.CreditBet).ToList();

                context.Transactions.RemoveRange(creditBetTransactions);
            }

            //Aggiungo le Winning transactions
            var winninTransactions = sportEvent.BetEvents.Select(be => be.Bet)
                .Where(b => b.BetStatusId == BetStatus.Won)
                .Select(b => CreateCloseBetTransaction(b, sportEvent.EventDate))
                .ToList();

            context.Transactions.AddRange(winninTransactions);
            context.SaveChanges();

            return sportEvent;
        }

        public SportEvent GetNextUnmatchedEventInMultiple(int matchedBetId)
        {
            var matchedBet = _matchedBetsRepository.LoadMatchedBet(matchedBetId);
            return matchedBet.Bets.SelectMany(b => b.BetEvents).Select(be => be.SportEvent).OrderBy(se => se.EventDate)
                .FirstOrDefault(se => se.BetEvents.Count == 1);
        }

        public MatchedBet DeleteLastMatchedSingleInMultiple(int matchedBetId)
        {
            var matchedBet = _matchedBetsRepository.LoadMatchedBet(matchedBetId);
            //TUTTA QUESTA LOGICA VA SPOSTATA NELLA BUSINESS!!!!
            var sportEvent = matchedBet.Bets.SelectMany(b => b.BetEvents).Select(be => be.SportEvent).OrderByDescending(se => se.EventDate)
                .FirstOrDefault(se => se.BetEvents.Count > 1);

            //Se sportevent ha status diverso da unknown, non permettere la modifica
            if (sportEvent.Happened != null) throw new Exception("Cannot remove Bet on Verified event!");

            var betEventToRemove = sportEvent.BetEvents.Single(be => be.Bet.BetType != BetType.MultipleBack);
            var betToRemove = betEventToRemove.Bet;

            context.BetEvents.Remove(betEventToRemove);
            context.Bets.Remove(betToRemove);
            context.Transactions.RemoveRange(betToRemove.Transactions);

            context.SaveChanges();
            return matchedBet;
        }

        private static MatchedBet CreateMatchedBet(string eventDescription, int userId)
        {
            return new MatchedBet
            {
                EventDescription = eventDescription,
                Status = MatchedBetStatus.Open,
                UserAccountId = userId,
                Bets = new List<Bet>()                
            };
        }

        private static SportEvent CreateSportEvent(DateTime eventDate, string eventDescription)
        {
            return new SportEvent
            {
                EventDate = eventDate,
                EventDescription = eventDescription,
                BetEvents = new List<BetEvent>()
            };
        }

        private static BetEvent CreateBetEvent(double quote, BetEventType betEventType)
        {
            return new BetEvent
            {
                BetStatusId = BetStatus.Open,
                Quote = quote,
                BetEventType = betEventType               
            };
        }

        private Bet CreateBet(string betDescription, DateTime betDate, double quote, double betAmount, BetType betType)
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
                BetType = betType,
                BetEvents = new List<BetEvent>(),
                Transactions = new List<Transaction>()                
            };
        }

        private double ComputeResponsability(BetType betType, double betAmount, double betQuote)
        {
            return betType == BetType.SingleLay ? MatchedBetControllerModelHelper.ComputeLayResponsability(betQuote, betAmount) : betAmount;
        }

        public Transaction CreateOpenBetTransaction(Bet bet)
        {
            var transaction = new Transaction
            {
                Date = bet.BetDate,
                Amount = GetOpenBetAmount(bet),
                Bet = bet,
                BrokerAccountId = bet.BrokerAccountId,
                TransactionTypeId = TransactionType.OpenBet,
                Validated = false,
                UserAccountId = bet.UserAccountId
            };
            bet.Transactions.Add(transaction);
            return transaction;
        }

        public Transaction CreateCloseBetTransaction(Bet bet, DateTime date)
        {
            if (bet.IsWon())
            {
                var transaction = new Transaction
                {
                    Date = date,
                    Amount = bet.ProfitLoss - GetOpenBetAmount(bet),
                    Bet = bet,
                    BrokerAccountId = bet.BrokerAccountId,
                    TransactionTypeId = TransactionType.CreditBet,
                    Validated = false,
                    UserAccountId = bet.UserAccountId
                };
                bet.Transactions.Add(transaction);
                return transaction;
            }
            return null;
        }

        public double GetOpenBetAmount(Bet bet)
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
    }

    internal static class MatchedBetControllerModelHelper
    {
        public static double LayBrokerNetGain = 0.95;

        public static Bet ForMatchedBet(this Bet bet, MatchedBet matchedBet)
        {
            bet.MatchedBet = matchedBet;
            matchedBet.Bets.Add(bet);
            return bet;
        }

        public static Bet ForBrokerAccount(this Bet bet, BrokerAccount brokerAccount)
        {
            bet.BrokerAccount = brokerAccount;
            brokerAccount.Bets.Add(bet);            
            return bet;
        }

        public static Bet ForBrokerAccount(this Bet bet, int brokerAccountId)
        {
            bet.BrokerAccountId = brokerAccountId;
            return bet;
        }

        public static Bet ForUserAccount(this Bet bet, UserAccount userAccount)
        {
            bet.UserAccount = userAccount;            
            return bet;
        }

        public static Bet ForUserAccount(this Bet bet, int userAccountId)
        {
            bet.UserAccountId = userAccountId;
            return bet;
        }

        public static BetEvent ForSportEvent(this BetEvent betEvent, SportEvent sportEvent)
        {
            betEvent.SportEvent = sportEvent;
            sportEvent.BetEvents.Add(betEvent);
            return betEvent;
        }

        public static BetEvent ForBet(this BetEvent betEvent, Bet bet)
        {
            betEvent.Bet = bet;
            bet.BetEvents.Add(betEvent);
            return betEvent;
        }
        public static void UpdateBetStatus(this BetEvent betEvent, bool? happened)
        {
            if (happened == null) betEvent.BetStatusId = BetStatus.Open;
            else betEvent.BetStatusId = (bool)happened
                    ? betEvent.BetEventType == BetEventType.BackHappen ? BetStatus.Won : BetStatus.Loss
                    : betEvent.BetEventType == BetEventType.BackHappen ? BetStatus.Loss : BetStatus.Won;
        }

        public static void UpdateBetStatusAndAmount(this Bet bet)
        {
            if (bet.BetEvents.Any(be => be.BetStatusId == BetStatus.Loss))
            {
                bet.BetStatusId = BetStatus.Loss;
            }
            else
            {
                if (bet.BetEvents.Any(be => be.BetStatusId == BetStatus.Open))
                {
                    bet.BetStatusId = BetStatus.Open;
                }
                else
                {
                    bet.BetStatusId = BetStatus.Won;
                }
            }
            RecomputeBetResponsabilityAndProfit(bet);
        }

        public static void UpdateMatchedBetStatus(this MatchedBet matchedBet)
        {
            if (matchedBet.Bets.Any(b => b.BetStatusId == BetStatus.Open)) matchedBet.Status = MatchedBetStatus.Open;
            else matchedBet.Status = MatchedBetStatus.Closed;          
        }

        private static void RecomputeBetResponsabilityAndProfit(Bet bet)
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
                    bet.ProfitLoss = bet.BetAmount * LayBrokerNetGain;
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

        public static MatchedBet MatchedBet(this SportEvent sportEvent)
        {
            return sportEvent.BetEvents.First().Bet.MatchedBet;
        }

        public static IEnumerable<SportEvent> SportEvents(this MatchedBet matchedBet)
        {
            return matchedBet.Bets.SelectMany(b => b.BetEvents).Select(be => be.SportEvent).GroupBy(se => se.Id)
                .Select(grp => grp.First()).ToList();
        }
    }
}