using System.Linq;
using System.Web.Mvc;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System.Data.Entity;
using MatchedBetsTracker.BusinessLogic;

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
        public ActionResult Index(bool showInactive = false)
        {
            var brokerAccounts = _context.BrokerAccounts
                                         .Include(b => b.Transactions)
                                         .Include(b => b.Bets)
                                         .Include(b => b.Bets.Select(be => be.BetEvents))
                                         .Include(b => b.Owner)
                                         .ToList();

            var userAccounts = _context.UserAccounts
                                        .Include(u => u.BrokerAccounts);

            var accountsWithSummary = MatchedBetHandler.CreateAccountsSummary(
                brokerAccounts.Select(MatchedBetHandler.CreateAccountWithSummeries)
                              .ToList(), userAccounts.ToList(), showInactive);

            return View(accountsWithSummary);
        }

        public ActionResult Details(int id)
        {
            var brokerAccount = _context.BrokerAccounts.SingleOrDefault(account => account.Id == id);

            if (brokerAccount == null) return HttpNotFound();

            var transactions = _context.Transactions.Where(t => t.BrokerAccountId == id)
                                    .Include(t => t.TransactionType)
                                    .Include(t => t.Bet)
                                    .Include(b => b.Bet.BetEvents)
                                    .Include(t => t.BrokerAccount)
                                    .Include(t => t.UserAccount)
                                    .OrderBy(t => t.Date)
                                    .ToList();

            var bets = _context.Bets.Where(b => b.BrokerAccountId == id)
                .Include(b => b.Status)
                .Include(b => b.BrokerAccount)
                .Include(b => b.MatchedBet)
                .Include(b => b.UserAccount)
                .Include(b => b.BetEvents)
                .OrderBy(b => b.BetDate)
                .ToList();

            return View(new BrokerAccountDetailsViewModel
            {
                BrokerAccount = brokerAccount,
                Transactions = transactions,
                Bets = bets               
            });
        }

        public ActionResult New()
        {
            var viewModel = new BrokerAccountFormViewModel
            {
                BrokerAccount = new BrokerAccount(),
                UserAccounts = _context.UserAccounts.ToList()
            };

            return View("BrokerAccountForm", viewModel);
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
                brokerAccountInDb.IntialAmount = brokerAccount.IntialAmount;
                brokerAccountInDb.Name = brokerAccount.Name;
                brokerAccountInDb.Password = brokerAccount.Password;
                brokerAccountInDb.UserName = brokerAccount.UserName;
                brokerAccountInDb.Notes = brokerAccount.Notes;
                brokerAccountInDb.Active = brokerAccount.Active;
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "BrokerAccount");
        }

        public ActionResult Edit(int id)
        {

            var brokerAccount = _context.BrokerAccounts.SingleOrDefault(b => b.Id == id);

            if (brokerAccount == null)
                return HttpNotFound();

            var viewModel = new BrokerAccountFormViewModel
            {
                BrokerAccount = brokerAccount,
                UserAccounts = _context.UserAccounts.ToList()
            };

            return View("BrokerAccountForm", viewModel);
        }
    }
}