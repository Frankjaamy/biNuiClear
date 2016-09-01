using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;

namespace biNUICLeAR
{
    class Animator
    {
        public Texture2D Texture { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        private int currentFrame = 0;
        private int totalFrames = 3;
        private int width;
        private int height;
        private bool animationDirUp = true;

        private int frameCounter;

        public Animator(Texture2D texture, int row, int column)
        {
            Texture = texture;
            Row = row;
            Column = column;
            width = Texture.Width / Column;
            height = Texture.Height / Row;
        }

        public void Update()
        {
            frameCounter++;
            if (frameCounter > 12)
            {
                if (animationDirUp)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame--;
                }

                if (currentFrame == totalFrames)
                {
                    if (animationDirUp == true)
                    {
                        animationDirUp = false;
                        totalFrames = 0;
                    }
                    else
                    {
                        animationDirUp = true;
                        totalFrames = 2;
                    }
                }

                frameCounter = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, Vector2 direction)
        {
            //Change all for new charactersheet
            Column = 0;
            //Up and down texture pos
            if (direction.X == 0)
            {
                if (direction.Y < 0)
                    Row = 3;
                else if (direction.Y > 0)
                    Row = 0;
                else
                    Row = 0;
            }
            //Right movement
            else if (direction.X > 0)
            {
                if (direction.Y == 0)
                {
                    Row = 6;
                }
                else if (direction.Y < 0)
                {
                    Row = 5;
                }
                else if (direction.Y > 0)
                {
                    Row = 1;
                }

            }
            else if (direction.X < 0)
            {
                if (direction.Y == 0)
                {
                    Row = 7;
                }
                else if (direction.Y < 0)
                {
                    Row = 4;
                }
                else if (direction.Y > 0)
                {
                    Row = 2;
                }
            }

            Column += currentFrame;
            Rectangle sourceRectangle = new Rectangle(width * Column, height * Row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White,0.0f,Vector2.Zero,SpriteEffects.None,1.0f);
        }
    }
}
