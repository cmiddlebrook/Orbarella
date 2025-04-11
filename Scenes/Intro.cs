using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;


namespace Orbarella;

public class Intro : GameScene
{
    private Texture2D _title;
    private Song _introMusic;

    public Intro(SceneManager sm, InputHelper ih)
        : base(sm, ih)
    {
        _name = "intro";
    }

    public override void LoadContent()
    {
        _title = Calimoe.AssetManager.LoadTexture("TitleScreen");
        _introMusic = Calimoe.AssetManager.LoadMusic("Empty Streets");
    }

    public override void Enter()
    {
        MediaPlayer.Play(_introMusic);
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Volume = 0.5f;
    }

    public override void Exit()
    {
        MediaPlayer.Stop();
    }

    public override void HandleInput()
    {
        if (_ih.KeyPressed(Keys.Space) || _ih.KeyPressed(Keys.Enter))
        {
            _sm.SwitchScene("scene1");
        }
    }

    public override void Update(GameTime gt)
    {
        HandleInput();

        base.Update(gt);
    }

    public override void Draw(SpriteBatch sb)
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        sb.Draw(_title, Vector2.Zero, Color.White);
        sb.End();

    }

}
