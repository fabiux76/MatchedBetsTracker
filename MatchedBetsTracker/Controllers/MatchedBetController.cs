using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View();
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

            return RedirectToAction("Index", "Transaction");
        }
    }
}