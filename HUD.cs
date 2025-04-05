using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbarella;

public class HUD
{
    private enum EndLevelState
    {
        None,
        GameOver,
        LevelComplete
    }
    private Clock _clock;
    private TextObject _clockText;
    private TextObject _endLevelText;
    private EndLevelState _endLevelState = EndLevelState.None;
    private NightmareMeter _nightmareMeter;
    private int _score = 0;
    private TextObject _scoreText;

    public int Score
    {
        set
        {
            _score = value;
            _scoreText.Text = "Score: " + _score.ToString();
        }
    }

    public float NightmareLevel
    {
        set => _nightmareMeter.SetLevel(value);
    }

    public HUD(AssetManager am, Clock clock)
    {
        _clock = clock;
        _clockText = new TextObject(am.LoadFont("Score"));
        _clockText.CenterHorizontal(12);

        _endLevelText = new TextObject(am.LoadFont("Title"), "Game Over!");
        _endLevelText.Colour = Color.Orange;
        _endLevelText.Scale = 3.0f;
        _endLevelText.Shadow = true;
        _endLevelText.CenterBoth();

        _nightmareMeter = new NightmareMeter(am);

        _scoreText = new TextObject(am.LoadFont("Score"), "Score: 0");
        _scoreText.Position = new Vector2(40, 12);

    }

    public void Update(GameTime gt)
    {
        _clockText.Text = _clock.GameTime.ToString(@"hh\:mm");
        _clockText.Update(gt);
        _scoreText.Update(gt);

        if (_endLevelState != EndLevelState.None)
        {
            _endLevelText.Update(gt);
        }
    }

    public void WinLevel()
    {
        _endLevelState = EndLevelState.LevelComplete;
        _endLevelText.Text = "Level Complete!";
    }

    public void LoseLevel()
    {
        _endLevelState = EndLevelState.GameOver;
        _endLevelText.Text = "Game Over!";
    }

    public void Draw(SpriteBatch sb)
    {
        _clockText.Draw(sb);
        _scoreText.Draw(sb);
        _nightmareMeter.Draw(sb);

        if (_endLevelState != EndLevelState.None)
        {
            _endLevelText.Draw(sb);
        }
    }
}
