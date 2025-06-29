using System;
using System.Collections.Generic;
using UnityEngine;

namespace N.GridSystems
{
    public static class GridSystemExtensions
    {
        public enum GridSearchMode { FourDir, EightDir }

        private static readonly Vector2Int[] FourDirections = {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };

        private static readonly Vector2Int[] EightDirections = {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0),
            new Vector2Int(1, 1), new Vector2Int(1, -1),
            new Vector2Int(-1, -1), new Vector2Int(-1, 1)
        };


        /// <summary>
        /// BFS search to find all connected tiles from a starting point that satisfy the condition.
        /// </summary>
        public static List<Vector2Int> BFS<T>(this GridSystem<T> grid, Vector2Int start, Predicate<T> condition, GridSearchMode mode = GridSearchMode.FourDir)
        {
            var result = new List<Vector2Int>();
            if (!grid.IsInsideGridBounds(start) || !condition(grid.GetTileValue(start))) return result;

            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var directions = mode == GridSearchMode.FourDir ? FourDirections : EightDirections;

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                foreach (var dir in directions)
                {
                    var next = current + dir;
                    if (visited.Contains(next)) continue;
                    if (!grid.IsInsideGridBounds(next)) continue;
                    if (!condition(grid.GetTileValue(next))) continue;

                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }

            return result;
        }

        /// <summary>
        /// DFS search to find all connected tiles from a starting point that satisfy the condition.
        /// </summary>
        public static List<Vector2Int> DFS<T>(this GridSystem<T> grid, Vector2Int start, Predicate<T> condition, GridSearchMode mode = GridSearchMode.FourDir)
        {
            var result = new List<Vector2Int>();
            if (!grid.IsInsideGridBounds(start) || !condition(grid.GetTileValue(start))) return result;

            var visited = new HashSet<Vector2Int>();
            var stack = new Stack<Vector2Int>();
            var directions = mode == GridSearchMode.FourDir ? FourDirections : EightDirections;

            stack.Push(start);
            visited.Add(start);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                result.Add(current);

                foreach (var dir in directions)
                {
                    var next = current + dir;
                    if (visited.Contains(next)) continue;
                    if (!grid.IsInsideGridBounds(next)) continue;
                    if (!condition(grid.GetTileValue(next))) continue;

                    stack.Push(next);
                    visited.Add(next);
                }
            }

            return result;
        }

        public static Vector3 GetCenterWorldPositionOfListTiles<T>(this GridSystem<T> grid, List<Vector2Int> vector2s)
        {
            if (vector2s == null || vector2s.Count == 0) return Vector2.zero;

            Vector3 sum = Vector3.zero;
            foreach (var vector in vector2s)
            {
                sum += grid.GetCenterWorldPositionOfTile(vector.x, vector.y);
            }
            return sum / vector2s.Count;
        }
    }
}
