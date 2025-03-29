using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbarella;
public class Nightmare
{
    private string Name { get; }
    public Texture2D Texture { get; }
    public Color Colour { get; }

    public Color SootheColour { get; }

    public Nightmare(AssetManager am, NightmareData data)
    {
        Name = data.Name;
        Texture = am.LoadTexture(data.Texture);
        Colour = new Color(data.Colour[0], data.Colour[1], data.Colour[2]);
        SootheColour = new Color(255 - data.Colour[0], 255 - data.Colour[1], 255 - data.Colour[2]);
    }
}
