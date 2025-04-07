using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbarella;
public class Dreamer : GameObject
{
    private enum DreamState
    {
        None,
        Dreaming,
        Soothed,
    }

    private const int BASE_DREAM_DURATION = 10;
    private const int BASE_SOOTHED_DURATION = 5;

    private AssetManager _am;
    private Color _defaultWindowColour = new Color(34, 46, 59);
    private SpriteObject _dream;
    private float _dreamDuration;
    private Vector2 _dreamSpriteOrigin;
    private Vector2 _dreamPosition;
    private float _dreamTimer;
    private SpriteObject _happyFeedback = null;
    private SoundEffect _soothedGood;
    private SoundEffect _soothedGreat;
    private float _soothedTimer;
    private DreamState _dreamState = DreamState.None;
    private List<Nightmare> _nightmares;
    private Nightmare _nightmare;
    private SpriteObject _window;

    static Random _random = new Random();

    public bool IsDreaming => _dreamState == DreamState.Dreaming;

    public Dreamer(AssetManager am, Vector2 windowPosition, List<Nightmare> nightmares, int dreamDurationModifier)
    {
        _am = am;
        _nightmares = nightmares;

        _window = new SpriteObject(_am.LoadTexture("window"), windowPosition, Vector2.Zero, 1.0f);
        _window.Colour = _defaultWindowColour;
        _dreamSpriteOrigin = new Vector2(16, 16); // centered for 32px sprites
        _dreamPosition = new Vector2(_window.Position.X + 11, _window.Position.Y + 19); // offset from the window
        _dreamDuration = _random.Next(1, 10) + BASE_DREAM_DURATION + (dreamDurationModifier * 1.2f);
        _dreamTimer = _dreamDuration;
        _soothedTimer = BASE_SOOTHED_DURATION;
        _soothedGood = _am.LoadSoundFx("soothed-good");
        _soothedGreat = _am.LoadSoundFx("soothed-great");
    }

    protected override void UpdateBounds()
    {
        Bounds = _window.Bounds;
    }
    public override void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        switch (_dreamState)
        {
            case DreamState.Dreaming:
                {
                    _dreamTimer -= delta;
                    if (_dreamTimer <= 0)
                    {
                        StopNightmare();
                    }
                    break;
                }


            case DreamState.Soothed:
                {
                    _happyFeedback.Position -= new Vector2(0, delta * 100);
                    _happyFeedback.Scale *= 1.0f - (delta * 0.1f);

                    _soothedTimer -= delta;
                    if (_soothedTimer <= 0)
                    {
                        _soothedTimer = BASE_SOOTHED_DURATION;
                        _dreamState = DreamState.None;
                    }
                    break;
                }


            case DreamState.None:
            default:
                {
                    break;
                }


        }

        base.Update(gt);
    }

    public override void Draw(SpriteBatch sb)
    {
        _window.Draw(sb);

        switch (_dreamState)
        {
            case DreamState.Dreaming:
                {
                    _dream.Draw(sb);
                    break;
                }

            case DreamState.Soothed:
                {
                    _happyFeedback.Draw(sb);
                    break;
                }

            case DreamState.None:
            default:
                {
                    break;
                }
        }


        base.Draw(sb);
    }

    public void StartNightmare()
    {
        _dreamState = DreamState.Dreaming;
        _dreamTimer = _dreamDuration;
        _nightmare = _nightmares[_random.Next(_nightmares.Count)];
        _window.Colour = _nightmare.Colour;
        _dream = new SpriteObject(_nightmare.Texture, _dreamPosition, Vector2.Zero, 1.0f);
        _dream.Origin = _dreamSpriteOrigin;
        _happyFeedback = null;
    }

    protected void StopNightmare()
    {
        _dreamState = DreamState.None;
        _window.Colour = _defaultWindowColour;
    }

    public bool SootheNightmare(Color orbColour)
    {
        _dreamState = DreamState.Soothed;
        _window.Colour = _defaultWindowColour;
        _soothedTimer = BASE_SOOTHED_DURATION;
        var isCorrectColour = orbColour == _nightmare.Colour;
        var happySprite = isCorrectColour ? "great" : "good";
        var happyScale = isCorrectColour ? 2f : 1.5f;
        var sound = isCorrectColour ? _soothedGreat : _soothedGood;
        sound.Play();
        _happyFeedback = new SpriteObject(_am.LoadTexture(happySprite), _dreamPosition, Vector2.Zero, happyScale);
        _happyFeedback.Origin = _dreamSpriteOrigin;
        _dream = null;

        return isCorrectColour;
    }





}

