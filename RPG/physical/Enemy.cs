using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG.Physical;

public class Enemy
{
    // Texture and position
    private readonly Texture2D _spriteSheet;
    private Vector2 _position;

    // Animation properties
    private int _currentFrame;
    private double _frameTimer;
    private const double FrameDelay = 0.1; // seconds per frame
    private int FrameCount; // number of animation frames
    private int FrameWidth;
    private int FrameHeight;

    // Movement properties
    private float _speed = 100f;
    private Vector2 _size => new Vector2(FrameWidth, FrameHeight);

    // Reference to the player
    private Player _player;

    public Enemy(Texture2D spriteSheet, Vector2 startingPosition, Player player, int frameCount)
    {
        _spriteSheet = spriteSheet;
        _position = startingPosition;
        _player = player;
        _currentFrame = 0;
        _frameTimer = 0;

        // Set animation properties
        FrameCount = frameCount;

        // Calculate frame dimensions (must be stored in fields/properties)
        FrameWidth = _spriteSheet != null ? _spriteSheet.Width / FrameCount : 0;
        FrameHeight = _spriteSheet?.Height ?? 0;
    }

    public void Update(GameTime gameTime)
    {
        // Calculate direction to player
        Vector2 direction = _player.Position - _position - _size / 2;
        bool isMoving = direction != Vector2.Zero;

        // Only animate when moving
        if (isMoving)
        {
            direction.Normalize();

            // Update animation
            _frameTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_frameTimer >= FrameDelay)
            {
                _frameTimer = 0;
                _currentFrame = (_currentFrame + 1) % FrameCount;
            }

            // Move towards player
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _position += direction * _speed * deltaTime;
        }
        else
        {
            // Reset to first frame when not moving
            _currentFrame = 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_spriteSheet != null)
        {
            // Calculate source rectangle for current frame
            Rectangle sourceRect = new Rectangle(
                _currentFrame * FrameWidth,
                0, // Assuming all animation frames are in one row
                FrameWidth,
                FrameHeight
            );

            spriteBatch.Draw(
                _spriteSheet,
                _position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }
}
