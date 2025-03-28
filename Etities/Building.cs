using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Orbarella;
public class Building
{

    private SpriteObject _building;
    private int _streetLevel;
    List<SpriteObject> _windows;
    private Color _defaultWindowColour = new Color(34, 46, 59);
    private List<Nightmare> _nightmares;
    static Random _random = new Random();

    public Rectangle Bounds => _building.Bounds;

    public Building(AssetManager am,
                    BuildingData data,
                    int rightEdge, 
                    int streetLevel,
                    List<Nightmare> nightmares)
    {
        _nightmares = nightmares;
        _streetLevel = streetLevel;
        var buildingTx = am.LoadTexture(data.Name);
        var position = new Vector2(rightEdge - buildingTx.Width - 10, streetLevel - buildingTx.Height);
        _building = new SpriteObject(buildingTx, position, Vector2.Zero, 1.0f);
        CreateWindows(am.LoadTexture("window"), data.Windows);
    }

    private void CreateWindows(Texture2D windowTX, List<WindowPosition> windows)
    {
        _windows = new List<SpriteObject>();
        foreach (WindowPosition wp in windows)
        {
            var position = new Vector2(_building.Position.X + wp.X, _building.Position.Y + wp.Y);
            var window = new SpriteObject(windowTX, position, Vector2.Zero, 1.0f);
            window.Colour = _defaultWindowColour;
            //window.Colour = GetRandomColour();
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
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        _building.Update(gt);
        foreach (SpriteObject window in _windows)
        {
            window.Update(gt);
        }
    }

    public void SpawnDreamer()
    {
        var index = _random.Next(_windows.Count);
        var window = _windows[index];
        window.Colour = GetRandomColour();
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
