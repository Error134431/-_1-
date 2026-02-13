using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using DriverSearch.Core.Algorithms;
using DriverSearch.Core.Models;

namespace DriverSearch.Tests
{
    [TestFixture]
    public class AlgorithmTests
    {
        private List<Driver> _drivers;
        private OrderLocation _order;

        [SetUp]
        public void Setup()
        {
            _drivers = new List<Driver>
            {
                new Driver(1, 10, 10),
                new Driver(2, 20, 20),
                new Driver(3, 5, 5),
                new Driver(4, 100, 100),
                new Driver(5, 15, 15),
                new Driver(6, 30, 30),
                new Driver(7, 1, 1)
            };

            _order = new OrderLocation(0, 0);
        }

        [Test]
        public void BruteForceAlgorithm_ReturnsCorrectNearestDrivers()
        {
            var algorithm = new BruteForceAlgorithm();
            var results = algorithm.FindNearestDrivers(_drivers, _order, 3);

            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(7, results[0].Driver.Id);
            Assert.AreEqual(3, results[1].Driver.Id);
            Assert.AreEqual(1, results[2].Driver.Id);
        }

        [Test]
        public void SpatialGridAlgorithm_ReturnsSameAsBruteForce()
        {
            var bruteForce = new BruteForceAlgorithm();
            var spatialGrid = new SpatialGridAlgorithm(10);

            var expected = bruteForce.FindNearestDrivers(_drivers, _order, 5);
            var actual = spatialGrid.FindNearestDrivers(_drivers, _order, 5);

            Assert.AreEqual(expected.Count, actual.Count);

            var expectedIds = expected.Select(r => r.Driver.Id).OrderBy(id => id).ToList();
            var actualIds = actual.Select(r => r.Driver.Id).OrderBy(id => id).ToList();

            Assert.AreEqual(expectedIds, actualIds);
        }

        [Test]
        public void QuadTreeAlgorithm_ReturnsSameAsBruteForce()
        {
            var bruteForce = new BruteForceAlgorithm();
            var quadTree = new QuadTreeAlgorithm(200, 200);

            var expected = bruteForce.FindNearestDrivers(_drivers, _order, 5);
            var actual = quadTree.FindNearestDrivers(_drivers, _order, 5);

            Assert.AreEqual(expected.Count, actual.Count);

            var expectedIds = expected.Select(r => r.Driver.Id).OrderBy(id => id).ToList();
            var actualIds = actual.Select(r => r.Driver.Id).OrderBy(id => id).ToList();

            Assert.AreEqual(expectedIds, actualIds);
        }
    }
}