using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbarella;
public class Dreamer
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

    public bool IsDreaming => _isDreaming;

    public Dreamer(SpriteObject window, List<Nightmare> nightmares)
    {
        _window = window;
        _window.Colour = _defaultWindowColour;
        _nightmares = nightmares;
        _dreamDuration = _random.Next(15, 30);
        _dreamTimer = _dreamDuration;
    }

    public void Update(GameTime gt)
    {
        float delta = (float)gt.ElapsedGameTime.TotalSeconds;

        if (_isDreaming)
        {
            _dreamTimer -= delta;
            if (_dreamTimer <= 0)
            {
                _isDreaming = false;
                _window.Colour = _defaultWindowColour;                
            }
        }

    }

    public void Draw(SpriteBatch sb)
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
        var nightmare = _nightmares[_random.Next(_nightmares.Count)];
        _window.Colour = nightmare.Colour;
        var nightmarePos = new Vector2(_window.Position.X - 5, _window.Position.Y +4);
        _dream = new SpriteObject(nightmare.Texture, nightmarePos, Vector2.Zero, 1.0f);
    }


}

