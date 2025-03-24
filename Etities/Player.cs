using CALIMOE;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Orbarella;
public class Player
{
    private enum MoveState
    {
        MovingLeft,
        MovingRight,
        Stopped,
    }

    private SpriteObject _wizard;
    private SpriteObject _barrel;
    private SpriteObject _base;
    private float _wizardScale = 3.0f;
    private float _cannonScale = 2.5f;
    private Vector2 _wizardPosition;
    private Vector2 _basePositionOffset;
    private Vector2 _barrelPositionOffset;
    private Rectangle _bounds;
    private int _rightEdge;
    private MoveState _moveState = MoveState.Stopped;

    private float Speed { get; set; } = 400f;

    public Rectangle Bounds => _bounds;

    public Texture2D _boundsTest {  get; set; }

    public Player(Texture2D wizard, Texture2D cannonBase, Texture2D cannonBarrel, int streetLevel, int rightEdge)
    {
        _rightEdge = rightEdge;
        int wizardHeight = (int)(wizard.Height * _wizardScale);
        _wizardPosition = new Vector2(80, streetLevel - wizardHeight);
        _wizard = new SpriteObject(wizard, _wizardPosition, Vector2.Zero, _wizardScale);
        int barrelHeight = (int)(cannonBarrel.Height * _cannonScale);
        _barrelPositionOffset = new Vector2(74, wizardHeight - barrelHeight);
        _barrel = new SpriteObject(cannonBarrel, _wizardPosition + _barrelPositionOffset, Vector2.Zero, _cannonScale);
        int baseHeight = (int)(cannonBase.Height * _cannonScale);
        _basePositionOffset = new Vector2(82, wizardHeight - baseHeight);
        _base = new SpriteObject(cannonBase, _wizardPosition + _basePositionOffset, Vector2.Zero, _cannonScale);
        int wizardWidth = (int)(wizard.Width * _wizardScale);
        int cannonWidth = (int)(cannonBarrel.Width * _cannonScale);
        _bounds = new Rectangle(_wizard.Bounds.X, _wizard.Bounds.Y, wizardWidth + cannonWidth, wizardHeight);
    }

    public void Update(GameTime gt)
    {
        switch (_moveState)
        {
            case MoveState.MovingLeft:
            {
                _wizardPosition.X = Math.Max(0, (float)(_wizard.Position.X - Speed * gt.ElapsedGameTime.TotalSeconds));
                UpdatePositions();
                _moveState = MoveState.Stopped;
                break;
            }

            case MoveState.MovingRight:
            {
                _wizardPosition.X = Math.Min(_rightEdge - _bounds.Width, 
                    (float)(_wizard.Position.X + Speed * gt.ElapsedGameTime.TotalSeconds));
                UpdatePositions();
                _moveState = MoveState.Stopped;
                break;
            }

            case MoveState.Stopped:
            default:
                {
                    break;
                }
        }
        _wizard.Update(gt);
        _barrel.Update(gt);
        _base.Update(gt);
    }

    private void UpdatePositions()
    {
        _wizard.Position = _wizardPosition;
        _barrel.Position = _wizardPosition + _barrelPositionOffset;
        _base.Position = _wizardPosition + _basePositionOffset;
        _bounds.X = _wizard.Bounds.X;
        _bounds.Y = _wizard.Bounds.Y;
    }


    public void Draw(SpriteBatch sb)
    {
        sb.Draw(_boundsTest, _bounds, Color.White);
        _wizard.Draw(sb);
        _barrel.Draw(sb);
        _base.Draw(sb);
    }

    public void Reset()
    {
        _wizard.Reset();
        _barrel.Reset();
        _base.Reset();
    }

    public void MoveLeft()
    {
        _moveState = MoveState.MovingLeft;
    }

    public void MoveRight()
    {
        _moveState = MoveState.MovingRight;
    }

}
