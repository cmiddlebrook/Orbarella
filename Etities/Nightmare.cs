using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Orbarella;
public class Nightmare
{
    private string Name { get; set; }
    private string Texture { get; set; }
    private Color Colour { get; set; }

    public Nightmare(NightmareData data)
    {
        Name = data.Name;
        Texture = data.Texture;
        Colour = new Color(data.Colour[0], data.Colour[1], data.Colour[2]);
    }
}
