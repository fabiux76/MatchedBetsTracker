using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System;
using System.Linq;
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
        public ActionResult Index(bool showClosed = false)
        {
            var matchedBets = _context.MatchedBets
                                      .Where(matchedBet => showClosed || matchedBet.Status == MatchedBetStatus.Open)
                                      .Include(matchedBet => matchedBet.UserAccount)
                                      .ToList();

            return View(matchedBets);
        }

        public ActionResult NewSimple()
        {
            var viewModel = new SimpleMatchedBetFormViewModel
            {
                BetDate = DateTime.Now,
                EventDate = DateTime.Now,            
                BrokerAccounts = _context.BrokerAccounts
                                         .Where(broker => broker.Active)
                                         .ToList()
            };

            return View("SimpleMatchedBetForm", viewModel);
        }

        public ActionResult AddSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel)
        {
            if (!ModelState.IsValid)
            {
                matchedBetViewModel.BrokerAccounts = _context.BrokerAccounts.ToList();
                return View("SimpleMatchedBetForm", matchedBetViewModel);
            }

            //Devo fare diverse attività:
            //0. Devo creare la matchedBet
            var brockerAccout = _context.BrokerAccounts
                                .Where(ba => ba.Id == matchedBetViewModel.BackBrokerAccountId)
                                .SingleOrDefault();

            var objects = MatchedBetHandler.CreateObjectsForSimpleMatchedBet(matchedBetViewModel, brockerAccout.OwnerId);            

            _context.MatchedBets.Add(objects.MatchedBet);
            _context.Bets.AddRange(objects.Bets);
            _context.BetEvents.AddRange(objects.BetEvents);
            _context.Transactions.AddRange(objects.Transactions);
            _context.SportEvents.AddRange(objects.SportEvents);

            _context.SaveChanges();

            return RedirectToAction("Details", objects.MatchedBet);
        }

        public ActionResult Delete(int id)
        {            
            var matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
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

        public ActionResult Close(int id, MatchedBetStatus status)
        {
            var matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
                .Single(mb => mb.Id == id);

            //Devo cambiare il stato del MatchedBet
            matchedBet.Status = status;

            //Devo cambiare lo stato delle bet e calcolare il profitto\perdita
            var winningBets = matchedBet.Bets.Where(bet => bet.IsWinning(status)).ToList();
            winningBets.ForEach(bet =>
            {
                bet.BetStatusId = BetStatus.Won;                
            });

            var losingBets = matchedBet.Bets.Where(bet => !bet.IsWinning(status)).ToList();
            losingBets.ForEach(bet =>
            {
                bet.BetStatusId = BetStatus.Loss;
            });


            //TUTTO STO GIRO PERCJHE' NON MI AGGIORNA LO Status se aggiorno il BetStatusId...
            //Ci dev'ssere per forza un altro modo...
            //////////////
            _context.SaveChanges();
            
            matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
                .Single(mb => mb.Id == id);            

            matchedBet.Bets.ForEach(bet => MatchedBetHandler.RecomputeBetResponsabilityAndProfit(bet));

            winningBets = matchedBet.Bets.Where(bet => bet.IsWinning(status)).ToList();
            //////////////

            //Devo aggiungere la nuova transazione di CreditBet sulla scommessa vincente
            var winningTransactions = winningBets.Select(bet => MatchedBetHandler.CreateCloseBetTransaction(bet)).ToList();

            _context.Transactions.AddRange(winningTransactions);
            _context.SaveChanges();

            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult ReOpen(int id)
        {
            var matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
                .Single(mb => mb.Id == id);

            //Devo cambiare il stato del MatchedBet
            matchedBet.Status = MatchedBetStatus.Open;

            //Devo cambiare lo stato delle bet e calcolare il profitto\perdita
            matchedBet.Bets.ForEach(bet => bet.BetStatusId = BetStatus.Open) ;
            
            //TUTTO STO GIRO PERCJHE' NON MI AGGIORNA LO Status se aggiorno il BetStatusId...
            //Ci dev'ssere per forza un altro modo...
            //////////////
            _context.SaveChanges();

            matchedBet = _context.MatchedBets
                .Include(mb => mb.Bets)
                .Include(mb => mb.Bets.Select(b => b.BetEvents))
                .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                .Include(mb => mb.Bets.Select(b => b.Status))
                .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                .Include(mb => mb.Bets.Select(b => b.UserAccount))
                .Include(mb => mb.Bets.Select(b => b.Transactions))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
                .Single(mb => mb.Id == id);
            //////////////

            matchedBet.Bets.ForEach(MatchedBetHandler.RecomputeBetResponsabilityAndProfit);

            //Devo eliminare la transazione di CreditBet sulla scommessa vincente
            var creditBetTransactions = matchedBet.Bets.SelectMany(bet => bet.Transactions)
                .Where(t => t.TransactionTypeId == TransactionType.CreditBet).ToList();

            
            _context.Transactions.RemoveRange(creditBetTransactions);
            _context.SaveChanges();

            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult Details(int id)
        {
            var matchedBet = _context.MatchedBets
                                    .Include(mb => mb.Bets)
                                    .Include(mb => mb.Bets.Select(b => b.BetEvents))
                                    .Include(mb => mb.Bets.Select(b => b.BetEvents.Select(betEvent => betEvent.SportEvent)))
                                    .Include(mb => mb.Bets.Select(b => b.Status))
                                    .Include(mb => mb.Bets.Select(b => b.BrokerAccount))
                                    .Include(mb => mb.Bets.Select(b => b.UserAccount))
                                    .Include(mb => mb.Bets.Select(b => b.Transactions))
                                    .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.TransactionType)))
                                    .Include(mb => mb.Bets.Select(b => b.Transactions.Select(t => t.UserAccount)))
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

        //Da fare mettendo a afattor comune il codice sopra
        private MatchedBet LoadMatchedBet(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult OpenParseView()
        {
            var viewModel = new NinjaBetOddsmatcherViewModel()
            {
                Text = ""
            };

            return View("ParseNinjaBetOddsmatcherView", viewModel);
        }

        public ActionResult ParseNinjaBet(string Text)
        {
            var viewModel = MatchedBetHandler.Parse(Text);

            viewModel.BetDate = DateTime.Now;
            viewModel.BrokerAccounts = _context.BrokerAccounts
                .Where(broker => broker.Active)
                .ToList();

            return View("SimpleMatchedBetForm", viewModel);
        }
    }
}