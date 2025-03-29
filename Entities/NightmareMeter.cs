using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbarella;
public class NightmareMeter
{
    private const int MAX_TICKS = 24;
    private SpriteObject _container;
    private Texture2D _tick;
    private Vector2[] _tickPositions;
    private Color[] _tickColours;
    private int _numTicks;

    public int NumTicks => MAX_TICKS;

    public NightmareMeter(AssetManager am)
    {
        _container = new SpriteObject(am.LoadTexture("nightmare-meter-container"), new Vector2(8, 8), Vector2.Zero, 1.0f);
        _tick = am.LoadTexture("mightmare-meter-tick");
        SetupMeter();
    }
    private void SetupMeter()
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

    public void SetLevel(float level)
    {
        _numTicks = (int)Math.Floor(level);
    }

    public void Update(GameTime gt)
    { 

    }

    public void Draw(SpriteBatch sb)
    {
        _container.Draw(sb);
        for (int i = 0; i < _numTicks; i++)
        {
            sb.Draw(_tick, _container.Position + _tickPositions[i], _tickColours[i]);
        }
    }
}
