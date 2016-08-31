using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using GameActor;
namespace biNUICLeAR
{
    public enum BasicType
    {
        Block,
        Road
    };

    public enum TileType
    {
        RoadRegular = 1 ,
        RoadRight,
        RoadLeftRight,
        RoadBottomLeft,
        RoadTop,
        RoadDown,
        RoadTopDown,
        RoadBottomRight,
        Grass,
        RoadTopLeft
    }
    class MapReader
    {
        public int[,,] MapLevelOne = {
            { {1,3,1}, {0,9,14},{1,1,1},  {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {1,3,1}, {0,9,5}, {1,10,1}, {1,7,6}, {1,5,1}, {1,7,1}, {1,1,1}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {1,3,1}, {0,9,5}, {1,3,1},  {0,9,6}, {1,3,1}, {0,9,1}, {1,3,1}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {1,3,1}, {0,9,5}, {1,3,1},  {0,9,3}, {1,10,1},{1,7,2}, {1,2,1}, {0,9,1}, {1,3,1}, {0,0,0}, {0,0,0}},
            { {1,4,1}, {1,7,2}, {1,5,1},  {1,7,2}, {1,2,1}, {0,9,3}, {1,3,1}, {0,9,2}, {1,1,1}, {1,7,1}, {1,8,1}},
            { {0,9,3}, {1,3,1}, {0,9,2},  {1,1,1}, {1,7,3}, {1,8,1}, {0,9,2}, {1,3,1}, {0,9,2}, {0,0,0}, {0,0,0}},
            { {0,9,3}, {1,3,1}, {0,9,2},  {1,3,1}, {0,9,6}, {1,3,1}, {0,9,2}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {0,9,3}, {1,3,1}, {0,9,2},  {1,3,1}, {0,9,6}, {1,3,1}, {0,9,2}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {0,9,3}, {1,3,1}, {0,9,2},  {1,1,1}, {1,7,6}, {1,8,1}, {0,9,2}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {0,9,3}, {1,3,1}, {0,9,2},  {1,3,1}, {0,9,9}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {0,9,3}, {1,3,1}, {0,9,2},  {1,3,1}, {0,9,9}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
            { {1,7,3}, {1,6,1}, {1,7,2},  {1,8,1}, {0,9,9}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}, {0,0,0}},
        };

        public void drawMapTile(int BasicType, TileType tileType)
        {

        }
        public bool initMap(int[,,] levelData,Tiles[,] mapData, TileType[,] textureTypeMap)
        {
            if(mapData.GetLength(0) == 0 || mapData.GetLength(1) == 0)
            {
                return false;
            }

            for(int i = 0; i < levelData.GetLength(0); i++)
            {
                int index = 0;
                for (int j = 0; j < levelData.GetLength(1);j++)
                {
                    
                    for(int a = 0; a < levelData[i,j,2];a++)
                    {
                        BasicType t = (BasicType)levelData[i, j, 0];
                        for(int x = 0; x < 8 ; x++)
                        {
                            for(int y = 0; y < 8; y++)
                            {
                                if(t == BasicType.Block)
                                    mapData[i*8 + x, index*8+y].isBlock = true;
                                else if (t == BasicType.Road)
                                    mapData[i*8 + x, index*8+y].isBlock = false;
                            }
                        }
                        textureTypeMap[i, index] = (TileType)levelData[i, j, 1];
                        index++; ;
                    }
                }
            }
            return true;
        }
    }
}
