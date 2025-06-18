using UnityEngine;

namespace N.GridSystems
{
    public class RectangleGrid : GridLayoutBase
    {
        private readonly Vector2 tileSize;
        private readonly float spacing;
        private readonly Vector2 offset;
        private readonly AxisInfo axis;

        public RectangleGrid(PlaneType planeType, Vector2Int gridSize, Vector2 tileSize, float spacing, Vector3 center)
            : base(GridType.Rectangle, planeType)
        {
            this.tileSize = tileSize;
            this.spacing = spacing;
            this.offset = tileSize + new Vector2(spacing, spacing);
            this.axis = AxisInfo.FromPlane(planeType);

            this.GridCenter = center;
            this.GridOrigin = CalculateGridOrigin(gridSize, tileSize, spacing, center);
        }

        public override Vector3 CalculateGridOrigin(Vector2Int gridSize, Vector2 tileSize, float spacing, Vector3 gridCenter)
        {
            var fullSize = new Vector2(gridSize.x * offset.x, gridSize.y * offset.y);
            return gridCenter - axis.MakeVector(fullSize.x, fullSize.y) * 0.5f;
        }

        public override Vector3 GetCenterWorldPositionOfTile(int x, int y)
        {
            float px = x * offset.x + offset.x * 0.5f;
            float py = y * offset.y + offset.y * 0.5f;
            return axis.MakeVector(px, py) + GridOrigin;
        }

        public override Vector2Int GetGridCoordinatesFromWorld(Vector3 worldPos)
        {
            var local = worldPos - GridOrigin;
            int gx = Mathf.FloorToInt(axis.GetX(local) / offset.x);
            int gy = Mathf.FloorToInt(axis.GetY(local) / offset.y);
            return new Vector2Int(gx, gy);
        }

        public override void DrawTileOutline(int x, int y, Color color, float duration)
        {
            Vector3 tileCenter = GetCenterWorldPositionOfTile(x, y);
            float halfX = tileSize.x * 0.5f;
            float halfY = tileSize.y * 0.5f;

            Vector3[] corners = new[]
            {
                axis.MakeVector(-halfX, -halfY) + tileCenter,
                axis.MakeVector(-halfX,  halfY) + tileCenter,
                axis.MakeVector( halfX,  halfY) + tileCenter,
                axis.MakeVector( halfX, -halfY) + tileCenter,
            };

            for (int i = 0; i < 4; i++)
                Debug.DrawLine(corners[i], corners[(i + 1) % 4], color, duration);
        }

        public override void DrawBoundsNoMargin(Vector2Int gridSize, Color color, float duration)
        {
            float totalW = gridSize.x * tileSize.x + (gridSize.x - 1) * spacing;
            float totalH = gridSize.y * tileSize.y + (gridSize.y - 1) * spacing;

            Vector3 blInner = GridOrigin + axis.MakeVector(spacing * 0.5f, spacing * 0.5f);
            Vector3 brInner = blInner + axis.MakeVector(totalW, 0);
            Vector3 trInner = blInner + axis.MakeVector(totalW, totalH);
            Vector3 tlInner = blInner + axis.MakeVector(0, totalH);

            Debug.DrawLine(blInner, brInner, color, duration);
            Debug.DrawLine(brInner, trInner, color, duration);
            Debug.DrawLine(trInner, tlInner, color, duration);
            Debug.DrawLine(tlInner, blInner, color, duration);
        }

        public override void DrawBoundsWithSpacing(Vector2Int gridSize, Color color, float duration)
        {
            var size = new Vector2(gridSize.x * offset.x, gridSize.y * offset.y);
            Vector3 bl = GridOrigin;
            Vector3 br = bl + axis.MakeVector(size.x, 0);
            Vector3 tr = bl + axis.MakeVector(size.x, size.y);
            Vector3 tl = bl + axis.MakeVector(0, size.y);

            Debug.DrawLine(bl, br, color, duration);
            Debug.DrawLine(br, tr, color, duration);
            Debug.DrawLine(tr, tl, color, duration);
            Debug.DrawLine(tl, bl, color, duration);
        }
    }
}
