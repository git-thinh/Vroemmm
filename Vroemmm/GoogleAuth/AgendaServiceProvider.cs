using Google.Apis.Authentication;
using Google.Apis.Calendar.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vroemmm.GoogleAuth
{
    public static class AgendaServiceProvider
    {
        public static CalendarService GetAgendaService(this Controller controller)
        {
            var calenderService = controller.Session["calenderService"];

            if (calenderService != null) return (CalendarService)calenderService;
            
            var authenticator = controller.Session["authenticator"];
            if (authenticator == null)
            {
                throw new NoAuthenticatorException();
            }
            
            calenderService = new CalendarService((IAuthenticator)authenticator);

            controller.Session["calenderService"] = calenderService;

            return (CalendarService)calenderService;
        }        
    }
}