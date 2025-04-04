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
    private enum PlayState
    {
        InPlay,
        GameOver,
        Sunrise,
        Paused,
    }

    private const float NEW_NIGHTMARE_SPAWN = 2.2f;
    private const float CITY_NIGHTMARE_RATE = 1.5f;

    private SoundEffectInstance _cannonRollFx;
    private SoundEffectInstance _cannonRotateFx;
    private Song _cityAmbience;

    private Texture2D _background;
    private Texture2D _streetBase;

    private List<Building> _buildings = new List<Building>();
    private float _cityNightmareLevel;
    private Clock _clock;
    private TextObject _clockText;
    private Color _darkTint = new Color(60, 60, 100);
    private TextObject _gameOverText;
    private float _newDreamerTimer = NEW_NIGHTMARE_SPAWN;
    private float _nightmareIncrement;
    private List<Nightmare> _nightmares = new List<Nightmare>();
    private NightmareMeter _nightmareMeter;
    private int _numResidents;
    private Orb _orb;
    private Player _player;
    private PlayState _playState = PlayState.InPlay;
    int _score = 0;
    private TextObject _scoreText;

    // statics
    static Random _random = new Random();


    public int WindowWidth => _background.Width;
    public int WindowHeight => _background.Height;

    public Scene1(SceneManager sm, AssetManager am, InputHelper ih)
        : base(sm, am, ih)
    {
        _name = "scene1";
        _clearColour = new Color(0x10, 0x10, 0x10);
        _clock = new Clock(TimeSpan.FromMinutes(0.15), TimeSpan.FromHours(0), TimeSpan.FromHours(6));
    }


    public override void LoadContent()
    {
        // audio
        _cannonRollFx = _am.LoadLoopedSoundFx("cannon-roll");
        _cannonRotateFx = _am.LoadLoopedSoundFx("cannon-rotate");
        _cityAmbience = _am.LoadMusic("city-ambience");

        // basic graphics
        _background = _am.LoadTexture("Scene1");
        _streetBase = _am.LoadTexture("street-base");
        int streetLevel = WindowHeight - _streetBase.Height;
        Rectangle playArea = new Rectangle(0, 0, WindowWidth, WindowHeight);

        // text 
        _clockText = new TextObject(_am.LoadFont("Score"));
        _clockText.CenterHorizontal(12);
        _gameOverText = new TextObject(_am.LoadFont("Title"), "Game Over!");
        _gameOverText.Colour = Color.Orange;
        _gameOverText.Scale = 3.0f;
        _gameOverText.Shadow = true;
        _gameOverText.CenterBoth();
        _scoreText = new TextObject(_am.LoadFont("Score"), "Score: 0");
        _scoreText.Position = new Vector2(40, 12);

        // game objects
        _nightmareMeter = new NightmareMeter(_am);

        LoadBuildings(playArea, streetLevel);

        Texture2D cannonBarrel = _am.LoadTexture("CannonBarrel");
        _player = new Player(   _am.LoadTexture("wizard"),
                                _am.LoadTexture("CannonBase"),
                                cannonBarrel, 
                                streetLevel + 20,
                                WindowWidth);
        _player._boundsTest = _am.LoadTexture("bounds-test");

        LoadNightmares();
        _orb = new Orb(_am, new Vector2(cannonBarrel.Width - 20, -7), playArea, _nightmares);

        _nightmareIncrement = _nightmareMeter.NumTicks / _numResidents * CITY_NIGHTMARE_RATE;
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
            _numResidents += building.NumResidents;
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
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_cityAmbience);
    }

    public override void HandleInput(GameTime gt)
    {
        if (_playState != PlayState.Paused)
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

            if (_ih.StartKeyPress(Keys.A) || _ih.StartKeyPress(Keys.D))
            {
                _cannonRollFx.Play();
            }
            else if (_ih.KeyUp(Keys.A) && _ih.KeyUp(Keys.D))
            {
                _cannonRollFx.Stop();
            }

            if (_ih.KeyDown(Keys.E))
            {
                _player.RotateBarrelRight();
            }
            else if (_ih.KeyDown(Keys.Q))
            {
                _player.RotateBarrelLeft();
            }

            if (_ih.StartKeyPress(Keys.E) || _ih.StartKeyPress(Keys.Q))
            {
                _cannonRotateFx.Play();
            }
            else if (_ih.KeyUp(Keys.E) && _ih.KeyUp(Keys.Q))
            {
                _cannonRotateFx.Stop();
            }
        }

        if (_ih.KeyPressed(Keys.P))
        {
            if (_playState == PlayState.Paused)
            {
                MediaPlayer.Resume();
                _clock.Paused = false;
                _playState = PlayState.InPlay;
            }
            else
            {
                MediaPlayer.Pause();
                _clock.Paused = true;
                _playState = PlayState.Paused;
            }
        }

    }

    public override void Update(GameTime gt)
    {
        _clock.Update(gt);

        switch (_playState)
        {
            case PlayState.InPlay:
                {
                    float delta = (float)gt.ElapsedGameTime.TotalSeconds;

                    if (_clock.Finished)
                    {
                        _gameOverText.Text = "Level Complete!";
                        _playState = PlayState.Sunrise;
                    }
                    _newDreamerTimer -= delta;
                    if (_newDreamerTimer < 0)
                    {
                        StartNightmare();
                    }

                    HandleCollisions();

                    _cityNightmareLevel = 0f;
                    foreach (Building building in _buildings)
                    {
                        building.Update(gt);
                        _cityNightmareLevel += (building.NumDreaming * _nightmareIncrement);
                    }
                    _nightmareMeter.SetLevel(_cityNightmareLevel);
                    _nightmareMeter.Update(gt);
                    if (_nightmareMeter.IsFull)
                    {
                        _playState = PlayState.GameOver;
                    }

                    _clockText.Text = _clock.GameTime.ToString(@"hh\:mm");
                    _clockText.Update(gt);
                    _player.Update(gt);
                    _orb.Update(gt, _player.CannonData);
                    _scoreText.Update(gt);
                    
                    break;
                }

            case PlayState.GameOver:
                {
                    _gameOverText.Update(gt);
                    break;
                }
            case PlayState.Sunrise:
                {
                    _gameOverText.Update(gt);
                    break;
                }
            case PlayState.Paused:
            default:
                break;
        }

        HandleInput(gt);

        base.Update(gt);
    }

    private void StartNightmare()
    {
        int numBuildingsTested = 0;
        int numMaxBuildings = _buildings.Count - 1; // -1 to account for the blank house on every level
        bool newNightmare = false;
        while (!newNightmare && numBuildingsTested < numMaxBuildings)
        {
            var buildingIdx = _random.Next(numMaxBuildings); 
            newNightmare = _buildings[buildingIdx].StartNightmare();
            if (newNightmare)
            {
                _newDreamerTimer = _newDreamerTimer = NEW_NIGHTMARE_SPAWN;
                return;
            }
            numBuildingsTested++;
        }
    }

    private void HandleCollisions()
    {
        if (_orb.State != Orb.OrbState.InFlight) return;

        foreach (Building building in _buildings)
        {
            if (_orb.Bounds.Intersects(building.Bounds))
            {
                var (isCollision, isCorrectColour) = building.HandleCollisions(_orb);
                if (isCollision)
                {
                    _orb.Reload();
                    _score += isCorrectColour ? 100 : 10;
                    _scoreText.Text = "Score: " + _score.ToString();
                }
            }
        }
    }


    public override void Draw(SpriteBatch sb)
    {
        var darkToLightTint = Color.Lerp(_darkTint, Color.White, _clock.Progress);

        sb.Draw(_background, Vector2.Zero, darkToLightTint);
        DrawStreetBase(sb, darkToLightTint);

        _scoreText.Draw(sb);
        _clockText.Draw(sb);

        foreach (Building building in _buildings)
        {
            building.Colour = darkToLightTint;
            building.Draw(sb);
        }
        _nightmareMeter.Draw(sb);

        _orb.Draw(sb);
        _player.Draw(sb);

        if (_playState == PlayState.GameOver || _playState == PlayState.Sunrise)
        {
            _gameOverText.Draw(sb);
        }
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
