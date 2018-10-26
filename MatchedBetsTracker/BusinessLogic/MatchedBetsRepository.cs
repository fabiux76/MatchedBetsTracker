using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchedBetsTracker.Models;
using System.Data.Entity;

namespace MatchedBetsTracker.BusinessLogic
{
    public interface IMatchedBetsRepository
    {
        //Carica il MatchedBet e tutte le classi associate fino a SportEvent 
        MatchedBet LoadMatchedBet(int matchedBetId);

        //Carica lo SportEvent e tutte le classi associate fino a MatchedBet
        SportEvent LoadSportEvent(int sportEventId);

        void DeleteMatchedBet(int matchedBetId);

        BrokerAccount LoadBrokerAccount(int brokerAccountId);

        List<MatchedBet> LoadAllMatchedBets();
        List<BrokerAccount> LoadAllBrokerAccounts();
    }

    public class MatchedBetsRepository : IMatchedBetsRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchedBetsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public MatchedBet LoadMatchedBet(int matchedBetId)
        {
            return _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(be => be.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(be => be.SportEvent.BetEvents)))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
                .Single(mb => mb.Id == matchedBetId);
        }

        public SportEvent LoadSportEvent(int sportEventId)
        {
            return _context.SportEvents
                .Include(se => se.BetEvents)
                .Include(se => se.BetEvents.Select(be => be.Bet))
                .Include(se => se.BetEvents.Select(be => be.Bet.BetEvents))
                .Include(se => se.BetEvents.Select(be => be.Bet.Transactions))
                .Include(se => se.BetEvents.Select(be => be.Bet.MatchedBet))
                .Include(se => se.BetEvents.Select(be => be.Bet.MatchedBet.Bets))
                .Single(sp => sp.Id == sportEventId);
        }

        public void DeleteMatchedBet(int matchedBetId)
        {
            var matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
                .Single(mb => mb.Id == matchedBetId);

            var transactions = matchedBet.Bets.SelectMany(bet => bet.Transactions).ToList();
            var bets = matchedBet.Bets.ToList();
            var betEvents = matchedBet.Bets.SelectMany(b => b.BetEvents);
            var sportEvents = matchedBet.Bets.SelectMany(b => b.BetEvents)
                .Select(be => be.SportEvent).ToList();

            transactions.ForEach(transaction => _context.Transactions.Remove(transaction));
            bets.ForEach(bet => _context.Bets.Remove(bet));
            _context.SportEvents.RemoveRange(sportEvents);
            _context.BetEvents.RemoveRange(betEvents);
            _context.MatchedBets.Remove(matchedBet);

            _context.SaveChanges();
        }

        public BrokerAccount LoadBrokerAccount(int brokerAccountId)
        {
            return _context.BrokerAccounts.Find(brokerAccountId);
        }

        public List<MatchedBet> LoadAllMatchedBets()
        {
            return _context.MatchedBets                
                            .Include(matchedBet => matchedBet.UserAccount)
                            .ToList();
        }

        public List<BrokerAccount> LoadAllBrokerAccounts()
        {
            return _context.BrokerAccounts.ToList();
        }


    }
}