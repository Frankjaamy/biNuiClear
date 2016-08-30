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
            Zoom = 2;
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

            if (mouseState.ScrollWheelValue < previousScrollValue && Zoom > 1)
            {
                Zoom-= (float)0.25;
            }
            else if (mouseState.ScrollWheelValue > previousScrollValue && Zoom < 5)
            {
                Zoom+= (float)0.25;
            }
            previousScrollValue = mouseState.ScrollWheelValue;

            // movement
            if (keyboardState.IsKeyDown(Keys.Up))
                Position -= new Vector2(0, 250) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Down))
                Position += new Vector2(0, 250) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Left))
                Position -= new Vector2(250, 0) * deltaTime;

            if (keyboardState.IsKeyDown(Keys.Right))
                Position += new Vector2(250, 0) * deltaTime;
        }

    }
}
