using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

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
                                    .ToList();

            return View(transactions);
        }
    }
}