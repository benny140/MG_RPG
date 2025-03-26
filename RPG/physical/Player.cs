using System;
using System.Collections.Generic;
using System.Linq;
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

        // Projectile Fields
        private Texture2D _projectileTexture;
        private List<Projectile> _projectiles = new List<Projectile>();
        private readonly float _shootCooldown = 0.2f;
        private float _shootTimer = 0f;
        private int _screenHeight;
        private int _screenWidth;
        private Vector2 _lastAimDirection = Vector2.Zero;
        private const float DeadzoneThreshold = 0.2f;

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

        public void LoadProjectileContent(
            Texture2D projectileTexture,
            int screenWidth,
            int screenHeight
        )
        {
            _projectileTexture = projectileTexture;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void SetPosition(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var gamePadState = GamePad.GetState(PlayerIndex.One); // For player 1 controller
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Reset movement vector each frame
            Vector2 movement = Vector2.Zero;
            bool isMoving = false;

            // Handle keyboard movement input
            if (keyboardState.IsKeyDown(Keys.Up))
                movement.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.Down))
                movement.Y += 1;
            if (keyboardState.IsKeyDown(Keys.Left))
                movement.X -= 1;
            if (keyboardState.IsKeyDown(Keys.Right))
                movement.X += 1;

            // Handle gamepad movement input (left thumbstick)
            if (gamePadState.IsConnected)
            {
                Vector2 thumbstick = gamePadState.ThumbSticks.Left;
                thumbstick.Y *= -1; // Invert Y axis to match screen coordinates

                // Add controller input with deadzone check (ignore small movements)
                if (thumbstick.Length() > 0.2f)
                {
                    movement += thumbstick;
                }

                // Optional: Add D-pad support
                if (gamePadState.DPad.Up == ButtonState.Pressed)
                    movement.Y -= 1;
                if (gamePadState.DPad.Down == ButtonState.Pressed)
                    movement.Y += 1;
                if (gamePadState.DPad.Left == ButtonState.Pressed)
                    movement.X -= 1;
                if (gamePadState.DPad.Right == ButtonState.Pressed)
                    movement.X += 1;
            }

            // Check if any movement occurred
            isMoving = movement != Vector2.Zero;

            // Normalize and apply movement
            if (isMoving)
            {
                movement.Normalize();
                SetPosition(
                    Position.X + movement.X * speed * deltaTime,
                    Position.Y + movement.Y * speed * deltaTime
                );
            }

            // Handle animation based on primary direction
            if (isMoving)
            {
                // Determine primary direction for animation
                if (Math.Abs(movement.X) > Math.Abs(movement.Y))
                {
                    // Horizontal movement dominant
                    if (movement.X > 0)
                    {
                        Animate(gameTime, _rightFrames);
                        _currentTexture = _textureRight;
                    }
                    else
                    {
                        Animate(gameTime, _leftFrames);
                        _currentTexture = _textureLeft;
                    }
                }
                else
                {
                    // Vertical movement dominant
                    if (movement.Y > 0)
                    {
                        Animate(gameTime, _downFrames);
                        _currentTexture = _textureDown;
                    }
                    else
                    {
                        Animate(gameTime, _upFrames);
                        _currentTexture = _textureUp;
                    }
                }
            }
            else
            {
                // Reset to stationary frame
                _currentFrame = 0;
                _sourceRectangle = new Rectangle(0, 0, _frameSize, _frameSize);
                _currentTexture = _texturePlayer;
            }

            // Projectile Updates
            UpdateShooting(gameTime);
            UpdateProjectiles(gameTime);
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

        private void UpdateShooting(GameTime gameTime)
        {
            _shootTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            var gamePadState = GamePad.GetState(PlayerIndex.One);
            bool tryToShoot = gamePadState.Triggers.Right > 0.5f;

            // Get current right thumbstick input
            Vector2 aimDirection = gamePadState.ThumbSticks.Right;
            aimDirection.Y *= -1; // Invert Y axis if needed

            // Only update last aim direction if input exceeds the deadzone
            if (aimDirection.Length() > DeadzoneThreshold)
            {
                _lastAimDirection = aimDirection;
                _lastAimDirection.Normalize(); // Ensures consistent knockback/shooting
            }

            if (tryToShoot && _shootTimer <= 0)
            {
                // If not actively aiming, use last recorded aim direction (if any)
                if (aimDirection.Length() <= DeadzoneThreshold && _lastAimDirection != Vector2.Zero)
                {
                    aimDirection = _lastAimDirection;
                }

                if (aimDirection != Vector2.Zero)
                {
                    Shoot(aimDirection);
                    _shootTimer = _shootCooldown;
                }
            }
        }

        private void Shoot(Vector2 direction)
        {
            direction.Normalize();
            Vector2 spawnPosition = Position - new Vector2(_frameSize / 2);
            // Position + new Vector2(_frameSize / 2) - new Vector2(_projectileTexture.Width / 2);

            Projectile projectile = _projectiles.FirstOrDefault(p => !p.IsActive);
            if (projectile == null)
            {
                projectile = new Projectile(_screenWidth, _screenHeight);
                _projectiles.Add(projectile);
            }

            projectile.Initialize(_projectileTexture, spawnPosition, direction);
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            foreach (var projectile in _projectiles)
            {
                projectile.Update(gameTime);
            }
        }

        public void DrawProjectiles(SpriteBatch spriteBatch)
        {
            foreach (var projectile in _projectiles.Where(p => p.IsActive))
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
