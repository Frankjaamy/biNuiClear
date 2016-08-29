using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

using biNUICLeAR;
namespace GameActor
{
    class BasicActor
    {
        public Texture2D PlayerTexture;
        public Vector2 Position;

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

        virtual public void Update(){;}
        virtual public void Draw(SpriteBatch sb) {;}
    }
    class Tiles: BasicActor
    {
        public bool Active;
        public bool isBlock;
        public bool isFlag;

        public Tiles()
        {
            isBlock = false;
            isFlag = false;
        }
        public override void Initialize(Texture2D texture, Vector2 position)
        {
            PlayerTexture = texture;

            Position = position;

            Active = true;

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

    class Soldier : BasicActor
    {
        public int _health;
        public float _speed = 1.6f;

        public int currentPathIndex = 0;

        public Vector2 destPosition;
        public Vector2 startPosition;
        public Vector2 direction = new Vector2(0f,0f);

        Animator animator;

        public int Health
        {
            set { _health = value; }
            get { return _health; }
        }

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
            Debug.Write(position);
            PlayerTexture = texture;
            Position = position;
            DestPosition = position;

            _health = 100;
            animator = new Animator(PlayerTexture, 4, 12);
        }
        public override void Update()
        {
            animator.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Now using Animator instead
            //spriteBatch.Draw(PlayerTexture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            animator.Draw(spriteBatch, Position, direction);
        }

        public bool march()
        {
            if ( Vector2.Distance(Position,DestPosition) <= 0.05f)
            {
                return true;
            }
            else
            {
                Position.X += direction.X/ ConstValues.getTileSize;
                Position.Y += direction.Y/ ConstValues.getTileSize;
                return false;
            }

        }

    }
}
