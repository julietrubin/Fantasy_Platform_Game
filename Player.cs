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

    public class Player : Microsoft.Xna.Framework.GameComponent
    {
        private static KeyboardState keyboard;
        private static GamePadState gamePad; 
        private static WorldObject mPlayer;
        private static Constants mConstants;
        private World mWorld;
       
        private Texture2D mPlayerRightTexture;
        private Texture2D mPlayerRightTexture2;
        private Texture2D mPlayerLeftTexture;
        private Texture2D mPlayerLeftTexture2;
        private Texture2D mPlayerRightJumpTexture;
        private Texture2D mPlayerLeftJumpTexture;
        private Texture2D splatRight;
        private Texture2D splatLeft; 

        private Vector2 xVelocity;
        private Vector2 yVelocity;

        private int lastDir;
        private bool up;
        private bool down;
        private bool onLedge;
        private bool rightSide;
       

        public Player(Game1 game, Constants c)
            : base(game)
        {
            mConstants = c;

        }

        public override void Initialize()
        {
            up = false;
            down = false;
            onLedge = false;
            rightSide = true;
            base.Initialize();
        }

        public void LoadContent()
        {
            mPlayerRightTexture = Game.Content.Load<Texture2D>(mConstants.PlayerRightTexture);
            mPlayerRightTexture2 = Game.Content.Load<Texture2D>(mConstants.PlayerRightTexture2);
            mPlayerLeftTexture = Game.Content.Load<Texture2D>(mConstants.PlayerLeftTexture);
            mPlayerLeftTexture2 = Game.Content.Load<Texture2D>(mConstants.PlayerLeftTexture2);
            mPlayerRightJumpTexture = Game.Content.Load<Texture2D>(mConstants.PlayerJumpRightTexture);
            mPlayerLeftJumpTexture = Game.Content.Load<Texture2D>(mConstants.PlayerJumpLeftTexture);
            splatRight = Game.Content.Load<Texture2D>(mConstants.PlayerSplatRight);
            splatLeft = Game.Content.Load<Texture2D>(mConstants.PlayerSplatLeft);
            load();
        }

        public void load()
        {
            mPlayer = new WorldObject(mPlayerRightTexture);
            mWorld.SetPlayerComponent(mPlayer);
        }
        internal void SetWorldComponent(World world)
        {
            mWorld = world;
        }


        public override void Update(GameTime gameTime)
        {
            if (mPlayer.mTexture == splatLeft || mPlayer.mTexture == splatRight)
            {
                
                System.Threading.Thread.Sleep(2000);
                
                mPlayer.mTexture = rightSide ? mPlayerRightTexture : mPlayerLeftTexture;
                mPlayer.Position = new Vector2(mPlayer.Position.X, mWorld.towerBounds.Bottom - mWorld.groundHeight - mPlayer.Height);
                mWorld.cameraPosition = new Vector2(0, 0);
               // mWorld.lives--; 
            }
            
            keyboard = Keyboard.GetState();
            gamePad = GamePad.GetState(PlayerIndex.One);
            bool moveRight = keyboard.IsKeyDown(Keys.Right) || gamePad.IsButtonDown(Buttons.DPadRight) || gamePad.IsButtonDown(Buttons.LeftThumbstickRight);
            bool moveLeft = keyboard.IsKeyDown(Keys.Left) || gamePad.IsButtonDown(Buttons.DPadLeft) || gamePad.IsButtonDown(Buttons.LeftThumbstickLeft);
            bool jumpUp = keyboard.IsKeyDown(Keys.Up) || gamePad.IsButtonDown(Buttons.A);
            int direction = 0;

           if (moveRight)
           {
                if (!up && !down && (gameTime.TotalGameTime.Milliseconds) % 400 == 0)
                {
                    mPlayer.mTexture = mPlayer.mTexture == mPlayerRightTexture ? mPlayerRightTexture2 : mPlayerRightTexture;

                    if (!mWorld.onGround() && !onLedge)
                    {
                        down = true;
                    }
                    
                }
                else if (!rightSide)
                {
                    mPlayer.mTexture = mPlayerRightTexture;
                }
              
                rightSide = true;
                direction = 1;
            }
            else if (moveLeft)
           {
               if (!up && !down && (gameTime.TotalGameTime.Milliseconds) % 400 == 0)
                {
                    mPlayer.mTexture = mPlayer.mTexture == mPlayerLeftTexture ? mPlayerLeftTexture2 : mPlayerLeftTexture;

                    if (!mWorld.onGround() && !onLedge)
                    {
                        down = true; 
                    }
                }
               else if (rightSide)
               {
                   mPlayer.mTexture = mPlayerLeftTexture;
               }
                rightSide = false; 
                direction = -1;
            }
          
            bool jumpOffPlatform = onLedge && mWorld.leftLedge(direction);
            onLedge = jumpOffPlatform ? false : onLedge;

            // If they have switched direction
            if ((up || down) && (moveLeft || moveRight) && (lastDir != direction))
            {
                if (xVelocity.X == 0)
                    xVelocity = direction * mConstants.WalkVelocity;
                else
                    xVelocity = direction * mConstants.SwitchVector;
            }
            
            lastDir = direction; 
            if(up)
            {
                //onGround = false;
                if (yVelocity.Y > 0)
                {
                    mPlayer.Position += (xVelocity + -yVelocity) * gameTime.ElapsedGameTime.Milliseconds;
                    yVelocity -= mConstants.Gravity;
                }
                else
                {
                    mPlayer.highestJump = (int)mPlayer.Position.Y; 
                    up = false;
                    down = true;
                    mPlayer.mTexture = rightSide ? mPlayerRightTexture : mPlayerLeftTexture; 
                }
            }
            else if (down)
            {
                if (mWorld.landedOnLedge())
                {
                    down = false;
                    onLedge = true;
                    mPlayer.mTexture = rightSide ? mPlayerRightTexture : mPlayerLeftTexture; 
                }

                else if (mWorld.onGround())
                {
                    down = false;
                    onLedge = false;
                    //onGround = true; 

                    if (mPlayer.Position.Y == mWorld.towerBounds.Bottom - mWorld.groundHeight)
                    {
                        mPlayer.mTexture = rightSide ? splatRight : splatLeft;
                    }
                    else
                    {
                        mPlayer.mTexture = rightSide ? mPlayerRightTexture : mPlayerLeftTexture;
                    }
                }

                else
                {
                    
                    mPlayer.Position += (xVelocity + yVelocity) * gameTime.ElapsedGameTime.Milliseconds;
                    yVelocity += mConstants.Gravity;
                    
                }
            }
           
            else if (jumpUp || jumpOffPlatform)
            {
                up = true;

                yVelocity = jumpOffPlatform ? mConstants.JumpOffPlatformVelocity : mConstants.JumpVelocity;

                    xVelocity = direction * mConstants.WalkVelocity;
                

                if (mPlayer.mTexture == mPlayerRightTexture)
                    mPlayer.mTexture = mPlayerRightJumpTexture;
                if (mPlayer.mTexture == mPlayerLeftTexture)
                    mPlayer.mTexture = mPlayerLeftJumpTexture;
            }
            else if ((moveRight || moveLeft))
            {
                mPlayer.Position += direction * mConstants.WalkVelocity * gameTime.ElapsedGameTime.Milliseconds;
            }
            if (mPlayer.Position.X < 0)
            {
                mPlayer.Position = new Vector2(0, mPlayer.Position.Y);
            }
            
            if (mPlayer.Position.X > 9000)
            {
                mPlayer.Position = new Vector2(9000, mPlayer.Position.Y);
            }
         
            base.Update(gameTime);
        }

       
    }
}