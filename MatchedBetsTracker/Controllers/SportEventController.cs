using MatchedBetsTracker.BusinessLogic;
using MatchedBetsTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatchedBetsTracker.Controllers
{
    public class SportEventController : Controller
    {
        private ApplicationDbContext _context;
        private IMatchedBetModelController _matchedBetController;

        public SportEventController(IMatchedBetModelController matchedBetController)
        {
            _context = new ApplicationDbContext();
            this._matchedBetController = matchedBetController;
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: SportEvent
        public ActionResult Index()
        {
            var model = _context.SportEvents.ToList();

            return View(model);
        }

        public ActionResult ChangeStatus(int id, bool? newState)
        {
            //Mettere la logica, ma prima un bel javascript che chiede conferma
            //Sistemare anche la logica di cancellazione che ha dei problemi (non vengono cancellate BetEvents (se non in cascade?) e le SportEvents
            //Fare pulizia delle SportEvents e BetEvents sopravvissute in precedenza
            //Inserire la parte di javascript di gesione della tabella
            //per ogni sportevent fare vedere le bet aperte (BetEvent e Bet ?) con ammontare e accredito nel caso l'evento si verifichi o no
            //La pagina sportevents è corretto cge le mostri tutte, così come la pagina MAtchedBets (così in questo mdoo vediamo le informazioni dai due estremi)
            //... però poi prevederei un'ulteriore pagina che fa vedere le cose aperte: sportEvent da assegnare, matchedEvent (multiple) da coprire e transazioni da validare. Tutte in una stessa pagina

            return Content("WILL CHANGE STATUS TO " + newState + " FOR EVENT " + id);
            //_matchedBetController.SetHappenStatusOnEvent(id, newState);
        }
    }
}