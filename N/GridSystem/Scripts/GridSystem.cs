using System;
using UnityEngine;

namespace N.GridSystem
{
    /// <summary>
    /// Specifies the plane on which the grid lies.
    /// </summary>
    public enum PlaneType { XY, XZ, }
    public class GridSystem<T>
    {
        private readonly Vector2Int gridSize;
        private readonly float cellSize;
        private readonly float spacing;
        private readonly PlaneType planeType;
        private readonly Vector3 centerGrid;
        private readonly Vector3 originGrid;
        private readonly T[,] gridArray;
        private readonly CoordinatesConverter coordinatesConverter;

        internal Vector2Int GridSize => gridSize;
        internal float CellSize => cellSize;
        internal float Spacing => spacing;
        internal PlaneType GridPlane => planeType;
        internal Vector3 Center => centerGrid;
        internal Vector3 Origin => originGrid;


        /// <summary>
        /// Generates a grid system based on the specified grid size, cell size, grid type, and origin position.
        /// </summary>
        /// <param name="gridSize">Size of the grid.</param>
        /// <param name="cellSize">Size of each cell in the grid.</param>
        /// <param name="planeType">Type of the grid (XY_Plane or XZ_Plane).</param>
        /// <param name="origin">The origin position in the world.</param>
        /// <param name="drawGridLines">Whether to draw the grid lines for visualization.</param>
        /// <returns>A new instance of the GridSystem.</returns>
        internal static GridSystem<T> GenerateGrid(Vector2Int gridSize, float cellSize, float spacing, PlaneType planeType, Vector3 center, bool drawGridLines = false)
        {
            var _origin = CalculateOrigin(gridSize, cellSize, spacing, planeType, center);
            var _converter = planeType switch
            {
                PlaneType.XY => new XYConverter() as CoordinatesConverter,
                PlaneType.XZ => new XZConverter() as CoordinatesConverter,
                _ => throw new ArgumentException("Invalid GridType provided"),
            };

            var _gridSystem = new GridSystem<T>(gridSize, cellSize, spacing, planeType, center, _origin, _converter);

            if (drawGridLines)
                _gridSystem.DrawGridLines();

            return _gridSystem;
        }

        private static Vector3 CalculateOrigin(Vector2Int gridSize, float cellSize, float spacing, PlaneType gridType, Vector3 origin)
        {
            var _offset = cellSize + spacing;
            return gridType switch
            {
                PlaneType.XY => new Vector3(origin.x - (gridSize.x * _offset * 0.5f), origin.y - (gridSize.y * _offset * 0.5f), origin.z),
                PlaneType.XZ => new Vector3(origin.x - (gridSize.x * _offset * 0.5f), origin.y, origin.z - (gridSize.y * _offset * 0.5f)),
                _ => throw new ArgumentException("Invalid GridType provided"),
            };
        }

        private GridSystem(Vector2Int gridSize, float cellSize, float spacing, PlaneType planeType, Vector3 center, Vector3 origin, CoordinatesConverter converter)
        {
            this.gridSize = gridSize;
            this.cellSize = cellSize;
            this.spacing = spacing;
            this.planeType = planeType;
            this.centerGrid = center;
            this.originGrid = origin;
            this.coordinatesConverter = converter;
            this.gridArray = new T[gridSize.x, gridSize.y];
        }

        private void DrawGridLines()
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    this.coordinatesConverter.DrawGridCellOutline(x, y, cellSize, spacing, originGrid, Color.red, 1200f);
                }
            }
        }

        internal Vector3 GetGridCenterWorldPosition(Vector2Int grid) => GetGridCenterWorldPosition(grid.x, grid.y);
        internal Vector3 GetGridCenterWorldPosition(int x, int y) => coordinatesConverter.GetGridCenterWorldPosition(x, y, cellSize, spacing, originGrid);
        internal Vector2Int GetGridCoordinates(Vector3 worldPosition) => coordinatesConverter.WorldPositionToGridCoordinates(worldPosition, cellSize, spacing, originGrid);

        /// <summary>
        /// Retrieves the value stored in the grid at the specified grid coordinates.
        /// </summary>
        internal T GetValue(Vector3 worldPosition) => GetValue(GetGridCoordinates(worldPosition));
        internal T GetValue(Vector2Int grid) => GetValue(grid.x, grid.y);
        internal T GetValue(int x, int y) => IsValid(x, y) ? gridArray[x, y] : default;

        /// <summary>
        /// Sets the value in the grid at the specified grid coordinates.
        /// </summary>
        /// internal void SetValue(Vector2Int grid, T value) => SetValue(grid.x, grid.y, value);
        internal void SetValue(Vector2Int grid, T value) => SetValue(grid.x, grid.y, value);
        internal void SetValue(int x, int y, T value)
        {
            if (IsValid(x, y)) gridArray[x, y] = value;
        }

        /// <summary>
        /// Validates whether the specified grid coordinates are within the bounds of the grid.
        /// </summary>
        internal bool IsValid(Vector2Int grid) => IsValid(grid.x, grid.y);
        internal bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < gridSize.x && y < gridSize.y;

        /// <summary>
        /// A coordinate converter base class for converting between world position and grid coordinates.
        /// </summary>
        internal abstract class CoordinatesConverter
        {
            internal abstract void DrawGridCellOutline(int x, int y, float cellSize, float spacing, Vector3 origin, Color color, float duration);
            protected abstract Vector3[] GetCellCorners(int x, int y, float cellSize, float spacing, Vector3 origin);
            internal abstract Vector3 GetGridCenterWorldPosition(int x, int y, float cellSize, float spacing, Vector3 origin);
            internal abstract Vector2Int WorldPositionToGridCoordinates(Vector3 worldPosition, float cellSize, float spacing, Vector3 origin);
        }

        /// <summary>
        /// A coordinate converter for 2D grids, where the grid lies on the X-Y plane.
        /// </summary>
        internal class XYConverter : CoordinatesConverter
        {
            internal override void DrawGridCellOutline(int x, int y, float cellSize, float spacing, Vector3 origin, Color color, float duration)
            {
                var corners = GetCellCorners(x, y, cellSize, spacing, origin);
                for (int i = 0; i < corners.Length; i++)
                {
                    Debug.DrawLine(corners[i], corners[(i + 1) % corners.Length], color, duration);
                }
            }

            protected override Vector3[] GetCellCorners(int x, int y, float cellSize, float spacing, Vector3 origin)
            {
                var center = GetGridCenterWorldPosition(x, y, cellSize, spacing, origin);
                return new[]
                {
                    center + new Vector3(-0.5f, -0.5f,0) * cellSize,
                    center + new Vector3(-0.5f, 0.5f,0) * cellSize,
                    center + new Vector3(0.5f, 0.5f,0) * cellSize,
                    center + new Vector3(0.5f, -0.5f, 0) * cellSize
                };
            }

            internal override Vector3 GetGridCenterWorldPosition(int x, int y, float cellSize, float spacing, Vector3 origin)
            {
                var offset = cellSize + spacing;
                return new Vector3(x * offset + offset * 0.5f, y * offset + offset * 0.5f, 0) + origin;
            }

            internal override Vector2Int WorldPositionToGridCoordinates(Vector3 worldPosition, float cellSize, float spacing, Vector3 origin)
            {
                var gridPosition = (worldPosition - origin) / (cellSize + spacing);
                return new Vector2Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y));
            }
        }

        /// <summary>
        /// A coordinate converter for 3D grids, where the grid lies on the X-Z plane.
        /// </summary>
        internal class XZConverter : CoordinatesConverter
        {
            internal override void DrawGridCellOutline(int x, int y, float cellSize, float spacing, Vector3 origin, Color color, float duration)
            {
                var corners = GetCellCorners(x, y, cellSize, spacing, origin);
                for (int i = 0; i < corners.Length; i++)
                {
                    Debug.DrawLine(corners[i], corners[(i + 1) % corners.Length], color, duration);
                }
            }

            protected override Vector3[] GetCellCorners(int x, int y, float cellSize, float spacing, Vector3 origin)
            {
                var center = GetGridCenterWorldPosition(x, y, cellSize, spacing, origin);
                return new[]
                {
                    center + new Vector3(-0.5f, 0, -0.5f) * cellSize,
                    center + new Vector3(-0.5f, 0, 0.5f) * cellSize,
                    center + new Vector3(0.5f, 0, 0.5f) * cellSize,
                    center + new Vector3(0.5f, 0, -0.5f) * cellSize
                };
            }
            internal override Vector3 GetGridCenterWorldPosition(int x, int y, float cellSize, float spacing, Vector3 origin)
            {
                var offset = cellSize + spacing;
                return new Vector3(x * offset + offset * 0.5f, 0, y * offset + offset * 0.5f) + origin;
            }

            internal override Vector2Int WorldPositionToGridCoordinates(Vector3 worldPosition, float cellSize, float spacing, Vector3 origin)
            {
                var gridPosition = (worldPosition - origin) / (cellSize + spacing);
                return new Vector2Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.z));
            }
        }
    }
}