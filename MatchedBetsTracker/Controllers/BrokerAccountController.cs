using System.Linq;
using System.Web.Mvc;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System.Data.Entity;

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
            var brokerAccounts = _context.BrokerAccounts.ToList();

            return View(brokerAccounts);
        }

        public ActionResult Details(int id)
        {
            var brokerAccount = _context.BrokerAccounts.SingleOrDefault(account => account.Id == id);

            if (brokerAccount == null) return HttpNotFound();

            var transactions = _context.Transactions.Where(t => t.BrokerAccountId == id)
                                    .Include(t => t.TransactionType)
                                    .Include(t => t.Bet)
                                    .Include(t => t.BrokerAccount)
                                    .OrderBy(t => t.Date)
                                    .ToList();

            return View(new BrokerAccountDetailsViewModel
            {
                BrokerAccount = brokerAccount,
                Transactions = transactions
            });
        }

        public ActionResult New()
        {
            return View("BrokerAccountForm");
        }

        [HttpPost]
        public ActionResult Save(BrokerAccount brokerAccount)
        {
            if (brokerAccount.Id == 0)
            {
                _context.BrokerAccounts.Add(brokerAccount);
            }
            else
            {
                var brokerAccountInDb = _context.BrokerAccounts.Single(b => b.Id == brokerAccount.Id);

                //Da sostituire con AutoMapper
                brokerAccountInDb.Id = brokerAccount.Id;
                brokerAccountInDb.IntialAmount = brokerAccount.Id;
                brokerAccountInDb.Name = brokerAccount.Name;
                brokerAccountInDb.Password = brokerAccount.Password;
                brokerAccountInDb.UserName = brokerAccount.UserName;
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "BrokerAccount");
        }

        public ActionResult Edit(int id)
        {
            var brokerAccount = _context.BrokerAccounts.SingleOrDefault(b => b.Id == id);

            if (brokerAccount == null)
                return HttpNotFound();

            return View("BrokerAccountForm", brokerAccount);
        }
    }
}