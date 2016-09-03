using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameActor;

namespace biNUICLeAR
{
    
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    public static class ConstValues
    {
        const int TileSize = 16;

        const int TilesVertical = 48;
        const int TilesHorizontal = 64;

        const int ScreenWidth = 1024;
        const int ScreenHeight = 768;
 
        public static int getTileSize
        {
            get { return TileSize; }
        }
        public static int getTilesVertical
        {
            get { return TilesVertical; }
        }
        public static int getTilesHorizontal
        {
            get { return TilesHorizontal; }
        }
        public static int getScreenWidth
        {
            get { return ScreenWidth; }
        }
        public static int getScreenHeight
        {
            get { return ScreenHeight; }
        }

    }
    public struct DrawInfo
    {
        public Texture2D texture;
        public Vector2 position;
        public DrawInfo(Texture2D t, Vector2 v)
        {
            texture = t;
            position = v;
        }
    };
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<DrawInfo> flags;

        MapScene mReader;

        private List<Soldier> refugees;

        MouseState currentMouseState;
        KeyboardState currentKeyState;


        bool isGameStart = false;
        Vector2 endPosition = new Vector2(ConstValues.getTilesHorizontal - 2, ConstValues.getTilesVertical - 3);
        public bool IsGameStart
        {
            set { isGameStart = value; }
            get { return isGameStart; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public bool isActorInTheBlock(Vector2 currentBlock, ActorType aType)
        {

            return true;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = ConstValues.getScreenWidth;
            graphics.PreferredBackBufferHeight = ConstValues.getScreenHeight;
            graphics.ApplyChanges();

           
            mReader = new MapScene();
            flags = new List<DrawInfo>();

            refugees = new List<Soldier>();
            for (int i = 0; i < 1; i++)
            {
                Soldier tempSoldier = new Soldier();
                tempSoldier.currentPathIndex = 0;
                refugees.Add(tempSoldier);
            }
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D textureGround = Content.Load<Texture2D>("Graphics\\tileground");
            Texture2D textureBlock = Content.Load<Texture2D>("Graphics\\tileblock");
            Texture2D textureMine = Content.Load<Texture2D>("Graphics\\werewolf");

            // Make texture change here
            Texture2D textureSoldier = Content.Load<Texture2D>("Graphics\\charactersprite");
            

            SpriteFont sf = Content.Load<SpriteFont>("Number");
            mReader.initMap(textureGround, textureMine,textureBlock,sf);
            mReader.updateMap();
            foreach (Soldier p in refugees)
            {
                p.Initialize(textureSoldier, p.Position);
            }

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            UpdatePlayerInput(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            mReader.drawMap(spriteBatch);
            foreach (Soldier element in refugees)
                element.Draw(spriteBatch);
            foreach(DrawInfo flag in flags)
                spriteBatch.Draw(flag.texture, flag.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void UpdatePlayerInput(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                //Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(camera.GetViewMatrix()));
                mReader.DetectBlocks((int)mousePosition.X / ConstValues.getTileSize, (int)mousePosition.Y / ConstValues.getTileSize);
            }

            else if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                //Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(camera.GetViewMatrix()));
                mReader.RevealBlock((int)mousePosition.X/ConstValues.getTileSize, (int)mousePosition.Y/ConstValues.getTileSize);
            }
            else if(currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                foreach (Soldier s in refugees)
                    s.recalculatePath(mReader.GetMap, new Vector2((int)currentMouseState.X / ConstValues.getTileSize, (int)currentMouseState.Y / ConstValues.getTileSize));
            }
            else if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                //Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(camera.GetViewMatrix()));
                if (mReader.PlaceFlag((int)mousePosition.X / ConstValues.getTileSize, (int)mousePosition.Y / ConstValues.getTileSize))
                {
                    flags.Add(new DrawInfo(Content.Load<Texture2D>("Graphics//Flag.png"), new Vector2(ConstValues.getTileSize * ((int)mousePosition.X / ConstValues.getTileSize), ConstValues.getTileSize * ((int)mousePosition.Y / ConstValues.getTileSize))));
                }
                else
                {

                    for (int i = 0; i < flags.Count; i++)
                    {
                        DrawInfo f = flags.ElementAt(i);
                        if ((f.position.X == ConstValues.getTileSize * ((int)mousePosition.X / ConstValues.getTileSize) &&
                            (f.position.Y == ConstValues.getTileSize * ((int)mousePosition.Y / ConstValues.getTileSize))))
                        {
                            flags.Remove(f);
                        }
                    }
                }
                
            }

            if (currentKeyState.IsKeyDown(Keys.Back))
            {
               

            }
            if (currentKeyState.IsKeyDown(Keys.Space))
            {
                IsGameStart = !IsGameStart;
                foreach(Soldier s in refugees)
                {
                    s.recalculatePath(mReader.GetMap, endPosition);
                }
            }
            if (IsGameStart)
            {

                foreach (Soldier element in refugees)
                {
                    element.Update();
                    if (element.endofpath())
                    {
                        return;
                    }                    
                    element.march(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height,ref mReader);
                }
            }
        }

        private void UpdateBlocks(float posX, float posY)
        {
            int indexX = (int)posX / ConstValues.getTileSize;
            int indexY = (int)posY / ConstValues.getTileSize;
            if (indexX < 0 || indexY < 0 || indexX > ConstValues.getTilesHorizontal - 1 || indexY > ConstValues.getTilesVertical - 1)
            {
                return;
            }    
        }

        private void restartLevel()
        {
            
        }

        private void updateFlags(float posX, float posY)
        {
            int indexX = (int)posX / ConstValues.getTileSize;
            int indexY = (int)posY / ConstValues.getTileSize;
            if (indexX < 0 || indexY < 0 || indexX > ConstValues.getTilesHorizontal - 1 || indexY > ConstValues.getTilesVertical - 1)
            {
                return;
            }
            foreach (Soldier element in refugees)
            {
                element.currentPathIndex = 0;
            }
        }
    }
}
