using System.Numerics;
using Lumina.Excel.Sheets;

namespace HuntAutomator.Utils;

internal static class MapCoordinates
{
    public static Vector3 MapToApproxWorld(Vector2 mapPos, Map map)
    {
        // Inverse of Dalamud.Utility.MapUtil.ConvertWorldCoordXZToMapCoord.
        static float Convert(float coord, uint scale, int offset)
            => (coord - 1f - (0.02f * offset) - (2048f / scale)) / 0.02f;

        return new Vector3(
            Convert(mapPos.X, map.SizeFactor, map.OffsetX),
            0f,
            Convert(mapPos.Y, map.SizeFactor, map.OffsetY));
    }
}
