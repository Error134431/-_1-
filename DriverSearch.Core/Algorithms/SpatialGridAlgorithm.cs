using System;
using System.Collections.Generic;
using System.Linq;
using DriverSearch.Core.Models;

namespace DriverSearch.Core.Algorithms
{
    public class SpatialGridAlgorithm : IDriverSearchAlgorithm
    {
        public string AlgorithmName => "Spatial Grid (Пространственная сетка)";

        private readonly int _cellSize;

        public SpatialGridAlgorithm(int cellSize = 10)
        {
            _cellSize = cellSize;
        }

        public List<SearchResult> FindNearestDrivers(
            IEnumerable<Driver> drivers,
            OrderLocation order,
            int count = 5)
        {
            // Группируем по ячейкам
            var grid = new Dictionary<(int, int), List<Driver>>();

            foreach (var driver in drivers)
            {
                int cellX = driver.X / _cellSize;
                int cellY = driver.Y / _cellSize;

                if (!grid.ContainsKey((cellX, cellY)))
                    grid[(cellX, cellY)] = new List<Driver>();

                grid[(cellX, cellY)].Add(driver);
            }

            int orderCellX = order.X / _cellSize;
            int orderCellY = order.Y / _cellSize;

            var nearest = new List<SearchResult>();
            int radius = 0;

            while (nearest.Count < count && radius < 10)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        var cell = (orderCellX + dx, orderCellY + dy);
                        if (grid.ContainsKey(cell))
                        {
                            foreach (var driver in grid[cell])
                            {
                                double distance = Math.Sqrt(
                                    Math.Pow(driver.X - order.X, 2) +
                                    Math.Pow(driver.Y - order.Y, 2));
                                nearest.Add(new SearchResult(driver, distance));
                            }
                        }
                    }
                }

                nearest = nearest
                    .GroupBy(r => r.Driver.Id)
                    .Select(g => g.First())
                    .OrderBy(r => r.Distance)
                    .Take(count)
                    .ToList();

                radius++;
            }

            return nearest.Take(count).ToList();
        }
    }
}