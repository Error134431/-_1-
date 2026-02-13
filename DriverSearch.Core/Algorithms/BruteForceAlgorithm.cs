using System;
using System.Collections.Generic;
using System.Linq;
using DriverSearch.Core.Models;

namespace DriverSearch.Core.Algorithms
{
    public class BruteForceAlgorithm : IDriverSearchAlgorithm
    {
        public string AlgorithmName => "Brute Force (Полный перебор)";

        public List<SearchResult> FindNearestDrivers(
            IEnumerable<Driver> drivers,
            OrderLocation order,
            int count = 5)
        {
            return drivers
                .Select(d => new SearchResult(d, CalculateDistance(d, order)))
                .OrderBy(r => r.Distance)
                .Take(count)
                .ToList();
        }

        private double CalculateDistance(Driver driver, OrderLocation order)
        {
            return Math.Sqrt(Math.Pow(driver.X - order.X, 2) + Math.Pow(driver.Y - order.Y, 2));
        }
    }
}