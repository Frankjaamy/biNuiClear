using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameActor;

namespace biNUICLeAR
{
    class MapScene
    {
        public Tiles[,] mapTiles;
        int[,] offset ={
                {-1,-1},
                {0,-1},
                {1,-1},
                {-1,1},
                {0,1},
                {1,1},
                {-1,0},
                {1,0}
            };
        public Tiles[,] GetMap
        {
            get { return mapTiles; }
        }

        public void drawMapTile()
        {

        }
        public bool initMap(Texture2D textGround, Texture2D mined,Texture2D mask, SpriteFont f = null)
        {
            mapTiles = new Tiles[ConstValues.getTilesVertical, ConstValues.getTilesHorizontal];

            for (int i = 0; i < ConstValues.getTilesVertical; i++)
            {
                for (int j = 0; j < ConstValues.getTilesHorizontal; j++)
                {
                    mapTiles[i, j] = new Tiles();
                    mapTiles[i, j].Position = new Vector2(j * ConstValues.getTileSize, i * ConstValues.getTileSize);
                    mapTiles[i, j].PlayerTexture = textGround;
                    mapTiles[i, j].TextureMask = mask;
                    mapTiles[i, j].TextureMined = mined;
                    mapTiles[i, j].initFont(f);
                }
            }
            Random rm = new Random();
            for (int i = 0; i < 50; i++)
            {
                int indexX = rm.Next(1, ConstValues.getTilesVertical);
                int indexY = rm.Next(1, ConstValues.getTilesHorizontal);
                if(indexX<6 && indexY < 6)
                {
                    continue;
                }
                mapTiles[indexX, indexY].isMined = true;
            }
            return true;
        }

        public void drawMap(SpriteBatch spriteBatch)
        {
            foreach (Tiles p in mapTiles)
            {
                if (!p.isTemporaryRevealed && !p.isRevealed)
                {
                    spriteBatch.Draw(p.TextureMask, p.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    if (!p.isMined)
                    {
                        spriteBatch.Draw(p.PlayerTexture, p.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }
                    else
                    {
                        spriteBatch.Draw(p.TextureMined, p.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }
                    if (!p.isSafe)
                    {
                        if (!p.isMined)
                        {
                            spriteBatch.DrawString(p.numberMines, p.mineCount.ToString(), p.Position, Color.Red);
                        }
                    }
                }
            }
        }
        public void modifyValues(ref int x, ref int y)
        {
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;
            x = x > (ConstValues.getTilesVertical - 1) ? (ConstValues.getTilesVertical - 1) : x;
            y = y > (ConstValues.getTilesHorizontal - 1) ? (ConstValues.getTilesHorizontal - 1) : y;
        }
        public void updateMap()
        {
            for (int i = 0; i < ConstValues.getTilesVertical; i++)
            {
                for (int j = 0; j < ConstValues.getTilesHorizontal; j++)
                {
                    if (mapTiles[i, j].isMined)
                    {
                        continue;
                    }
                    for (int k = 0; k < 8; ++k)
                    {
                        int x = i + offset[k, 0];
                        int y = j + offset[k, 1];
                        modifyValues(ref x, ref y);
                        if (mapTiles[x, y].isMined)
                        {
                            mapTiles[i, j].mineCount++;
                        }
                    }
                    if (mapTiles[i, j].mineCount == 0)
                    {
                        mapTiles[i, j].isSafe = true;
                    }
                }
            }
            int a = 0;
        }
        public void resetMap()
        {
            foreach (Tiles t in mapTiles)
            {
                t.isBlock = false;
                t.isTemporaryRevealed = false;
                t.isRevealed = false;
                t.isFlag = false;
                t.isMined = false;
                t.isSafe = false;
                t.isSearched = false;
                t.mineCount = 0;
            }

            Random rm = new Random();
            for (int i = 0; i < 500; i++)
            {
                int indexX = rm.Next(1, ConstValues.getTilesVertical);
                int indexY = rm.Next(1, ConstValues.getTilesHorizontal);
                if (indexX < 6 && indexY < 6)
                {
                    continue;
                }
                mapTiles[indexX, indexY].isMined = true;
            }
        }
        public void quickResetMapDetection()
        {
            foreach (Tiles t in mapTiles)
            {
                t.isTemporaryRevealed = false;
            }
        }
        public bool RevealBlock(int x, int y, bool furtherReveal = true)
        {
            if (x < 0 || y < 0 || y > ConstValues.getTilesVertical - 1 || x > ConstValues.getTilesHorizontal - 1)
            {
                return false;
            }
            if (mapTiles[y, x].isSearched)
            {
                return false;
            }
            if (!furtherReveal)
            {

                mapTiles[y, x].isTemporaryRevealed = true;
                return true;
            }
            mapTiles[y, x].isRevealed = true;
            mapTiles[y, x].isSearched = true;
            if (mapTiles[y, x].isSafe)
            {
                for (int i = 0; i < 8; i++)
                {
                    int tempX = x + offset[i, 0];
                    int tempY = y + offset[i, 1];
                    RevealBlock(tempX, tempY);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool PlaceFlag(int x, int y)
        {
            if (x < 0 || y < 0 || y > ConstValues.getTilesVertical - 1 || x > ConstValues.getTilesHorizontal - 1)
            {
                return false;
            }
            if(!mapTiles[y,x].isRevealed)
                mapTiles[y, x].isFlag = !mapTiles[y,x].isFlag;
            return mapTiles[y, x].isFlag;
        }

        public bool DetectBlocks(int x, int y)
        {
            if (x < 0 || y < 0 || y > ConstValues.getTilesVertical - 1 || x > ConstValues.getTilesHorizontal - 1)
            {
                return false;
            }
            if (!mapTiles[y, x].isSafe && mapTiles[y, x].isRevealed)
            {
                int flagsCount = 0;
                for (int k = 0; k < 8; ++k)
                {
                    int indexX = x + offset[k, 0];
                    int indexY = y + offset[k, 1];
                    modifyValues(ref indexX, ref indexY);
                    if (mapTiles[indexY, indexX].isFlag)
                    {
                        flagsCount++;
                    }
                }
                if(flagsCount == mapTiles[y, x].mineCount)
                {
                    for (int k = 0; k < 8; ++k)
                    {
                        int indexX = x + offset[k, 0];
                        int indexY = y + offset[k, 1];
                        modifyValues(ref indexX, ref indexY);
                        if (!mapTiles[indexY,indexX].isFlag)
                        {
                            mapTiles[indexY, indexX].isRevealed = true;
                        }  
                    }
                }
            }
            return true;
        }
    }
}
