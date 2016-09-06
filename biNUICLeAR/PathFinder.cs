using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using biNUICLeAR;
using GameActor;

namespace biNUICLeAR
{
    class PathFinder
    {
        public List<PathNode> openList;
        public List<PathNode> closeList;

        public List<PathNode> pathList;
        public int pathNodeCount;
        public PathNode startNode;
        public PathNode endNode;

        private int attempts = 0;
        public List<PathNode> getPathNodes
        {
            get { return pathList; }
        } 
        public PathFinder()
        {

        } 
        public void initialize()
        {
            openList = new List<PathNode>();
            closeList = new List<PathNode>();
            pathList = new List<PathNode>();

            startNode = new PathNode();
            endNode = new PathNode();

        }

        public void clearLists()
        {
            openList.Clear();
            closeList.Clear();
            pathNodeCount = 0;
            pathList.Clear();
        }

        public void initNode(PathNode current,PathNode father,int x,int y,int endX,int endY)
        {
            current.x = x;
            current.y = y;
            current.fatherNode = father;
            current.nextNode = null;
            if (father != null)
            {
                current.valueF = father.valueF + 1;
            }
            else
            {
                current.valueF = 0;
            }

            current.valueG = Math.Abs(endX - x) + Math.Abs(endY - y);
            current.valueH = current.valueG + current.valueF;
        }
        public PathNode getMinCost(List<PathNode> path)
        {
            int index = 1;
            PathNode min = path[0];
            if(min == null)
            {
                return min;
            }
            for (; index < path.Count - 1; index++)
            {
                if(path[index].valueH < min.valueH)
                {
                    min = path[index];
                }
            }
            return min;
        }

        public void addNode(PathNode node, List<PathNode> list)
        {

        }

        public void removeNode(PathNode node, List<PathNode> list)
        {

        }

        public bool checkNodeInList(int x, int y, List<PathNode> list)
        {
            foreach(var node in list){
                if(node.x == x && node.y == y)
                {
                    return true;
                }
            }
            return false;
        }
        public bool PathFinding(Tiles[,] map, int startX, int startY, int endX, int endY, int sizeX = 1, int sizeY = 1, bool allowGoThroughDanger = false, bool ignoreAllObstacles = false)
        {
            if(attempts >= 2)
            {
                attempts = 0;
                return false;
            }
            attempts += 1;
            clearLists();

            this.startNode.x = startX;
            this.startNode.y = startY;

            this.endNode.x = endX;
            this.endNode.y = endY;
            if (isUnRevealed(map, endX, endY,ignoreAllObstacles,sizeX,sizeY))
            {
                return false;
            }
            int [,] offset = { 
                {0,1},
                {0,-1},
                {-1,0},
                {1,0},
                {-1,1},
                {-1,-1},
                {1,1},
                {1,-1}
            };

            PathNode pStart = new PathNode();
            initNode(pStart, null, startX, startY, endX, endY);
            openList.Add(pStart);

            PathNode pCurrent = null;
            while(openList.Count > 0)
            {
                pCurrent = getMinCost(openList);
                if(pCurrent.x == endX && pCurrent.y == endY)
                {
                    break;
                }
                else
                {
                    closeList.Add(pCurrent);
                    openList.Remove(pCurrent);

                    for(int i = 0; i < 8; i++)
                    {
                        int x = pCurrent.x + offset[i, 0];
                        int y = pCurrent.y + offset[i,1];
                        if(x < 0 || y < 0 || x >= ConstValues.getTilesHorizontal || y >= ConstValues.getTilesVertical)
                        {
                            continue;
                        }
                        else
                        {
                            if(!checkNodeInList(x,y,openList) && !checkNodeInList(x, y, closeList) && !isUnRevealed(map, x, y, ignoreAllObstacles,sizeX,sizeY) && ((!isMined(map,x,y, ignoreAllObstacles,sizeX, sizeY))||allowGoThroughDanger))
                            {
                                PathNode chosenNode = new PathNode();
                                initNode(chosenNode, pCurrent, x, y, endX, endY);
                                openList.Add(chosenNode);
                            }
                        }
                    }
                }

            }

            if(openList.Count<=1 && (pCurrent.x!=endX || pCurrent.y != endY))
            {
                if(!PathFinding(map,startX,startY,endX,endY,sizeX,sizeY,true))
                {
                    pathNodeCount = 0;
                    return false;
                }
                attempts = 0;
                return true;
            }
            else
            {
                while(pCurrent!= null)
                {
                    PathNode node = new PathNode();
                    node.x = pCurrent.x;
                    node.y = pCurrent.y;

                    pathList.Add(node);
                    pCurrent = pCurrent.fatherNode;
                    pathNodeCount++;
                }
                pathList.Reverse();
                attempts = 0;
                return true;
            }
        }

        public bool isUnRevealed(Tiles[,] map, int y, int x,  bool ignoreAllObstacles,int sizeY = 1, int sizeX = 1)
        {
            if (ignoreAllObstacles == true)
                return false;
            if(x < 0 || y < 0)
            {
                return true;
            }
            if(x + sizeX >ConstValues.getTilesVertical || y + sizeY > ConstValues.getTilesHorizontal)
            {
                return true;
            }
            for (int i = 0; i < sizeX; ++i)
            {
                for(int j = 0; j < sizeY; j++)
                {
                    int indexX = x + i;
                    int indexY = y + j;
                    indexX = indexX >= ConstValues.getTilesVertical ? ConstValues.getTilesVertical - 1 : indexX;
                    indexY = indexY >= ConstValues.getTilesHorizontal ? ConstValues.getTilesHorizontal - 1 : indexY;
                    indexY = indexY < 0 ? 0 : indexY;
                    indexX = indexX < 0 ? 0 : indexX;
                    if (!map[indexX, indexY].isRevealed)
                    {
                        return true;
                    }
                }
            }
            return false;  
        }

        public bool isMined(Tiles[,] map,int y, int x, bool ignoreAllObstacles, int sizeY = 1, int sizeX = 1)
        {
            if (ignoreAllObstacles == true)
                return false;
            for (int i = 0; i < sizeX; ++i)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    int indexX = x + i;
                    int indexY = y + j;
                    indexX = indexX >= ConstValues.getTilesVertical ? ConstValues.getTilesVertical - 1 : indexX;
                    indexY = indexY >= ConstValues.getTilesHorizontal ? ConstValues.getTilesHorizontal - 1 : indexY;
                    indexY = indexY < 0 ? 0 : indexY;
                    indexX = indexX < 0 ? 0 : indexX;
                    if (map[indexX, indexY].isMined && map[indexX,indexY].isRevealed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    class PathNode
    {
        public int x;
        public int y;

        public int valueF;
        public int valueG;
        public int valueH;

        public PathNode fatherNode;
        public PathNode nextNode;


    }

}
