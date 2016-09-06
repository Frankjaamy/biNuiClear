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
        const int StartPressureTimeSeconds = 1 * 60;//60 sec
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

        public static int getStartPressureTime
        {
            get { return StartPressureTimeSeconds; }
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
        Timer timer;
        private List<Soldier> refugees;
        private List<Enemy> enemies;
        float secondsBetweenUpdate;
        float secondsBetweenPressureSpawn;
        MouseState currentMouseState;
        KeyboardState currentKeyState;

        String endOutPut;
        SpriteFont gameInfo;

        float timeIntervals = 50.0f;
        bool isGameStart = false;
        bool isGameFinish = false;

        Vector2 endPosition = new Vector2(ConstValues.getTilesHorizontal - 1, ConstValues.getTilesVertical - 1);
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
            timer = new Timer();

            refugees = new List<Soldier>();
            enemies = new List<Enemy>();
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
            

            SpriteFont sf = Content.Load<SpriteFont>("Information");
            gameInfo = sf;
            SpriteFont generalFont = Content.Load<SpriteFont>("Number");
            mReader.initMap(textureGround, textureMine,textureBlock,generalFont);
            mReader.updateMap();
            timer.init(generalFont);
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
            UpdateEVERYTHING(gameTime);

          
            if (IsGameStart)
            {
                timer.Update(gameTime);
                foreach (Soldier element in refugees)
                {
                    element.Update();
                    if (element.endofpath())
                    {
                        return;
                    }
                    element.march(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, ref mReader);
                }
            }

            isGameFinish = false;
            foreach (Soldier element in refugees)
            {
                if (((int)element.Position.X / ConstValues.getTileSize != (int)endPosition.X) ||
                    ((int)element.Position.Y / ConstValues.getTileSize != (int)endPosition.Y))
                {
                    break;
                }
                isGameFinish = true;
            }

            if (isGameFinish)
            {
                isGameStart = false;
                //game end
                endOutPut = string.Format(
@"           Game End.  
         Soldiers Left: {0}   
Time Used:{1,3:00}:{2,3:000}:{3,3:000}"
                                ,refugees.Count,timer.minute, timer.seconds, timer.miniseconds);
            }
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
            timer.draw(spriteBatch);
            foreach (Soldier element in refugees)
                element.Draw(spriteBatch);
            foreach (Enemy element in enemies)
            {
                element.Draw(spriteBatch);
            }

            foreach(DrawInfo flag in flags)
                spriteBatch.Draw(flag.texture, flag.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            if (isGameFinish)
            {
                Vector2 size = gameInfo.MeasureString(endOutPut);
                spriteBatch.DrawString(gameInfo, endOutPut, new Vector2(ConstValues.getScreenWidth / 2 - size.X/2, ConstValues.getScreenHeight / 2 - size.Y/2), Color.Black);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
        //Better name for function
        private void UpdateEVERYTHING(GameTime gameTime)
        {
            secondsBetweenUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
            secondsBetweenPressureSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (secondsBetweenPressureSpawn >= ConstValues.getStartPressureTime)
            {
                secondsBetweenPressureSpawn = 0;
                Enemy temp = new Enemy();
                Texture2D textureEnemy = Content.Load<Texture2D>("Graphics\\charactersprite");
                temp.Initialize(textureEnemy, new Vector2(0, 0));
                enemies.Add(temp);
            }

            foreach (Enemy element in enemies)
            {
                if (refugees.Count > 0)
                    if (secondsBetweenUpdate >= 10 || element.closeToRefugee(refugees[0].Position))
                    {
                        if (element.closeToRefugee(refugees[0].Position))
                        {
                            if(element.death(refugees[0].Position))
                            {
                                enemies.Remove(element);
                                refugees.Remove(refugees[0]);
                                return;
                            }

                            element.isHuntingMode = true;
                        }
                        else
                            element.isHuntingMode = false;
                        element.recalculatePath(mReader.GetMap, updateGoal(element));
                    }
            }
            if (secondsBetweenUpdate >= 10)
                secondsBetweenUpdate = 0;
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                mReader.DetectBlocks((int)mousePosition.X / ConstValues.getTileSize, (int)mousePosition.Y / ConstValues.getTileSize);
            }
            else if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                mReader.RevealBlock((int)mousePosition.X/ConstValues.getTileSize, (int)mousePosition.Y/ConstValues.getTileSize);
                if (((int)mousePosition.X / ConstValues.getTileSize) <= ConstValues.getTilesHorizontal && ((int)mousePosition.Y / ConstValues.getTileSize) <= ConstValues.getTilesVertical && ((int)mousePosition.X / ConstValues.getTileSize) > 0 && ((int)mousePosition.Y / ConstValues.getTileSize) > 0)
                    if (mReader.mapTiles[(int)mousePosition.Y / ConstValues.getTileSize, (int)mousePosition.X / ConstValues.getTileSize].isMined && mReader.mapTiles[(int)mousePosition.Y / ConstValues.getTileSize, (int)mousePosition.X / ConstValues.getTileSize].isRevealed)
                    {
                        for (int i = 0; i <=5; i++)
                        {
                            Enemy temp = new Enemy();
                            Texture2D textureEnemy = Content.Load<Texture2D>("Graphics\\charactersprite");
                            temp.Initialize(textureEnemy, mReader.mapTiles[(int)mousePosition.Y / ConstValues.getTileSize, (int)mousePosition.X / ConstValues.getTileSize].Position);
                            enemies.Add(temp);
                        }

                    }
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
                if(mReader.GetMap[(int)mousePosition.Y / ConstValues.getTileSize,(int)mousePosition.X / ConstValues.getTileSize].isRevealed)
                {
                    foreach (Soldier s in refugees)
                        s.recalculatePath(mReader.GetMap, new Vector2((int)currentMouseState.X / ConstValues.getTileSize, (int)currentMouseState.Y / ConstValues.getTileSize));
                }
                else
                {
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
            }

            if (currentKeyState.IsKeyDown(Keys.Back))
            {
                if (isGameFinish)
                {
                    isGameStart = isGameFinish = false;
                    mReader.resetMap();
                    mReader.updateMap();
                    timer.reset();
                    
                    refugees.Clear();
                    refugees = new List<Soldier>();
                    for (int i = 0; i < 1; i++)
                    {
                        Soldier tempSoldier = new Soldier();
                        tempSoldier.currentPathIndex = 0;
                        refugees.Add(tempSoldier);
                    }
                    Texture2D textureSoldier = Content.Load<Texture2D>("Graphics\\charactersprite");
                    foreach (Soldier p in refugees)
                    {
                        p.Initialize(textureSoldier, p.Position);
                    }

                }
            }
            if (currentKeyState.IsKeyDown(Keys.Space))
            {
                mReader.RevealBlock(0, 0);
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
                foreach (Enemy element in enemies)
                {
                    element.Update();
                    if (element.endofpath())
                    {
                        element.recalculatePath(mReader.GetMap, updateGoal(element));
                    }
                    element.march(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, ref mReader);
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

        private Vector2 updateGoal(Enemy e)
        {
            if (e.isHuntingMode == true)
            {
                return refugees[0].Position/ ConstValues.getTileSize;
            }
            else
            {
                Random random = new Random((int)e.Position.LengthSquared());
                int nrVerti = random.Next(ConstValues.getTilesVertical);
                int nrHoriz = random.Next(ConstValues.getTilesHorizontal);
                return new Vector2(nrHoriz, nrVerti);
            }

        }
    }
}
