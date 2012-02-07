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
    public class AI : Microsoft.Xna.Framework.GameComponent
    {
        private World mWorld;
        private Constants mConstants;
        private List<WorldObject> mAI;
        private Texture2D AITexture;
        private Vector2 AISpeed;
        bool moveRight;

         public AI(Game game, Constants c)
            : base(game)
        {
            mConstants = c;
        }

        public override void Initialize()
        {
            mAI = new List<WorldObject>();
            AISpeed = new Vector2(1.5f, 0f);
            moveRight = true; 
            base.Initialize();
        }

        public void LoadContent()
        {
            AITexture = Game.Content.Load<Texture2D>("bad");
            load();
        }

        public void load()
        {
            int AINum = mWorld.level < 3 ? 10 : 20;
            for (int i = 0; i < AINum; i++)
            {
                WorldObject bug = new WorldObject(AITexture);
                mAI.Add(bug);
            }
            mWorld.SetAIComponent(mAI);
        }

        internal void SetWorldComponent(World world)
        {
            mWorld = world;
          
        }


        public override void Update(GameTime gameTime)
        {
            foreach (WorldObject AIobj in mAI)
            {
                
                if (moveRight)
                {
                    if (AIobj.Position.X + AIobj.Width < AIobj.endPosition.X)
                    {

                        AIobj.Position += AISpeed;
                    }
                    else
                    {
                        moveRight = false;
                    }
                }
                if (!moveRight)
                {
                    if (AIobj.Position.X > AIobj.startPosition.X)
                    {
                        AIobj.Position -= AISpeed;
                    }
                    else
                    {
                        moveRight = true;
                    }
                }
            }
            base.Update(gameTime);
        }
    }
    
}