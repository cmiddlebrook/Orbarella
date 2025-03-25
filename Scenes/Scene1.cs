using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Orbarella;
public class Scene1 : GameScene
{
    private Texture2D _background;
    private Texture2D _streetBase;
    private Texture2D _house;
    private Vector2 _housePosition;
    private float _houseScale = 4.0f;
    private Player _player;
    private Orb _orb;

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

        _house = _am.LoadTexture("house4");
        _housePosition = new Vector2(WindowWidth - (_house.Width * _houseScale), 
            WindowHeight - _streetBase.Height - (_house.Height * _houseScale));
        _player = new Player(   _am.LoadTexture("wizard"),
                                _am.LoadTexture("CannonBase"), 
                                _am.LoadTexture("CannonBarrel"), 
                                streetLevel,
                                WindowWidth);
        _player._boundsTest = _am.LoadTexture("bounds-test");
        _orb = new Orb(_am.LoadTexture("orb-green"), _player);
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
        _player.Update(gt);
        _orb.Update(gt);
        HandleInput(gt);
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.Draw(_background, Vector2.Zero, Color.White);
        DrawStreetBase(sb);
        sb.Draw(_house, _housePosition, null, Color.White, 0f, Vector2.Zero, _houseScale, SpriteEffects.None, 0f);
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
