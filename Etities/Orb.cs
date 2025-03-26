using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbarella;
public class Orb
{
    private enum OrbState
    {
        ReadyPosition,
        InFlight,
    }
    private SpriteObject _orb;
    private Vector2 _positionOffset;
    private OrbState _orbState = OrbState.ReadyPosition;

    public Orb(Texture2D orb, Vector2 positionOffset)
    {
        _positionOffset = positionOffset;
        _orb = new SpriteObject(orb);
        _orb.Origin = new Vector2(orb.Width / 2, orb.Height / 2);
    }

    public void Update(GameTime gt, CannonData cannonData)
    {
        // move the orb with the barrel as it rotates
        float rad = MathHelper.ToRadians(cannonData.CannonAngle - 90f); // Subtract 90 to account for the initial right-facing angle
        float xOffset = _positionOffset.X * (float)Math.Cos(rad) - _positionOffset.Y * (float)Math.Sin(rad);
        float yOffset = _positionOffset.X * (float)Math.Sin(rad) + _positionOffset.Y * (float)Math.Cos(rad);
        _orb.Position = cannonData.CannonOrigin + new Vector2(xOffset, yOffset);
    }

    public void Draw(SpriteBatch sb)
    {
        _orb.Draw(sb);
    }

}
