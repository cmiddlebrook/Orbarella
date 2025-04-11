using CALIMOE;
using Microsoft.Xna.Framework;

namespace Orbarella;
public class Nightmare
{
    private string Name { get; }
    public string TexturePath { get; }
    public Color Colour { get; }

    public Nightmare(NightmareData data)
    {
        Name = data.Name;
        TexturePath = data.Texture;
        Colour = new Color(data.Colour[0], data.Colour[1], data.Colour[2]);
    }
}
