using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Orbarella;
public class Scene1 : GameScene
{
    private Texture2D _background;
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
    }
}
