using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpTest
{
    class ProgressBarSprite : Sprite
    {

        public ProgressBarSprite(Texture2D texture)
        {
            Texture = texture;
            PercentageToShow = 1.0f;
            Alpha = 1.0f;
            Color = Color.White;
            BacgroundColor = Color.Gray;
            Scale = 0f;
            Centered = true;
        }

        public Texture2D Texture { get; set; }
        public float PercentageToShow { get; set; }
        public Color BacgroundColor { get; set; }

        public override Vector2 Size()
        {
            return new Vector2(Texture.Width, Texture.Height);
        }

        public override void Draw(SpriteBatch sb)
        {
            Vector2 origin;
            if (Centered)
            {
                origin = new Vector2(Texture.Width/2.0f, Texture.Height/2.0f);
            }
            else
            {
                origin = Vector2.Zero;
            }
            Rectangle foregroundSourceRect = new Rectangle(0, 0, (int) (Texture.Width * PercentageToShow), Texture.Height);

            sb.Draw(Texture, Position, null, new Color(BacgroundColor, Alpha), Rotation, origin, Scale, SpriteEffects.None, 0);
            sb.Draw(Texture, Position, foregroundSourceRect, new Color(Color, Alpha), Rotation, origin, Scale, SpriteEffects.None, 0);
        }
    }
}
