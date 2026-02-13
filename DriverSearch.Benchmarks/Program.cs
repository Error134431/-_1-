using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Order;
using DriverSearch.Core.Algorithms;
using DriverSearch.Core.Models;

namespace DriverSearch.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class DriverSearchBenchmark
    {
        private List<Driver> _drivers;
        private OrderLocation _order;
        private BruteForceAlgorithm _bruteForce;
        private SpatialGridAlgorithm _spatialGrid;
        private QuadTreeAlgorithm _quadTree;

        [Params(100, 1000, 10000)]
        public int DriverCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            _drivers = new List<Driver>();

            for (int i = 1; i <= DriverCount; i++)
            {
                _drivers.Add(new Driver(i, random.Next(0, 1000), random.Next(0, 1000)));
            }

            _order = new OrderLocation(500, 500);
            _bruteForce = new BruteForceAlgorithm();
            _spatialGrid = new SpatialGridAlgorithm(20);
            _quadTree = new QuadTreeAlgorithm(1000, 1000);
        }

        [Benchmark(Description = "Brute Force")]
        public List<SearchResult> BruteForceSearch()
        {
            return _bruteForce.FindNearestDrivers(_drivers, _order, 5);
        }

        [Benchmark(Description = "Spatial Grid")]
        public List<SearchResult> SpatialGridSearch()
        {
            return _spatialGrid.FindNearestDrivers(_drivers, _order, 5);
        }

        [Benchmark(Description = "QuadTree")]
        public List<SearchResult> QuadTreeSearch()
        {
            return _quadTree.FindNearestDrivers(_drivers, _order, 5);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DriverSearchBenchmark>();
        }
    }
}