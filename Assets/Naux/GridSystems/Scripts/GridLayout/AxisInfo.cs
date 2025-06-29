using System;
using UnityEngine;

namespace N.GridSystems
{
    public readonly struct AxisInfo
    {
        public readonly Func<float, float, Vector3> MakeVector;
        public readonly Func<Vector3, float> GetX;
        public readonly Func<Vector3, float> GetY;
        public readonly Func<Vector3, float> GetZ;

        public AxisInfo(Func<float, float, Vector3> makeVector,
            Func<Vector3, float> getX,
            Func<Vector3, float> getY,
            Func<Vector3, float> getZ)
        {
            MakeVector = makeVector;
            GetX = getX;
            GetY = getY;
            GetZ = getZ;
        }

        public static AxisInfo FromPlane(PlaneType planeType) => planeType switch
        {
            PlaneType.XY => new AxisInfo(
                (x, y) => new Vector3(x, y, 0),
                v => v.x,
                v => v.y,
                v => v.z
            ),
            PlaneType.XZ => new AxisInfo(
                (x, y) => new Vector3(x, 0, y),
                v => v.x,
                v => v.z,
                v => v.y
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(planeType), planeType, null)
        };
    }
}
