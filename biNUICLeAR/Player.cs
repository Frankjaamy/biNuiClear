using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using biNUICLeAR;
namespace GameActor
{
    public enum ActorType
    {
        typeSoldier,
        typeEnemyWander
    } ;
    class BasicActor
    {
        public Texture2D PlayerTexture;
        public Vector2 Position;
        public int sizeX = 1;
        public int sizeY = 1;

        protected ActorType actorType;
        protected Animator animator;

        public ActorType GetActorType
        {
            get { return actorType; }
        }
        public int Width
        {
            get { return (PlayerTexture.Width/12); }
        }
        public int Height
        {
            get { return (PlayerTexture.Height/4); }
        }

        virtual public void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            Position = position;
        }

        virtual public void Update(){ animator.Update(); }
        virtual public void Draw(SpriteBatch sb) {;}
    }
    
    class Tiles: BasicActor
    {
        public bool Active;

        public bool isBlock;

        public bool isMined;
        public bool isSafe;
        public bool isFlag;
        public bool isRevealed;
        public bool isSearched;
        public bool isTemporaryRevealed;
        public SpriteFont numberMines;
        public int mineCount = 0;

        public Texture2D TextureMask;
        public Tiles()
        {
            isBlock = false;
            isFlag = false;
            isMined = false;
            isSafe = false;
            isRevealed = false;
            isTemporaryRevealed = false;
            isSearched = false;
        }
        public override void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            Position = position;

            Active = true;
        }

        public void initFont(SpriteFont f)
        {
            numberMines = f;
        }
        public void UpdateTexture(Texture2D texture)
        {
            PlayerTexture = texture;
        }
        public override void Update()
        {
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        // Game related methods
        public bool IsBlock
        {
            set { isBlock = value; }
            get { return isBlock; }
        }
    }
    class MoveableActor : BasicActor
    {
        public Vector2 destPosition;
        public Vector2 startPosition;
        public Vector2 direction = new Vector2(0f,0f);

        public float speed;
        public int health;

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
    }
    class Soldier : MoveableActor
    {
        
        public int currentPathIndex = 0;
        PathFinder pathFinder;

        public int detectRadius = 3;

        public Vector2 Direction
        {
            set { direction = value; }
            get { return direction; }
        }

        public Vector2 StartPosition
        {
            set { startPosition = value; }
            get { return startPosition; }
        }

        public Vector2 DestPosition
        {
            set { destPosition = value; }
            get { return destPosition; }
        }
        public override void Initialize(Texture2D texture, Vector2 position)
        {
            startPosition = position;
            PlayerTexture = texture;
            this.Position = position;
            DestPosition = position;

            actorType = ActorType.typeSoldier; 
            Health = 100;
            speed = 0.50f;
            sizeX = 1;
            sizeY = 1;
            animator = new Animator(PlayerTexture, 8, 3);

            pathFinder = new PathFinder();
            pathFinder.initialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Now using Animator instead
            animator.Draw(spriteBatch, Position, direction);
        }
        public void march(int viewpostWidth, int viewportHeight, ref MapScene map)
        {
            if ( Vector2.Distance(Position,DestPosition) <= 0.05f * ConstValues.getTileSize)
            {
                currentPathIndex += 1;
                if (currentPathIndex < pathFinder.getPathNodes.Count)
                {
                    int x = pathFinder.getPathNodes[currentPathIndex].x * ConstValues.getTileSize;
                    int y = pathFinder.getPathNodes[currentPathIndex].y * ConstValues.getTileSize;
                    DestPosition = new Vector2(x, y);
                    Direction = DestPosition - Position;
                }
                
            }
            else
            {
                Position.X += direction.X * speed/ ConstValues.getTileSize;
                Position.Y += direction.Y * speed / ConstValues.getTileSize;
            }
            /*
            map.quickResetMapDetection();
            int indexY = (int)Position.Y / ConstValues.getTileSize;
            int indexX = (int)Position.X / ConstValues.getTileSize;
            for (int i = indexX - detectRadius; i< indexX + sizeX + detectRadius; i++)
            {
                for (int j = indexY - detectRadius; j < indexY + sizeY + detectRadius; j++)
                {
                    if(i < 0 || j<0 || i>ConstValues.getTilesHorizontal || j > ConstValues.getTilesVertical)
                    {
                        continue;
                    }
                    map.RevealBlock(i,j,false);
                }                
            }
            */
        }
        public void recalculatePath(Tiles[,] map, Vector2 endPoint)
        {
            currentPathIndex = 0;
            pathFinder.PathFinding(map, (int)Position.X/ConstValues.getTileSize, (int)Position.Y/ConstValues.getTileSize, (int)endPoint.X, (int)endPoint.Y,sizeX,sizeY);
        }
        public bool endofpath()
        {
            if (pathFinder.getPathNodes.Count <= 0)
            {

                return true;
            }
            else
                return false;
        }
    }

    class Enemy : MoveableActor
    {
        public int searchingRadius;
        public float wandererSpeed = 1.5f;
        public bool isSearchingMode;
        public bool isHuntingMode;
        public bool isDead;

        public Vector2 Direction
        {
            set { direction = value; }
            get { return direction; }
        }
        public Vector2 StartPosition
        {
            set { startPosition = value; }
            get { return startPosition; }
        }
        public Vector2 DestPosition
        {
            set { destPosition = value; }
            get { return destPosition; }
        }

        public override void Initialize(Texture2D texture, Vector2 position)
        {
            startPosition = position;
            PlayerTexture = texture;
            this.Position = position;
            DestPosition = position;
            actorType = ActorType.typeSoldier;
            animator = new Animator(PlayerTexture, 8, 3);

            isHuntingMode = isDead = isSearchingMode = false;
            searchingRadius = 2;
        }
        public override void Update()
        {
            animator.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            animator.Draw(spriteBatch, Position, direction);
        }

        public bool closeToRefugee(Vector2 pos)
        {
            float distance = Vector2.Distance(this.Position, pos);
            if (distance < 0)
                distance *= -1;
            if (distance < (ConstValues.getTileSize*2))
            {
                return true;
            }
            else
                return false;
        }

        public bool searchingTarget()
        {
            return true;
        }
    }
}
