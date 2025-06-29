using UnityEngine;

namespace N.GridSystems
{
    public abstract class GridLayoutBase
    {
        public readonly GridType GridType;
        public readonly PlaneType PlaneType;
        public Vector3 GridOrigin;
        public Vector3 GridCenter;

        protected GridLayoutBase(GridType gridType, PlaneType planeType)
        {
            GridType = gridType;
            PlaneType = planeType;
        }

        public abstract Vector3 CalculateGridOrigin(Vector2Int gridSize, Vector2 tileSize, float spacing, Vector3 gridCenter);
        public abstract Vector3 GetCenterWorldPositionOfTile(int x, int y);
        public abstract Vector2Int GetGridCoordinatesFromWorld(Vector3 worldPos);
        public abstract void DrawTileOutline(int x, int y, Color color, float duration);
        public abstract void DrawBoundsNoMargin(Vector2Int gridSize, Color color, float duration);
        public abstract void DrawBoundsWithSpacing(Vector2Int gridSize, Color color, float duration);

        public void DrawGridOutline(Vector2Int gridSize, Color color, float duration)
        {
            for (int x = 0; x < gridSize.x; x++)
                for (int y = 0; y < gridSize.y; y++)
                    DrawTileOutline(x, y, color, duration);
        }
    }
}
