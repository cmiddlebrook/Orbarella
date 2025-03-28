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
    List<Dreamer> _dreamers;
    private List<Nightmare> _nightmares;
    static Random _random = new Random();

    public Rectangle Bounds => _building.Bounds;

    public List<Dreamer> Dreamers => _dreamers;

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
        _dreamers = new List<Dreamer>();
        foreach (WindowPosition wp in windows)
        {
            var position = new Vector2(_building.Position.X + wp.X, _building.Position.Y + wp.Y);
            var window = new SpriteObject(windowTX, position, Vector2.Zero, 1.0f);

            var dreamer = new Dreamer(window, _nightmares);
            _dreamers.Add(dreamer);
        }
    }

    public void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        _building.Update(gt);
        foreach (Dreamer dreamer in _dreamers)
        {
            dreamer.Update(gt);
        }
    }

    public void StartNightmare()
    {
        bool newNightmare = false;
        while (!newNightmare)
        {
            var dreamer = _dreamers[_random.Next(_dreamers.Count)];
            if (!dreamer.IsDreaming)
            {
                dreamer.StartNightmare();
                newNightmare = true;
            }
        }
    }

    public void Draw(SpriteBatch sb)
    {
        _building.Draw(sb);
        foreach (Dreamer dreamer in _dreamers)
        {
            dreamer.Draw(sb);
        }
    }
}
