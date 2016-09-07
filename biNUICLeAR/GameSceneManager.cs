using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Audio;

using GameActor;
namespace biNUICLeAR
{
    public enum GameStage
    {
        StageStartMenu,
        StageCutScene,
        StageLevelOne,
        StageLevelTwo,
        StageLevelThree
    }
    public enum TextureType
    {
        TexGround = 0,
        TexBlock,
        TexMine,
        TexFlag,
        TexSoldier,
        TexEnemy,
        TexCastle,
        TexBackgroundStartMenu
    }
    public enum FontType
    {
        fontGeneral = 0,
        fontNumber
    }
    class GameSceneManager
    {
        GraphicsDeviceManager graphics;

        public GameStage curStage;
        public Texture2D startMenuBackground;
        public Texture2D [] allTextures;
        public SpriteFont [] allFonts;
        public SoundEffectInstance[] allSounds;

        bool isAllowInput = true;
        float inputInterval = 50;

        MouseState currentMouseState;
        KeyboardState currentKeyState;

        bool isGameStart = false;
        bool isGameFinish = false;

        MapScene mReader;
        private List<Soldier> refugees;
        private List<Enemy> enemies;

        List<DrawInfo> flags;
        Timer timer;

        Vector2 endPosition = new Vector2(OptionValues.getTilesHorizontal - 1, OptionValues.getTilesVertical - 1);
        SoundEffectInstance backGroundMusic;

        float secondsBetweenUpdate;
        float secondsBetweenPressureSpawn;
        float secondsSinceLastUpdate;

        String endOutPut;
        SpriteFont gameInfo;
        public void Initialize(GraphicsDeviceManager gm)
        {
            graphics = gm;

            mReader = new MapScene();
            flags = new List<DrawInfo>();
            timer = new Timer();

            refugees = new List<Soldier>();
            enemies = new List<Enemy>();

            curStage = GameStage.StageStartMenu;

        }
        public void initLevel()
        {
            if(curStage == GameStage.StageCutScene)
            {
                return;
            }
            if (curStage == GameStage.StageCutScene)
            {
                return;
            }
            switch (curStage)
            {
                case GameStage.StageLevelOne:
                    {
                        OptionValues.getTilesVertical = 16;
                        OptionValues.getTilesHorizontal = 16;
                        OptionValues.getScreenWidth = 16 * OptionValues.getTileSize;
                        OptionValues.getScreenHeight = 16 * OptionValues.getTileSize;
                        OptionValues.getSoldierCount = 3;
                        OptionValues.getMinesCount = 30;
                        OptionValues.getEnemyCount = 1;
                    }
                    break;
                case GameStage.StageLevelTwo:
                    {
                        OptionValues.getTilesVertical = 32;
                        OptionValues.getTilesHorizontal = 32;
                        OptionValues.getScreenWidth = 32 * OptionValues.getTileSize;
                        OptionValues.getScreenHeight = 32 * OptionValues.getTileSize;
                        OptionValues.getSoldierCount = 6;
                        OptionValues.getMinesCount = 150;
                        OptionValues.getEnemyCount = 3;
                    }
                    break;
                case GameStage.StageLevelThree:
                    {
                        OptionValues.getTilesVertical = 48;
                        OptionValues.getTilesHorizontal = 64;
                        OptionValues.getScreenWidth = 64 * OptionValues.getTileSize;
                        OptionValues.getScreenHeight = 48 * OptionValues.getTileSize;
                        OptionValues.getSoldierCount = 10;
                        OptionValues.getMinesCount = 500;
                        OptionValues.getEnemyCount = 5;
                    }
                    break;
                default:
                    break;
            }
            
            refugees.Clear();
            enemies.Clear();
            for (int i = 0; i < OptionValues.getSoldierCount; i++)
            {
                Soldier tempSoldier = new Soldier();
                tempSoldier.currentPathIndex = 0;
                tempSoldier.Initialize(allTextures[(int)TextureType.TexSoldier], new Vector2(i * 10, 0));

                Random rand = new Random((int)tempSoldier.Position.LengthSquared());
                tempSoldier.speed = rand.Next(50, 75);
                tempSoldier.speed /= 100;

                refugees.Add(tempSoldier);
            }
            mReader.initMap(allTextures[(int)TextureType.TexGround], allTextures[(int)TextureType.TexMine], allTextures[(int)TextureType.TexBlock], allFonts[(int)FontType.fontNumber]);
            mReader.updateMap();

            endPosition = new Vector2(OptionValues.getTilesHorizontal - 1, OptionValues.getTilesVertical - 1);       
        }
        public void LoadGameContent(Texture2D [] textures, SpriteFont [] fonts, SoundEffectInstance [] sounds)
        {
            startMenuBackground = textures[(int)TextureType.TexBackgroundStartMenu];
            gameInfo = fonts[(int)FontType.fontGeneral];
            timer.init(fonts[(int)FontType.fontNumber]);

            allTextures = textures;
            allFonts = fonts;
            allSounds = sounds;
            // music
            backGroundMusic = sounds[0];
        }
        public bool checkBoundaries(Vector2 v)
        {
            if (v.X < 0 || v.Y < 0 || v.X > OptionValues.getScreenWidth || v.Y > OptionValues.getScreenHeight)
                return false;
            return true;
        }

        public bool IsGameStart
        {
            set { isGameStart = value; }
            get { return isGameStart; }
        }

        public void UpdatePlayerInput(GameTime gameTime)
        {
            //input thing
            if (!isAllowInput)
            {
                inputInterval -= gameTime.ElapsedGameTime.Milliseconds;
                if (inputInterval <= 0.0f)
                {
                    inputInterval = 100.0f;
                    isAllowInput = true;
                }
                return;
            }

#if true
            currentMouseState = Mouse.GetState();
            currentKeyState = Keyboard.GetState();
            if(curStage == GameStage.StageStartMenu)
            {
                if (currentKeyState.IsKeyDown(Keys.D1) || currentKeyState.IsKeyDown(Keys.D2) || currentKeyState.IsKeyDown(Keys.D3))
                {
                    isAllowInput = false;
                    if(currentKeyState.IsKeyDown(Keys.D1))
                        curStage = GameStage.StageLevelOne;
                    else if (currentKeyState.IsKeyDown(Keys.D2))
                        curStage = GameStage.StageLevelTwo;
                    else if (currentKeyState.IsKeyDown(Keys.D3))
                        curStage = GameStage.StageLevelThree;
                    initLevel();
                    IsGameStart = !IsGameStart;
                    backGroundMusic.Stop();
                    backGroundMusic.Play();
                    mReader.RevealBlock(0, 0);
                }
                return;
            }

            if (currentKeyState.IsKeyDown(Keys.Back))
            {
                isAllowInput = false;
                if (isGameFinish)
                {
                    isGameStart = isGameFinish = false;
                    mReader.resetMap();
                    mReader.updateMap();
                    timer.reset();

                    refugees.Clear();
                    refugees = new List<Soldier>();
                    for (int i = 0; i < OptionValues.getSoldierCount; i++)
                    {
                        Soldier tempSoldier = new Soldier();
                        tempSoldier.currentPathIndex = 0;
                        refugees.Add(tempSoldier);
                    }
                    Texture2D textureSoldier = allTextures[(int)TextureType.TexEnemy];
                    foreach (Soldier p in refugees)
                    {
                        p.Initialize(textureSoldier, p.Position);
                    }

                }
            }
            if (currentKeyState.IsKeyDown(Keys.Space))
            {
                isAllowInput = false;
                mReader.RevealBlock(0, 0);
                IsGameStart = !IsGameStart;
                backGroundMusic.Stop();
                backGroundMusic.Play();
                foreach (Soldier s in refugees)
                {
                    s.recalculatePath(mReader.GetMap, endPosition);
                }
            }
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
            if (!checkBoundaries(mousePosition))
            {
                return;
            }
            if (currentMouseState.LeftButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed)
            {
                isAllowInput = false;
                mReader.DetectBlocks((int)mousePosition.X / OptionValues.getTileSize, (int)mousePosition.Y / OptionValues.getTileSize);
            }
            else if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                isAllowInput = false;
                if (mReader.GetMap[(int)mousePosition.Y / OptionValues.getTileSize, (int)mousePosition.X / OptionValues.getTileSize].isFlag)
                {
                    for (int i = 0; i < flags.Count; i++)
                    {
                        DrawInfo f = flags.ElementAt(i);
                        if ((f.position.X == OptionValues.getTileSize * ((int)mousePosition.X / OptionValues.getTileSize) &&
                            (f.position.Y == OptionValues.getTileSize * ((int)mousePosition.Y / OptionValues.getTileSize))))
                        {
                            flags.Remove(f);
                        }
                    }
                }
                mReader.RevealBlock((int)mousePosition.X / OptionValues.getTileSize, (int)mousePosition.Y / OptionValues.getTileSize);
                if ((((int)mousePosition.X / OptionValues.getTileSize) <= OptionValues.getTilesHorizontal) && (((int)mousePosition.Y / OptionValues.getTileSize) <= OptionValues.getTilesVertical) && (((int)mousePosition.X / OptionValues.getTileSize) > 0) && (((int)mousePosition.Y / OptionValues.getTileSize) > 0))
                {
                    isAllowInput = false;
                    if (mReader.mapTiles[(int)mousePosition.Y / OptionValues.getTileSize, (int)mousePosition.X / OptionValues.getTileSize].isMined && mReader.mapTiles[(int)mousePosition.Y / OptionValues.getTileSize, (int)mousePosition.X / OptionValues.getTileSize].isRevealed)
                    {
                        for (int i = 0; i <= 5; i++)
                        {
                            Enemy temp = new Enemy();
                            Texture2D textureEnemy = allTextures[(int)TextureType.TexEnemy];
                            temp.Initialize(textureEnemy, mReader.mapTiles[(int)mousePosition.Y / OptionValues.getTileSize, (int)mousePosition.X / OptionValues.getTileSize].Position);
                            enemies.Add(temp);
                        }
                    }
                }

            }
            else if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                if (mReader.GetMap[(int)mousePosition.Y / OptionValues.getTileSize, (int)mousePosition.X / OptionValues.getTileSize].isRevealed)
                {
                    isAllowInput = true;
                    foreach (Soldier s in refugees)
                        s.recalculatePath(mReader.GetMap, new Vector2((int)currentMouseState.X / OptionValues.getTileSize, (int)currentMouseState.Y / OptionValues.getTileSize));
                }
                else
                {
                    isAllowInput = false;
                    if (mReader.PlaceFlag((int)mousePosition.X / OptionValues.getTileSize, (int)mousePosition.Y / OptionValues.getTileSize))
                    {
                        flags.Add(new DrawInfo(allTextures[(int)TextureType.TexFlag], new Vector2(OptionValues.getTileSize * ((int)mousePosition.X / OptionValues.getTileSize), OptionValues.getTileSize * ((int)mousePosition.Y / OptionValues.getTileSize))));
                    }
                    else
                    {
                        for (int i = 0; i < flags.Count; i++)
                        {
                            DrawInfo f = flags.ElementAt(i);
                            if ((f.position.X == OptionValues.getTileSize * ((int)mousePosition.X / OptionValues.getTileSize) &&
                                (f.position.Y == OptionValues.getTileSize * ((int)mousePosition.Y / OptionValues.getTileSize))))
                            {
                                flags.Remove(f);
                            }
                        }
                    }
                }
            }
#endif
        }
        public void drawGame(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(curStage == GameStage.StageStartMenu)
            {
                graphics.PreferredBackBufferWidth = startMenuBackground.Width;
                graphics.PreferredBackBufferHeight = startMenuBackground.Height;
                graphics.ApplyChanges();

                spriteBatch.Draw(startMenuBackground,Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                return;
            }

            graphics.PreferredBackBufferWidth = OptionValues.getScreenWidth;
            graphics.PreferredBackBufferHeight = OptionValues.getScreenHeight;
            graphics.ApplyChanges();
            mReader.drawMap(spriteBatch);
            timer.draw(spriteBatch);
            foreach (Soldier element in refugees)
                element.Draw(spriteBatch);
            foreach (Enemy element in enemies)
            {
                element.Draw(spriteBatch);
            }

            foreach (DrawInfo flag in flags)
                spriteBatch.Draw(flag.texture, flag.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            if (isGameFinish)
            {
                Vector2 size = gameInfo.MeasureString(endOutPut);
                spriteBatch.DrawString(gameInfo, endOutPut, new Vector2(OptionValues.getScreenWidth / 2 - size.X / 2, OptionValues.getScreenHeight / 2 - size.Y / 2), Color.Black);
            }
        }
        public void UpdateGame(GameTime gameTime)
        {
            if (IsGameStart)
            {
                timer.Update(gameTime);
                foreach (Soldier element in refugees)
                {
                    element.Update();
                    if (!element.endofpath())
                        element.march(ref mReader);
                }
            }

            isGameFinish = false;
            foreach (Soldier element in refugees)
            {
                if (((int)element.Position.X / OptionValues.getTileSize != (int)endPosition.X) ||
                    ((int)element.Position.Y / OptionValues.getTileSize != (int)endPosition.Y))
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
                                , refugees.Count, timer.minute, timer.seconds, timer.miniseconds);
            }
        }

        public void UpdateActors(GameTime gameTime)
        {
            secondsBetweenUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
            secondsBetweenPressureSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (secondsBetweenPressureSpawn >= OptionValues.getStartPressureTime)
            {
                secondsBetweenPressureSpawn = 0;
                Enemy temp = new Enemy();
                Texture2D textureEnemy = allTextures[(int)TextureType.TexEnemy];
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
                            secondsSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (secondsSinceLastUpdate < 0.5)
                                break;
                            else
                                secondsSinceLastUpdate = 0;
                            if (element.death(refugees[0].Position))
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

            if (IsGameStart)
            {
                foreach (Soldier element in refugees)
                {
                    element.Update();
                    if (!element.endofpath())
                    {
                        element.march(ref mReader);
                    }
                }
                foreach (Enemy element in enemies)
                {
                    element.Update();
                    if (element.endofpath())
                    {
                        element.recalculatePath(mReader.GetMap, updateGoal(element));
                    }
                    element.march(ref mReader);
                }
            }
        }

        private Vector2 updateGoal(Enemy e)
        {
            if (e.isHuntingMode == true)
            {
                return refugees[0].Position / OptionValues.getTileSize;
            }
            else
            {
                Random random = new Random((int)e.Position.LengthSquared());
                int nrVerti = random.Next(OptionValues.getTilesVertical);
                int nrHoriz = random.Next(OptionValues.getTilesHorizontal);
                return new Vector2(nrHoriz, nrVerti);
            }

        }
    }
}
