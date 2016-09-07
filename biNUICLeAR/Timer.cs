using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biNUICLeAR
{
    class Timer
    {
        public int minute;
        public int seconds;
        public float miniseconds;


        SpriteFont timerTexture;
        public Timer()
        {
            minute = seconds = 0;
            miniseconds = 0.0f;
        }
        public void init(SpriteFont t)
        {
            timerTexture = t;
        }
        public void Update(GameTime gametime)
        {
            miniseconds += gametime.ElapsedGameTime.Milliseconds;
            if (miniseconds >= 1000.0f)
            {
                seconds += 1;
                miniseconds = 0;
            }
            if (seconds >= 60)
            {
                seconds = 0;
                minute++;
            }
        }
        public void reset()
        {
            minute = seconds = 0;
            miniseconds = 0.0f;
        }
        public void draw(SpriteBatch batch)
        {
            String output = String.Format("{0,3:00}:{1,3:000}:{2,3:000}", minute, seconds, miniseconds);
            Vector2 size = timerTexture.MeasureString(output);
            batch.DrawString(timerTexture, output, new Vector2(OptionValues.getScreenWidth - size.X, 0), Color.White);
        }
    }
}
