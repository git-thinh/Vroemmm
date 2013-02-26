using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vroemmm.Logic;
using Vroemmm.Models;
using Vroemmm.GoogleAuth;

namespace Vroemmm.Controllers
{
    public partial class TravelController : Controller
    {
        private readonly IDistanceCalculator distanceCalculator;
        private readonly RouteCalculator routeCalculator;
        public TravelController(IDistanceCalculator distanceCalculator)
        {
            this.distanceCalculator = distanceCalculator;
            this.routeCalculator = new RouteCalculator(distanceCalculator);
        }

        public virtual ActionResult Index(string calendarId, int year, int month)
        {
            try
            {
                var calendarService = this.GetAgendaService();
                var importer = new AgendaImporter(calendarService);
                var events = importer.GetEventsModel(calendarId);

                var locationOptimizer = new LocationOptimizer();
                events = locationOptimizer.OptimizeLocations(events);

                var start = new DateTime(year, month, 1);
                var end = start.AddMonths(1);
                var eventsOfSelectedMonth = events.Where(e => e.Start >= start && e.End <= end);

                var routes = routeCalculator.CalculateRoutes(eventsOfSelectedMonth, "Van koetsveldstraat 39, Utrecht");
                var model = routeCalculator.CalculateTravelCosts(routes);

                return View(model);
            }
            catch (NoAuthenticatorException)
            {
                return RedirectToRoute("GoogleAuthentication", new { callbackurl = Request.Url });
            }


        }
    }
}
