using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbarella;
public class NightmareMeter : GameObject
{
    private const int MAX_TICKS = 24;
    private SpriteObject _container;
    private SpriteObject _skull;
    private Texture2D _tick;
    private Vector2[] _tickPositions;
    private Color[] _tickColours;
    private int _numTicks;


    public NightmareMeter()
    {
        _skull = new SpriteObject("gameover", new Vector2(6, 10), Vector2.Zero, 1.0f);
        _container = new SpriteObject("nightmare-meter-container", new Vector2(12, 36), Vector2.Zero, 1.0f);
        _tick = Calimoe.AssetManager.LoadTexture("mightmare-meter-tick");
        SetupMeter();
    }
    protected void SetupMeter()
    {
        // positions are relative to the container
        _tickPositions = new Vector2[MAX_TICKS];
        int x = 2;
        int y = _container.Bounds.Height - _tick.Height - 4;
        for (int i = 0; i < MAX_TICKS; i++)
        {
            _tickPositions[i] = new Vector2(x, y);
            y -= 8;
        }

        // colours go from green to red, bottom to top
        _tickColours = new Color[MAX_TICKS];
        for (int i = 0; i < MAX_TICKS; i++)
        {
            float tickNorm = i / (float)(MAX_TICKS - 1);
            _tickColours[i] = tickNorm < 0.5f
                ? Color.Lerp(Color.Green, Color.Yellow, tickNorm * 2)
                : Color.Lerp(Color.Yellow, Color.Red, (tickNorm - 0.5f) * 2);
        }
    }


    /// <summary>
    /// Sets the meter's progress level based on a normalized value.
    /// The level should be a float between 0 and 1, where 0 represents no progress
    /// and 1 represents full progress. The number of ticks will be adjusted accordingly,
    /// </summary>
    /// <param name="level">A float between 0 and 1 representing the progress level.</param>
    public void SetLevel(float level)
    {
        _numTicks = Math.Min((int)(level * MAX_TICKS), MAX_TICKS);
    }

    protected override void UpdateBounds()
    {
        _bounds = _container.Bounds;
    }

    public override void Update(GameTime gt)
    { 

        base.Update(gt);
    }

    public override void Draw(SpriteBatch sb)
    {
        _container.Draw(sb);
        _skull.Draw(sb);
        for (int i = 0; i < _numTicks; i++)
        {
            sb.Draw(_tick, _container.Position + _tickPositions[i], _tickColours[i]);
        }
    }
}
