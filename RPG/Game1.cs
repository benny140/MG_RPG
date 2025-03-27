using Dcrew.Camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG.Physical;

namespace RPG;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly int screenHeight = 1080;
    private readonly int screenWidth = 1920;

    // Declare Camera
    private Camera _camera;

    // Get Player Textures
    private Texture2D _texturePlayer;
    private Texture2D _textureWalkDown;
    private Texture2D _textureWalkUp;
    private Texture2D _textureWalkLeft;
    private Texture2D _textureWalkRight;

    // Get Other Textures
    private Texture2D _textureBackground;
    private Texture2D _textureBall;
    private Texture2D _textureSkull;

    // Declare Physical Objects
    private Player _player;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = screenWidth; // Width of the screen
        _graphics.PreferredBackBufferHeight = screenHeight; // Height of the screen
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load Textures
        _texturePlayer = Content.Load<Texture2D>("Player/player");
        _textureWalkDown = Content.Load<Texture2D>("Player/walkDown");
        _textureWalkUp = Content.Load<Texture2D>("Player/walkUp");
        _textureWalkLeft = Content.Load<Texture2D>("Player/walkLeft");
        _textureWalkRight = Content.Load<Texture2D>("Player/walkRight");
        _textureBackground = Content.Load<Texture2D>("background");
        _textureBall = Content.Load<Texture2D>("ball");
        _textureSkull = Content.Load<Texture2D>("skull");

        // Load the Player
        _player = new(
            _texturePlayer,
            _textureWalkUp,
            _textureWalkDown,
            _textureWalkLeft,
            _textureWalkRight,
            new Vector2(100, 100),
            96
        );

        // Load the Projectiles
        _player.LoadProjectileContent(_textureBall, 2496, 2496);

        // Initialize the camera
        _camera = new Camera(Vector2.Zero, 0, Vector2.One);
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        // Update the player
        _player.Update(gameTime);

        // Camera Start ==============================================================================
        // Update the camera to follow the player (with deadzone)
        Vector2 playerPosition = _player.Position;
        Vector2 cameraPosition = _camera.XY;

        // Define deadzone bounds (adjust values as needed)
        float deadzoneWidth = 320f; // Horizontal deadzone size
        float deadzoneHeight = 180f; // Vertical deadzone size

        // Calculate player's distance from camera center
        float deltaX = playerPosition.X - cameraPosition.X;
        float deltaY = playerPosition.Y - cameraPosition.Y;

        // X-axis deadzone
        if (deltaX > deadzoneWidth)
        {
            cameraPosition.X = playerPosition.X - deadzoneWidth;
        }
        else if (deltaX < -deadzoneWidth)
        {
            cameraPosition.X = playerPosition.X + deadzoneWidth;
        }

        // Y-axis deadzone
        if (deltaY > deadzoneHeight)
        {
            cameraPosition.Y = playerPosition.Y - deadzoneHeight;
        }
        else if (deltaY < -deadzoneHeight)
        {
            cameraPosition.Y = playerPosition.Y + deadzoneHeight;
        }

        // Clamp the camera position to the background bounds
        cameraPosition.X = MathHelper.Clamp(cameraPosition.X, 0, _textureBackground.Width);
        cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, 0, _textureBackground.Height);

        _camera.XY = cameraPosition;
        // Camera End ===============================================================================

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin drawing with the camera's view matrix
        _spriteBatch.Begin(transformMatrix: _camera.View());
        _spriteBatch.Draw(_textureBackground, new Vector2(0, 0), Color.White);
        _player.Draw(_spriteBatch);
        _player.DrawProjectiles(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
