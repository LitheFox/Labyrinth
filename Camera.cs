using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth_Redux   
{
    public class Camera
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation
        public Vector2 Origin { get; set; }
        public Game1 Host { get; set; }
        public Camera(Game1 host)
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
            ZoomX = 1f;
            ZoomY = 1f;
            Host = host;
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }// if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float ZoomX { get; set; }
        public float ZoomY { get; set; }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(ZoomX, ZoomY, 1)) *
                                         Matrix.CreateTranslation(new Vector3(Host.Graphics.PreferredBackBufferWidth / 2, Host.Graphics.PreferredBackBufferHeight / 2, 0)) *
                                         Matrix.CreateTranslation(new Vector3(Origin, 0));
            return _transform;
        }

    }
}
