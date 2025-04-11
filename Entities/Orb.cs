using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Orbarella;
public class Orb : GameObject
{
    public enum OrbState
    {
        ReadyPosition,
        Charging,
        InFlight,
    }

    private const int MAX_CHARGE_TICKS = 11;
    private const double TICK_TIME = 1.0f / MAX_CHARGE_TICKS;
    private const float GRAVITY = 50f;
    private const float BASE_SPEED = 4f;
    private const float MIN_PULSE_SCALE = 1.1f;
    private const float MAX_PULSE_SCALE = 1.3f;
    private const float IN_FLIGHT_SCALE = 1.2f;

    private SoundEffectInstance _chargeOrbFx;
    private CannonData _cannonData;
    private SoundEffect _fireFx;
    private SpriteObject _orb;
    private SpriteObject _progressBarContainer;
    private Texture2D _progressBarTick;
    private Vector2[] _progressBarTickPositions;
    private Rectangle _playArea;
    private Vector2 _positionOffset;
    private Vector2 _trajectory;
    private double _chargeTimer = 0f;
    private int _numChargeTicks = 0;
    private OrbState _orbState = OrbState.ReadyPosition;
    private bool _pulseGrow = true;
    private List<Color> _orbColours = new List<Color>();
    private int _currentColour = 0;


    public OrbState State => _orbState;


    public Orb(Vector2 positionOffset, Rectangle playArea, List<Nightmare> nightmares)
    {
        _chargeOrbFx = Calimoe.AssetManager.LoadLoopedSoundFx("charge-orb");
        _fireFx = Calimoe.AssetManager.LoadSoundFx("fire");

        _positionOffset = positionOffset;
        _progressBarContainer = new SpriteObject(Calimoe.AssetManager.LoadTexture("progress-bar-container"));
        _progressBarTick = Calimoe.AssetManager.LoadTexture("progress-bar-tick");
        _playArea = playArea;
        var orb = Calimoe.AssetManager.LoadTexture("orb-greyscale");
        _orb = new SpriteObject(orb);
        _orb.Origin = new Vector2(orb.Width / 2, orb.Height / 2);
        SetupProgressBar();
        SetupOrbColours(nightmares);
        UpdateBounds();
        //_drawBounds = true;
    }

    private void SetupProgressBar()
    {
        // positions are relative to the container
        _progressBarTickPositions = new Vector2[11];
        int x = 4;
        int y = 2;
        for (int i = 0; i < 11; i++)
        {
            _progressBarTickPositions[i] = new Vector2(x, y);
            x += 8;
        }
    }

    private void SetupOrbColours(List<Nightmare> nightmares)
    {
        foreach (Nightmare nightmare in nightmares)
        {
            _orbColours.Add(nightmare.Colour);
        }
        UpdateColour();
    }

    protected override void UpdateBounds()
    {
        Bounds = _orb.Bounds;
    }

    public void Update(GameTime gt, CannonData cannonData)
    {
        _cannonData = cannonData;
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        switch (_orbState)
        {
            case OrbState.ReadyPosition:
                {
                    if (_pulseGrow)
                    {
                        _orb.Scale += delta;
                        if (_orb.Scale >= MAX_PULSE_SCALE) _pulseGrow = false;
                    }
                    else
                    {
                        _orb.Scale -= delta;
                        if (_orb.Scale <= MIN_PULSE_SCALE) _pulseGrow = true;
                    }

                    // move the orb with the barrel as it rotates
                    float rad = MathHelper.ToRadians(cannonData.CannonAngle - 90f); // Subtract 90 to account for the initial right-facing angle
                    float xOffset = _positionOffset.X * (float)Math.Cos(rad) - _positionOffset.Y * (float)Math.Sin(rad);
                    float yOffset = _positionOffset.X * (float)Math.Sin(rad) + _positionOffset.Y * (float)Math.Cos(rad);
                    _trajectory = new Vector2(xOffset, yOffset);
                    _orb.Position = _cannonData.CannonOrigin + _trajectory;
                    _progressBarContainer.Position = _orb.Position + new Vector2(-40, -50);
                    break;
                }
            case OrbState.Charging:
                {
                    _chargeTimer += gt.ElapsedGameTime.TotalSeconds;
                    if (_numChargeTicks == MAX_CHARGE_TICKS)
                    {
                        _chargeOrbFx.Stop();
                    }
                    break;
                }
            case OrbState.InFlight:
                {
                    if (_orb.Bounds.Right < 0 ||
                        _orb.Bounds.Left > _playArea.Width ||
                        _orb.Bounds.Bottom < 0 ||
                        _orb.Bounds.Top > _playArea.Height)
                    {
                        Reload();
                    }
                    else
                    {
                        _orb.Scale = IN_FLIGHT_SCALE;
                        _orb.Rotation += delta * BASE_SPEED;
                        float speed = (BASE_SPEED + _numChargeTicks);
                        _orb.Position += _trajectory * speed * delta;
                        _trajectory.Y += GRAVITY * delta;
                    }
                    break;
                }
        }

        _progressBarContainer.Update(gt);
        _orb.Update(gt);

        base.Update(gt);
    }

    public override void Draw(SpriteBatch sb)
    {
        _orb.Draw(sb);

        if (_orbState == OrbState.Charging)
        {
            _progressBarContainer.Draw(sb);
            _numChargeTicks = Math.Min((int)(_chargeTimer / TICK_TIME), MAX_CHARGE_TICKS);
            for (int i = 0; i < _numChargeTicks; i++)
            {
                sb.Draw(_progressBarTick, _progressBarContainer.Position + _progressBarTickPositions[i], Color.White);
            }
        }

        base.Draw(sb);
    }

    public void Reload()
    {
        _chargeTimer = 0f;
        _numChargeTicks = 0;
        _chargeOrbFx.Stop();
        _orbState = OrbState.ReadyPosition;
    }
    public void StartCharging()
    {
        if (_orbState == OrbState.ReadyPosition)
        {
            _chargeTimer = 0f;
            _numChargeTicks = 0;
            _orbState = OrbState.Charging;
            _chargeOrbFx.Play();
        }
    }

    public void Launch()
    {
        if (_orbState == OrbState.Charging)
        {
            _chargeOrbFx.Stop();
            _orbState = OrbState.InFlight;
            _fireFx.Play();
        }
    }

    public void SelectNextColour()
    {
        _currentColour = (_currentColour + 1) % _orbColours.Count;
        UpdateColour();
    }

    public void SelectPreviousColour()
    {
        _currentColour = (_currentColour - 1 + _orbColours.Count) % _orbColours.Count;
        UpdateColour();
    }

    private void UpdateColour()
    {
        _orb.Colour = _orbColours[_currentColour];
        _colour = _orbColours[_currentColour];
    }

    public void Silence()
    {
        _chargeOrbFx.Stop();
    }

}
