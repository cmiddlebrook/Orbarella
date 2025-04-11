using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace Orbarella;
public class Player : GameObject
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

    private SoundEffectInstance _cannonRollFx;
    private SoundEffectInstance _cannonRotateFx;

    private SpriteObject _wizard;
    private SpriteObject _barrel;
    private SpriteObject _base;
    private Vector2 _wizardPosition;
    private Vector2 _basePositionOffset;
    private Vector2 _barrelPositionOffset;
    private int _rightEdge;
    private MoveState _moveState = MoveState.Stopped;
    private BarrelState _barrelState = BarrelState.Stopped;

    const float ROTATION_SPEED = 300.0f;
    const float MIN_ANGLE = 0f;
    const float MAX_ANGLE = 90f; 
    private float Angle { get; set; }

    private float Speed { get; set; } = 400f;
    public Texture2D _boundsTest {  get; set; }

    public CannonData CannonData => new CannonData(_barrel.Position, Angle);

    public Player(int streetLevel, int rightEdge)
    {
        _cannonRollFx = Calimoe.AssetManager.LoadLoopedSoundFx("cannon-roll");
        _cannonRotateFx = Calimoe.AssetManager.LoadLoopedSoundFx("cannon-rotate");

        _rightEdge = rightEdge;
        _wizard = new SpriteObject("wizard", _wizardPosition, Vector2.Zero, 1.0f);
        _wizardPosition = new Vector2(80, streetLevel - _wizard.Bounds.Height);
        var barrelOrigin = new Vector2(20, 35);
        _barrel = new SpriteObject("CannonBarrel", _wizardPosition + _barrelPositionOffset, Vector2.Zero, 1.0f);
        _barrelPositionOffset = new Vector2(100, _wizard.Bounds.Height - _barrel.Bounds.Height + barrelOrigin.Y);
        _barrel.Origin = barrelOrigin;
        _base = new SpriteObject("CannonBase", _wizardPosition + _basePositionOffset, Vector2.Zero, 1.0f);
        _basePositionOffset = new Vector2(68, _wizard.Bounds.Height - _base.Bounds.Height);
        Angle = 90f;
        UpdateBounds();
        //_drawBounds = true;
    }

    protected override void UpdateBounds()
    {
        _bounds = new Rectangle(_wizard.Bounds.X, _wizard.Bounds.Y, _wizard.Bounds.Width + _barrel.Bounds.Width, _wizard.Bounds.Height);
    }

    public override void Update(GameTime gt)
    {
        if (_cannonRollFx.State == SoundState.Stopped && (_moveState == MoveState.MovingLeft || _moveState == MoveState.MovingRight))
        {
            _cannonRollFx.Play();
        }

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
                {
                    _cannonRollFx.Stop();
                    break;
                }
            default:
                {
                    break;
                }
        }

        if (_cannonRotateFx.State == SoundState.Stopped && (_barrelState == BarrelState.RotatingLeft || _barrelState == BarrelState.RotatingRight))
        {
            _cannonRotateFx.Play();
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
                    _cannonRotateFx.Stop();
                    break;
                }

        }

        // recalculate the positions of the wizard, barrel and base sprites
        _wizard.Position = _wizardPosition;
        _barrel.Position = _wizardPosition + _barrelPositionOffset;
        _base.Position = _wizardPosition + _basePositionOffset;
        
        _moveState = MoveState.Stopped;
        _barrelState = BarrelState.Stopped;

        // rotate the cannon barrel
        Angle = MathHelper.Clamp(Angle, MIN_ANGLE, MAX_ANGLE);
        _barrel.Rotation = MathHelper.ToRadians(Angle - 90); // -90 to account for the 90 degree starting angle

        // update the sprites
        _wizard.Update(gt);
        _barrel.Update(gt);
        _base.Update(gt);

        base.Update(gt);
    }


    public override void Draw(SpriteBatch sb)
    {
        _wizard.Draw(sb);
        _barrel.Draw(sb);
        _base.Draw(sb);

        base.Draw(sb);
    }

    public override void Reset()
    {
        _wizard.Reset();
        _barrel.Reset();
        _base.Reset();
        Silence();
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

    public void Silence()
    {
        _cannonRollFx.Stop();
        _cannonRotateFx.Stop();
    }

}
