using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;

namespace Orbarella;
public class Scene1 : GameScene
{
    private const float NEW_NIGHTMARE_SPAWN = 3f;

    private Texture2D _background;
    private Texture2D _streetBase;
    private List<Building> _buildings = new List<Building>();
    private List<Nightmare> _nightmares = new List<Nightmare>();
    private Player _player;
    private Orb _orb;
    private float _newDreamerTimer = NEW_NIGHTMARE_SPAWN;
    static Random _random = new Random();


    public int WindowWidth => _background.Width;
    public int WindowHeight => _background.Height;

    public Scene1(SceneManager sm, AssetManager am, InputHelper ih)
        : base(sm, am, ih)
    {
        _name = "scene1";
        _clearColour = new Color(0x10, 0x10, 0x10);
    }

    public override void LoadContent()
    {
        _background = _am.LoadTexture("Scene1");
        _streetBase = _am.LoadTexture("street-base");
        int streetLevel = WindowHeight - _streetBase.Height;
        Rectangle playArea = new Rectangle(0, 0, WindowWidth, WindowHeight);

        LoadBuildings(playArea, streetLevel);

        Texture2D cannonBarrel = _am.LoadTexture("CannonBarrel");
        _player = new Player(   _am.LoadTexture("wizard"),
                                _am.LoadTexture("CannonBase"),
                                cannonBarrel, 
                                streetLevel + 20,
                                WindowWidth);
        _player._boundsTest = _am.LoadTexture("bounds-test");
        _orb = new Orb( _am.LoadTexture("orb-green2"), 
                        _am.LoadTexture("progress-bar-empty"),
                        _am.LoadTexture("progress-bar-tick"),
                        new Vector2(cannonBarrel.Width - 20, -7), 
                        playArea);

        LoadNightmares();
    }

    private void LoadBuildings(Rectangle playArea, int streetLevel)
    {
        string json = File.ReadAllText("Data/buildings.json");
        var buildings = JsonSerializer.Deserialize<List<BuildingData>>(json);

        int rightEdge = playArea.Width;
        foreach (BuildingData data in buildings)
        {
            var building = new Building(_am, data, rightEdge, streetLevel, _nightmares);
            _buildings.Add(building);
            rightEdge = building.Bounds.Left;
        }
    }

    private void LoadNightmares()
    {
        string json = File.ReadAllText("Data/nightmares.json");
        var nightmares = JsonSerializer.Deserialize<List<NightmareData>>(json);

        foreach (NightmareData data in nightmares)
        {
            _nightmares.Add(new Nightmare(_am, data));
        }
    }

    public override void Enter()
    {
        StartNightmare();
    }

    public override void HandleInput(GameTime gt)
    {
        if (_ih.KeyDown(Keys.A))
        {
            _player.MoveLeft();
        }
        else if (_ih.KeyDown(Keys.D))
        {
            _player.MoveRight();
        }
        else if (_ih.StartKeyPress(Keys.Space))
        {
            _orb.StartCharging();
        }
        else if (_ih.KeyReleased(Keys.Space))
        {
            _orb.Launch();
        }

        if (_ih.KeyDown(Keys.E))
        {
            _player.RotateBarrelRight();
        }
        else if (_ih.KeyDown(Keys.Q))
        {
            _player.RotateBarrelLeft();
        }


        if (_ih.KeyPressed(Keys.P))
        {
            //_state = GameState.Paused;
        }

    }

    public override void Update(GameTime gt)
    {
        base.Update(gt);

        float delta = (float)gt.ElapsedGameTime.TotalSeconds;
        _newDreamerTimer -= delta;


        if (_newDreamerTimer < 0)
        {
            StartNightmare();
        }

        foreach (Building building in _buildings)
        {
            building.Update(gt);
        }
        _player.Update(gt);
        _orb.Update(gt, _player.CannonData);
        HandleInput(gt);
    }

    private void StartNightmare()
    {
        var buildingIdx = _random.Next(_buildings.Count - 1); // -1 to account for the blank house on every level
        _buildings[buildingIdx].StartNightmare();
        _newDreamerTimer = _newDreamerTimer = NEW_NIGHTMARE_SPAWN;
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.Draw(_background, Vector2.Zero, Color.White);
        DrawStreetBase(sb);
        foreach (Building building in _buildings)
        {
            building.Draw(sb);
        }
        _orb.Draw(sb);
        _player.Draw(sb);
    }

    private void DrawStreetBase(SpriteBatch sb)
    {
        int y = _background.Height - _streetBase.Height;
        for (int x = 0; x < WindowWidth; x += _streetBase.Width)
        {
            sb.Draw(_streetBase, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
