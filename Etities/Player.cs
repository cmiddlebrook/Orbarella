using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Orbarella;
public class Player
{
    private SpriteObject _wizard;
    private SpriteObject _base;
    private SpriteObject _barrel;
    private float _cannonScale = 2.5f;
    private float _wizardScale = 3.0f;
    private Vector2 _wizardPosition;
    private Vector2 _basePosition;
    private Vector2 _barrelPosition;

    public Player(Texture2D wizard, Texture2D cannonBase, Texture2D cannonBarrel, int streetLevel)
    {
        _wizardPosition = new Vector2(80, streetLevel - (int)(wizard.Width * _wizardScale));
        _wizard = new SpriteObject(wizard, _wizardPosition, Vector2.Zero, _wizardScale);
        _basePosition = new Vector2(_wizardPosition.X + 68, streetLevel - (int)(cannonBase.Height * _cannonScale));
        _base = new SpriteObject(cannonBase, _basePosition, Vector2.Zero, _cannonScale);
        _barrelPosition = new Vector2(_basePosition.X + 7, streetLevel - (int)(cannonBarrel.Height * _cannonScale));
        _barrel = new SpriteObject(cannonBarrel, _barrelPosition, Vector2.Zero, _cannonScale);
    }

    public void Update(GameTime gt)
    {   
        _wizard.Update(gt);
        _base.Update(gt);
        _barrel.Update(gt);
    }

    public void Draw(SpriteBatch sb)
    {
        _wizard.Draw(sb);
        _barrel.Draw(sb);
        _base.Draw(sb);
    }

    public void Reset()
    {
        _wizard.Reset();
        _base.Reset();
        _barrel.Reset();
    }

}
