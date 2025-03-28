using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbarella;
public class Building
{
    private SpriteObject _building;
    private int _streetLevel;
    List<SpriteObject> _windows;
    static Random _random = new Random();

    public Rectangle Bounds => _building.Bounds;

    public Building(Texture2D building, Texture2D window, int rightEdge, int streetLevel, List<WindowPosition> windows)
    {
        _streetLevel = streetLevel;
        var position = new Vector2(rightEdge - building.Width - 10, streetLevel - building.Height);
        _building = new SpriteObject(building, position, Vector2.Zero, 1.0f);
        CreateWindows(window, windows);
    }

    private void CreateWindows(Texture2D windowTX, List<WindowPosition> windows)
    {
        _windows = new List<SpriteObject>();
        foreach (WindowPosition wp in windows)
        {
            var position = new Vector2(_building.Position.X + wp.X, _building.Position.Y + wp.Y);
            var window = new SpriteObject(windowTX, position, Vector2.Zero, 1.0f);
            window.Colour = GetRandomColour();
            _windows.Add(window);
        }
    }

    private Color GetRandomColour()
    {
        var r = _random.Next(0, 256);
        var g = _random.Next(0, 256);
        var b = _random.Next(0, 256);
        return new Color(r, g, b);
    }

    public void Update(GameTime gt)
    {
        _building.Update(gt);
        foreach (SpriteObject window in _windows)
        {
            window.Update(gt);
        }
    }

    public void Draw(SpriteBatch sb)
    {
        _building.Draw(sb);
        foreach (SpriteObject window in _windows)
        {
            window.Draw(sb);
        }
    }
}
