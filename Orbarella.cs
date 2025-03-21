using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CALIMOE;

namespace Orbarella;

public class Orbarella : Calimoe
{

    public Orbarella()
    {
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _showFPS = false;
        _fallbackTextureSize = 64;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent(); // ensure the AssetManager gets loaded

        var scene1 = new Scene1(_sm, _am, _ih);

        _sm.AddScene(scene1);
        _sm.SwitchScene("scene1");

        _graphics.PreferredBackBufferWidth = scene1.WindowWidth;
        _graphics.PreferredBackBufferHeight = scene1.WindowHeight;
        _graphics.ApplyChanges();

    }

    protected override void Update(GameTime gt)
    {
        _sm.Update(gt);
        base.Update(gt);
    }

    protected override void Draw(GameTime gt)
    {
        if (ClearColour != Color.Transparent)
        {
            GraphicsDevice.Clear(ClearColour);
        }

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _sm.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gt);
    }
}
