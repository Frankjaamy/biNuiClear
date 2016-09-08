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
        StageGame
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
        TexCutScene,
        TexBackgroundStartMenu
    }
    public enum SoundType
    {
        sBackground = 0,
        sClickRegular,
        sClickMonster,
        sClickMove,
        sDeathYell,
        sDeathBloodSplat,
        sSuccess,
        sGameOver
    }
    public enum FontType
    {
        fontGeneral = 0,
        fontNumber
    }
    class GameSceneManager
    {
        GraphicsDeviceManager graphics;
        public int level;
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
        private List<Soldier> soldiers;
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

        float sizeFont = 0.5f;
        public void Initialize(GraphicsDeviceManager gm)
        {
            graphics = gm;

            mReader = new MapScene();
            flags = new List<DrawInfo>();
            timer = new Timer();

            soldiers = new List<Soldier>();
            enemies = new List<Enemy>();

            curStage = GameStage.StageStartMenu;

        }
        public void initLevel()
        {
            if(curStage == GameStage.StageCutScene)
            {
                OptionValues.getTilesVertical = 48;
                OptionValues.getTilesHorizontal = 64;
                OptionValues.getScreenWidth = 64 * OptionValues.getTileSize;
                OptionValues.getScreenHeight = 48 * OptionValues.getTileSize;
                OptionValues.getSoldierCount = 20;
                OptionValues.getMinesCount = 0;
                OptionValues.getEnemyCount = 100;
                sizeFont = 1.5f;

                soldiers.Clear();
                enemies.Clear();
                for (int i = 0; i < OptionValues.getSoldierCount; i++)
                {
                    Soldier tempSoldier = new Soldier();
                    tempSoldier.currentPathIndex = 0;
                    tempSoldier.Initialize(allTextures[(int)TextureType.TexSoldier], new Vector2(i * 10, 0));

                    Random rand = new Random((int)tempSoldier.Position.LengthSquared());
                    tempSoldier.speed = rand.Next(50, 75);
                    tempSoldier.speed /= 100;

                    soldiers.Add(tempSoldier);
                }
                endPosition = new Vector2(OptionValues.getTilesHorizontal - 2, OptionValues.getTilesVertical - 1);
                mReader.initMap(allTextures[(int)TextureType.TexGround], allTextures[(int)TextureType.TexMine], allTextures[(int)TextureType.TexBlock], allTextures[(int)TextureType.TexCastle], endPosition, allFonts[(int)FontType.fontNumber]);
                mReader.updateMap();
                return;
            }
            switch (level)
            {
                case 1:
                    {
                        OptionValues.getTilesVertical = 16;
                        OptionValues.getTilesHorizontal = 16;
                        OptionValues.getScreenWidth = 16 * OptionValues.getTileSize;
                        OptionValues.getScreenHeight = 16 * OptionValues.getTileSize;
                        OptionValues.getSoldierCount = 3;
                        OptionValues.getMinesCount = 30;
                        OptionValues.getEnemyCount = 1;
                        sizeFont = 0.5f;
                    }
                    break;
                case 2:
                    {
                        OptionValues.getTilesVertical = 32;
                        OptionValues.getTilesHorizontal = 32;
                        OptionValues.getScreenWidth = 32 * OptionValues.getTileSize;
                        OptionValues.getScreenHeight = 32 * OptionValues.getTileSize;
                        OptionValues.getSoldierCount = 6;
                        OptionValues.getMinesCount = 150;
                        OptionValues.getEnemyCount = 3;
                        sizeFont = 1.0f;
                    }
                    break;
                case 3:
                    {
                        OptionValues.getTilesVertical = 48;
                        OptionValues.getTilesHorizontal = 64;
                        OptionValues.getScreenWidth = 64 * OptionValues.getTileSize;
                        OptionValues.getScreenHeight = 48 * OptionValues.getTileSize;
                        OptionValues.getSoldierCount = 10;
                        OptionValues.getMinesCount = 500;
                        OptionValues.getEnemyCount = 5;
                        sizeFont = 1.5f;
                    }
                    break;
                default:
                    break;
            }
            
            soldiers.Clear();
            enemies.Clear();
            for (int i = 0; i < OptionValues.getSoldierCount; i++)
            {
                Soldier tempSoldier = new Soldier();
                tempSoldier.currentPathIndex = 0;
                tempSoldier.Initialize(allTextures[(int)TextureType.TexSoldier], new Vector2(i * 10, 0));

                Random rand = new Random((int)tempSoldier.Position.LengthSquared());
                tempSoldier.speed = rand.Next(50, 75);
                tempSoldier.speed /= 100;

                soldiers.Add(tempSoldier);
            }
            endPosition = new Vector2(OptionValues.getTilesHorizontal - 2, OptionValues.getTilesVertical - 1);
            mReader.initMap(allTextures[(int)TextureType.TexGround], allTextures[(int)TextureType.TexMine], allTextures[(int)TextureType.TexBlock], allTextures[(int)TextureType.TexCastle], endPosition, allFonts[(int)FontType.fontNumber]);
            mReader.updateMap();  
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
            if((int)v.X/OptionValues.getTileSize > OptionValues.getTilesHorizontal || (int)v.Y/OptionValues.getTileSize > OptionValues.getTilesVertical)            
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
                    if (currentKeyState.IsKeyDown(Keys.D1))
                        level = 1;
                    else if (currentKeyState.IsKeyDown(Keys.D2))
                        level = 2;
                    else if (currentKeyState.IsKeyDown(Keys.D3))
                        level = 3;
                    curStage = GameStage.StageCutScene;
                    initLevel();
                }
                return;
            }

            if (isGameFinish && currentKeyState.IsKeyDown(Keys.Back))
            {
                isAllowInput = false;
                if (isGameFinish)
                {
                    curStage = GameStage.StageStartMenu;
                    isGameStart = isGameFinish = false;
                    mReader.resetMap();
                    mReader.updateMap();
                    timer.reset();

                    soldiers.Clear();
                    enemies.Clear();
                    flags.Clear();
                }
            }
            if (!isGameFinish)
            {
                Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);
                if (!checkBoundaries(mousePosition))
                {
                    return;
                }
                if (currentMouseState.LeftButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Pressed)
                {
                    allSounds[(int)SoundType.sClickRegular].Stop();
                    allSounds[(int)SoundType.sClickRegular].Play();
                    isAllowInput = false;
                    List<Vector2> list = new List<Vector2>();
                    mReader.DetectBlocks((int)mousePosition.X / OptionValues.getTileSize, (int)mousePosition.Y / OptionValues.getTileSize,ref list);
                    foreach(Vector2 v in list)
                    {
                        isAllowInput = false;
                        if (true)
                        {
                            allSounds[(int)SoundType.sClickMonster].Stop();
                            allSounds[(int)SoundType.sClickMonster].Play();
                            for (int i = 0; i <= 5; i++)
                            {
                                Enemy temp = new Enemy();
                                Texture2D textureEnemy = allTextures[(int)TextureType.TexEnemy];
                                temp.Initialize(textureEnemy, mReader.mapTiles[(int)v.X, (int)v.Y].Position);
                                enemies.Add(temp);
                            }
                        }
                    }

                }
                else if (currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    allSounds[(int)SoundType.sClickRegular].Stop();
                    allSounds[(int)SoundType.sClickRegular].Play();

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
                            allSounds[(int)SoundType.sClickMonster].Stop();
                            allSounds[(int)SoundType.sClickMonster].Play();
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
                    allSounds[(int)SoundType.sClickMove].Stop();
                    allSounds[(int)SoundType.sClickMove].Play();
                    if (mReader.GetMap[(int)mousePosition.Y / OptionValues.getTileSize, (int)mousePosition.X / OptionValues.getTileSize].isRevealed)
                    {
                        isAllowInput = true;
                        foreach (Soldier s in soldiers)
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

            if (curStage == GameStage.StageCutScene)
            {
                graphics.PreferredBackBufferWidth = allTextures[(int)TextureType.TexCutScene].Width;
                graphics.PreferredBackBufferHeight = allTextures[(int)TextureType.TexCutScene].Height;
                graphics.ApplyChanges();

                spriteBatch.Draw(allTextures[(int)TextureType.TexCutScene], Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                return;
            }

            graphics.PreferredBackBufferWidth = OptionValues.getScreenWidth;
            graphics.PreferredBackBufferHeight = OptionValues.getScreenHeight;
            graphics.ApplyChanges();          
            if (isGameFinish)
            {
                mReader.drawMap(spriteBatch);
                Vector2 size = gameInfo.MeasureString(endOutPut);
    
                spriteBatch.DrawString(gameInfo, endOutPut, new Vector2(OptionValues.getScreenWidth / 2 - size.X / 2 * sizeFont, OptionValues.getScreenHeight / 2 - size.Y / 2 * sizeFont), Color.White,0,Vector2.Zero, sizeFont, SpriteEffects.None,0);
            }
            else
            {
                mReader.drawMap(spriteBatch);
                timer.draw(spriteBatch);
                foreach (Soldier element in soldiers)
                    element.Draw(spriteBatch);
                foreach (Enemy element in enemies)
                {
                    element.Draw(spriteBatch);
                }

                foreach (DrawInfo flag in flags)
                    spriteBatch.Draw(flag.texture, flag.position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
        }
        public void UpdateGame(GameTime gameTime)
        {
            if(curStage == GameStage.StageStartMenu)
            {
                return;
            }
            if(curStage == GameStage.StageCutScene)
            {
                curStage = GameStage.StageGame;
                initLevel();
                IsGameStart = true;
                backGroundMusic.Stop();
                backGroundMusic.Play();
                mReader.RevealBlock(0, 0);
                mReader.RevealBlock((int)endPosition.X, (int)endPosition.Y);
                return;
            }
            if (IsGameStart)
            {
                timer.Update(gameTime);
                foreach (Soldier element in soldiers)
                {
                    element.Update();
                    if (!element.endofpath())
                        element.march(ref mReader);
                }
            }

            isGameFinish = false;
            if (soldiers.Count <= 0)
            {
                isGameFinish = true;
            }
            else
            {
                foreach (Soldier element in soldiers)
                {
                    int indexX = (int)Math.Round(element.Position.X / OptionValues.getTileSize);
                    int indexY = (int)Math.Round(element.Position.Y / OptionValues.getTileSize);

                    if ((indexX != (int)endPosition.X) ||
                        (indexY != (int)endPosition.Y))
                    {
                        break;
                    }
                    isGameFinish = true;
                }
            }

            if (isGameFinish)
            {
                backGroundMusic.Stop();
                if (soldiers.Count > 0)
                {
                    allSounds[(int)SoundType.sSuccess].IsLooped = false;
                    allSounds[(int)SoundType.sSuccess].Play();
                    isGameStart = false;
                    //game end
                    endOutPut = string.Format(
    @"   Yeah! You Made It!  
         Soldiers Left: {0}   
Time Used:{1,3:00}:{2,3:000}:{3,3:000}"
                                    , soldiers.Count, timer.minute, timer.seconds, timer.miniseconds);
                }

                else
                {
                    allSounds[(int)SoundType.sGameOver].IsLooped = false;
                    allSounds[(int)SoundType.sGameOver].Play();
                    isGameStart = false;
                    //game end
                    endOutPut = string.Format(
    @"       You Failed.  
         Soldiers Left: {0}   
Time Used:{1,3:00}:{2,3:000}:{3,3:000}"
                                    , soldiers.Count, timer.minute, timer.seconds, timer.miniseconds);
                }
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
                if (soldiers.Count > 0)
                    if (secondsBetweenUpdate >= 10 || element.closeToRefugee(soldiers[0].Position))
                    {
                        if (element.closeToRefugee(soldiers[0].Position))
                        {
                            secondsSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (secondsSinceLastUpdate < 0.5)
                                break;
                            else
                                secondsSinceLastUpdate = 0;
                            if (element.death(soldiers[0].Position))
                            {
                                enemies.Remove(element);
                                soldiers.Remove(soldiers[0]);
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
                foreach (Soldier element in soldiers)
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
                return soldiers[0].Position / OptionValues.getTileSize;
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
