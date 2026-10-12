using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Intersect.Client.Framework.Plugins.Interfaces;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace Intersect.Android.Poc;

public sealed class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch? _spriteBatch;
    private Texture2D? _pixel;
    private string _clientFrameworkAssembly = "unresolved";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        _clientFrameworkAssembly = typeof(IClientNetworkHelper).Assembly.GetName().Name ?? "unknown";
        _graphics.IsFullScreen = true;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { XnaColor.White });
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(XnaColor.CornflowerBlue);
        if (_spriteBatch is null || _pixel is null)
        {
            return;
        }

        var viewport = GraphicsDevice.Viewport;
        var banner = new XnaRectangle(
            viewport.Width / 8,
            viewport.Height / 3,
            viewport.Width * 3 / 4,
            viewport.Height / 3
        );
        _spriteBatch.Begin();
        _spriteBatch.Draw(_pixel, banner, XnaColor.DarkGreen);
        _spriteBatch.End();
    }
}
