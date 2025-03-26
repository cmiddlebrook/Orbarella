
using Microsoft.Xna.Framework;

namespace Orbarella;
public class CannonData
{
    public Vector2 CannonOrigin { get; }
    public float CannonAngle { get; }

    public CannonData(Vector2 cannonOrigin, float cannonAngle)
    {
        CannonOrigin = cannonOrigin;
        CannonAngle = cannonAngle;
    }
}

