using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbarella;
public class Orb
{
    private enum OrbState
    {
        ReadyPosition,
        InFlight,
    }
    private SpriteObject _orb;
    private OrbState _orbState = OrbState.ReadyPosition;
    private Player _player;

    public Orb(Texture2D orb, Player player)
    {
        _player = player;
        _orb = new SpriteObject(orb, player.OrbPosition, Vector2.Zero, 1.0f);
        _orb.Origin = new Vector2(orb.Width / 2, orb.Height / 2);
    }

    public void Update(GameTime gt)
    {
        _orb.Position = _player.OrbPosition;
    }

    public void Draw(SpriteBatch sb)
    {
        _orb.Draw(sb);
    }

}
