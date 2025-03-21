using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private readonly int screenHeight = 1080;
    private readonly int screenWidth = 1920;

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
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_textureBackground, new Vector2(0, 0), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
