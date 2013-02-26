using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vroemmm.Logic;
using Moq;
using Vroemmm.Models;
using System.Collections.Generic;
using System.Linq;

namespace Vroemmm.Tests
{
    [TestClass]
    public class RouteCalculatorTests
    {
        private readonly IDistanceCalculator distanceCalculator;
        private readonly RouteCalculator routeCalculator;
        private string homeAddress = "Van koetsveldstraat 39, Utrecht";

        public RouteCalculatorTests()
        {
            var distanceCalculatorMock = new Mock<IDistanceCalculator>();
            distanceCalculatorMock.Setup(d => d.CalculateDistance(It.IsAny<string>(), It.IsAny<string>())).Returns(10);
            this.distanceCalculator = distanceCalculatorMock.Object;
            this.routeCalculator = new RouteCalculator(distanceCalculator);
        }

        [TestMethod]
        public void FilterEvents_removes_events_without_location()
        {
        }

        [TestMethod]
        public void FilterEvents_orders_events_on_start_date()
        {
            
        }

        [TestMethod]
        public void CalculateRoutes_uses_origin_from_previous_event_when_within_3_hours()
        {
            var events = GetEvents();

            var routes = routeCalculator.CalculateRoutes(events, homeAddress);

            // day two
            Assert.AreEqual(homeAddress, routes.ElementAt(2).Origin);
            Assert.AreEqual(events.ElementAt(1).Location, routes.ElementAt(2).Destination);
            Assert.AreEqual(events.ElementAt(1).Location, routes.ElementAt(3).Origin);
            Assert.AreEqual(events.ElementAt(2).Location, routes.ElementAt(3).Destination);
            Assert.AreEqual(events.ElementAt(2).Location, routes.ElementAt(4).Origin);
            Assert.AreEqual(homeAddress, routes.ElementAt(4).Destination);
        }

        [TestMethod]
        public void CalculateRoutes_returns_routes_to_destination_and_return_routes_to_home_address()
        {
            var events = GetEvents();

            var routes = routeCalculator.CalculateRoutes(events, homeAddress);

            Assert.AreEqual(homeAddress, routes.ElementAt(0).Origin);
            Assert.AreEqual(events.ElementAt(0).Location, routes.ElementAt(0).Destination);
            Assert.AreEqual(events.ElementAt(0).Location, routes.ElementAt(1).Origin);
            Assert.AreEqual(homeAddress, routes.ElementAt(1).Destination);
        }

        private List<EventModel> GetEvents()
        {
            var events = new List<EventModel>();

            var date = new DateTime(2013, 02, 01);

            events.Add(new EventModel()
            {
                Start = date,
                End = date.AddHours(4),
                Location = "Koekoeksplein 2a, Utrecht",
                Summary = "Koekoek"
            });

            events.Add(new EventModel()
            {
                Start = date.AddDays(1),
                End = date.AddDays(1).AddHours(3),
                Location = "Botter 40-75, Lelystad",
                Summary = "Triangel"
            });

            events.Add(new EventModel()
            {
                Start = date.AddDays(1).AddHours(4),
                End =  date.AddDays(1).AddHours(8),
                Location = "Buizerdweg 4, Lelystad",
                Summary = "Tjotter buiten"
            });
            return events;
        }



        
    }
}
