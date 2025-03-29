using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;


namespace Orbarella;
public class Building : GameObject
{

    private SpriteObject _building;
    private int _streetLevel;
    List<Dreamer> _dreamers;
    private List<Nightmare> _nightmares;
    private int _totalDreamers = 0;
    private int _numDreaming = 0;
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
        _dreamers = new List<Dreamer>();
        foreach (WindowPosition wp in windows)
        {
            var position = new Vector2(_building.Position.X + wp.X, _building.Position.Y + wp.Y);
            var window = new SpriteObject(windowTX, position, Vector2.Zero, 1.0f);

            var dreamer = new Dreamer(window, _nightmares);
            _dreamers.Add(dreamer);
        }
        _totalDreamers = _dreamers.Count;
    }

    public override void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        _building.Update(gt);
        foreach (Dreamer dreamer in _dreamers)
        {
            dreamer.Update(gt);
        }
    }

    public bool StartNightmare()
    {
        if (_numDreaming < _totalDreamers)
        {
            var dreamer = _dreamers[_random.Next(_dreamers.Count)];
            if (!dreamer.IsDreaming)
            {
                dreamer.StartNightmare();
                _numDreaming++;
                return true;
            }
        }
        return false;
    }

    public void StopNightmare(Dreamer dreamer, bool isCorrectColour)
    {
        _numDreaming--;
        dreamer.StopNightmare(true, isCorrectColour);
    }

    public (bool isCollision, bool isCorrectColour) HandleCollisions(Orb orb)
    {
        bool isCollision = false;
        bool isCorrectColour = false;
        foreach (Dreamer dreamer in _dreamers)
        {
            if (dreamer.IsDreaming && orb.Bounds.Intersects(dreamer.Bounds))
            {
                isCollision = true;
                isCorrectColour = (dreamer.IsColourMatch(orb.Colour));
                StopNightmare (dreamer, isCorrectColour);
            }
        }

        return (isCollision, isCorrectColour);
    }

    public override void Draw(SpriteBatch sb)
    {
        _building.Draw(sb);
        foreach (Dreamer dreamer in _dreamers)
        {
            dreamer.Draw(sb);
        }
    }
}
