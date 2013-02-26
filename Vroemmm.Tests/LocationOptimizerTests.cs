using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vroemmm.Logic;
using Vroemmm.Models;
using System.Collections.Generic;
using System.Linq;

namespace Vroemmm.Tests
{
    [TestClass]
    public class LocationOptimizerTests
    {
        [TestMethod]
        public void Optimizer_copies_location_if_summary_is_equal()
        {
            var events = GetEvents();

            var optimizer = new LocationOptimizer();
            var optimizedEvents = optimizer.OptimizeLocations(events);

            Assert.AreEqual(events.First().Location, optimizedEvents.ElementAt(1).Location);
            Assert.AreEqual("", optimizedEvents.ElementAt(2).Location);
            Assert.AreEqual(events.Count, optimizedEvents.Count());
        }

        private List<EventModel> GetEvents()
        {
            var events = new List<EventModel>();
            events.Add(new EventModel() { Location = "Kerkstraat 1, Lelystad", Summary = "Koekoek" });
            events.Add(new EventModel() { Location = "", Summary = "Koekoek" });
            events.Add(new EventModel() { Location = "", Summary = "Onderzoek bradly bij Koekoek" });
            return events;
        }
    }
}
