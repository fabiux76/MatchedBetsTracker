using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MatchedBetsTracker.Models;

namespace MatchedBetsTracker.Controllers
{
    public class BrokerAccountController : Controller
    {
        private ApplicationDbContext _context;

        public BrokerAccountController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: BrokerAccount
        public ActionResult Index()
        {
            var brokerAccounts = _context.BrokerAccounts;

            return View(brokerAccounts);
        }
    }
}