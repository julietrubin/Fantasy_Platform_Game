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
    public class Spikes : Microsoft.Xna.Framework.GameComponent
    {
        private World mWorld;
        private Constants mConstants;
        private List<WorldObject> mSpikes;
        private Texture2D spikesTexture;

        public Spikes(Game game, Constants c)
            : base(game)
        {
            mConstants = c;
        }

        public override void Initialize()
        {
            mSpikes = new List<WorldObject>();
            base.Initialize();
        }

        public void LoadContent()
        {
            spikesTexture = Game.Content.Load<Texture2D>("spike");
            load();
        }

        public void load()
        {
            int spikeNum = mWorld.level == 1 ? 10 : 20; 
            for (int i = 0; i < spikeNum; i++)
            {
                WorldObject spike = new WorldObject(spikesTexture);
                mSpikes.Add(spike);
            }
            mWorld.SetSpikesComponent(mSpikes);
        }

        internal void SetWorldComponent(World world)
        {
            mWorld = world;

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }

}