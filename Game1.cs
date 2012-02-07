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
    public class HighScore
    {
        public String name;
        public int score;
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont gameOverFont;
        SpriteFont font; 
        Constants mConstants;

        Player mPlayer;
        Platform mPlatform;
        World mWorld;
        AI mAI;
        Spikes mSpikes; 
       

        SpriteFont menuFont; 
        SpriteBin Sprites;
        MenuManager mMenus;
        float mVolume;
        bool quit = false;
        bool startGame = false;
        public bool go;
        public int lives;
        public int level;
        public bool endScreen;
        int i = 0;
        bool LevelUp;
        bool win;
        int score;
        bool gameOver; 


        public Game1()
        {
            mConstants = Constants.GetInstance();
            mConstants.LoadWorld("Content/constants.xml");
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = mConstants.PreferredBackBufferWidth;
            this.graphics.PreferredBackBufferHeight = mConstants.PreferredBackBufferHeight;
           // this.graphics.IsFullScreen = true;

            mPlayer = new Player(this, mConstants);
            mPlatform = new Platform(this, mConstants);
            mWorld = new World(this, mConstants);
            mAI = new AI(this, mConstants);
            mSpikes = new Spikes(this, mConstants);

            Components.Add(mWorld);
            Components.Add(mPlatform);
            Components.Add(mPlayer);
            Components.Add(mAI);
            Components.Add(mSpikes);

            mPlayer.SetWorldComponent(mWorld);
            mPlatform.SetWorldComponent(mWorld);
            mAI.SetWorldComponent(mWorld);
            mSpikes.SetWorldComponent(mWorld);
            mWorld.setTowerBounds(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            endScreen = false;
            gameOver = false;
            mVolume = 1;
           
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            lives = 3;
            level = 1;
            LevelUp = false;
            score = 0;
             
            base.Initialize();
            
        }
      
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            gameOverFont = Content.Load<SpriteFont>(mConstants.GameOverFont);
            font = Content.Load<SpriteFont>(mConstants.ScoreFont);
            mPlayer.LoadContent();
            mPlatform.LoadContent();
            mAI.LoadContent();
            mSpikes.LoadContent();

            menuFont = Content.Load<SpriteFont>("SpriteFont1");
            Sprites = new SpriteBin(menuFont);
            CreateMenus(menuFont, false);
        }

        protected override void UnloadContent()
        {
        }

        /// <summary>
        ///  CreateMenus creates the Main Menu and the Confirm Quit menu, and
        ///  activates the main menu.
        /// </summary>
        /// <param name="menuFont"></param>
        protected void CreateMenus(SpriteFont menuFont, bool pause)
        {
          
            Menu mainMenu = new Menu(menuFont, "Main Menu", true);
            Menu confirmQuit = new Menu(menuFont, "Quit?", false);

            mainMenu.AddMenuItem("New Game", () =>
            {
                if (!startGame)
                {
                    startGame = true;
                    SoundEffect.MasterVolume = mVolume; 
                    Initialize();
                    mWorld.level = 1; 
                    mWorld.Initialize();
                    mPlatform.Initialize();
                    mPlayer.Initialize();
                    mAI.Initialize();
                    mWorld.load();
                    mPlayer.load();
                    mPlatform.load();
                    mAI.load();
                    mSpikes.Initialize();
                    mSpikes.load(); 
                }
            });
           
            if (pause)
            {
                
                mainMenu.AddMenuItem("Resume Game", () => { startGame = true; SoundEffect.MasterVolume = mVolume; System.Threading.Thread.Sleep(500);});
            }
            mainMenu.AddChangableMenuItem("Volume", Content.Load<Texture2D>("ScaleFull"), mVolume, SetVolume);
            mainMenu.AddMenuItem("Quit", () => { confirmQuit.Enabled = true; mainMenu.Enabled = false; });
            confirmQuit.AddMenuItem("Yes", () => { this.Exit(); });
            confirmQuit.AddMenuItem("No", () => { confirmQuit.Enabled = false; mainMenu.Enabled = true; });
            mMenus = new MenuManager();
            mMenus.AddMenu(mainMenu);
            mMenus.AddMenu(confirmQuit);

           
        }

        void SetVolume(float newVolume)
        {
            mVolume = newVolume;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                this.Exit();
            }
            if (quit == true)
            {
                this.Exit();
            }
            if (mWorld.gameOver())
            {
                score = mWorld.score; 
                startGame = false;
               // System.Threading.Thread.Sleep(500);
                CreateMenus(menuFont, false);
                endScreen = true;
                gameOver = true; 
                
            }
           
            mMenus.Update(gameTime);
            KeyboardManager.GetInstance().Update(gameTime);
            AnimationManager.GetInstance().Update(gameTime);

            if (startGame && !endScreen)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
                if (keyboardState.IsKeyDown(Keys.Pause) || gamePadState.IsButtonDown(Buttons.Start))
                {
                    System.Threading.Thread.Sleep(500);
                    startGame = false;
                    CreateMenus(menuFont, true);
                }
                base.Update(gameTime);
            }
            if (lives > mWorld.lives)
            {

                if (lives - 2 == mWorld.lives)
                {
                    mWorld.lives++;
                }
                if (!gameOver)
                {
                mWorld.load();
                mPlayer.Initialize();
                mPlayer.load();
                mAI.Initialize();
                mAI.load();
                }
                lives--;
                
            }
            if (level < mWorld.level)
            {
                score = mWorld.score;
                    mWorld.Initialize();
                    mPlatform.Initialize();
                    mPlayer.Initialize();
                    mAI.Initialize();
                    mWorld.load();
                    mPlayer.load();
                    mPlatform.load();
                    mAI.load();
                    mSpikes.Initialize();
                    mSpikes.load();
                
                    System.Threading.Thread.Sleep(500);
                    level++;
                    endScreen = true;
               
                if (level == 4)
                {
                    win = true;
                }
                else
                {
                    LevelUp = true;
                }
               
            }
            gameOver = false; 
        }
                        
        
        protected override void Draw(GameTime gameTime)
        {
           GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.CornflowerBlue);
         //  spriteBatch.Begin();
           
           if (endScreen)
           {
               if (i < 250)
               {
                   i++;
                   spriteBatch.Begin();
                   Console.WriteLine("yes");
                   if (LevelUp)
                   {
                       spriteBatch.DrawString(gameOverFont, "Congratulations you beat level " + (mWorld.level -1 ).ToString(),
                         new Vector2(430, 280), Color.White);
                   }
                   else if (win)
                   {
                       spriteBatch.DrawString(gameOverFont, "Congratulations you beat the game!",
                        new Vector2(430, 280), Color.White);
                   }
                   else
                   {
                       spriteBatch.DrawString(gameOverFont, "Game Over!",
                        new Vector2(480, 280), Color.White);
                   }
                   spriteBatch.DrawString(gameOverFont, "score " + (mWorld.score * 10).ToString(),
                    new Vector2(500, 320), Color.White);
                   spriteBatch.End();
               }
               else
               {
                   if (win)
                   {
                       startGame = false;
                       CreateMenus(menuFont, false);
                   }
                    i = 0; 
                   endScreen = false;
                   win = false;
                   mWorld.score = 0;
                   LevelUp = false; 

               }
           }
          else if (startGame)
           {
               base.Draw(gameTime);
           }
           else
           {
               spriteBatch.Begin();
               mMenus.Draw(spriteBatch);
               spriteBatch.End();
               
           }
           
        }
    }
}