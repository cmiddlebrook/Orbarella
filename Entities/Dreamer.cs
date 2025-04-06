using CALIMOE;
using Microsoft.Xna.Framework;
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
        SoothedGood,
        SoothedGreat,
    }

    private const int BASE_DREAM_DURATION = 10;
    private const int BASE_SOOTHE_DURATION = 5;
    private Color _defaultWindowColour = new Color(34, 46, 59);
    private SpriteObject _dream;
    private float _dreamDuration;
    private Vector2 _dreamSpriteOrigin;
    private Vector2 _dreamPosition;
    private float _dreamTimer;
    private float _sootheTimer;
    private DreamState _dreamState = DreamState.None;
    private List<Nightmare> _nightmares;
    private Nightmare _nightmare;
    private SpriteObject _soothedGood;
    private SpriteObject _soothedGreat;
    private SpriteObject _window;

    static Random _random = new Random();

    public bool IsDreaming => _dreamState == DreamState.Dreaming;

    public Dreamer(AssetManager am, Vector2 windowPosition, List<Nightmare> nightmares, int dreamDurationModifier)
    {
        _nightmares = nightmares;

        _window = new SpriteObject(am.LoadTexture("window"), windowPosition, Vector2.Zero, 1.0f);
        _window.Colour = _defaultWindowColour;
        _dreamSpriteOrigin = new Vector2(16, 16); // centered for 32px sprites
        _dreamPosition = new Vector2(_window.Position.X + 11, _window.Position.Y + 19); // offset from the window

        // the sprites for the "happy feedback" need their position adjusted to account 
        // for the scaling up of the size
        _soothedGood = new SpriteObject(am.LoadTexture("good"), _dreamPosition, Vector2.Zero, 1.5f);
        _soothedGood.Origin = _dreamSpriteOrigin;
        //_soothedGood.DrawBounds = true;
        _soothedGreat = new SpriteObject(am.LoadTexture("great"), _dreamPosition, Vector2.Zero, 2f);
        _soothedGreat.Origin = _dreamSpriteOrigin;
        //_soothedGreat.DrawBounds = true;
        _dreamDuration = _random.Next(1, 10) + BASE_DREAM_DURATION + (dreamDurationModifier * 1.2f);
        _dreamTimer = _dreamDuration;
        _sootheTimer = BASE_SOOTHE_DURATION;
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
                        StopNightmare(false, Color.White);
                    }
                    break;
                }


            case DreamState.SoothedGood:
                {
                    _soothedGood.Position -= new Vector2(0, delta * 100);
                    _soothedGood.Scale *= 1.0f - (delta * 0.1f);
                    break;
                }
            case DreamState.SoothedGreat:
                {
                    _soothedGreat.Position -= new Vector2(0, delta * 100);
                    _soothedGreat.Scale *= 1.0f - (delta * 0.1f);
                    break;
                }

            case DreamState.None:
            default:
                {
                    break;
                }


        }

        if (_dreamState == DreamState.SoothedGood || _dreamState == DreamState.SoothedGreat)
        {
            _sootheTimer -= delta;
            if (_sootheTimer <= 0)
            {
                _sootheTimer = BASE_SOOTHE_DURATION;
                _dreamState = DreamState.None;
            }
            //_soothedGood.Update(gt);
            //_soothedGreat.Update(gt);
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

            case DreamState.SoothedGood:
                {
                    _soothedGood.Draw(sb);
                    break;
                }

            case DreamState.SoothedGreat:
                {
                    _soothedGreat.Draw(sb);
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
    }

    public bool StopNightmare(bool isSoothed, Color orbColour)
    {
        _dreamState = DreamState.None;
        _window.Colour = _defaultWindowColour;

        if (isSoothed)
        {
            _dreamState = _nightmare.Colour == orbColour ? DreamState.SoothedGreat : DreamState.SoothedGood;
        }

        return _nightmare.Colour == orbColour;
    }



}

