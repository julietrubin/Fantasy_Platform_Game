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

    public class World : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Random rand;
        private SpriteBatch mSpriteBatch;
        private Constants mConstants;
        public Rectangle towerBounds { get; private set; } 
        public int SCREENHEIGHT{get; private set;}

        private WorldObject mPlayer;
        private List<WorldObject> mPlatform;
        private List<WorldObject> mMovingPlatform; 
        private List<WorldObject> coins;
        private List<WorldObject> mSpikes;
        private List<WorldObject> AI; 
        private WorldObject mCurrentPlatform;
        private Texture2D bloodTexture;
        private WorldObject blood; 
  
       
        public Vector2 cameraPosition;
         public int groundHeight { get; private set; }
        public int score;
       
        private SpriteFont font;
        SoundEffect coinSound;
        SoundEffect landSound;
        SoundEffect fallSound;
        SoundEffect dieSound; 

       List<Texture2D> background;
       List<Rectangle> mainFrame;
       Texture2D ground;
       public int lives;
       bool onMovingPlatform;
       public bool moveRight;

       Texture2D endTexture;
       WorldObject endSpot;
       public int level;
       private bool die;
       private bool die2; 


        List<Rectangle> mainFrameGround;
      
        int topOfBackGround = -1200;
        int frames = 5; 

        public World(Game game, Constants c)
            : base(game)
        {
            mConstants = c;
            level = 1;
            
        }

        public override void Initialize()
        {
            rand = new Random();
            
            cameraPosition = new Vector2(0, 0);
            lives = 3;
            onMovingPlatform = false;
            moveRight = true;
           // score = 0;
            base.Initialize();
            
        }

       protected override void LoadContent()
        {
           mSpriteBatch = new SpriteBatch(Game.GraphicsDevice);
           font = Game.Content.Load<SpriteFont>(mConstants.ScoreFont);
           coinSound = Game.Content.Load<SoundEffect>("Bleep");
           landSound = Game.Content.Load<SoundEffect>("Click01");
           dieSound = Game.Content.Load<SoundEffect>("claps");
           fallSound = Game.Content.Load<SoundEffect>("bomb"); 
          // hills = Game.Content.Load<Texture2D>("hills");
           ground = Game.Content.Load<Texture2D>("ground");
           groundHeight = ground.Height;
           endTexture = Game.Content.Load<Texture2D>("house");
           endSpot = new WorldObject(endTexture);
          
           endSpot.Position = new Vector2(9000 - endSpot.Width, towerBounds.Bottom - ground.Height - endSpot.Height);
          // endSpot.Position = new Vector2(400, towerBounds.Bottom - ground.Height - endSpot.Height);
           load(); 
        }

        public void load()
        {
            background = new List<Texture2D>();
            mainFrame = new List<Rectangle>();
            mainFrameGround = new List<Rectangle>();
            background.Add(Game.Content.Load<Texture2D>("1"));
            background.Add(Game.Content.Load<Texture2D>("2"));
            background.Add(Game.Content.Load<Texture2D>("3"));
            background.Add(Game.Content.Load<Texture2D>("4"));
            background.Add(Game.Content.Load<Texture2D>("5"));
            bloodTexture = Game.Content.Load<Texture2D>("blood");
            blood = new WorldObject(bloodTexture);
            for (int i = 0; i < frames; i++)
            {
                mainFrame.Add(new Rectangle(2000 * i, topOfBackGround, 2000, 2000));
                mainFrameGround.Add(new Rectangle(2000 * i, towerBounds.Bottom - ground.Height, 1293, 98));
            }
            die = false;
            die2 = false; 
            mCurrentPlatform = new WorldObject();
            base.LoadContent();
        }
        
        internal void setTowerBounds(int width, int height)
        {
            towerBounds = new Rectangle(0, 0, width, height);
        }

        internal void SetPlayerComponent(WorldObject player)
        {
            mPlayer = player;
            mPlayer.Position = new Vector2(80,  towerBounds.Bottom - ground.Height - mPlayer.Height);
            blood.Position = mPlayer.Position;
        }

        internal void SetPlatformComponent(List<WorldObject> platform, List<WorldObject> movingPlatform, List<WorldObject> coin)
        {
            mPlatform = platform;
            mMovingPlatform = movingPlatform; 
            coins = coin;

            for (int i = 0; i < frames - 1; i++)
            {
                mMovingPlatform[i].Position = new Vector2(mainFrameGround[i].Right, towerBounds.Bottom - groundHeight);
                mMovingPlatform[i].startPosition = new Vector2(mainFrameGround[i].Right, towerBounds.Bottom - groundHeight);
                mMovingPlatform[i].endPosition = new Vector2(2000 * (i+1), towerBounds.Bottom - groundHeight);
            }
        }
        
        internal void SetAIComponent(List<WorldObject> ai)
        {
            AI = ai;
            int gap = level < 3 ? 540 : 270;
            int AINum = level < 3 ? 2 : 4;
            for (int i = 0; i < frames; i++)
            {
                for (int n = 0; n < AINum; n++)
                {
                    AI[(i * AINum) + n].Position = new Vector2(mainFrameGround[i].Left + (n * gap) + 500, towerBounds.Bottom - groundHeight - AI[i].Height);
                    AI[(i * AINum) + n].startPosition = new Vector2(mainFrameGround[i].Left + (n * gap), towerBounds.Bottom - groundHeight - AI[i].Height);
                    AI[(i * AINum) + n].endPosition = new Vector2(mainFrameGround[i].Right - ((AINum - 1 - n) * gap), towerBounds.Bottom - groundHeight - AI[i].Height);
                }
            }
        }

        internal void SetSpikesComponent(List<WorldObject> spikes)
        {
            
            mSpikes = spikes;
            int gap = level == 1 ? 600 : 300;

            int spikesPerGround = level == 1 ? 2 : 4;

            for (int i = 0; i < frames; i++)
            {
                for (int n = 0; n < spikesPerGround; n++)
                {
                    mSpikes[(i * spikesPerGround) + n].Position = new Vector2(mainFrameGround[i].Left + (n * gap), towerBounds.Bottom - groundHeight - mSpikes[i].Height);
                   
                }
            }
            
        }

        internal bool landedOnLedge()
        {
            if (landed(mPlatform)) return true;
            if (landed(mMovingPlatform))
            {
                onMovingPlatform = true; 
                return true;
            }
            return false;
        }

        internal bool stillOnMoving()
        {
            if (onMovingPlatform 
                && mCurrentPlatform .Middle >= mCurrentPlatform.Left
                && mPlayer.Middle <= mCurrentPlatform.Right
                && mPlayer.Position.Y ==  mCurrentPlatform.Top - mPlayer.Height)
            {
                return true; 

            }
            onMovingPlatform = false; 
            return false; 
        }

        private bool landed(List<WorldObject> platformList)
        {
            foreach (WorldObject platform in platformList)
            {
                if (mPlayer.Middle >= platform.Left
                            && mPlayer.Middle <= platform.Right)
                {
                    if (platform.above && mPlayer.Bottom >= platform.Top)
                    {
                        mCurrentPlatform = platform;
                        platform.above = false;
                        landSound.Play();
                        mPlayer.Position = new Vector2(mPlayer.Position.X, platform.Top - mPlayer.Height);
                        return true;
                    }
                    else if (mPlayer.Bottom <= platform.Top)
                    {
                        platform.above = true;
                    }
                }
                if (mPlayer.Bottom >= platform.Top)
                {
                    platform.above = false;
                }
            }
            return false;
        }

        internal bool leftLedge(int sign)
        {
            switch (sign)
            {
                case 0: return false;
                case 1: return (mPlayer.Middle >= mCurrentPlatform.Right);
                case -1: return (mPlayer.Middle <= mCurrentPlatform.Left);
                default: return false;
            }
        }

        internal bool onGround()
        {   
            foreach(Rectangle groundObj in mainFrameGround)
            {
                if (mPlayer.Bottom >= towerBounds.Bottom - 12 - ground.Height
                    && mPlayer.Middle >= groundObj.Left - (int)cameraPosition.X
                    && mPlayer.Middle <= groundObj.Right - (int)cameraPosition.X)
                {
                    if (mPlayer.highestJump < towerBounds.Bottom / 6)
                    {
                        cameraPosition.Y = 0;
                        mPlayer.Position = new Vector2(mPlayer.Position.X, towerBounds.Bottom - ground.Height);
                        //lives--;
                        fallSound.Play();
                       // AI.Clear();
                        die = true; 
                        return true;
                    }
                    else
                    {
                        cameraPosition.Y = 0;
                       landSound.Play();
                        mPlayer.Position = new Vector2(mPlayer.Position.X, towerBounds.Bottom - ground.Height - mPlayer.Height);
                        return true;
                    }
                    
                }
            }
            if (mPlayer.Position.Y > towerBounds.Bottom)
            {
               // mPlayer.Position = new Vector2(200, towerBounds.Bottom - ground.Height - mPlayer.Height);
                cameraPosition = new Vector2(0, 0);
                
                fallSound.Play();
                System.Threading.Thread.Sleep(800);
                lives--;
                //die = false; 
            }
            return false;
        }


        private void removeLostObjects(List<WorldObject> listObj)
        {
            for (int i = 0; i < listObj.Count(); i++)
            {
                if (listObj.ElementAt(i).Top > towerBounds.Bottom - cameraPosition.Y)
                {
                    listObj.RemoveAt(i);
                }
            }
        }

        public bool gameOver()
        {
            if (lives < 0)
            {
                lives = 3; 
                return true; 
            }
            return false; 
            
        }

        public override void Update(GameTime gameTime)
        {
            
            int midX = towerBounds.Right / 2 - (int)mPlayer.Width/2;
            int midY = towerBounds.Bottom / 2 - (int)mPlayer.Height/2;

            if (mPlayer.Position.Y < midY - cameraPosition.Y)
            {
                cameraPosition.Y = midY - (int)mPlayer.Position.Y;
            }
            else if (mPlayer.Position.Y <= midY && mPlayer.Position.Y > midY - cameraPosition.Y)
            {
                cameraPosition.Y = midY - (int)mPlayer.Position.Y;
            }
            if (mPlayer.Position.X > midX - cameraPosition.X && mPlayer.Position.X <= 9000 - midX)
            {
                cameraPosition.X = midX - (int)mPlayer.Position.X;
            }
            else if (mPlayer.Position.X >= midX &&
               mPlayer.Position.X < midX - cameraPosition.X)
            {
                cameraPosition.X = midX - (int)mPlayer.Position.X;
            }
            for (int i = 0; i < coins.Count(); i++)
            {
                if (mPlayer.Intersects(coins.ElementAt(i)))
                {
                    score+=10;
                    coins.RemoveAt(i);
                    coinSound.Play();
                }
            }
            mainFrame.Clear();
            mainFrameGround.Clear(); 
            for (int i = 0; i < frames; i++)
            {
                mainFrame.Add(new Rectangle((int)cameraPosition.X + 2000 * i, topOfBackGround + (int)cameraPosition.Y, 2000, 2000));
                mainFrameGround.Add(new Rectangle((int)cameraPosition.X + 2000 * i, towerBounds.Bottom - ground.Height + (int)cameraPosition.Y, 1293, 98));
            }


            for (int i = 0; i < AI.Count; i++)
            {
                WorldObject ai = AI.ElementAt(i);

                if (mPlayer.Right > ai.Left && mPlayer.Left < ai.Right
                    && mPlayer.Top < ai.Top && mPlayer.Bottom < ai.Bottom
                    && mPlayer.Bottom > ai.Top)
                {
                    AI.RemoveAt(i);
                    score += 100;
                    
                }
                   
                else if (mPlayer.Right > ai.Left && mPlayer.Left < ai.Right
                    && mPlayer.Bottom > ai.Top && mPlayer.Top < ai.Top)
                {
                    dieSound.Play();
                    //System.Threading.Thread.Sleep(800);
                   // lives--;
                    die = true; 
                   // mPlayer.Position = new Vector2(0, towerBounds.Bottom - ground.Height - mPlayer.Height);
                  //  cameraPosition = new Vector2(0, 0);
                }
            }
            foreach (WorldObject spikesObj in mSpikes)
            {
               /* if (mPlayer.Right > spikesObj.Left && mPlayer.Left < spikesObj.Right
                    && mPlayer.Top < spikesObj.Top && mPlayer.Bottom < spikesObj.Bottom
                    && mPlayer.Bottom > spikesObj.Top)*/
             // if (mPlayer.Intersects(spikesObj))
                if (mPlayer.Right - 25 > spikesObj.Left && mPlayer.Left + 25 < spikesObj.Right
                    && mPlayer.Bottom > spikesObj.Top && mPlayer.Top < spikesObj.Top)
                {
                    dieSound.Play(); 
                    //System.Threading.Thread.Sleep(800);
                    die = true; 
                    //lives--;
                //    cameraPosition = new Vector2(0, 0);
                }
            }

            if (onMovingPlatform && stillOnMoving())
            {
                if (moveRight)
                {
                    mPlayer.Position += new Vector2(2f, 0);
                }
                else
                {
                    mPlayer.Position -= new Vector2(2f, 0);
                }
            }
            if (mPlayer.Intersects(endSpot))
            {
                level++; 
            }

          blood.Position = new Vector2(mPlayer.Position.X, towerBounds.Bottom - ground.Height - mPlayer.Height);
          if (!die)
          {
              base.Update(gameTime);
          }
            if (die2)
            {
                lives--;
                System.Threading.Thread.Sleep(800);
                die = false;
                die2 = false;
                cameraPosition = new Vector2(0, 0);
               // cameraPosition = new Vector2(0, 0);
            }
        }




        public override void Draw(GameTime gameTime)
        {
            mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            
            for (int i = 0; i < frames; i++)
            {
                mSpriteBatch.Draw(background.ElementAt(i), mainFrame.ElementAt(i), Color.White);
                mSpriteBatch.Draw(ground, mainFrameGround.ElementAt(i), Color.White);
            }
            
            if (die)
            {
                Console.WriteLine("in");
                blood.Draw(mSpriteBatch, cameraPosition);
                die2 = true;
                die = false;
               
            }
           
            mPlayer.Draw(mSpriteBatch, cameraPosition);

            foreach (WorldObject platform in mPlatform)
            {
                platform.Draw(mSpriteBatch, cameraPosition);
            }
            foreach (WorldObject platform in mMovingPlatform)
            {
                platform.Draw(mSpriteBatch, cameraPosition);
            }
            foreach (WorldObject coin in coins)
            {
                coin.Draw(mSpriteBatch, cameraPosition);
            }
            foreach (WorldObject AIobj in AI)
            {
                AIobj.Draw(mSpriteBatch, cameraPosition);
            }
           
            foreach (WorldObject spikesObj in mSpikes)
            {
                spikesObj.Draw(mSpriteBatch, cameraPosition);
            }
            endSpot.Draw(mSpriteBatch, cameraPosition);
           

            mSpriteBatch.DrawString(font, "score " + (score * 10).ToString(),
                new Vector2(50, 50), Color.Red);
            mSpriteBatch.DrawString(font, "lives " + ((lives< 0)? "0" : lives.ToString()),
                new Vector2(50, 70), Color.Red);
            mSpriteBatch.DrawString(font, "level " + level.ToString(),
                new Vector2(50, 90), Color.Red);

           
            mSpriteBatch.End();

        }


    }
}