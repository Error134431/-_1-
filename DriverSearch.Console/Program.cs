using System;
using System.Collections.Generic;
using System.Linq;
using DriverSearch.Core.Algorithms;
using DriverSearch.Core.Models;

namespace DriverSearch.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Создаем тестовых водителей
            var drivers = new List<Driver>
            {
                new Driver(1, 10, 10),
                new Driver(2, 20, 20),
                new Driver(3, 5, 5),
                new Driver(4, 100, 100),
                new Driver(5, 15, 15),
                new Driver(6, 30, 30),
                new Driver(7, 1, 1)
            };

            var order = new OrderLocation(0, 0);

            // ========== АЛГОРИТМ 1: BRUTE FORCE ==========
            Console.WriteLine("=== Алгоритм 1: Полный перебор (Brute Force) ===\n");

            var bruteForce = new BruteForceAlgorithm();
            Console.WriteLine($"Заказ в точке: (0, 0)");
            Console.WriteLine($"Всего водителей: {drivers.Count}\n");

            var results1 = bruteForce.FindNearestDrivers(drivers, order, 5);

            Console.WriteLine("5 ближайших водителей:");
            Console.WriteLine("ID\t(X,Y)\t\tРасстояние");
            Console.WriteLine("--------------------------------");

            foreach (var result in results1)
            {
                Console.WriteLine($"{result.Driver.Id}\t({result.Driver.X}, {result.Driver.Y})\t{result.Distance:F2}");
            }

            // ========== АЛГОРИТМ 2: SPATIAL GRID ==========
            Console.WriteLine("\n\n=== Алгоритм 2: Пространственная сетка (Spatial Grid) ===\n");

            var spatialGrid = new SpatialGridAlgorithm(cellSize: 10);
            Console.WriteLine($"Размер ячейки: 10x10");

            var results2 = spatialGrid.FindNearestDrivers(drivers, order, 5);

            Console.WriteLine("5 ближайших водителей:");
            Console.WriteLine("ID\t(X,Y)\t\tРасстояние");
            Console.WriteLine("--------------------------------");

            foreach (var result in results2)
            {
                Console.WriteLine($"{result.Driver.Id}\t({result.Driver.X}, {result.Driver.Y})\t{result.Distance:F2}");
            }

            // ========== АЛГОРИТМ 3: QUADTREE ==========
            Console.WriteLine("\n\n=== Алгоритм 3: QuadTree (Квадродерево) ===\n");

            var quadTree = new QuadTreeAlgorithm(200, 200);
            Console.WriteLine($"Размер карты: 200x200");
            Console.WriteLine($"Максимум водителей в узле: 4\n");

            var results3 = quadTree.FindNearestDrivers(drivers, order, 5);

            Console.WriteLine("5 ближайших водителей:");
            Console.WriteLine("ID\t(X,Y)\t\tРасстояние");
            Console.WriteLine("--------------------------------");

            foreach (var result in results3)
            {
                Console.WriteLine($"{result.Driver.Id}\t({result.Driver.X}, {result.Driver.Y})\t{result.Distance:F2}");
            }

            // ========== СРАВНЕНИЕ ВСЕХ ТРЕХ АЛГОРИТМОВ ==========
            Console.WriteLine("\n\n=== СРАВНЕНИЕ АЛГОРИТМОВ ===");
            Console.WriteLine("==========================================");

            Console.WriteLine($"\nBrute Force:     {string.Join(", ", results1.Select(r => r.Driver.Id))}");
            Console.WriteLine($"Spatial Grid:    {string.Join(", ", results2.Select(r => r.Driver.Id))}");
            Console.WriteLine($"QuadTree:        {string.Join(", ", results3.Select(r => r.Driver.Id))}");

            bool same12 = results1.Select(r => r.Driver.Id).SequenceEqual(results2.Select(r => r.Driver.Id));
            bool same13 = results1.Select(r => r.Driver.Id).SequenceEqual(results3.Select(r => r.Driver.Id));
            bool same23 = results2.Select(r => r.Driver.Id).SequenceEqual(results3.Select(r => r.Driver.Id));

            Console.WriteLine("\n=== РЕЗУЛЬТАТЫ СОВПАДАЮТ? ===");
            Console.WriteLine($"Brute Force vs Spatial Grid: {same12}");
            Console.WriteLine($"Brute Force vs QuadTree:     {same13}");
            Console.WriteLine($"Spatial Grid vs QuadTree:    {same23}");

            Console.ReadKey();
        }
    }
}