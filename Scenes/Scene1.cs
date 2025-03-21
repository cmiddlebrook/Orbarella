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
    private Texture2D _cannonSheet;
    private Texture2D _cannonBase;
    private float _cannonScale = 2.5f;
    private Rectangle _cannonBarrelRect;
    private Vector2 _cannonBasePosition;
    private Vector2 _cannonBarrelPosition;
    private Texture2D _wizardSheet;
    private Vector2 _wizardPosition;
    private float _wizardScale = 3.0f;
    private Rectangle _wizardRect;

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
        _streetBase = _am.LoadTexture("StreetBase");
        int streetLevel = WindowHeight - (int)(_streetBase.Height * _houseScale);

        _house = _am.LoadTexture("house4a");
        _housePosition = new Vector2(WindowWidth - (_house.Width * _houseScale), WindowHeight - (_house.Height * _houseScale));
        _wizardSheet = _am.LoadTexture("wizard-sheet");
        _wizardRect = new Rectangle(0, 160, 32, 32);
        _wizardPosition = new Vector2(80, streetLevel - (int)(32 * _wizardScale));
        _cannonSheet = _am.LoadTexture("cannon-split-sheet");
        _cannonBarrelRect = new Rectangle(0, 0, 62, 46);
        _cannonBarrelPosition = new Vector2(_wizardPosition.X + 54, streetLevel - (46 * _cannonScale));
        _cannonBase = _am.LoadTexture("CannonBase");
        _cannonBasePosition = new Vector2(_cannonBarrelPosition.X, streetLevel - (_cannonBase.Height * _cannonScale));
    }

    public override void HandleInput(GameTime gt)
    {

    }

    public override void Update(GameTime gt)
    {
        base.Update(gt);
        HandleInput(gt);
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.Draw(_background, Vector2.Zero, Color.White);
        DrawStreetBase(sb);
        sb.Draw(_wizardSheet, _wizardPosition, _wizardRect, Color.White, 0f, Vector2.Zero, _wizardScale, SpriteEffects.None, 0f);
        sb.Draw(_house, _housePosition, null, Color.White, 0f, Vector2.Zero, _houseScale, SpriteEffects.None, 0f);
        sb.Draw(_cannonSheet, _cannonBarrelPosition, _cannonBarrelRect, Color.White, 0f, Vector2.Zero, _cannonScale, SpriteEffects.None, 0f);
        sb.Draw(_cannonBase, _cannonBasePosition, null, Color.White, 0f, Vector2.Zero, _cannonScale, SpriteEffects.None, 0f);
    }

    private void DrawStreetBase(SpriteBatch sb)
    {
        int y = _background.Height - (int)(_streetBase.Height * _houseScale);
        for (int x = 0; x < WindowWidth; x += _streetBase.Width)
        {
            sb.Draw(_streetBase, new Vector2(x, y), null, Color.White, 0f, Vector2.Zero, _houseScale, SpriteEffects.None, 0f);
        }
    }
}
