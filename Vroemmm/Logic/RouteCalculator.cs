using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vroemmm.Models;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Vroemmm.GoogleApiHelpers;

namespace Vroemmm.Logic
{
    public class RouteCalculator
    {
        private readonly IDistanceCalculator distanceCalculator;

        public RouteCalculator(IDistanceCalculator distanceCalculator)
        {
            this.distanceCalculator = distanceCalculator;
        }

        public IEnumerable<RouteModel> CalculateRoutes(IEnumerable<EventModel> events, string homeAddress)
        {
            events = FilterEvents(events);
            
            var routes = new List<RouteModel>();
            // create routes from events
            // if an event has a previous event within 3 hours, use this as origin
            for (int i = 0; i < events.Count(); i++)
            {
                var currentEvent = events.ElementAt(i);

                // check for previous event
                var previousEvent = events.ElementAtOrDefault(i-1);
                var previousEventIsWithin3Hours = previousEvent == null ? false : previousEvent.End >= currentEvent.Start.AddHours(-3);
                if (previousEventIsWithin3Hours)
                {
                    // from previous location to this location
                    routes.Add(new RouteModel()
                    {
                        Date = currentEvent.Start,
                        Origin = previousEvent.Location,
                        Destination = currentEvent.Location,
                        OriginEvent = previousEvent,
                        DestinationEvent = currentEvent
                    });
                }
                else
                {
                    // from home to this location
                    routes.Add(new RouteModel()
                    {
                        Date = currentEvent.Start,
                        Origin = homeAddress,
                        Destination = currentEvent.Location,
                        DestinationEvent = currentEvent
                    });
                }

                var nextEvent = events.ElementAtOrDefault(i+1);
                var nextEventIsWithin3Hours = nextEvent == null ? false : nextEvent.Start <= currentEvent.End.AddHours(3);
                if (nextEventIsWithin3Hours)
                {
                    // already added 
                }
                else
                {
                    // from this location to home
                    routes.Add(new RouteModel()
                    {
                        Date = currentEvent.End,
                        Origin = currentEvent.Location,
                        Destination = homeAddress,
                        OriginEvent = currentEvent
                    });
                }             
            }

            foreach (var route in routes)
            {
                var distanceInMeters = distanceCalculator.CalculateDistance(route.Origin, route.Destination);
                route.Distance = Math.Round(((decimal)distanceInMeters / 1000), 2);  
            }
            
            return routes;
        }

        public IEnumerable<EventModel> FilterEvents(IEnumerable<EventModel> events)
        {
            return events.Where(e => !string.IsNullOrEmpty(e.Location)).OrderBy(e => e.Start);
        }

        public TravelCostsModel CalculateTravelCosts(IEnumerable<RouteModel> routes)
        {

            var model = new TravelCostsModel();
            model.Routes = FormatRoutes(routes);
            model.TotalDistance = routes.Sum(r => r.Distance);
            model.CostsPerKilometer = 0.19M;
            model.TotalCosts = model.TotalDistance * model.CostsPerKilometer;
            return model;
        }

        public IEnumerable<RouteModel> FormatRoutes(IEnumerable<RouteModel> routes)
        {
            foreach (var route in routes)
            {
                var eventSummary = route.OriginEvent != null ? route.OriginEvent.Summary : "Thuis";
                route.Origin = string.Format("{0} ({1})", route.Origin, eventSummary);

                eventSummary = route.DestinationEvent != null ? route.DestinationEvent.Summary : "Thuis";
                route.Destination = string.Format("{0} ({1})", route.Destination, eventSummary);
            }
            return routes;
        }
    }
}