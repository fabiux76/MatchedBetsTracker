using System;
using System.Collections.Generic;
using System.Linq;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System.Data.Entity;
using System.Runtime.InteropServices;

namespace MatchedBetsTracker.BusinessLogic
{
    public interface IMatchedBetModelController
    {
        MatchedBetCreatedObjects CreateObjectsForSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel,
            int userId);

        void SetHappenStatusOnEvent(int sportEventId, bool? newHappenStatus);

    }

    public class MatchedBetModelController : IMatchedBetModelController
    {
        
        private ApplicationDbContext _context;

        public MatchedBetModelController()
        {
            _context = new ApplicationDbContext();
        }

        public MatchedBetCreatedObjects CreateObjectsForSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel,
            int userId)
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

            return new MatchedBetCreatedObjects
            {
                MatchedBet = matchedBet,
                Bets = new List<Bet> { firstBet, secondBet },
                BetEvents = new List<BetEvent> { firstBetEvent, secondBetEvent },
                Transactions = new List<Transaction> { firstBetTransaction, secondBetTransaction },
                SportEvents = new List<SportEvent> { sportEvent }
            };
        }

        //E' giusto che non venga ritornato nulla?!?! In questo caso dovrebbe essere lui ad occuparsi del salvataggio su DB...

        public void SetHappenStatusOnEvent(int sportEventId, bool? newHappenStatus)
        {
            //Verifico qual'è lo stato precedente
            var sportEvent = _context.SportEvents
                                .Include(se => se.BetEvents)
                                .Include(se => se.BetEvents.Select(be => be.Bet))
                                .Include(se => se.BetEvents.Select(be => be.Bet.Transactions))
                                .Include(se => se.BetEvents.Select(be => be.Bet.MatchedBet))
                                .SingleOrDefault(sp => sp.Id == sportEventId);

            bool? previousState = sportEvent.Happened;

            //Verifico intanto se lo stato esistente è diverso dallo stato attuale. Se no, ritorno senza fare nulla
            if (previousState == newHappenStatus) return;

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

                _context.Transactions.RemoveRange(creditBetTransactions);
            }

            //Aggiungo le Winning transactions
            var winninTransactions = sportEvent.BetEvents.Select(be => be.Bet)
                .Where(b => b.BetStatusId == BetStatus.Won)
                .Select(CreateCloseBetTransaction)
                .ToList();

            _context.Transactions.AddRange(winninTransactions);
            _context.SaveChanges();
        }

        private byte GetBetStatus(bool? happened, bool isLay)
        {
            if (happened == null) return BetStatus.Open;
            return (bool)happened 
                            ? isLay ? BetStatus.Loss : BetStatus.Won 
                            : isLay ? BetStatus.Won : BetStatus.Loss;
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
                BetType = betType
            };
        }

        private double ComputeResponsability(BetType betType, double betAmount, double betQuote)
        {
            return betType == BetType.SingleLay ? MatchedBetControllerModelHelper.ComputeLayResponsability(betQuote, betAmount) : betAmount;
        }

        public Transaction CreateOpenBetTransaction(Bet bet)
        {
            return new Transaction
            {
                Date = bet.BetDate,
                Amount = GetOpenBetAmount(bet),
                Bet = bet,
                BrokerAccountId = bet.BrokerAccountId,
                TransactionTypeId = TransactionType.OpenBet,
                Validated = false,
                UserAccountId = bet.UserAccountId
            };
        }

        public Transaction CreateCloseBetTransaction(Bet bet)
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
        public static double layBrokerNetGain = 0.95;

        public static Bet ForMatchedBet(this Bet bet, MatchedBet matchedBet)
        {
            bet.MatchedBet = matchedBet;
            return bet;
        }

        public static Bet ForBrokerAccount(this Bet bet, BrokerAccount brokerAccount)
        {
            bet.BrokerAccount = brokerAccount;
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
            return betEvent;
        }

        public static BetEvent ForBet(this BetEvent betEvent, Bet bet)
        {
            betEvent.Bet = bet;
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
            else matchedBet.Status = MatchedBetStatus.BackWon; //TODO: PER CONVENZIONE. In realtà l'unico stato sensato è Closed or Open            
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
    }
}