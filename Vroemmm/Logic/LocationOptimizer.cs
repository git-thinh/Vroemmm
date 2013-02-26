using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vroemmm.Models;

namespace Vroemmm.Logic
{
    public class LocationOptimizer
    {
        /// <summary>
        /// Events with the same name, will get the same location.
        /// </summary>
        public IEnumerable<EventModel> OptimizeLocations(IEnumerable<EventModel> events)
        {
            var eventsWithOutLocation = events.Where(e => string.IsNullOrEmpty(e.Location));

            foreach (var evnt in eventsWithOutLocation)
            {
                var eventWithSameSummary = events.FirstOrDefault(e => e.Summary == evnt.Summary && !string.IsNullOrEmpty(e.Location));
                if (eventWithSameSummary != null)
                {
                    evnt.Location = eventWithSameSummary.Location;
                }
            }

            return events;
        }
    }
}
