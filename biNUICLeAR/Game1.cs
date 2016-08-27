using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        const int ScreenWidth = TilesHorizontal * TileSize;
        const int ScreenHeight = TilesVertical * TileSize; 

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
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tiles[,] mapTiles;
        Soldier soldier;

        MouseState currentMouseState;
        KeyboardState currentKeyState;

        PathFinder pathFinder;
        bool isGameStart = false;

        int currentPathIndex = 0;

        Vector2 startPosition;
        Vector2 endPosition = new Vector2(63,23);
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

            mapTiles = new Tiles[ConstValues.getTilesVertical, ConstValues.getTilesHorizontal];
            Random num = new Random();

            for(int i=0; i< ConstValues.getTilesVertical; i++)
            {
                for(int j=0;j< ConstValues.getTilesHorizontal; j++)
                {
                    mapTiles[i,j] = new Tiles();
                    mapTiles[i, j].IsBlock = true;
                }
            }

            soldier = new Soldier();

            pathFinder = new PathFinder();
            pathFinder.initialize();
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
            int mapPosIndex = 0;
            foreach(Tiles p in mapTiles)
            {
                 Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + textureBlock.Width*(mapPosIndex% ConstValues.getTilesHorizontal)
                    , GraphicsDevice.Viewport.TitleSafeArea.Y+textureBlock.Height*(mapPosIndex / ConstValues.getTilesHorizontal));

                p.Initialize(p.isBlock ==true? textureBlock:textureGround, playerPosition);

                mapPosIndex ++;
            }

            Texture2D textureSoldier = Content.Load<Texture2D>("Graphics\\werewolf");
            Vector2 imageStartPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
                                                + (ConstValues.getTilesVertical / 2) * ConstValues.getTileSize);

            soldier.Initialize(textureSoldier, imageStartPosition);

            startPosition = new Vector2(0, ConstValues.getTilesVertical / 2);
            endPosition = new Vector2(ConstValues.getTilesHorizontal - 1, ConstValues.getTilesVertical / 2);



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
            UpdatePlayer(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach(Tiles p in mapTiles)
            {
                p.Draw(spriteBatch);
            }              

            soldier.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {

            //soldier.Position.X += gameTime.ElapsedGameTime;
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                UpdateBlocks(mousePosition.X, mousePosition.Y);
                pathFinder.PathFinding(mapTiles, (int)soldier.Position.X / ConstValues.getTileSize, (int)soldier.Position.Y / ConstValues.getTileSize, 63, 24);
            }

            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                updateFlags(mousePosition.X, mousePosition.Y);
                pathFinder.PathFinding(mapTiles, (int)soldier.Position.X / ConstValues.getTileSize, (int)soldier.Position.Y / ConstValues.getTileSize, 63, 24);
            }


            if (currentKeyState.IsKeyDown(Keys.Space))
            {
                IsGameStart = !IsGameStart;
            }

            if(currentKeyState.IsKeyDown(Keys.Back))
            {
                currentPathIndex = 0;
                soldier.Position.X = startPosition.X * ConstValues.getTileSize;
                soldier.Position.Y = startPosition.Y * ConstValues.getTileSize;
                pathFinder.PathFinding(mapTiles, (int)startPosition.X, (int)startPosition.Y, (int)endPosition.X, (int)endPosition.Y);

                soldier.DestPosition = soldier.Position;
            }
            if (IsGameStart)
            {
                if (pathFinder.getPathNodes.Count <= 0)
                {
                    return;
                }
                if (soldier.march())
                {
                    currentPathIndex +=1;
                    if(currentPathIndex < pathFinder.getPathNodes.Count)
                    {
                        int x = pathFinder.getPathNodes[currentPathIndex].x * ConstValues.getTileSize;
                        int y = pathFinder.getPathNodes[currentPathIndex].y * ConstValues.getTileSize;
                        soldier.DestPosition = new Vector2(x, y);
                        soldier.Direction = soldier.DestPosition - soldier.Position;
                    }

                }

                soldier.Position.X = MathHelper.Clamp(soldier.Position.X, 0, GraphicsDevice.Viewport.Width - soldier.Width);
                soldier.Position.Y = MathHelper.Clamp(soldier.Position.Y, 0, GraphicsDevice.Viewport.Height - soldier.Height);
            }
        }

        private void UpdateBlocks(float posX,float posY)
        {
            int[,] offset =
            {
                {0,0},
                {-1,0},
                {0,1},
                {1,0},
                {0,-1},
                {1,1},
                {-1,-1},
                {1,-1},
                {-1,1},
            };
            for(int i = 0; i < 9; i++)
            {
                int indexX = (int)posX / ConstValues.getTileSize;
                int indexY = (int)posY / ConstValues.getTileSize;
                indexX += offset[i,0];
                indexY += offset[i,1];
                if (indexX < 0 || indexY <0 || indexX>ConstValues.getTilesHorizontal-1 || indexY>ConstValues.getTilesVertical - 1)
                {
                    return;
                }
                mapTiles[indexY, indexX].isBlock = false;
                mapTiles[indexY, indexX].UpdateTexture(Content.Load<Texture2D>("Graphics\\tileground"));
            }


            currentPathIndex = 0;
        }

        private void updateFlags(float posX, float posY)
        {
            int[,] offset =
                       {
                {0,0},
                {-1,0},
                {0,1},
                {1,0},
                {0,-1},
                {1,1},
                {-1,-1},
                {1,-1},
                {-1,1},
            };
            for (int i = 0; i < 9; i++)
            {
                int indexX = (int)posX / ConstValues.getTileSize;
                int indexY = (int)posY / ConstValues.getTileSize;
                indexX += offset[i, 0];
                indexY += offset[i, 1];
                if (indexX < 0 || indexY < 0 || indexX > ConstValues.getTilesHorizontal - 1 || indexY > ConstValues.getTilesVertical - 1)
                {
                    return;
                }
                mapTiles[indexY, indexX].isFlag = true&&(!mapTiles[indexY,indexX].IsBlock);
                if (mapTiles[indexY, indexX].isFlag)
                {
                    mapTiles[indexY, indexX].UpdateTexture(Content.Load<Texture2D>("Graphics\\tileFlag"));
                }
            }
            currentPathIndex = 0;
        }
    }

}
