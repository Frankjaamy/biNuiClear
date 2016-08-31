using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biNUICLeAR
{
    public class Camera
    {
        private readonly Viewport _viewport;

        private int previousScrollValue;
        private MouseState mouseState;

        public Camera(Viewport viewport)
        {
            _viewport = viewport;

            Rotation = 0;
            Zoom = 1;
            Origin = new Vector2(0, viewport.Height / 2f);
            Position = Vector2.Zero;
            previousScrollValue = Mouse.GetState().ScrollWheelValue;

        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Vector2 Origin { get; set; }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue < previousScrollValue && Zoom > 0.01f)
            {
                Zoom-= (float)0.25;
                if (Zoom <= 0.01f)
                {
                    Zoom = 0.2f;
                }
            }
            else if (mouseState.ScrollWheelValue > previousScrollValue && Zoom < 1)
            {
                Zoom+= (float)0.25;
            }
            previousScrollValue = mouseState.ScrollWheelValue;

            // movement
            if (keyboardState.IsKeyDown(Keys.Up))
                if(Position.Y > 0)
                    Position -= new Vector2(0, 1000) * deltaTime;
                    
            if (keyboardState.IsKeyDown(Keys.Down))
                if(Position.Y <ConstValues.getMapHeight)
                    Position += new Vector2(0, 1000) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Left))
                if (Position.X > 0)
                    Position -= new Vector2(1000, 0) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Right))
                if (Position.X < ConstValues.getMapWidth)
                    Position += new Vector2(1000, 0) * deltaTime;
        }

    }
}
