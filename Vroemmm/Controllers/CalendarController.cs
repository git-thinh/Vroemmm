using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vroemmm.Models;
using Vroemmm.Logic;
using Google.Apis.Authentication;
using Vroemmm.GoogleAuth;
using Google.Apis.Calendar.v3;

namespace Vroemmm.Controllers
{
    public partial class CalendarController : Controller
    {
        public virtual ActionResult SelectCalendar()
        {
            try
            {
                var calendarService = this.GetAgendaService();
                var importer = new AgendaImporter(calendarService);
                var model = importer.GetCalendarsModel();
                if (model.Count() == 1)
                {
                    // only one agenda, so select it by default and move on the other 
                }
                return View(model);
            }
            catch (NoAuthenticatorException)
            {
                return RedirectToRoute("GoogleAuthentication", new { callbackurl = Request.Url });
            }
        }

        public virtual ActionResult SelectEvents(string calendarId)
        {
            var model = new SelectEventsModel()
            {
                CalendarId = calendarId
            };
            return View(model);
        }

        public virtual ActionResult ShowEvents(string calendarId)
        {
            try
            {
                var calendarService = this.GetAgendaService();
                var importer = new AgendaImporter(calendarService);
                var model = importer.GetEventsModel(calendarId);
                Session["eventModel"] = model;
                return View(model);
            }
            catch (NoAuthenticatorException)
            {
                return RedirectToRoute("GoogleAuthentication", new { callbackurl = Request.Url });
            }
        }
    }
}
