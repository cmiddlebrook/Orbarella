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
    private Rectangle _playArea;
    private Vector2 _positionOffset;
    private Vector2 _trajectory;
    private float _speed = 10f;
    private OrbState _orbState = OrbState.ReadyPosition;

    public Orb(Texture2D orb, Vector2 positionOffset, Rectangle playArea)
    {
        _positionOffset = positionOffset;
        _playArea = playArea;
        _orb = new SpriteObject(orb);
        _orb.Origin = new Vector2(orb.Width / 2, orb.Height / 2);
    }

    public void Update(GameTime gt, CannonData cannonData)
    {
        switch (_orbState)
        {
            case OrbState.ReadyPosition:
                {
                    // move the orb with the barrel as it rotates
                    float rad = MathHelper.ToRadians(cannonData.CannonAngle - 90f); // Subtract 90 to account for the initial right-facing angle
                    float xOffset = _positionOffset.X * (float)Math.Cos(rad) - _positionOffset.Y * (float)Math.Sin(rad);
                    float yOffset = _positionOffset.X * (float)Math.Sin(rad) + _positionOffset.Y * (float)Math.Cos(rad);
                    _trajectory = new Vector2(xOffset, yOffset);
                    _orb.Position = cannonData.CannonOrigin + _trajectory;
                    break;
                }
            case OrbState.InFlight:
                {
                    if (_orb.Bounds.Right < 0 ||
                        _orb.Bounds.Left > _playArea.Width ||
                        _orb.Bounds.Bottom < 0 ||
                        _orb.Bounds.Top > _playArea.Height)
                    {
                        _orbState = OrbState.ReadyPosition;
                    }
                    else
                    {
                        _trajectory.Y += 1f;
                        _orb.Position += _trajectory * _speed * (float)gt.ElapsedGameTime.TotalSeconds;
                    }
                    break;
                }
        }
    }

    public void Draw(SpriteBatch sb)
    {
        _orb.Draw(sb);
    }

    public void Launch()
    {
        _orbState = OrbState.InFlight;
    }

}
