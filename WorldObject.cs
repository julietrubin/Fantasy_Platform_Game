using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpTest
{
    public class WorldObject
    {
        public Texture2D mTexture { get; set; }
        public Vector2 mPosition;
        public Color color = Color.White;
        public bool above;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public int highestJump = 800;
        

        public WorldObject(Texture2D texture)
        {
            mTexture = texture;
            above = false;
        }

        public WorldObject()
        {
            above = false;
        } 

        public WorldObject(Texture2D texture, Color col)
        {
            mTexture = texture;
            color = col;
            above = false;
        }

        public int Width
        {
            get
            {
                if (mTexture == null)
                {
                    return 0;
                }
                return (int)mTexture.Width;
            }
        }

        public int Height
        {
            get
            {
                if (mTexture == null)
                {
                    return 0;
                }
                return (int)mTexture.Height;
            }
        }


        public Vector2 Position
        {
            get
            {
                return mPosition;
            }
            set
            {
                mPosition = value;
            }
        }

        public int Top
        {
            get
            {
                return (int)mPosition.Y;
            }
        }
        public int Bottom
        {
            get
            {
                return (int)mPosition.Y + Height;
            }
        }
        public int Left
        {
            get
            {
                return (int)mPosition.X;
            }
        }
        public int Right
        {
            get
            {
                return (int)mPosition.X + Width;
            }
        }
        public int Middle
        {
            get
            {
                return (int)(mPosition.X + (Width / 2));
            }
        }
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + (Width/2),Position.Y + Height/2);
            }
        }
        public bool Intersects(WorldObject wo)
        {
            if ((wo.Right > Left && Right > wo.Left)
                && wo.Top < Bottom && Top < wo.Bottom)
            {
                return true; 
            }
            return false; 
            
        }

        public void Draw(SpriteBatch sb, Vector2 screenTop)
        {
            sb.Draw(mTexture, Position + screenTop, color);
        }
    }
}