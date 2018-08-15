using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MatchedBetsTracker.ViewModels;

namespace MatchedBetsTracker.Controllers
{
    public class BetController : Controller
    {
        private ApplicationDbContext _context;

        public BetController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        // GET: Bet
        public ActionResult Index()
        {
            var bets = _context.Bets
                                    .Include(b => b.Status)
                                    .Include(b => b.BrokerAccount)
                                    .Include(b => b.MatchedBet)
                                    .ToList();

            return View(bets);
        }
        public ActionResult New()
        {
            var viewModel = new BetFormViewModel
            {
                BetStatuses = _context.BetStatuses.ToList(),
                BrokerAccounts = _context.BrokerAccounts.ToList(),
                Bet = new Bet
                {
                    BetDate = DateTime.Now,
                    EventDate = DateTime.Now
                }
            };

            return View("BetForm", viewModel);
        }

        public ActionResult Save(Bet bet)
        {
            if (bet.Id == 0)
            {
                _context.Bets.Add(bet);
            }
            else
            {
                var betInDb = _context.Bets.Single(t => t.Id == bet.Id);

                //Da sostituire con AutoMapper
                betInDb.BetAmount = bet.BetAmount;
                betInDb.BetDate = bet.BetDate;
                betInDb.BetDescription = bet.BetDescription;
                betInDb.BetStatusId = bet.BetStatusId;
                betInDb.BrokerAccountId = bet.BrokerAccountId;
                betInDb.EventDate = bet.EventDate;
                betInDb.EventDescription = bet.EventDescription;
                betInDb.Id = bet.Id;
                betInDb.IsLay = bet.IsLay;
                betInDb.MatchedBetId = bet.MatchedBetId;
                betInDb.ProfitLoss = bet.ProfitLoss;
                betInDb.Quote = bet.Quote;
                betInDb.Responsability = bet.Responsability;
                betInDb.Validated = bet.Validated;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Bet");
        }

        public ActionResult Edit(int id)
        {
            var bet = _context.Bets
                                    .Include(b => b.Status)
                                    .Include(b => b.BrokerAccount)
                                    .SingleOrDefault(t => t.Id == id);

            if (bet == null)
                return HttpNotFound();

            var viewModel = new BetFormViewModel
            {
                BetStatuses = _context.BetStatuses.ToList(),
                BrokerAccounts = _context.BrokerAccounts.ToList(),
                Bet = bet
            };

            return View("BetForm", viewModel);
        }
    }
}