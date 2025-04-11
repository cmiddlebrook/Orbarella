using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CALIMOE;

namespace Orbarella;

public class Orbarella : Calimoe
{

    public Orbarella()
    {
        IsMouseVisible = true;
        ShowFPS = false;
        _fallbackTextureSize = 32;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent(); // ensure the AssetManager gets loaded

        var scene1 = new Scene1(_sm, _ih);

        _windowSize = new Point(scene1.WindowWidth, scene1.WindowHeight);
        _worldSize = _windowSize;
        ApplyResolution(false);

        _sm.AddScene(new Intro(_sm, _ih));
        _sm.AddScene(scene1);
        _sm.SwitchScene("intro");
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

        _sm.Draw(_spriteBatch);

        base.Draw(gt);
    }
}
