using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Orbarella;
public class Building : GameObject
{
    private SpriteObject _building;
    private int _streetLevel;
    List<Dreamer> _dreamers;
    private List<Nightmare> _nightmares;
    private int _numResidents = 0;
    static Random _random = new Random();    

    public int NumResidents => _numResidents;

    public int NumDreaming => _dreamers.Count(dreamer => dreamer.IsDreaming);


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
        UpdateBounds();
        //_drawBounds = true;
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
        _numResidents = _dreamers.Count;
    }

    protected override void UpdateBounds()
    {
        Bounds = _building.Bounds;
    }

    public override void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        _building.Update(gt);
        foreach (Dreamer dreamer in _dreamers)
        {
            dreamer.Update(gt);
        }

        base.Update(gt);
    }

    public bool StartNightmare()
    {
        if (NumDreaming < _numResidents)
        {
            var dreamer = _dreamers[_random.Next(_dreamers.Count)];
            if (!dreamer.IsDreaming)
            {
                dreamer.StartNightmare();
                return true;
            }
        }
        return false;
    }

    public void StopNightmare(Dreamer dreamer, bool isCorrectColour)
    {
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
                break;
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

        base.Draw(sb);
    }
}
