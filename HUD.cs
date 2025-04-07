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
    private EndLevelState _endLevelState = EndLevelState.InPlay;
    private NightmareMeter _nightmareMeter;
    private int _numScorePanels = 1;
    private int _scoreDigitWidth = 24;
    private Color _panelTextColour;
    private float _panelScale = 1.8f;
    private int _score = 0;
    private Vector2 _scorePanelMiddlePosition;

    private TextObject _clockText;
    private TextObject _mainPanelText;
    private TextObject _subPanelText;
    private TextObject _scoreText;

    private SpriteObject _clockPanel1;
    private SpriteObject _clockPanel2;
    private SpriteObject _mainPanel;
    private SpriteObject _scorePanelLeft;
    private SpriteObject _scorePanelRight;
    private Texture2D _scorePanelMiddle;

    public int Score
    {
        set
        {
            _score = value;
            var scoreString = _score.ToString();
            _scoreText.Text = "Score: " + scoreString;
            _numScorePanels = scoreString.Length;
            _scorePanelRight.Position = new Vector2(_scorePanelLeft.Bounds.Right + ((_numScorePanels + 1) * _scoreDigitWidth), 12);
        }
    }

    public float NightmareLevel
    {
        set => _nightmareMeter.SetLevel(value);
    }

    public HUD(AssetManager am, Clock clock)
    {
        _panelTextColour = new Color(30, 30, 60);

        _clock = clock;
        _clockPanel1 = new SpriteObject(am.LoadTexture("ui-panel-left"), new Vector2(544, 12), Vector2.Zero, _panelScale);
        _clockPanel2 = new SpriteObject(am.LoadTexture("ui-panel-right"), new Vector2(598, 12), Vector2.Zero, _panelScale);
        _clockText = new TextObject(am.LoadFont("Score"));
        _clockText.CenterHorizontal(18);
        _clockText.Colour = _panelTextColour;

        _mainPanel = new SpriteObject(am.LoadTexture("ui-panel-full4"), new Vector2(216, 150), Vector2.Zero, 6f);
        _mainPanelText = new TextObject(am.LoadFont("EndPanels"), "Game Over!");
        _mainPanelText.Colour = _panelTextColour;
        _mainPanelText.ConfigureShadow(3, Color.LightGray);
        _mainPanelText.CenterHorizontal(200);

        _nightmareMeter = new NightmareMeter(am);

        _subPanelText = new TextObject(am.LoadFont("EndPanels"), "Press ENTER to continue");
        _subPanelText.Colour = Color.LightGray;
        _subPanelText.Scale = 0.4f;
        _subPanelText.ConfigureShadow(3, Color.Black);
        _subPanelText.CenterHorizontal(350);

        _scorePanelLeft = new SpriteObject(am.LoadTexture("ui-panel-left"), new Vector2(40, 12), Vector2.Zero, _panelScale);
        _scorePanelRight = new SpriteObject(am.LoadTexture("ui-panel-right"), new Vector2(152, 12), Vector2.Zero, _panelScale);
        _scorePanelMiddle = am.LoadTexture("ui-panel-middle");
        _scorePanelMiddlePosition = new Vector2(_scorePanelLeft.Bounds.Right + 1, 12);
        _scoreText = new TextObject(am.LoadFont("Score"), "Score: 0");
        _scoreText.Position = new Vector2(52, 18);
        _scoreText.Colour = _panelTextColour;

    }

    public void Update(GameTime gt)
    {
        _clockText.Text = _clock.GameTime.ToString(@"hh\:mm");
        _clockText.Update(gt);
        _scoreText.Update(gt);

        if (_endLevelState != EndLevelState.InPlay)
        {
            _mainPanelText.Update(gt);
        }

        if (_endLevelState == EndLevelState.LevelComplete)
        {
            _subPanelText.Update(gt);
        }
    }

    public void Play()
    {
        _endLevelState = EndLevelState.InPlay;
    }
    public void WinLevel()
    {
        _endLevelState = EndLevelState.LevelComplete;
        _mainPanelText.Text = "Next Level!";
    }

    public void LoseGame()
    {
        _endLevelState = EndLevelState.GameLost;
        _mainPanelText.Text = "Game Over!";
        _subPanelText.Text = "Final Score: " + _score.ToString();
    }

    public void WinGame()
    {
        _endLevelState = EndLevelState.GameWon;
        _mainPanelText.Text = " YOU WIN!";
        _subPanelText.Text = "Final Score: " + _score.ToString();
    }

    public void Draw(SpriteBatch sb)
    {
        _clockPanel1.Draw(sb);
        _clockPanel2.Draw(sb);
        _clockText.Draw(sb);

        _scorePanelLeft.Draw(sb);

        for (int i = 0; i < _numScorePanels; i++)
        {
            var panelOffset = new Vector2(i * _scoreDigitWidth, 0);
            sb.Draw(_scorePanelMiddle, _scorePanelMiddlePosition + panelOffset, null, Color.White, 0f, 
                Vector2.Zero, _panelScale, SpriteEffects.None, 0);
        }

        _scorePanelRight.Draw(sb);
        _scoreText.Draw(sb);

        _nightmareMeter.Draw(sb);

        switch (_endLevelState)
        {
            case EndLevelState.GameLost:
            case EndLevelState.GameWon:
            case EndLevelState.LevelComplete:
                {
                    _mainPanel.Draw(sb);
                    _mainPanelText.Draw(sb);
                    _subPanelText.Draw(sb);
                    break;
                }
            case EndLevelState.InPlay:
            default:
                break;
        }

    }
}
