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
            Zoom = 0.6f;
            Origin = new Vector2(0, viewport.Height / 2f);
            Position = Vector2.Zero;
            previousScrollValue = Mouse.GetState().ScrollWheelValue;

        }
        private Vector2 position;
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                ValidatePosition();
            }
        }
        public float Rotation { get; set; }
        private float zoom;
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = MathHelper.Max(value, 0.2f);
                ValidateZoom();
                ValidatePosition();
            }
        }
        public Vector2 Origin { get; set; }

        public Matrix GetViewMatrix()
        {
            return
                Matrix.CreateTranslation(new Vector3(-position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(zoom, zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
        private Rectangle? limits;
        public Rectangle? Limits {
            set
            {
                limits = value;
                ValidateZoom();
                ValidatePosition();
            }
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue < previousScrollValue && Zoom > 0.01f)
            {
                Zoom -= (float)0.25;
                if (Zoom <= 0.01f)
                {
                    Zoom = 0.2f;
                }
            }
            else if (mouseState.ScrollWheelValue > previousScrollValue && Zoom <1)
            {
                Zoom += (float)0.25;
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

        private void ValidateZoom()
        {
            if (limits.HasValue)
            {
                float minZoomX = (float)_viewport.Width / limits.Value.Width;
                float minZoomY = (float)_viewport.Height / limits.Value.Height;
                zoom = MathHelper.Max(zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }

        private void ValidatePosition()
        {
            if (limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(GetViewMatrix()));
                Vector2 cameraSize = new Vector2(_viewport.Width, _viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(limits.Value.Left, limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(limits.Value.Right, limits.Value.Bottom);
                Vector2 positionOffset = position - cameraWorldMin;
                position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }
    }
}
