﻿using CALIMOE;
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

    public Player(AssetManager am, int streetLevel, int rightEdge)
    {
        _cannonRollFx = am.LoadLoopedSoundFx("cannon-roll");
        _cannonRotateFx = am.LoadLoopedSoundFx("cannon-rotate");

        _rightEdge = rightEdge;
        var wizard = am.LoadTexture("wizard");
        _wizardPosition = new Vector2(80, streetLevel - wizard.Height);
        _wizard = new SpriteObject(wizard, _wizardPosition, Vector2.Zero, 1.0f);
        var barrelOrigin = new Vector2(20, 35);
        var cannonBarrel = am.LoadTexture("CannonBarrel");
        _barrelPositionOffset = new Vector2(100, wizard.Height - cannonBarrel.Height + barrelOrigin.Y);
        _barrel = new SpriteObject(cannonBarrel, _wizardPosition + _barrelPositionOffset, Vector2.Zero, 1.0f);
        _barrel.Origin = barrelOrigin;
        var cannonBase = am.LoadTexture("CannonBase");
        _basePositionOffset = new Vector2(68, wizard.Height - cannonBase.Height);
        _base = new SpriteObject(cannonBase, _wizardPosition + _basePositionOffset, Vector2.Zero, 1.0f);
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
