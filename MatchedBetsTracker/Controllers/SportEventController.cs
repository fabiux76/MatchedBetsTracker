using System.Collections.Generic;
using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace MatchedBetsTracker.Controllers
{
    public class SportEventController : Controller
    {
        private ApplicationDbContext _context;
        private IMatchedBetModelController _matchedBetController;

        public SportEventController(IMatchedBetModelController matchedBetController,
                                    ApplicationDbContext context)
        {
            _context = context;
            this._matchedBetController = matchedBetController;
        }

        // GET: SportEvent
        public ActionResult Index(bool showClosed = false)
        {
            return View(PrepareIndexModel(showClosed));
        }

        public ActionResult ChangeStatus(int id, bool? newState)
        {
            //Fare pulizia delle SportEvents e BetEvents sopravvissute in precedenza
            //per ogni sportevent fare vedere le bet aperte (BetEvent e Bet ?) con ammontare e accredito nel caso l'evento si verifichi o no
            //La pagina sportevents è corretto cge le mostri tutte, così come la pagina MAtchedBets (così in questo mdoo vediamo le informazioni dai due estremi)
            //... però poi prevederei un'ulteriore pagina che fa vedere le cose aperte: sportEvent da assegnare, matchedEvent (multiple) da coprire e transazioni da validare. Tutte in una stessa pagina

            //return Content("WILL CHANGE STATUS TO " + newState + " FOR EVENT " + id);
            var sportEvent = _matchedBetController.SetHappenStatusOnEvent(id, newState);

            //return View("Index", PrepareIndexModel());
            return RedirectToAction("Details", "MatchedBet", sportEvent.MatchedBet());
        }

        private SportEventsViewModel PrepareIndexModel(bool showClosed = false)
        {
            return new SportEventsViewModel
            {
                SportEvents =
                    _context.SportEvents
                        .Where(se => se.Happened == null || showClosed)
                        .Include(se => se.BetEvents)
                        .Include(se => se.BetEvents.Select(be => be.Bet))
                        .Include(se => se.BetEvents.Select(be => be.Bet.BrokerAccount))
                        .Include(se => se.BetEvents.Select(be => be.Bet.BrokerAccount))
                        .OrderByDescending(se => se.EventDate)
                        .ToList(),
                ShowClosed = showClosed
            };
        }
    }
}