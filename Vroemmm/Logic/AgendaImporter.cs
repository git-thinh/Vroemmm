using Google.Apis.Calendar.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vroemmm.Models;

namespace Vroemmm.Logic
{
    public class AgendaImporter
    {
        private readonly CalendarService calendarService;
        public AgendaImporter(CalendarService calendarService)
        {
            this.calendarService = calendarService;
        }

        public IEnumerable<CalendarModel> GetCalendarsModel()
        {
            var calendars = calendarService.CalendarList.List().Fetch();
            var model = new List<CalendarModel>();
            foreach (var calendar in calendars.Items)
            {
                model.Add(new CalendarModel()
                {
                    Name = calendar.Summary,
                    Id = calendar.Id
                });
            };
            return model;
        }

        public IEnumerable<EventModel> GetEventsModel(string calendarId)
        {
            var model = new List<EventModel>();

            var query = calendarService.Events.List(calendarId);
            query.MaxResults = 100000;
            if (query == null) return model;

            var events = query.Fetch().Items;
            var selectedEvents = events.Where(e => e.StartDateTime().HasValue && e.EndDateTime().HasValue);
            selectedEvents = selectedEvents.OrderBy(e => e.StartDateTime());
                        
            foreach (var evnt in selectedEvents)
            {
                model.Add(new EventModel()
                {                    
                    End = evnt.EndDateTime().Value,
                    Start = evnt.StartDateTime().Value,
                    Location = evnt.Location,
                    Summary = evnt.Summary
                });
            }
            return model;
        }
    }
}