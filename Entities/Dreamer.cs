using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbarella;
public class Dreamer : GameObject
{

    private Color _defaultWindowColour = new Color(34, 46, 59);
    private SpriteObject _window;
    private SpriteObject _dream;
    private List<Nightmare> _nightmares;
    private Nightmare _nightmare;
    private bool _isDreaming = false;
    private float _dreamDuration;
    private float _dreamTimer;
    static Random _random = new Random();

    public Rectangle Bounds => _window.Bounds;

    public bool IsDreaming => _isDreaming;

    public Dreamer(SpriteObject window, List<Nightmare> nightmares)
    {
        _window = window;
        _window.Colour = _defaultWindowColour;
        _nightmares = nightmares;
        _dreamDuration = _random.Next(20, 30);
        _dreamTimer = _dreamDuration;
    }

    public override void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        if (_isDreaming)
        {
            _dreamTimer -= delta;
            if (_dreamTimer <= 0)
            {
                StopNightmare(false, false);
            }
        }

    }

    public override void Draw(SpriteBatch sb)
    {
        _window.Draw(sb);
        if (_isDreaming)
        {
            _dream.Draw(sb);
        }
    }

    public void StartNightmare()
    {
        _isDreaming = true;
        _dreamTimer = _dreamDuration;
        _nightmare = _nightmares[_random.Next(_nightmares.Count)];
        _window.Colour = _nightmare.Colour;
        var nightmarePos = new Vector2(_window.Position.X - 5, _window.Position.Y +4);
        _dream = new SpriteObject(_nightmare.Texture, nightmarePos, Vector2.Zero, 1.0f);
    }

    public void StopNightmare(bool isSoothed, bool isCorrectColour)
    {
        _isDreaming = false;
        _window.Colour = _defaultWindowColour;

        if (isSoothed)
        {
            // TODO: display an animation to indicate a positive feedback
            // play some kind of sound
            // prevent this dreamer from starting a new nightmare for a few seconds
        }
    }

    public bool IsColourMatch(Color colour)
    {
        return _nightmare.Colour == colour;
    }


}

