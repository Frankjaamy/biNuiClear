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

        public static Vector2 endPos = new Vector2(0,0);
        static int soldiersCount = 5;
        public static int getSoldierCount
        {
            set { soldiersCount = value; }
            get { return soldiersCount; }
        }
        public static Vector2 EndPosition
        {
            set { endPos = value; }
            get { return endPos; }
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
            Texture2D[] allTextures = new Texture2D[27];
            allTextures[0] = Content.Load<Texture2D>("Graphics\\Ground");
            allTextures[1] = Content.Load<Texture2D>("Graphics\\baseBrick2");
            allTextures[2] = Content.Load<Texture2D>("Graphics\\skull");
            allTextures[3] = Content.Load<Texture2D>("Graphics\\Flag");
            allTextures[4] = Content.Load<Texture2D>("Graphics\\character");
            allTextures[5] = Content.Load<Texture2D>("Graphics\\regularenemy");

    
            allTextures[6] = Content.Load<Texture2D>("Graphics\\CastleSprite");
            allTextures[7] = Content.Load<Texture2D>("Graphics\\Cutscene");
            allTextures[8] = Content.Load<Texture2D>("Graphics\\Level Select");


            allTextures[9] = Content.Load<Texture2D>("Graphics\\blood2grave2");
            allTextures[10] = Content.Load<Texture2D>("Graphics\\chaseenemy.png");
            allTextures[11] = Content.Load<Texture2D>("Graphics\\block1");
            allTextures[12] = Content.Load<Texture2D>("Graphics\\block2");
            allTextures[13] = Content.Load<Texture2D>("Graphics\\block3");
            allTextures[14] = Content.Load<Texture2D>("Graphics\\block4");
            allTextures[15] = Content.Load<Texture2D>("Graphics\\block5");
            allTextures[16] = Content.Load<Texture2D>("Graphics\\block6");
            allTextures[17] = Content.Load<Texture2D>("Graphics\\block7");
            allTextures[18] = Content.Load<Texture2D>("Graphics\\block8");
            allTextures[19] = Content.Load<Texture2D>("Graphics\\block9");
            allTextures[20] = Content.Load<Texture2D>("Graphics\\block10");
            allTextures[21] = Content.Load<Texture2D>("Graphics\\block11");
            allTextures[22] = Content.Load<Texture2D>("Graphics\\block12");
            allTextures[23] = Content.Load<Texture2D>("Graphics\\block13");
            allTextures[24] = Content.Load<Texture2D>("Graphics\\block14");
            allTextures[25] = Content.Load<Texture2D>("Graphics\\block15");
            allTextures[26] = Content.Load<Texture2D>("Graphics\\block16");

            SpriteFont[] allFonts = new SpriteFont[6];
            allFonts[0] = Content.Load<SpriteFont>("Information");
            allFonts[1] = Content.Load<SpriteFont>("Number");

            SoundEffectInstance [] allSounds = new SoundEffectInstance[10];
            allSounds[0] = Content.Load<SoundEffect>("TeamAssaultLoop").CreateInstance();
            allSounds[0].IsLooped = true;
            allSounds[1] = Content.Load<SoundEffect>("Click on Regular Block").CreateInstance();
            allSounds[2] = Content.Load<SoundEffect>("Click on Monster Block").CreateInstance();
            allSounds[3] = Content.Load<SoundEffect>("Click to Move").CreateInstance();
            allSounds[4] = Content.Load<SoundEffect>("Character Death Yell").CreateInstance();
            allSounds[5] = Content.Load<SoundEffect>("Character Death Blood Splat").CreateInstance();
            allSounds[6] = Content.Load<SoundEffect>("Success").CreateInstance();
            allSounds[7] = Content.Load<SoundEffect>("Game Over").CreateInstance();
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
