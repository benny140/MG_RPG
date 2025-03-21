using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG.Physical
{
    public class Player
    {
        // Properties
        public Vector2 Position { get; private set; } // Player's position
        public Texture2D Texture { get; private set; } // Player's sprite texture
        public float Speed { get; set; } = 200f; // Movement speed in pixels per second
        public Rectangle Bounds =>
            new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height); // Bounding box for collision

        // Constructor
        public Player(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        // Update method (handles input and movement)
        public void Update(GameTime gameTime)
        {
            // Get the keyboard state
            var keyboardState = Keyboard.GetState();

            // Calculate movement direction
            Vector2 direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.W)) // Move up
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S)) // Move down
                direction.Y += 1;
            if (keyboardState.IsKeyDown(Keys.A)) // Move left
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D)) // Move right
                direction.X += 1;

            // Normalize the direction vector to prevent faster diagonal movement
            if (direction != Vector2.Zero)
                direction.Normalize();

            // Update position based on direction, speed, and elapsed time
            Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Draw method (renders the player)
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
