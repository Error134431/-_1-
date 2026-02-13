using System.Collections.Generic;
using DriverSearch.Core.Models;

namespace DriverSearch.Core.Algorithms
{
    public interface IDriverSearchAlgorithm
    {
        string AlgorithmName { get; }
        List<SearchResult> FindNearestDrivers(
            IEnumerable<Driver> drivers,
            OrderLocation order,
            int count = 5);
    }
}