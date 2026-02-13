using System;
using System.Collections.Generic;
using System.Linq;
using DriverSearch.Core.Models;

namespace DriverSearch.Core.Algorithms
{
    public class QuadTreeAlgorithm : IDriverSearchAlgorithm
    {
        public string AlgorithmName => "QuadTree (Квадродерево)";

        private readonly QuadTree _quadTree;
        private readonly int _width;
        private readonly int _height;

        public QuadTreeAlgorithm(int width, int height)
        {
            _width = width;
            _height = height;
            _quadTree = new QuadTree(0, 0, width, height);
        }

        public List<SearchResult> FindNearestDrivers(
            IEnumerable<Driver> drivers,
            OrderLocation order,
            int count = 5)
        {
            // Очищаем дерево
            _quadTree.Clear();

            // Вставляем всех водителей в дерево
            foreach (var driver in drivers)
            {
                _quadTree.Insert(driver);
            }

            // Ищем ближайших водителей
            var candidates = _quadTree.FindNearest(order.X, order.Y, count * 3);

            // Вычисляем расстояние и сортируем
            var results = new List<SearchResult>();
            foreach (var driver in candidates)
            {
                double distance = Math.Sqrt(
                    Math.Pow(driver.X - order.X, 2) +
                    Math.Pow(driver.Y - order.Y, 2));
                results.Add(new SearchResult(driver, distance));
            }

            // Сортируем по расстоянию и берем нужное количество
            return results
                .OrderBy(r => r.Distance)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Внутренняя реализация QuadTree
        /// </summary>
        private class QuadTree
        {
            private readonly int _x, _y, _width, _height;
            private readonly List<Driver> _drivers;
            private QuadTree _northWest;
            private QuadTree _northEast;
            private QuadTree _southWest;
            private QuadTree _southEast;
            private bool _divided;
            private const int MAX_DRIVERS = 4; // Максимум водителей в узле

            public QuadTree(int x, int y, int width, int height)
            {
                _x = x;
                _y = y;
                _width = width;
                _height = height;
                _drivers = new List<Driver>();
                _divided = false;
            }

            /// <summary>
            /// Вставка водителя в дерево
            /// </summary>
            public void Insert(Driver driver)
            {
                // Если точка не в этом узле - выходим
                if (!Contains(driver.X, driver.Y))
                    return;

                // Если есть место и узел не разделен
                if (_drivers.Count < MAX_DRIVERS && !_divided)
                {
                    _drivers.Add(driver);
                }
                else
                {
                    // Если еще не разделен - разделяем
                    if (!_divided)
                        Subdivide();

                    // Вставляем в дочерние узлы
                    _northWest?.Insert(driver);
                    _northEast?.Insert(driver);
                    _southWest?.Insert(driver);
                    _southEast?.Insert(driver);
                }
            }

            /// <summary>
            /// Поиск ближайших водителей
            /// </summary>
            public List<Driver> FindNearest(int x, int y, int maxResults)
            {
                var result = new List<Driver>();
                FindNearestRecursive(x, y, result, maxResults);
                return result;
            }

            private void FindNearestRecursive(int x, int y, List<Driver> result, int maxResults)
            {
                // Проверяем, может ли этот узел содержать ближайших
                if (!IsPotentiallyCloser(x, y, result))
                    return;

                // Добавляем водителей из текущего узла
                foreach (var driver in _drivers)
                {
                    result.Add(driver);
                }

                // Если узел разделен - проверяем дочерние узлы
                if (_divided)
                {
                    // Сортируем квадранты по расстоянию до точки поиска
                    var quadrants = new[]
                    {
                        (Quadrant: _northWest, Distance: _northWest?.DistanceToPoint(x, y) ?? double.MaxValue),
                        (Quadrant: _northEast, Distance: _northEast?.DistanceToPoint(x, y) ?? double.MaxValue),
                        (Quadrant: _southWest, Distance: _southWest?.DistanceToPoint(x, y) ?? double.MaxValue),
                        (Quadrant: _southEast, Distance: _southEast?.DistanceToPoint(x, y) ?? double.MaxValue)
                    };

                    // Сначала ищем в ближайших квадрантах
                    foreach (var quadrant in quadrants.OrderBy(q => q.Distance))
                    {
                        if (quadrant.Quadrant != null)
                        {
                            quadrant.Quadrant.FindNearestRecursive(x, y, result, maxResults);
                        }
                    }
                }
            }

            /// <summary>
            /// Проверяет, может ли узел содержать точки ближе, чем текущие найденные
            /// </summary>
            private bool IsPotentiallyCloser(int x, int y, List<Driver> currentResults)
            {
                if (currentResults.Count == 0)
                    return true;

                // Находим максимальное расстояние среди текущих результатов
                double maxDistance = double.MaxValue;

                // Вычисляем минимальное расстояние от точки до прямоугольника узла
                double minDistance = DistanceToPoint(x, y);

                return minDistance < maxDistance;
            }

            /// <summary>
            /// Расстояние от точки до ближайшей точки прямоугольника
            /// </summary>
            public double DistanceToPoint(int px, int py)
            {
                int dx = 0;
                int dy = 0;

                if (px < _x)
                    dx = _x - px;
                else if (px > _x + _width)
                    dx = px - (_x + _width);

                if (py < _y)
                    dy = _y - py;
                else if (py > _y + _height)
                    dy = py - (_y + _height);

                return Math.Sqrt(dx * dx + dy * dy);
            }

            /// <summary>
            /// Проверяет, содержит ли узел точку
            /// </summary>
            private bool Contains(int px, int py)
            {
                return px >= _x && px < _x + _width &&
                       py >= _y && py < _y + _height;
            }

            /// <summary>
            /// Разделяет узел на 4 квадранта
            /// </summary>
            private void Subdivide()
            {
                int halfWidth = _width / 2;
                int halfHeight = _height / 2;

                _northWest = new QuadTree(_x, _y, halfWidth, halfHeight);
                _northEast = new QuadTree(_x + halfWidth, _y, halfWidth, halfHeight);
                _southWest = new QuadTree(_x, _y + halfHeight, halfWidth, halfHeight);
                _southEast = new QuadTree(_x + halfWidth, _y + halfHeight, halfWidth, halfHeight);

                _divided = true;

                // Перемещаем существующих водителей в дочерние узлы
                foreach (var driver in _drivers)
                {
                    _northWest.Insert(driver);
                    _northEast.Insert(driver);
                    _southWest.Insert(driver);
                    _southEast.Insert(driver);
                }

                // Очищаем текущий узел
                _drivers.Clear();
            }

            /// <summary>
            /// Очищает дерево
            /// </summary>
            public void Clear()
            {
                _drivers.Clear();
                _divided = false;
                _northWest = null;
                _northEast = null;
                _southWest = null;
                _southEast = null;
            }
        }
    }
}