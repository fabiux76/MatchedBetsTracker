using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using MatchedBetsTracker.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MatchedBetsTracker.Controllers
{
    public class MatchedBetController : Controller
    {
        private readonly IMatchedBetModelController _matchedBetModelController;
        private readonly IMatchedBetsRepository _matchedBetsRepository;

        public MatchedBetController(IMatchedBetModelController matchedBetModelController, IMatchedBetsRepository matchedBetsRepository)
        {
            _matchedBetModelController = matchedBetModelController;
            _matchedBetsRepository = matchedBetsRepository;
        }

        // GET: MatchedBet
        public ActionResult Index(bool showClosed = false)
        {
            var matchedBets = _matchedBetsRepository.LoadAllMatchedBets()
                                                    .Where(matchedBet => showClosed || matchedBet.Status == MatchedBetStatus.Open)
                                                    .ToList();
            return View(matchedBets);
        }

        public ActionResult NewSimple()
        {
            var viewModel = new SimpleMatchedBetFormViewModel
            {
                BetDate = DateTime.Now,
                EventDate = DateTime.Now,            
                BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts()
                                         .Where(broker => broker.Active)
                                         .ToList()
            };

            return View("SimpleMatchedBetForm", viewModel);
        }

        public ActionResult AddSimpleMatchedBet(SimpleMatchedBetFormViewModel matchedBetViewModel)
        {
            if (!ModelState.IsValid)
            {
                matchedBetViewModel.BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts();
                return View("SimpleMatchedBetForm", matchedBetViewModel);
            }
            
            var matchedBet = _matchedBetModelController.CreateNewSimpleMatchedBet(matchedBetViewModel, matchedBetViewModel.BackBrokerAccountId);            
            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult AddMultipleMatchedBet(MultipleMatchedBetFormViewModel matchedBetViewModel)
        {
            if (!ModelState.IsValid)
            {
                matchedBetViewModel.BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts();
                return View("MultipleMatchedBetForm", matchedBetViewModel);
            }

            var matchedBet = _matchedBetModelController.CreateNewMultipleMatchedBet(matchedBetViewModel, matchedBetViewModel.MultipleBrokerAccountId);
            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult Delete(int id)
        {            
            _matchedBetModelController.DeleteMatchedBet(id);
            return RedirectToAction("Index");
        }

        public ActionResult NewSingleFormMultiple(int id)
        {            
            var sportEvent =  _matchedBetModelController.GetNextUnmatchedEventInMultiple(id);

            var viewModel = new MultipleMatchedBetAddSingleFormViewModel
            {
                BetDate = DateTime.Now,
                MatchedBetId = id,
                SportEvent = sportEvent,
                SportEventId = sportEvent.Id,
                BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts(),
                IsLay = true
            };

            return View("AddSingleToMultiple", viewModel);
        }

        public ActionResult DeleteSingleFromMultiple(int id)
        {
            var matchedBet = _matchedBetModelController.DeleteLastMatchedSingleInMultiple(id);
            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult AddSingleToMultiple(MultipleMatchedBetAddSingleFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts();
                return View("AddSingleToMultiple", viewModel);
            }
            
            var matchedBet =_matchedBetModelController.AddSingleToMultipleMatchedBet(viewModel);
            return RedirectToAction("Details", matchedBet);
        }

        public ActionResult Details(int id)
        {
            var matchedBet = _matchedBetsRepository.LoadMatchedBet(id);

            if (matchedBet == null) return HttpNotFound();

            //Uso questa discriminante per capire di che tipo è la matchedBet... dovrei piuttosto memorizzare l'informazione nel modello
            return matchedBet.Bets.Any(b => b.BetType == BetType.MultipleBack)
                ? View("DetailsMultiple", matchedBet)
                : View(matchedBet);
        }

        public ActionResult OpenParseView()
        {
            var viewModel = new NinjaBetOddsmatcherViewModel()
            {
                Text = "",
                Controller = "MatchedBet",
                Action = "ParseNinjaBet"
            };

            return View("ParseNinjaBetOddsmatcherView", viewModel);
        }

        public ActionResult OpenMultipleParseView()
        {
            var viewModel = new NinjaBetOddsmatcherViewModel()
            {
                Text = "",
                Controller = "MatchedBet",
                Action = "ParseNinjaBetMultiple"
            };

            return View("ParseNinjaBetOddsmatcherView", viewModel);
        }

        public ActionResult ParseNinjaBet(string Text)
        {
            var viewModel = MatchedBetHandler.Parse(Text);

            viewModel.BetDate = DateTime.Now;
            viewModel.BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts()
                .Where(broker => broker.Active)
                .ToList();

            return View("SimpleMatchedBetForm", viewModel);
        }

        public ActionResult ParseNinjaBetMultiple(string Text)
        {
            var viewModel = MatchedBetHandler.ParseMultipleMatchedBet(Text);

            viewModel.MultipleBetDate = DateTime.Now;
            viewModel.BrokerAccounts = _matchedBetsRepository.LoadAllBrokerAccounts()
                .Where(broker => broker.Active)
                .ToList();

            return View("MultipleMatchedBetForm", viewModel);

        }
    }
}