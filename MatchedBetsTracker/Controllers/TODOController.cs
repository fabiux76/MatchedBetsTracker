using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System.Data.Entity;

namespace MatchedBetsTracker.Controllers
{
    public class TodoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TodoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TODO
        public ActionResult Index()
        {
            var viewModel = new TodoViewModel
            {
                TransactionsToVerify = _context.Transactions.Include(t => t.TransactionType)
                    .Include(t => t.BrokerAccount)
                    .Include(t => t.UserAccount)
                    .Include(t => t.Bet)
                    .Include(t => t.Bet.BetEvents)
                    .Include(t => t.Bet.BetEvents.Select(be => be.SportEvent))
                    .Where(t => !t.Validated).ToList(),
                SportEventsToCheck = _context.SportEvents.Where(se => se.EventDate < DateTime.Now && se.Happened == null)
                    .Include(se => se.BetEvents)
                    .Include(se => se.BetEvents.Select(be => be.Bet))
                    .Include(se => se.BetEvents.Select(be => be.Bet.BrokerAccount))
                    .Include(se => se.BetEvents.Select(be => be.Bet.MatchedBet))
                    .OrderByDescending(se => se.EventDate).ToList(),
                MultipleBetsToLay = GetMultiplesToLay()
            };

            return View(viewModel);
        }

        private List<MatchedBet> GetMultiplesToLay()
        {
            var matchedBets = _context.MatchedBets.Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.UserAccount)
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(be => be.SportEvent)))
                .Where(mb => mb.Status == MatchedBetStatus.Open)
                .ToList();

            return matchedBets.Where(mb => HavePendingLayToDo(mb.SportEvents().ToList())).ToList();
        }

        private bool HavePendingLayToDo(List<SportEvent> sportEvnts)
        {
            return sportEvnts.Any(se => se.Happened == null) && 
                    sportEvnts.Where(se => se.BetEvents.Count == 2).All(se => se.EventDate < DateTime.Now) && 
                    sportEvnts.Exists(se => se.BetEvents.Count == 1);
        }
        
    }
}