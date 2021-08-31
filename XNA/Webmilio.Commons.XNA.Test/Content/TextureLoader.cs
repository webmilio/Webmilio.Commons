using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Webmilio.Electronics.Builder.Content;

namespace Webmilio.Commons.XNA.Test.Content
{
    public class TextureLoader : ContentLoader<Texture2D>
    {
        public override void Load(Game game)
        {
            base.Load(game);

            Pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });
        }

        public override string Path { get; } = "Textures";


        public Texture2D MenuBar { get; private set; }

        [Ignore]
        public Texture2D Pixel { get; private set; }
    }
}