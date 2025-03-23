using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG.Physical
{
    public class Player
    {
        private readonly Texture2D _texturePlayer; // Stationary texture
        private readonly Texture2D _textureUp;
        private readonly Texture2D _textureDown;
        private readonly Texture2D _textureLeft;
        private readonly Texture2D _textureRight;
        public Vector2 Position { get; private set; }
        private readonly Rectangle[] _upFrames;
        private readonly Rectangle[] _downFrames;
        private readonly Rectangle[] _leftFrames;
        private readonly Rectangle[] _rightFrames;
        private int _currentFrame;
        private double _timeSinceLastFrame;
        private const double FrameTime = 0.1; // Time between frames in seconds
        private Rectangle _sourceRectangle;
        private Vector2 _origin;
        private readonly int _frameSize;
        private Texture2D _currentTexture;
        private readonly float speed = 500f; // pixels per second

        public Player(
            Texture2D texturePlayer, // Stationary texture
            Texture2D textureUp,
            Texture2D textureDown,
            Texture2D textureLeft,
            Texture2D textureRight,
            Vector2 position,
            int frameSize
        )
        {
            _texturePlayer = texturePlayer;
            _textureUp = textureUp;
            _textureDown = textureDown;
            _textureLeft = textureLeft;
            _textureRight = textureRight;
            Position = position;
            _currentFrame = 0;
            _timeSinceLastFrame = 0;
            _frameSize = frameSize;
            _origin = new Vector2(_frameSize / 2, _frameSize / 2); // Center origin

            // Initialize the frames for each direction
            _upFrames = new Rectangle[4];
            _downFrames = new Rectangle[4];
            _leftFrames = new Rectangle[4];
            _rightFrames = new Rectangle[4];

            for (int i = 0; i < 4; i++)
            {
                _upFrames[i] = new Rectangle(i * _frameSize, 0, _frameSize, _frameSize);
                _downFrames[i] = new Rectangle(i * _frameSize, 0, _frameSize, _frameSize);
                _leftFrames[i] = new Rectangle(i * _frameSize, 0, _frameSize, _frameSize);
                _rightFrames[i] = new Rectangle(i * _frameSize, 0, _frameSize, _frameSize);
            }

            _sourceRectangle = new Rectangle(0, 0, _frameSize, _frameSize); // Default frame
            _currentTexture = _texturePlayer; // Default to stationary texture
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Handle movement and animation
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                SetPosition(Position.X, Position.Y - speed * deltaTime);
                Animate(gameTime, _upFrames);
                _currentTexture = _textureUp;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                SetPosition(Position.X, Position.Y + speed * deltaTime);
                Animate(gameTime, _downFrames);
                _currentTexture = _textureDown;
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                SetPosition(Position.X - speed * deltaTime, Position.Y);
                Animate(gameTime, _leftFrames);
                _currentTexture = _textureLeft;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                SetPosition(Position.X + speed * deltaTime, Position.Y);
                Animate(gameTime, _rightFrames);
                _currentTexture = _textureRight;
            }
            else
            {
                // Reset to the stationary texture and frame if no movement
                _currentFrame = 0;
                _sourceRectangle = new Rectangle(0, 0, _frameSize, _frameSize); // Stationary frame
                _currentTexture = _texturePlayer; // Use stationary texture
            }
        }

        private void Animate(GameTime gameTime, Rectangle[] frames)
        {
            _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastFrame >= FrameTime)
            {
                _currentFrame++;
                if (_currentFrame >= frames.Length)
                {
                    _currentFrame = 0;
                }

                _sourceRectangle = frames[_currentFrame];
                _timeSinceLastFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _currentTexture, // Use the current texture
                Position,
                _sourceRectangle,
                Color.White,
                0f,
                _origin,
                1f,
                SpriteEffects.None,
                0f
            );
        }
    }
}
