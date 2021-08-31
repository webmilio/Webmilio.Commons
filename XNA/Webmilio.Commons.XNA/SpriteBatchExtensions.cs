using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Webmilio.Commons.XNA
{
    public static class SpriteBatchExtensions
    {
        public static void DrawLine(this SpriteBatch spriteBatch, Texture2D pixel, Vector2 begin, Vector2 end, Color color, int thickness = 1)
        {
            Rectangle rect = new((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + thickness, thickness);
            var vect = Vector2.Normalize(begin - end);

            float angle = (float)Math.Acos(Vector2.Dot(vect, -Vector2.UnitX));

            if (begin.Y > end.Y)
                angle = MathHelper.TwoPi - angle;

            spriteBatch.Draw(pixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
        
        public static void DrawRectangle(this SpriteBatch spriteBatch, Texture2D texture, Rectangle rectangle, Color color, int thickness = 1, Color? fillColor = null)
        {
            spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, thickness), color);
            spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, thickness), color);
            spriteBatch.Draw(texture, new Rectangle(rectangle.Left, rectangle.Top, thickness, rectangle.Height), color);
            spriteBatch.Draw(texture, new Rectangle(rectangle.Right, rectangle.Top, thickness, rectangle.Height + thickness), color);

            if (fillColor.HasValue)
                spriteBatch.Draw(texture, new Rectangle(rectangle.Left + thickness, rectangle.Top + thickness, rectangle.Width - thickness, rectangle.Height - thickness), fillColor.Value);
        }
    }
}