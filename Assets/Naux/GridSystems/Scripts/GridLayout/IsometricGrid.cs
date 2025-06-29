using UnityEngine;

namespace N.GridSystems
{
    public class IsometricGrid : GridLayoutBase
    {
        private readonly Vector2 tileSize;
        private readonly float spacing;
        private readonly float halfWidth;
        private readonly float halfHeight;
        private readonly AxisInfo axis;

        public IsometricGrid(PlaneType planeType, Vector2Int gridSize, Vector2 tileSize, float spacing, Vector3 center)
            : base(GridType.Isometric, planeType)
        {
            this.tileSize = tileSize;
            this.spacing = spacing;
            this.halfWidth = (tileSize.x + spacing) * 0.5f;
            this.halfHeight = (tileSize.y + spacing) * 0.5f;
            this.axis = AxisInfo.FromPlane(planeType);

            this.GridCenter = center;
            this.GridOrigin = CalculateGridOrigin(gridSize, tileSize, spacing, center);
        }

        public override Vector3 CalculateGridOrigin(Vector2Int gridSize, Vector2 tileSize, float spacing, Vector3 gridCenter)
        {
            int w = gridSize.x;
            int h = gridSize.y;
            float centerOffsetX = (w - h) * halfWidth * 0.5f;
            float centerOffsetY = (w + h - 2) * halfHeight * 0.5f;
            return gridCenter - axis.MakeVector(centerOffsetX, centerOffsetY);
        }

        public override Vector3 GetCenterWorldPositionOfTile(int x, int y)
        {
            float px = (x - y) * halfWidth;
            float py = (x + y) * halfHeight;
            return axis.MakeVector(px, py) + GridOrigin;
        }

        public override Vector2Int GetGridCoordinatesFromWorld(Vector3 worldPos)
        {
            var local = worldPos - GridOrigin;
            float tx = axis.GetX(local) / halfWidth;
            float ty = axis.GetY(local) / halfHeight;
            int x = Mathf.FloorToInt((ty + tx) * 0.5f);
            int y = Mathf.FloorToInt((ty - tx) * 0.5f);
            return new Vector2Int(x, y);
        }

        public override void DrawTileOutline(int x, int y, Color color, float duration)
        {
            Vector3 tileCenter = GetCenterWorldPositionOfTile(x, y);
            float hw = tileSize.x * 0.5f;
            float hh = tileSize.y * 0.5f;

            Vector3[] corners = new[]
            {
                axis.MakeVector(  0,  hh) + tileCenter,
                axis.MakeVector( hw,   0) + tileCenter,
                axis.MakeVector(  0, -hh) + tileCenter,
                axis.MakeVector(-hw,   0) + tileCenter,
            };

            for (int i = 0; i < 4; i++)
                Debug.DrawLine(corners[i], corners[(i + 1) % 4], color, duration);
        }

        public override void DrawBoundsNoMargin(Vector2Int gridSize, Color color, float duration)
        {
            int w = gridSize.x;
            int h = gridSize.y;
            float hw = tileSize.x * 0.5f;
            float hh = tileSize.y * 0.5f;

            Vector3 bottom = GetCenterWorldPositionOfTile(0, 0) + axis.MakeVector(0, -hh);
            Vector3 left = GetCenterWorldPositionOfTile(0, h - 1) + axis.MakeVector(-hw, 0);
            Vector3 top = GetCenterWorldPositionOfTile(w - 1, h - 1) + axis.MakeVector(0, hh);
            Vector3 right = GetCenterWorldPositionOfTile(w - 1, 0) + axis.MakeVector(hw, 0);

            Debug.DrawLine(bottom, left, color, duration);
            Debug.DrawLine(left, top, color, duration);
            Debug.DrawLine(top, right, color, duration);
            Debug.DrawLine(right, bottom, color, duration);
        }

        public override void DrawBoundsWithSpacing(Vector2Int gridSize, Color color, float duration)
        {
            int w = gridSize.x;
            int h = gridSize.y;

            Vector3 bottom = GetCenterWorldPositionOfTile(0, 0) + axis.MakeVector(0, -halfHeight);
            Vector3 left = GetCenterWorldPositionOfTile(0, h - 1) + axis.MakeVector(-halfWidth, 0);
            Vector3 top = GetCenterWorldPositionOfTile(w - 1, h - 1) + axis.MakeVector(0, halfHeight);
            Vector3 right = GetCenterWorldPositionOfTile(w - 1, 0) + axis.MakeVector(halfWidth, 0);

            Debug.DrawLine(bottom, left, color, duration);
            Debug.DrawLine(left, top, color, duration);
            Debug.DrawLine(top, right, color, duration);
            Debug.DrawLine(right, bottom, color, duration);
        }
    }
}
