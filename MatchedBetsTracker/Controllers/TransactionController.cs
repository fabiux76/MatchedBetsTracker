using MatchedBetsTracker.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using MatchedBetsTracker.ViewModels;

namespace MatchedBetsTracker.Controllers
{
    public class TransactionController : Controller
    {
        private ApplicationDbContext _context;

        public TransactionController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Transaction
        public ActionResult Index()
        {
            var transactions = _context.Transactions
                                    .Include(t => t.TransactionType)
                                    .Include(t => t.BrokerAccount)
                                    .Include(t => t.Bet)
                                    .ToList();

            return View(transactions);
        }

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
            if (!ModelState.IsValid)
            {
                var viewModel = new TransactionFormViewModel
                {
                    TransactionTypes = _context.TransactionTypes.ToList(),
                    BrokerAccounts = _context.BrokerAccounts.ToList(),
                    Transaction = transaction
                };

                return View("TransactionForm", viewModel);
            }

            if (transaction.Id == 0)
            {
                _context.Transactions.Add(transaction);
            }
            else
            {
                var transactionInDb = _context.Transactions.Single(t => t.Id == transaction.Id);

                //Da sostituire con AutoMapper
                transactionInDb.Amount = transaction.Amount;
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
                                    .Include(t => t.Bet)
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
    }
}