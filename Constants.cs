using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JumpTest
{
    public class Constants
    {
        private static Constants mConstants;

       public String GameOverFont { get; private set; }
       public String ScoreFont { get; private set; }

        public Vector2 SwitchVector { get; private set; }
        public Vector2 WalkVelocity { get; private set; }
        public Vector2 JumpVelocity { get; private set; }
        public Vector2 JumpOffPlatformVelocity { get; private set; }
        public Vector2 Gravity { get; private set; }
        public Vector2 PlatformSpeed { get; private set; }

        public String PlayerRightTexture { get; private set; }
        public String PlayerRightTexture2 { get; private set; }
        public String PlayerLeftTexture { get; private set; }
        public String PlayerLeftTexture2 { get; private set; }
        public String PlayerJumpRightTexture { get; private set; }
        public String PlayerJumpLeftTexture { get; private set; }
        public String PlayerSplatRight { get; private set; }
        public String PlayerSplatLeft { get; private set; }
        public String Coin { get; private set; }
        public String MovingPlatform { get; private set; }
        public List<String> PlatformTextures { get; private set; }


        public float InitialLives { get; private set; }
       

        public int PreferredBackBufferWidth { get; private set; }
        public int PreferredBackBufferHeight { get; private set; }

        private Constants()
        {
        }

        public static Constants GetInstance()
        {
            if (mConstants == null)
            {
                mConstants = new Constants();
                //mConstants.mWorldObjects = l;
            }

            return mConstants;
        }

        public void LoadWorld(String worldFile)
        {
            using (XmlReader reader = XmlReader.Create(new StreamReader(worldFile)))
            {
                XDocument xml = XDocument.Load(reader);
                XElement root = xml.Root;
                foreach (XElement elem in root.Elements())
                {
                    XMLParse.AddValueToClassInstance(elem, this);
                }
            }
            //foreach (WorldObject o in mWorldObjects)
            //{
            //    e.Texture = mContentManager.Load<Texture2D>(e.TextureName);
            //    e.Init();
            //}
        }
    }
}
