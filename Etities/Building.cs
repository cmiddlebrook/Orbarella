using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Orbarella;
public class Building
{
    private SpriteObject _building;
    private int _streetLevel;
    List<WindowPosition> _windows;

    public Rectangle Bounds => _building.Bounds;

    public Building(Texture2D building, int rightEdge, int streetLevel, List<WindowPosition> windows)
    {
        _streetLevel = streetLevel;
        var position = new Vector2(rightEdge - building.Width, streetLevel - building.Height);
        _building = new SpriteObject(building, position, Vector2.Zero, 1.0f);
    }

    public void Update(GameTime gt)
    {
        _building.Update(gt);
    }

    public void Draw(SpriteBatch sb)
    {
        _building.Draw(sb);
    }
}
