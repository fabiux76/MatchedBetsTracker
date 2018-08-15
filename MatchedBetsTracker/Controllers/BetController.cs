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
                                    .ToList();

            return View(bets);
        }

        /*

        public int Id { get; set; }
        public string EventDescription { get; set; }
        public string BetDescription { get; set; }
        public DateTime BetDate { get; set; }
        public DateTime EventDate { get; set; }
        public bool Validated { get; set; }
        public bool IsLay { get; set; } //Lay = bancare
        public double Quote { get; set; }
        public double BetAmount { get; set; }
        public double Responsability { get; set; }
        public BetStatus Status { get; set; }
        public byte BetStatusId { get; set; }
        public double ProfitLoss { get; set; }
        public MatchedBet MatchedBet { get; set; }
        public int MatchedBetId { get; set; }
        */

            /*
        public ActionResult New()
        {
            var viewModel = new TransactionFormViewModel
            {
                TransactionTypes = _context.TransactionTypes.ToList(),
                BrokerAccounts = _context.BrokerAccounts.ToList(),
                Transaction = new Transaction
                {
                    Date = DateTime.Now
                }
            };

            return View("TransactionForm", viewModel);
        }

        public ActionResult Save(Transaction transaction)
        {
            if (transaction.Id == 0)
            {
                _context.Transactions.Add(transaction);
            }
            else
            {
                var transactionInDb = _context.Transactions.Single(t => t.Id == transaction.Id);

                //Da sostituire con AutoMapper
                transactionInDb.Amount = transaction.Amount;
                transactionInDb.BetId = transaction.BetId;
                transactionInDb.BrokerAccountId = transaction.BrokerAccountId;
                transactionInDb.Date = transaction.Date;
                transactionInDb.Id = transaction.Id;
                transactionInDb.TransactionTypeId = transaction.TransactionTypeId;
                transactionInDb.Validated = transaction.Validated;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Transaction");
        }

        public ActionResult Edit(int id)
        {
            var transaction = _context.Transactions
                                    .Include(t => t.TransactionType)
                                    .Include(t => t.BrokerAccount)
                                    .SingleOrDefault(t => t.Id == id);

            if (transaction == null)
                return HttpNotFound();

            var viewModel = new TransactionFormViewModel
            {
                TransactionTypes = _context.TransactionTypes.ToList(),
                BrokerAccounts = _context.BrokerAccounts.ToList(),
                Transaction = transaction
            };

            return View("TransactionForm", viewModel);
        }
        */
    }
}