using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vroemmm.GoogleApiHelpers;

namespace Vroemmm.Tests
{
    [TestClass]
    public class DistanceProviderTests
    {
        [TestMethod]
        public void CalculateDistance_returns_distance_in_meters()
        {
            var origin = "Van koetsveldstraat 39, Utrecht";
            var destination = "Kerkbuurt 39, Wijdenes";

            var provider = new GoogleMapsDistanceCalculator();
            var distance = provider.CalculateDistance(origin, destination);
            Assert.AreEqual(90773, distance);
        }

        [TestMethod]
        public void CalculateDistance_accepts_address_without_comma()
        {
            var origin = "Van koetsveldstraat 39 Utrecht";
            var destination = "Kerkbuurt 39, Wijdenes";

            var provider = new GoogleMapsDistanceCalculator();
            var distance = provider.CalculateDistance(origin, destination);
            Assert.AreEqual(90773, distance);
        }

        [TestMethod]
        public void CalculateDistance_returns_city_distance_when_street_cannot_be_found()
        {
            var origin = "Van koetsveldstrasdfasfdaat 39 Utrecht";
            var destination = "Kerkbuurt 39, Wijdenes";

            var provider = new GoogleMapsDistanceCalculator();
            var distance = provider.CalculateDistance(origin, destination);
            Assert.AreEqual(91914, distance);
        }

        [TestMethod]
        public void CalculateDistance_returns_0_when_address_cannot_be_found()
        {
            var origin = "asdfasdfasdfa";
            var destination = "Kerkbuurt 39, Wijdenes";

            var provider = new GoogleMapsDistanceCalculator();
            var distance = provider.CalculateDistance(origin, destination);
            Assert.AreEqual(0, distance);
        }
    }
}
