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
        const int TileSize = 64;

        const int MapTilesVertical = 12;
        const int MapTilesHorizontal = 16;

        const int TilesVertical = 12 * (512/64);
        const int TilesHorizontal = 16 * (512/64);

        const int ScreenWidth = 1920;
        const int ScreenHeight = 1080;

        const int MapWidth = TileSize * TilesHorizontal;
        const int MapHeight = TileSize * TilesVertical;


        public static int getMapWidth
        {
            get { return MapWidth; }
        }
        public static int getMapHeight
        {
            get { return MapHeight; }
        }
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
        public static int getMapTilesVertical
        {
            get { return MapTilesVertical; }
        }
        public static int getMapTilesHorizontal
        {
            get { return MapTilesHorizontal; }
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

        public Texture2D textureAdditional;
        public Vector2 positionAdditional;
    };

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Tiles[,] mapTiles;
        TileType[,] drawTiles;
        DrawInfo[,] drawTextures;

        List<DrawInfo> flags;

        MapReader mReader;

        private List<Soldier> refugees;
        private List<Enemy> enemies;

        MouseState currentMouseState;
        KeyboardState currentKeyState;

        Camera camera;
        bool isGameStart = false;

        Vector2 endPosition = new Vector2(ConstValues.getTilesHorizontal - 1, 1);
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
            enemies = new List<Enemy>();
            for (int i = 0; i < EnemyCoordinates.coords.Count(); i++)
            {
                enemies.Add(new Enemy());
            }
            mapTiles = new Tiles[ConstValues.getTilesVertical, ConstValues.getTilesHorizontal];

            for (int i = 0; i < ConstValues.getTilesVertical; i++)
            {
                for (int j = 0; j < ConstValues.getTilesHorizontal; j++)
                {
                    mapTiles[i, j] = new Tiles();
                    mapTiles[i, j].Position = new Vector2(j*ConstValues.getTileSize,i*ConstValues.getTileSize);
                }
            }

            drawTiles = new TileType[ConstValues.getMapTilesVertical, ConstValues.getMapTilesHorizontal];
            drawTextures = new DrawInfo[ConstValues.getMapTilesVertical, ConstValues.getMapTilesHorizontal];

            flags = new List<DrawInfo>();
            mReader = new MapReader();
            mReader.initMap(mReader.MapLevelOne, mapTiles, drawTiles);

            refugees = new List<Soldier>();

            for (int i = 0; i < 5; i++)
            {
                Soldier tempSoldier = new Soldier();
                tempSoldier.currentPathIndex = 0;
                refugees.Add(tempSoldier);
            }

            camera = new Camera(GraphicsDevice.Viewport);
            //camera.Limits = new Rectangle(0, 0, ConstValues.getMapWidth, ConstValues.getMapHeight);

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

            Texture2D[] textureList =
            {
                 Content.Load<Texture2D>("Graphics\\1"),
                 Content.Load<Texture2D>("Graphics\\2"),
                 Content.Load<Texture2D>("Graphics\\3"),
                 Content.Load<Texture2D>("Graphics\\4"),
                 Content.Load<Texture2D>("Graphics\\5"),
                 Content.Load<Texture2D>("Graphics\\6"),
                 Content.Load<Texture2D>("Graphics\\7"),
                 Content.Load<Texture2D>("Graphics\\8"),
                 Content.Load<Texture2D>("Graphics\\9"),
                 Content.Load<Texture2D>("Graphics\\10"),
            };
            int mapPosIndex = 0;
            for (int i = 0; i < ConstValues.getMapTilesVertical; i++)
            {
                for (int j = 0; j < ConstValues.getMapTilesHorizontal; j++)
                {
                    Vector2 texturePosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + textureBlock.Width * (mapPosIndex % ConstValues.getMapTilesHorizontal),
                     GraphicsDevice.Viewport.TitleSafeArea.Y + textureBlock.Height * (mapPosIndex / ConstValues.getMapTilesHorizontal));
                    int temp = (int)drawTiles[i, j] - 1;
                    drawTextures[i, j].texture = textureList[temp];
                    drawTextures[i, j].position = texturePosition;
                    mapPosIndex++;
                }

            }

            // Make texture change here
            Texture2D textureSoldier = Content.Load<Texture2D>("Graphics\\Actor");
            //Enemy positions
            int index = 0;
            foreach (Enemy e in enemies)
            {
                e.Initialize(textureSoldier, EnemyCoordinates.coords[index]*ConstValues.getTileSize);

                index++;
            }

            int indexSoldire = 0;
            foreach (Soldier element in refugees)
            {
                Vector2 imageStartPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + (indexSoldire*ConstValues.getTileSize));
                element.Initialize(textureSoldier, imageStartPosition);
                element.recalculatePath(mapTiles, endPosition);
                indexSoldire++;
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
            UpdatePlayer(gameTime);
            camera.Update(gameTime);
            foreach (Enemy e in enemies)
                e.Update();

            for (int i = 0; i < refugees.Count(); i++)
            {
                refugees[i].Update();
                for (int j = 0; j < enemies.Count(); j++)
                    if (enemies[j].closeToRefugee(refugees[i].Position))
                    {
                        refugees.RemoveAt(i);
                        enemies.RemoveAt(j);
                        i--;
                        j--;
                        if (i < 0 || j < 0)
                            return;
                    }
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
            // TODO: Add your drawing code here
            var viewMatrix = camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: viewMatrix);
            foreach (DrawInfo p in drawTextures)
                spriteBatch.Draw(p.texture, p.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            foreach (DrawInfo f in flags)
            {
                spriteBatch.Draw(f.textureAdditional, f.positionAdditional, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(f.texture, f.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);
            }
            foreach(Tiles p in mapTiles)
            {
                if(p.PlayerTexture != null)
                {
                    spriteBatch.Draw(p.PlayerTexture, p.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1.0f);
                }
            }
            foreach (Enemy e in enemies)
                e.Draw(spriteBatch);
            foreach (Soldier element in refugees)
                element.Draw(spriteBatch);


            spriteBatch.End();
            base.Draw(gameTime);
        }
        private void UpdatePlayer(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(camera.GetViewMatrix()));
                UpdateBlocks(worldPosition.X, worldPosition.Y);

                foreach (Soldier element in refugees)
                {
                    element.recalculatePath(mapTiles, endPosition);
                    element.currentPathIndex = 0;
                }
            }
            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                Vector2 worldPosition = Vector2.Transform(mousePosition, Matrix.Invert(camera.GetViewMatrix()));
                updateFlags(worldPosition.X, worldPosition.Y);

                foreach (Soldier element in refugees)
                {
                    element.recalculatePath(mapTiles, endPosition);
                    element.currentPathIndex = 0;
                }

            }

            if (currentKeyState.IsKeyDown(Keys.Space))
            {
                IsGameStart = !IsGameStart;
            }

            if (currentKeyState.IsKeyDown(Keys.Back))
            {
                restartLevel();
                foreach (Soldier element in refugees)
                {
                    element.currentPathIndex = 0;
                    element.Position.X = element.startPosition.X;
                    element.Position.Y = element.startPosition.Y;
                    element.recalculatePath(mapTiles, endPosition);

                    element.DestPosition = element.Position;
                }

            }
            if (IsGameStart)
            {

                foreach (Soldier element in refugees)
                {
                    if (element.endofpath())
                    {
                        return;
                    }
                    element.march(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                }

            }
        }

        private void UpdateBlocks(float posX, float posY)
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
                mapTiles[indexY, indexX].isBlock = false;
                mapTiles[indexY, indexX].UpdateTexture(Content.Load<Texture2D>("Graphics\\tileground"));
            }
        }

        private void restartLevel()
        {
            enemies.Clear();
            for (int k = 0; k < 60; k++)
            {
                enemies.Add(new Enemy());
            }
            mapTiles = new Tiles[ConstValues.getTilesVertical, ConstValues.getTilesHorizontal];
            Random num = new Random();

            for (int l = 0; l < ConstValues.getTilesVertical; l++)
            {
                for (int j = 0; j < ConstValues.getTilesHorizontal; j++)
                {
                    mapTiles[l, j] = new Tiles();
                    mapTiles[l, j].IsBlock = true;
                }
            }
            refugees.Clear();

            for (int m = 0; m < 10; m++)
            {
                Soldier tempSoldier = new Soldier();
                tempSoldier.currentPathIndex = 0;
                refugees.Add(tempSoldier);
            }

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture2D textureGround = Content.Load<Texture2D>("Graphics\\tileground");
            Texture2D textureBlock = Content.Load<Texture2D>("Graphics\\tileblock");
            int mapPosIndex = 0;
            foreach (Tiles p in mapTiles)
            {
                Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + textureBlock.Width * (mapPosIndex % ConstValues.getTilesHorizontal),
                    GraphicsDevice.Viewport.TitleSafeArea.Y + textureBlock.Height * (mapPosIndex / ConstValues.getTilesHorizontal));
                p.Initialize(p.isBlock == true ? textureBlock : textureGround, playerPosition);

                mapPosIndex++;
            }


            // Make texture change here
            Texture2D textureSoldier = Content.Load<Texture2D>("Graphics\\Actor");
            //Enemy positions
            for (int j = 0; j < 60; j++)
            {
                enemies[j].Initialize(textureSoldier, new Vector2((j + 1) * ConstValues.getTileSize, (j + 1) * ConstValues.getTileSize));
            }


            int i = 0;
            foreach (Soldier element in refugees)
            {
                Vector2 imageStartPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + (i * ConstValues.getTileSize), GraphicsDevice.Viewport.TitleSafeArea.Y
                       + (ConstValues.getTilesVertical / 2) * ConstValues.getTileSize);
                element.Initialize(textureSoldier, imageStartPosition);
                element.recalculatePath(mapTiles, new Vector2(63, 24));
                i++;
            }
            endPosition = new Vector2(ConstValues.getTilesHorizontal - 1, ConstValues.getTilesVertical / 2);
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
            mapTiles[indexY, indexX].isFlag = true && (!mapTiles[indexY, indexX].IsBlock);
            if (mapTiles[indexY, indexX].isFlag)
            {
                DrawInfo flag = new DrawInfo();
                flag.texture = Content.Load<Texture2D>("Graphics\\Flag");
                flag.position = new Vector2(posX - flag.texture.Width / 2, posY - flag.texture.Height / 2);
                flag.textureAdditional = Content.Load<Texture2D>("Graphics\\Flag_Range");
                flag.positionAdditional = new Vector2(posX - flag.textureAdditional.Width / 2, posY - flag.textureAdditional.Height / 2);
                flags.Add(flag);

                float[,] points =
                {
                    {posX-flag.textureAdditional.Width/2,posY-flag.textureAdditional.Height/2},
                    {posX+flag.textureAdditional.Width/2,posY-flag.textureAdditional.Height/2},
                    {posX-flag.textureAdditional.Width/2,posY+flag.textureAdditional.Height/2},
                    {posX+flag.textureAdditional.Width/2,posY+flag.textureAdditional.Height/2},
                    {posX,posY-flag.textureAdditional.Height/2},
                    {posX,posY+flag.textureAdditional.Height/2},
                    {posX-flag.textureAdditional.Width/2,posY},
                    {posX+flag.textureAdditional.Width/2,posY},

                };
                for(float x = posX - flag.textureAdditional.Width / 2; x< posX + flag.textureAdditional.Width / 2; x+=4.0f)
                {
                    for(float y = posY - flag.textureAdditional.Height / 2; y < posY + flag.textureAdditional.Height / 2;y+=4.0f)
                    {
                        indexX = (int )x / ConstValues.getTileSize;
                        indexY = (int)y / ConstValues.getTileSize;
                        if (indexX < 0 || indexY < 0 || indexX > ConstValues.getTilesHorizontal - 1 || indexY > ConstValues.getTilesVertical - 1)
                        {
                            continue;
                        }
                        mapTiles[indexY, indexX].isFlag = true && (!mapTiles[indexY, indexX].IsBlock);
                        mapTiles[indexY, indexX].PlayerTexture = Content.Load<Texture2D>("Graphics\\tileFlag");
                    }
                }
              
            }
        }
    }
}
