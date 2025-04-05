using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbarella;

public class HUD
{
    private enum EndLevelState
    {
        InPlay,
        GameLost,
        GameWon,
        LevelComplete
    }
    private Clock _clock;
    private TextObject _clockText;
    private TextObject _endLevelText;
    private EndLevelState _endLevelState = EndLevelState.InPlay;
    private NightmareMeter _nightmareMeter;
    private TextObject _pressSpaceText;
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
        _endLevelText.Scale = 3.2f;
        _endLevelText.Shadow = true;
        _endLevelText.CenterHorizontal(200);

        _nightmareMeter = new NightmareMeter(am);

        _pressSpaceText = new TextObject(am.LoadFont("Title"), "Press Enter to continue");
        _pressSpaceText.Colour = Color.LightGoldenrodYellow;
        _pressSpaceText.Scale = 1.6f;
        _pressSpaceText.ConfigureShadow(3, Color.Black);
        _pressSpaceText.CenterHorizontal(350);

        _scoreText = new TextObject(am.LoadFont("Score"), "Score: 0");
        _scoreText.Position = new Vector2(40, 12);

    }

    public void Update(GameTime gt)
    {
        _clockText.Text = _clock.GameTime.ToString(@"hh\:mm");
        _clockText.Update(gt);
        _scoreText.Update(gt);

        if (_endLevelState != EndLevelState.InPlay)
        {
            _endLevelText.Update(gt);
        }

        if (_endLevelState == EndLevelState.LevelComplete)
        {
            _pressSpaceText.Update(gt);
        }
    }

    public void Play()
    {
        _endLevelState = EndLevelState.InPlay;
    }
    public void WinLevel()
    {
        _endLevelState = EndLevelState.LevelComplete;
        _endLevelText.Text = "Next Level!";
    }

    public void LoseGame()
    {
        _endLevelState = EndLevelState.GameLost;
        _endLevelText.Text = "Game Over!";
    }

    public void WinGame()
    {
        _endLevelState = EndLevelState.GameWon;
        _endLevelText.Text = "You Win!";
    }

    public void Draw(SpriteBatch sb)
    {
        _clockText.Draw(sb);
        _scoreText.Draw(sb);
        _nightmareMeter.Draw(sb);

        switch (_endLevelState)
        {
            case EndLevelState.GameLost:
            case EndLevelState.GameWon:
                {
                    _endLevelText.Draw(sb);
                    break;
                }
            case EndLevelState.LevelComplete:
                {
                    _endLevelText.Draw(sb);
                    _pressSpaceText.Draw(sb);
                    break;
                }
            case EndLevelState.InPlay:
            default:
                break;
        }

    }
}
