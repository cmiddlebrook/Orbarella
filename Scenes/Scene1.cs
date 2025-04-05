using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace Orbarella;
public class Scene1 : GameScene
{
    private enum PlayState
    {
        InPlay,
        LevelCompleted,
        StartNewLevel,
        GameWon,
        GameLost,
        Paused,
    }


    // sound
    private Song _cityAmbience;

    // graphics
    private Texture2D _background;
    private Texture2D _streetBase;
    Rectangle _playArea;
    int _streetLevel;

    // systems
    private HUD _hud;
    private NightmareSystem _nightmareSystem = new NightmareSystem();

    // data
    private Dictionary<string, BuildingData> _buildingDict;
    private List<Building> _buildingList = new List<Building>();
    private List<Nightmare> _nightmareList = new List<Nightmare>();
    private List<LevelData> _levelList = new List<LevelData>();

    // gameplay
    private Clock _clock;
    private Color _darkTint = new Color(60, 60, 100);
    private int _levelID = 1;
    private Orb _orb;
    private Player _player;
    private PlayState _playState = PlayState.InPlay;
    int _score = 0;

    // statics
    static Random _random = new Random();


    public int WindowWidth => _background.Width;
    public int WindowHeight => _background.Height;

    public Scene1(SceneManager sm, AssetManager am, InputHelper ih)
        : base(sm, am, ih)
    {
        _name = "scene1";
        _clearColour = new Color(0x10, 0x10, 0x10);
        _clock = new Clock(TimeSpan.FromMinutes(6), TimeSpan.FromHours(0), TimeSpan.FromHours(6));
    }


    public override void LoadContent()
    {
        _cityAmbience = _am.LoadMusic("city-ambience");

        _background = _am.LoadTexture("Scene1");
        _streetBase = _am.LoadTexture("street-base");
        _streetLevel = WindowHeight - _streetBase.Height;
        _playArea = new Rectangle(0, 0, WindowWidth, WindowHeight); // window size set from background, so load that first!

        // keep these loads positioned here!
        LoadData();
        LoadLevel();

        _hud = new HUD(_am, _clock);
        _player = new Player(_am, _streetLevel + 20, WindowWidth);
        _orb = new Orb(_am, new Vector2(64, -7), _playArea, _nightmareList);
    }

    private void LoadData()
    {
        var nightmares = JsonSerializer.Deserialize<List<NightmareData>>(File.ReadAllText("Data/nightmares.json"));
        _nightmareList = nightmares.Select(data => new Nightmare(_am, data)).ToList();

        var buildingList = JsonSerializer.Deserialize<List<BuildingData>>(File.ReadAllText("Data/buildings.json"));
        _buildingDict = buildingList.ToDictionary(b => b.Name);

        _levelList = JsonSerializer.Deserialize<List<LevelData>>(File.ReadAllText("Data/levels.json")).ToList();
    }

    private void LoadLevel()
    {
        var level = _levelList.Find(l => l.ID == _levelID);
        int rightEdge = _playArea.Width;
        _buildingList.Clear();
        foreach (string houseName in level.Buildings)
        {
            var buildingData = _buildingDict[houseName];
            var building = new Building(_am, buildingData, rightEdge, _streetLevel, _nightmareList);
            _buildingList.Add(building);
            rightEdge = building.Bounds.Left;
        }
        _nightmareSystem.LoadLevel(_buildingList, level.NightmareSpawnRate, level.MaxNightmareFactor);
        _clock.Reset();
    }


    public override void Enter()
    {
        _nightmareSystem.StartNightmare();
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_cityAmbience);
    }

    public override void HandleInput(GameTime gt)
    {
        if (_playState == PlayState.InPlay)
        {
            if (_ih.KeyPressed(Keys.Down))
            {
                _orb.SelectNextColour();
            }
            else if (_ih.KeyPressed(Keys.Up))
            {
                _orb.SelectPreviousColour();
            }
            else if (_ih.KeyDown(Keys.A))
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
        }

        if (_playState == PlayState.LevelCompleted && _ih.KeyPressed(Keys.Enter))
        {
            _playState = PlayState.StartNewLevel;
        }


        if (_ih.KeyPressed(Keys.P))
        {
            if (_playState == PlayState.Paused)
            {
                MediaPlayer.Resume();
                _playState = PlayState.InPlay;
            }
            else
            {
                MediaPlayer.Pause();
                _playState = PlayState.Paused;
            }
        }

    }

    public override void Update(GameTime gt)
    {
        if (_playState != PlayState.InPlay)
        {
            _clock.Paused = true;
        }
        _clock.Update(gt);

        switch (_playState)
        {
            case PlayState.InPlay:
                {
                    _clock.Paused = false;
                    if (_clock.Finished)
                    {
                        _clock.Paused = true;
                        _levelID++;
                        _playState = (_levelID > _levelList.Count) ? PlayState.GameWon : PlayState.LevelCompleted;
                    }

                    _nightmareSystem.Update(gt);

                    HandleCollisions();

                    if (_nightmareSystem.CityLevel >= 1.0f)
                    {
                        _playState = PlayState.GameLost;
                    }

                    _player.Update(gt);
                    _orb.Update(gt, _player.CannonData);
                    
                    break;
                }

            case PlayState.StartNewLevel:
                {
                    LoadLevel();
                    _orb.Reload();
                    _hud.Play();
                    _playState = PlayState.InPlay;
                    break;
                }

            case PlayState.GameLost:
                {
                    _hud.LoseGame();
                    break;
                }
            case PlayState.LevelCompleted:
                {
                    _hud.WinLevel();
                    break;
                }
            case PlayState.GameWon:
                {
                    _hud.WinGame();
                    break;
                }
            case PlayState.Paused:
            default:
                break;
        }

        _hud.NightmareLevel = _nightmareSystem.CityLevel;
        _hud.Update(gt);

        HandleInput(gt);

        base.Update(gt);
    }



    private void HandleCollisions()
    {
        if (_orb.State != Orb.OrbState.InFlight) return;

        foreach (Building building in _buildingList)
        {
            if (_orb.Bounds.Intersects(building.Bounds))
            {
                var (isCollision, isCorrectColour) = building.HandleCollisions(_orb);
                if (isCollision)
                {
                    _orb.Reload();
                    _score += isCorrectColour ? 100 : 10;
                    _hud.Score = _score;
                }
            }
        }
    }


    public override void Draw(SpriteBatch sb)
    {
        var darkToLightTint = Color.Lerp(_darkTint, Color.White, _clock.Progress);
        //var darkToLightTint = Color.Lerp(Color.LightGray, Color.White, _clock.Progress); // for testing!

        sb.Draw(_background, Vector2.Zero, darkToLightTint);
        DrawStreetBase(sb, darkToLightTint);

        foreach (Building building in _buildingList)
        {
            building.Colour = darkToLightTint;
            building.Draw(sb);
        }

        _hud.Draw(sb);
        _orb.Draw(sb);
        _player.Draw(sb);

    }

    private void DrawStreetBase(SpriteBatch sb, Color timeTint)
    {
        int y = _background.Height - _streetBase.Height;
        for (int x = 0; x < WindowWidth; x += _streetBase.Width)
        {
            sb.Draw(_streetBase, new Vector2(x, y), null, timeTint, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
