using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace JumpTest
{
    public class Platform : Microsoft.Xna.Framework.GameComponent
    {
        private World mWorld;
        private Constants mConstants;
        private Random rand; 

        private List<WorldObject> mPlatform;
        private List<WorldObject> movingPlatform; 
        private List<WorldObject> coins;

        private List<Texture2D> mPlatformTextures;
        private Texture2D coinTexture;
        private Texture2D moving; 

        private int lastXPlatform;
        private int lastYPlatform;

        private Vector2 platformSpeed;
       
        

        public Platform(Game game, Constants c)
            : base(game)
        {
            mConstants = c;
            rand = new Random();
            mPlatformTextures = new List<Texture2D>();
            platformSpeed = mConstants.PlatformSpeed;
         
        }

        public override void Initialize()
        {
            mPlatform = new List<WorldObject>();
            movingPlatform = new List<WorldObject>();
            coins = new List<WorldObject>();
            
           
            base.Initialize();
        }

        public void LoadContent()
        {
            coinTexture = Game.Content.Load<Texture2D>(mConstants.Coin);
            moving = Game.Content.Load<Texture2D>(mConstants.MovingPlatform);
          
            foreach (String textureName in mConstants.PlatformTextures)
                mPlatformTextures.Add(Game.Content.Load<Texture2D>(textureName));
            load();
        }

        public void load()
        {
          lastXPlatform = 0;
          lastYPlatform = rand.Next(550, 610) - mWorld.groundHeight;
          while (addPlatform(new WorldObject(mPlatformTextures[rand.Next(0, 2)],
                   Color.Pink))) ;
          for (int i = 0; i < 4; i++)
          {
              movingPlatform.Add(new WorldObject(moving, Color.Pink));
          }
          mWorld.SetPlatformComponent(mPlatform, movingPlatform, coins);
        }

    

        internal void SetWorldComponent(World world)
        {
            mWorld = world;
            
        }
       

        private bool addPlatform(WorldObject newPlatform)
        {
            lastXPlatform += rand.Next(150, 300);
            int randYGap = rand.Next(90, 200);

            if (lastXPlatform > 10000)
                return false;

            mPlatform.Add(newPlatform);
            newPlatform.Position = new Vector2(lastXPlatform, lastYPlatform);
            addCoin(lastXPlatform, lastYPlatform, newPlatform.Width);
            bool up;
            if (lastYPlatform - randYGap < -mWorld.towerBounds.Bottom / 6)
            {
                up = false;
            }
            else if (lastYPlatform + randYGap > mWorld.towerBounds.Bottom * 5 / 6 - mWorld.groundHeight)
            {
                up = true;
            }
            else
            {
                up = rand.Next(0, 2) == 0;
            }

            randYGap = up ? -randYGap : randYGap;
            lastYPlatform += randYGap; 
            return true;
        }
        private void addCoin(int x, int y, int width)
        {
            WorldObject newCoin = new WorldObject(coinTexture);
            newCoin.Position = new Vector2(x + width/2 - newCoin.Width/2, y - 80);
            coins.Add(newCoin); 
        }

        public override void Update(GameTime gameTime)
        {
            foreach(WorldObject movePlatform in movingPlatform)
            {
                
                if (mWorld.moveRight)
                {
                    if (movePlatform.Position.X + movePlatform.Width < movePlatform.endPosition.X)
                    {
                        
                        movePlatform.Position += platformSpeed;
                    }
                    else
                    {
                        mWorld.moveRight = false;
                    }
                }
                if (!mWorld.moveRight)
                {
                    if (movePlatform.Position.X > movePlatform.startPosition.X)
                    {
                        movePlatform.Position -= platformSpeed;
                    }
                    else
                    {
                        mWorld.moveRight = true; 
                    }
                }
            }
            base.Update(gameTime);
        }
    }
}