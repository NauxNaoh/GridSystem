using System;
using UnityEngine;

namespace N.GridSystems
{
    /// <summary>
    /// Defines the type of grid layout used (e.g., Rectangle or Isometric).
    /// </summary>
    public enum GridType { Rectangle, Isometric }

    /// <summary>
    /// Defines the orientation plane of the grid (XY or XZ).
    /// </summary>
    public enum PlaneType { XY, XZ }

    public class GridSystem<T>
    {
        public readonly Vector2Int GridSize;
        public readonly Vector2 TileSize;
        public readonly float Spacing;

        private readonly GridLayoutBase layout;
        private readonly T[,] gridArray;

        public Vector3 GridCenter => layout.GridCenter;
        public Vector3 GridOrigin => layout.GridOrigin;


        /// <summary>
        /// Constructor for internal grid initialization.
        /// </summary>
        private GridSystem(GridLayoutBase layout, Vector2Int gridSize, Vector2 tileSize, float spacing)
        {
            this.GridSize = gridSize;
            this.TileSize = tileSize;
            this.Spacing = spacing;
            this.layout = layout;
            this.gridArray = new T[gridSize.x, gridSize.y];
        }

        /// <summary>
        /// Generates and returns a configured grid system with specified layout type, plane, size, and spacing.
        /// Optionally draws debug visuals in the editor.
        /// </summary>
        public static GridSystem<T> GenerateGrid(GridType gridType, PlaneType planeType, Vector2Int gridSize,
            Vector2 tileSize, float spacing, Vector3 center, bool draw = false)
        {
            if (tileSize.x <= 0f || tileSize.y <= 0f)
                throw new ArgumentException($"{nameof(tileSize)} dimensions must be > 0");
            if (spacing < 0f)
                throw new ArgumentException($"{nameof(spacing)} must be >= 0");
            if (gridSize.x <= 0 || gridSize.y <= 0)
                throw new ArgumentException($"{nameof(gridSize)} must be > 0");

            GridLayoutBase layout = gridType switch
            {
                GridType.Rectangle => new RectangleGrid(planeType, gridSize, tileSize, spacing, center),
                GridType.Isometric => new IsometricGrid(planeType, gridSize, tileSize, spacing, center),
                _ => throw new NotImplementedException()
            };

#if UNITY_EDITOR
            if (draw)
            {
                layout.DrawGridOutline(gridSize, Color.red, 500f);
                layout.DrawBoundsNoMargin(gridSize, Color.blue, 500f);
                layout.DrawBoundsWithSpacing(gridSize, Color.yellow, 500f);
            }
#endif

            return new GridSystem<T>(layout, gridSize, tileSize, spacing);
        }

        /// <summary>
        /// Returns the world position of the center of a tile at the given grid coordinates.
        /// </summary>
        public Vector3 GetCenterWorldPositionOfTile(int x, int y) => layout.GetCenterWorldPositionOfTile(x, y);

        /// <summary>
        /// Converts a world position to corresponding grid coordinates.
        /// </summary>
        public Vector2Int GetGridCoordinatesFromWorld(Vector3 worldPos) => layout.GetGridCoordinatesFromWorld(worldPos);

        /// <summary>
        /// Checks if the given grid coordinates are within grid bounds.
        /// </summary>
        public bool IsInsideGridBounds(Vector2Int gPos) => IsInsideGridBounds(gPos.x, gPos.y);

        /// <summary>
        /// Checks if the given x and y grid coordinates are within grid bounds.
        /// </summary>
        public bool IsInsideGridBounds(int x, int y) => x >= 0 && y >= 0 && x < GridSize.x && y < GridSize.y;

        /// <summary>
        /// Sets the value of the tile at the specified grid coordinates.
        /// </summary>
        public void SetTileValue(Vector2Int gPos, T value) => SetTileValue(gPos.x, gPos.y, value);

        /// <summary>
        /// Sets the value of the tile at the specified x and y grid coordinates.
        /// </summary>
        public void SetTileValue(int x, int y, T value)
        {
            if (IsInsideGridBounds(x, y))
                gridArray[x, y] = value;
        }

        /// <summary>
        /// Gets the value of the tile at the specified grid coordinates.
        /// </summary>
        public T GetTileValue(Vector2Int gPos) => GetTileValue(gPos.x, gPos.y);

        /// <summary>
        /// Gets the value of the tile at the specified x and y grid coordinates.
        /// Logs a warning if the position is out of bounds.
        /// </summary>
        public T GetTileValue(int x, int y)
        {
            if (IsInsideGridBounds(x, y))
                return gridArray[x, y];

            Debug.LogWarning($"[{nameof(GridSystem<T>)}] Tried to access tile out of bounds at ({x}, {y})");
            return default;
        }
    }
}
