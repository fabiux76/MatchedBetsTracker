using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MatchedBetsTracker.Controllers
{
    public class MatchedBetController : Controller
    {
        private ApplicationDbContext _context;

        public MatchedBetController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: MatchedBet
        public ActionResult Index()
        {
            var matchedBets = _context.MatchedBets
                        .ToList();

            return View(matchedBets);
        }

        public ActionResult NewSimple()
        {
            var viewModel = new SimpleMatchedBetFormViewModel
            {
                BetDate = DateTime.Now,
                EventDate = DateTime.Now,            
                BrokerAccounts = _context.BrokerAccounts.ToList()
            };

            return View("SimpleMatchedBetForm", viewModel);
        }

        public ActionResult AddSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel)
        {
            //Devo fare diverse attività:
            //0. Devo creare la matchedBet
            var matchedBet = MatchedBetHandler.CreateMatchedBet(matchedBetViewModel);

            //1. Creazione della scommessa di Puntata
            var backBet = MatchedBetHandler.CreateBackBet(matchedBetViewModel, matchedBet);

            //2. Creazione della scommessa di Bancata
            var layBet = MatchedBetHandler.CreateLayBet(matchedBetViewModel, matchedBet);

            //3. Creazione della Transazione di Puntata
            var backTransaction = MatchedBetHandler.CreateOpenBetTransaction(backBet);

            //4. Creazione della Transazione di Bancata
            var layTransaction = MatchedBetHandler.CreateOpenBetTransaction(layBet);

            _context.MatchedBets.Add(matchedBet);
            _context.Bets.Add(backBet);
            _context.Bets.Add(layBet);
            _context.Transactions.Add(backTransaction);
            _context.Transactions.Add(layTransaction);

            _context.SaveChanges();

            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult Delete(int id)
        {            
            var matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Single(mb => mb.Id == id);

            //Devo cancellare in cascata 
            /*
            var matchedBet = _context.MatchedBets
                .Single(mb => mb.Id == id);
            */

            var transactions = matchedBet.Bets.SelectMany(bet => bet.Transactions).ToList();
            transactions.ForEach(transaction => _context.Transactions.Remove(transaction));

            var bets = matchedBet.Bets.ToList();
            bets.ForEach(bet => _context.Bets.Remove(bet));
            
            _context.MatchedBets.Remove(matchedBet);
            
            _context.SaveChanges();

            var matchedBets = _context.MatchedBets
                        .ToList();

            return View("Index", matchedBets);
        }

        public ActionResult Close(int id, int status)
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var matchedBet = _context.MatchedBets
                                    .Include(mb => mb.Bets)
                                    .Include(mb => mb.Bets.Select(b => b.Status))
                                    .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                                    .Include(mb => mb.Bets.Select(b => b.Transactions))
                                    .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                                    .SingleOrDefault(mb => mb.Id == id);

            if (matchedBet == null) return HttpNotFound();

            //Eè possibile fare questo sotto con eager loading
            /*
            var bets = _context.Bets.Where(b => b.MatchedBetId == id)
                                    .Include(b => b.Status)
                                    .Include(b => b.BrokerAccount)
                                    .ToList();

            var transactions = _context.Transactions.Where(t => t.Bet.MatchedBetId == id)
                                    .Include(t => t.TransactionType)
                                    .Include(t => t.Bet)
                                    .ToList();
            */

            /*
            return View(new BrokerAccountDetailsViewModel
            {
                BrokerAccount = brokerAccount,
                Transactions = transactions
            });
            */

            return View(matchedBet);
        }
    }
}