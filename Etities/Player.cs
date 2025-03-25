using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbarella;
public class Player
{
    private enum MoveState
    {
        MovingLeft,
        MovingRight,
        Stopped,
    }

    private enum BarrelState
    {
        RotatingRight,
        RotatingLeft,
        Stopped
    }

    private SpriteObject _wizard;
    private SpriteObject _barrel;
    private SpriteObject _base;
    private Vector2 _wizardPosition;
    private Vector2 _basePositionOffset;
    private Vector2 _barrelPositionOffset;
    private Vector2 _orbPositionOffset;
    private Vector2 _orbPosition;
    private Rectangle _bounds;
    private int _rightEdge;
    private MoveState _moveState = MoveState.Stopped;
    private BarrelState _barrelState = BarrelState.Stopped;

    const float ROTATION_SPEED = 300.0f;
    const float MIN_ANGLE = 0f;
    const float MAX_ANGLE = 90f; 
    private float Angle { get; set; }

    private float Speed { get; set; } = 400f;
    private Rectangle Bounds => _bounds;
    public Texture2D _boundsTest {  get; set; }

    public Vector2 OrbPosition => _orbPosition;

    public Player(Texture2D wizard, Texture2D cannonBase, Texture2D cannonBarrel, int streetLevel, int rightEdge)
    {
        // the position offsets and the origin of the cannon barrel are hardcoded
        // to the specific sprites used for the cannon. If I later change the sprites, 
        // I'll need to recalculate the values used here
        _rightEdge = rightEdge;
        _wizardPosition = new Vector2(80, streetLevel - wizard.Height);
        _wizard = new SpriteObject(wizard, _wizardPosition, Vector2.Zero, 1.0f);
        var barrelOrigin = new Vector2(20, 35);
        _barrelPositionOffset = new Vector2(100, wizard.Height - cannonBarrel.Height + barrelOrigin.Y);
        _barrel = new SpriteObject(cannonBarrel, _wizardPosition + _barrelPositionOffset, Vector2.Zero, 1.0f);
        _barrel.Origin = barrelOrigin;
        _orbPositionOffset = new Vector2(cannonBarrel.Width - 20, -7);
        _basePositionOffset = new Vector2(68, wizard.Height - cannonBase.Height);
        _base = new SpriteObject(cannonBase, _wizardPosition + _basePositionOffset, Vector2.Zero, 1.0f);
        _bounds = new Rectangle(_wizard.Bounds.X, _wizard.Bounds.Y, wizard.Width + cannonBarrel.Width, wizard.Height);
        Angle = 90f;
    }

    public void Update(GameTime gt)
    {
        switch (_moveState)
        {
            case MoveState.MovingLeft:
            {
                _wizardPosition.X = Math.Max(0, (float)(_wizard.Position.X - Speed * gt.ElapsedGameTime.TotalSeconds));
                break;
            }

            case MoveState.MovingRight:
            {
                _wizardPosition.X = Math.Min(_rightEdge - _bounds.Width, 
                    (float)(_wizard.Position.X + Speed * gt.ElapsedGameTime.TotalSeconds));
                break;
            }

            case MoveState.Stopped:
            default:
                {
                    break;
                }
        }



        switch (_barrelState)
        {
            case BarrelState.RotatingRight:
                {
                    Angle += (float)(ROTATION_SPEED * gt.ElapsedGameTime.TotalSeconds);
                    break;
                }

                case BarrelState.RotatingLeft:
                {
                    Angle -= (float)(ROTATION_SPEED * gt.ElapsedGameTime.TotalSeconds);
                    break;
                }

            default:
            case BarrelState.Stopped:
                {
                    break;
                }

        }

        // recalculate the positions of the wizard, barrel and base sprites
        _wizard.Position = _wizardPosition;
        _barrel.Position = _wizardPosition + _barrelPositionOffset;
        _base.Position = _wizardPosition + _basePositionOffset;
        _bounds.X = _wizard.Bounds.X;
        _bounds.Y = _wizard.Bounds.Y;
        
        _moveState = MoveState.Stopped;
        _barrelState = BarrelState.Stopped;

        // rotate the cannon barrel
        Angle = MathHelper.Clamp(Angle, MIN_ANGLE, MAX_ANGLE);
        _barrel.Rotation = MathHelper.ToRadians(Angle - 90); // -90 to account for the 90 degree starting angle

        // move the ball with the barrel as it rotates
        float rad = MathHelper.ToRadians(Angle - 90f); // Subtract 90 to account for the initial right-facing angle
        float xOffset = _orbPositionOffset.X * (float)Math.Cos(rad) - _orbPositionOffset.Y * (float)Math.Sin(rad);
        float yOffset = _orbPositionOffset.X * (float)Math.Sin(rad) + _orbPositionOffset.Y * (float)Math.Cos(rad);
        _orbPosition = _barrel.Position + new Vector2(xOffset, yOffset);

        // update the sprites
        _wizard.Update(gt);
        _barrel.Update(gt);
        _base.Update(gt);
    }


    public void Draw(SpriteBatch sb)
    {
        //sb.Draw(_boundsTest, _bounds, Color.White);
        _wizard.Draw(sb);
        _barrel.Draw(sb);
        _base.Draw(sb);
    }

    public void Reset()
    {
        _wizard.Reset();
        _barrel.Reset();
        _base.Reset();
    }

    public void MoveLeft()
    {
        _moveState = MoveState.MovingLeft;
    }

    public void MoveRight()
    {
        _moveState = MoveState.MovingRight;
    }

    public void RotateBarrelRight()
    {
        _barrelState = BarrelState.RotatingRight;
    }

    public void RotateBarrelLeft()
    {
        _barrelState = BarrelState.RotatingLeft;
    }

}
