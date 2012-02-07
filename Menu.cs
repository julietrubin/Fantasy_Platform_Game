using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace JumpTest
{


    class MenuManager
    {
        private List<Menu> mMenus;


        public MenuManager()
        {
            mMenus = new List<Menu>();
        }
        public void AddMenu(Menu menu)
        {
            mMenus.Add(menu);
            menu.Parent = this;
        }

        public KeyboardState PrevKeyboadState { get; set; }
        public GamePadState PrevGamePadState { get; set; }

        public void Update(GameTime gametime)
        {
            foreach (Menu m in mMenus)
            {
                if (m.UpdateEnabled)
                {
                    m.Update(gametime);
                }
            }

        }

        public void Draw(SpriteBatch sb)
        {
            foreach (Menu m in mMenus)
            {
                if (m.DrawEnabled)
                {
                    m.Draw(sb);
                }
            }
        }
    }


    class Menu
    {
        public delegate void MenuItemSelected();
        public delegate void SetFloatValue(float newVal);

        private SpriteBin MenuSprites;

        public const int MenuCenterX = 520;
        public const int TitleY = 75;
        public const int StartElements = 200;
        public const int ElementDelta = 75;

        public bool UpdateEnabled { get; set; }
        public bool DrawEnabled { get; set; }

        public bool Enabled
        {
            get
            {
                return DrawEnabled && UpdateEnabled;
            }
            set
            {
                DrawEnabled = value;
                UpdateEnabled = value;
            }

        }
        public MenuManager Parent { get; set; }

        private TextSprite Title;
        private int currentElement;
        private List<MenuItem> elements;

        public Menu(SpriteFont menufont, string title, bool beginEnabled)
        {
            elements = new List<MenuItem>();
            MenuSprites = new SpriteBin(menufont);
            Title = MenuSprites.AddTextSprite(title);
            Title.Position = new Vector2(MenuCenterX, TitleY);
            Enabled = beginEnabled;
        }

        public void AddMenuItem(string text, MenuItemSelected action)
        {
            TextSprite elemSprite = MenuSprites.AddTextSprite(text);
            elemSprite.Position = new Vector2(MenuCenterX, elements.Count * ElementDelta + StartElements);
            SelectMenuItem m = new SelectMenuItem(elemSprite, action, this);
            m.HomePosition = elemSprite.Position;
            elements.Add(m);
            if (elements.Count == 1)
            {
                elements[0].Highlight();
            }
        }
       

        public void AddChangableMenuItem(string text, Texture2D barTexture, float initial, SetFloatValue action)
        {
            TextSprite elemSprite = MenuSprites.AddTextSprite(text);
            elemSprite.Position = new Vector2(MenuCenterX, elements.Count * ElementDelta + StartElements);
            ProgressBarSprite bar = new ProgressBarSprite(barTexture);


           bar.Position = new Vector2(MenuCenterX + 250, elements.Count * ElementDelta + StartElements);

           bar.PercentageToShow = initial;
          // bar.Position = elemSprite.Position + elemSprite;

            MenuSprites.Add(bar);
            ChangeMenuItem c = new ChangeMenuItem(elemSprite, bar, initial, action, this);
            elements.Add(c);



        }

        /// Add a method similar to AddMenuItem, that creates a menu item like a volume control.  This method
        /// should take as input parameters the Text to display, a Texture for the ProgressBar class, the initial 
        /// value (a float between 0 and 1, or a delegate that can be used to extract the value from the variable 
        /// we are controlling) and a delegate that you can use to change the controlling variable
        /// 
      

        public void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One); 

            if ((keyboardState.IsKeyDown(Keys.Up) && Parent.PrevKeyboadState.IsKeyUp(Keys.Up))
                || (gamePadState.IsButtonDown(Buttons.DPadUp) && Parent.PrevGamePadState.IsButtonUp(Buttons.DPadUp)))
            {
                elements[currentElement].UnHighlight();
                currentElement = currentElement - 1;
                if (currentElement < 0)
                    currentElement = 0;
                elements[currentElement].Highlight();
            }

            if ((keyboardState.IsKeyDown(Keys.Left) && Parent.PrevKeyboadState.IsKeyUp(Keys.Left))
               || (gamePadState.IsButtonDown(Buttons.DPadLeft) && Parent.PrevGamePadState.IsButtonUp(Buttons.DPadLeft)))
            {
                elements[currentElement].Decrease();
            }
            if ((keyboardState.IsKeyDown(Keys.Right) && Parent.PrevKeyboadState.IsKeyUp(Keys.Right))
                || (gamePadState.IsButtonDown(Buttons.DPadRight) && Parent.PrevGamePadState.IsButtonUp(Buttons.DPadRight)))
            {
                elements[currentElement].Increase();
            }


            if ((keyboardState.IsKeyDown(Keys.Down) && Parent.PrevKeyboadState.IsKeyUp(Keys.Down))
                || (gamePadState.IsButtonDown(Buttons.DPadDown) && Parent.PrevGamePadState.IsButtonUp(Buttons.DPadDown)))
            {
                elements[currentElement].UnHighlight();
                currentElement = currentElement + 1;
                if (currentElement >= elements.Count)
                    currentElement = elements.Count - 1;
                elements[currentElement].Highlight();
            }

            if ((keyboardState.IsKeyDown(Keys.Enter) && Parent.PrevKeyboadState.IsKeyUp(Keys.Enter))
                || (gamePadState.IsButtonDown(Buttons.A) && Parent.PrevGamePadState.IsButtonUp(Buttons.A))
                || (gamePadState.IsButtonDown(Buttons.Start) && Parent.PrevGamePadState.IsButtonUp(Buttons.Start)))
            {
                elements[currentElement].Select();
            }
            Parent.PrevKeyboadState = keyboardState;
            Parent.PrevGamePadState = gamePadState; 

        }

        public void Draw(SpriteBatch sb)
        {
            MenuSprites.Draw(sb);
        }
    }

    class MenuItem
    {
        public Vector2 HomePosition { get; set; }

        public virtual void Select()
        {

        }
        public virtual void Highlight()
        {
        }

        public virtual void UnHighlight()
        {

        }
        public virtual void Decrease()
        {

        }
        public virtual void Increase()
        {

        }
    }

    class ChangeMenuItem : MenuItem
    {
        public TextSprite Text { get; set; }
        public ProgressBarSprite bar { get; set; }
        public Menu.SetFloatValue Action { get; set; }
        public Menu Parent { get; set; }
        public float initial; 
    

        public ChangeMenuItem(TextSprite sprite, ProgressBarSprite bar, float initial, Menu.SetFloatValue action, Menu parent)
        {
                this.Text = sprite;
                this.Action = action;
                Text.Color = Color.White;
                Parent = parent;
                Text.Scale = 1.0f;
                this.bar = bar;
                this.initial = initial; 
               
        }
        public override void Highlight()
        {
            Text.Color = Color.Red;
            Text.Scale = 1.5f;
            bar.Scale = 1; 
         
        }
        public override void UnHighlight()
        {
            Text.Color = Color.White;
            Text.Scale = 1;
            bar.Scale = 0; 
        }

        public override void Decrease()
        {
            if (Action != null )
            {
                initial -= 0.05f;
                if (initial < 0)
                {
                    initial = 0; 
                }
                Action(initial);
                bar.PercentageToShow = initial; 
            }
            
        }
        public override void Increase()
        {
            if (Action != null)
            {
                initial += .05f;
                if (initial > 1)
                {
                    initial = 1; 
                }
                Action(initial);
                bar.PercentageToShow = initial; 
            }
        }

    }

    class SelectMenuItem : MenuItem
    {
        public TextSprite Text { get; set; }
        public Menu.MenuItemSelected Action { get; set; }
        public Menu Parent { get; set; }

        public SelectMenuItem(TextSprite sprite, Menu.MenuItemSelected action, Menu parent)
        {
            this.Text = sprite;
            this.Action = action;
            Text.Color = Color.White;
            Parent = parent;
            Text.Scale = 1.0f;
        }


        public override void Select()
        {
            if (Action != null)
            {
                Action();
            }
        }


        public override void  Highlight()
        {
            Text.Color = Color.Red;
            Text.Scale = 1.5f;
        }
        public override void UnHighlight()
        {
            Text.Color = Color.White;
            Text.Scale = 1;
        }
    }

    /// You may also want to create a subclass of MenuItem to handle the case of a menu item for a variable
    /// that you can control, like volume


}
