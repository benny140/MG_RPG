using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG.Physical;

public class Projectile
{
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Velocity { get; set; }
    public float Speed { get; set; } = 500f;
    public bool IsActive { get; set; }
    public Rectangle Bounds =>
        new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

    // Add these fields to track screen boundaries (image boundary not the actual screen width)
    private int _screenWidth;
    private int _screenHeight;

    public Projectile(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }

    public void Initialize(Texture2D texture, Vector2 position, Vector2 direction)
    {
        Texture = texture;
        Position = position;
        Velocity = direction * Speed;
        IsActive = true;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsActive)
            return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * deltaTime;

        // Use the stored screen dimensions
        if (
            Position.X < 0 - Texture.Width / 2
            || Position.X > _screenWidth - Texture.Width / 2
            || Position.Y < 0 - Texture.Width / 2
            || Position.Y > _screenHeight - Texture.Width / 2
        )
        {
            IsActive = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive)
            return;
        spriteBatch.Draw(Texture, Position, Color.White);
    }
}
