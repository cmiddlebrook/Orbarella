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

namespace Orbarella;
public class Scene1 : GameScene
{
    private Texture2D _background;
    private Texture2D _streetBase;
    private List<Building> _buildings = new List<Building>();
    private Player _player;
    private Orb _orb;
    private SpriteObject _nightmare;

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

        CreateBuildings(playArea, streetLevel);

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

        _nightmare = new SpriteObject(_am.LoadTexture("anger"), new Vector2(500, 200), Vector2.Zero, 1.0f);
    }

    private void CreateBuildings(Rectangle playArea, int streetLevel)
    {
        string json = File.ReadAllText("Data/buildings.json");
        List<BuildingData> buildings = JsonSerializer.Deserialize<List<BuildingData>>(json);

        int rightEdge = playArea.Width;
        foreach (BuildingData bd in buildings)
        {
            Building building = new Building(   _am.LoadTexture(bd.Name),
                                                _am.LoadTexture("window"),
                                                rightEdge, 
                                                streetLevel, 
                                                bd.Windows);
            _buildings.Add(building);
            rightEdge = building.Bounds.Left;
        }
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
        foreach (Building building in _buildings)
        {
            building.Update(gt);
        }
        _player.Update(gt);
        _orb.Update(gt, _player.CannonData);
        HandleInput(gt);
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
        //_nightmare.Draw(sb);
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
