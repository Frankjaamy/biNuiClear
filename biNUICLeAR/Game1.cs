using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameActor;
using Microsoft.Xna.Framework.Media;

namespace biNUICLeAR
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    public static class OptionValues
    {
        const int TileSize = 16;

        const int StartPressureTimeSeconds = 1 * 60;//60 sec
        static int TilesVertical = 16;
        static int TilesHorizontal = 16;

        static int ScreenWidth = TilesHorizontal*TileSize;
        static int ScreenHeight = TilesVertical * TileSize;

        static int soldiersCount = 5;
        public static int getSoldierCount
        {
            set { soldiersCount = value; }
            get { return soldiersCount; }
        }

        static int minesCount = 5;
        public static int getMinesCount
        {
            set { minesCount = value; }
            get { return minesCount; }
        }
        static int enemyCount = 5;
        public static int getEnemyCount
        {
            set { enemyCount = value; }
            get { return enemyCount; }
        }
        public static int getTileSize
        {
            get { return TileSize; }
        }
        public static int getTilesVertical
        {
            set { TilesVertical = value;}
            get { return TilesVertical; }
        }
        public static int getTilesHorizontal
        {
            set { TilesHorizontal = value; }
            get { return TilesHorizontal; }
        }
        public static int getScreenWidth
        {
            set { ScreenWidth = value; }
            get { return ScreenWidth; }
        }
        public static int getScreenHeight
        {
            set { ScreenHeight = value; }
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
        GameSceneManager gm;
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
            gm = new GameSceneManager();
            gm.Initialize(graphics);

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
            Texture2D[] allTextures = new Texture2D[10];
            allTextures[0] = Content.Load<Texture2D>("Graphics\\Ground");
            allTextures[1] = Content.Load<Texture2D>("Graphics\\baseBrick");
            allTextures[2] = Content.Load<Texture2D>("Graphics\\werewolf");
            allTextures[3] = Content.Load<Texture2D>("Graphics\\Flag");
            allTextures[4] = Content.Load<Texture2D>("Graphics\\character");
            allTextures[5] = Content.Load<Texture2D>("Graphics\\backupEnemy");

            allTextures[7] = Content.Load<Texture2D>("Graphics\\Zombies-Run");

            SpriteFont[] allFonts = new SpriteFont[6];
            allFonts[0] = Content.Load<SpriteFont>("Information");
            allFonts[1] = Content.Load<SpriteFont>("Number");

            SoundEffectInstance [] allSounds = new SoundEffectInstance[10];
            allSounds[0] = Content.Load<SoundEffect>("TeamAssaultLoop").CreateInstance();
            allSounds[0].IsLooped = true;
            
            gm.LoadGameContent(allTextures, allFonts, allSounds);

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
            gm.UpdateGame(gameTime);
            gm.UpdateActors(gameTime);
            gm.UpdatePlayerInput(gameTime);
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
            gm.drawGame(gameTime, spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
