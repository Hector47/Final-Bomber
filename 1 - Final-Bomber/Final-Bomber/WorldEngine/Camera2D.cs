﻿using System;
using FBLibrary;
using FBClient.Controls;
using FBLibrary.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FBClient.WorldEngine
{
    public enum CameraMode { Fixed, FollowPlayer }

    public class Camera2D
    {
        #region Fields

        private const float ZoomUpperLimit = 5.0f;
        private const float ZoomLowerLimit = .01f;

        private float _zoom;
        private float _initialZoom;
        private Matrix _transform;
        private Vector2 _position;
        private Vector2 _positionLag;
        private float _rotation;
        private Viewport _viewport;
        private readonly Point _mapSize;
        private readonly Vector2 _realMapSize;

        private CameraMode _cameraMode;

        #endregion

        #region Properties

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < ZoomLowerLimit)
                    _zoom = ZoomLowerLimit;
                if (_zoom > ZoomUpperLimit)
                    _zoom = ZoomUpperLimit;
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public void Move(Vector2 amount)
        {
            _position += amount;
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                /*
                float leftBarrier = (float)_viewport.Width * .5f / _zoom;
                float rightBarrier = _worldWidth - (float)_viewport.Width * .5f / _zoom;
                float topBarrier = _worldHeight - (float)_viewport.Height * .5f / _zoom;
                float bottomBarrier = (float)_viewport.Height * .5f / _zoom;


                _position.X = MathHelper.Clamp(_position.X, leftBarrier, rightBarrier);
                _position.Y = MathHelper.Clamp(_position.Y, bottomBarrier, topBarrier);
                */
            }
        }

        public Rectangle ViewportRectangle
        {
            get { return _viewport.Bounds; }
        }

        #endregion


        public Camera2D(Viewport viewport, Point mapSize, float initialZoom)
        {
            _initialZoom = initialZoom;
            _zoom = _initialZoom;
            _rotation = 0.0f;
            _position = Vector2.Zero;
            _positionLag = Vector2.Zero;
            _viewport = viewport;
            _mapSize = mapSize;

            _realMapSize = Engine.CellToVector(mapSize);

            _cameraMode = CameraMode.FollowPlayer;
        }

        public void Update(Vector2 position)
        {
            var dt = (float)TimeSpan.FromTicks(GameConfiguration.DeltaTime).TotalSeconds;

            if (_cameraMode == CameraMode.FollowPlayer)
                Position = position;
            else
            {
                // Move the camera to map center 
                // (and a little bit to the right to see the players' stats)
                if (Math.Abs(Zoom - _initialZoom) > 0.1f)
                {
                    Position = new Vector2((_realMapSize.X / 2) + (125 / Zoom), _realMapSize.Y / 2);
                }
                else
                    Position = new Vector2((_realMapSize.X / 2) + 150, _realMapSize.Y / 2);
            }

            // Adjust zoom if the mouse wheel has moved
            if (InputHandler.ScrollUp())
            {
                if (InputHandler.KeyPressed(Keys.LeftControl))
                    Zoom += 0.01f;
                else
                    Zoom += 0.1f;
            }
            else if (InputHandler.ScrollDown())
            {
                if (InputHandler.KeyPressed(Keys.LeftControl))
                    Zoom -= 0.01f;
                else
                    Zoom -= 0.1f;
            }

            // Reset zoom
            if (InputHandler.KeyPressed(Keys.Home))
            {
                Zoom = _initialZoom;
            }

            // Move the camera when the arrow keys are pressed
            Vector2 movement = Vector2.Zero;

            if (InputHandler.KeyDown(Keys.J))
                movement.X -= 1f;
            if (InputHandler.KeyDown(Keys.L))
                movement.X += 1f;
            if (InputHandler.KeyDown(Keys.I))
                movement.Y -= 1f;
            if (InputHandler.KeyDown(Keys.K))
                movement.Y += 1f;

            // Reset camera lag
            if (InputHandler.KeyPressed(Keys.Delete))
            {
                _positionLag = Vector2.Zero;
            }
            else if (movement != Vector2.Zero)
            {
                _positionLag += movement * (300 * dt);
            }

            Position += _positionLag;

            // Rotation
            if (InputHandler.KeyDown(Keys.PageDown))
            {
                if (InputHandler.KeyDown(Keys.LeftControl))
                    Rotation -= 10 * dt;
                else
                    Rotation -= dt;
            }
            else if (InputHandler.KeyDown(Keys.PageUp))
            {
                if (InputHandler.KeyDown(Keys.LeftControl))
                    Rotation += 10 * dt;
                else
                    Rotation += dt;
            }

            // Reset rotation
            if (InputHandler.KeyPressed(Keys.End))
            {
                Rotation = 0;
            }

            // Change camera mode
            if (InputHandler.KeyPressed(Keys.C))
            {
                if (_cameraMode == CameraMode.FollowPlayer)
                {
                    _cameraMode = CameraMode.Fixed;

                    // If the map doesn't fit the entire screen => we dezoom
                    if (_realMapSize.X > _viewport.Width)
                    {
                        Zoom /= 1.5f * (_realMapSize.X / _viewport.Width);
                    }
                    else if(_realMapSize.Y > _viewport.Height)
                    {
                        Zoom /=  1.5f * (_realMapSize.Y / _viewport.Height);
                    }
                }
                else
                {
                    _cameraMode = CameraMode.FollowPlayer;
                    Zoom = _initialZoom;
                }
            }
        }

        public Matrix GetTransformation()
        {
            switch (_cameraMode)
            {
                case CameraMode.Fixed:
                    _transform =
                       Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                       Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));
                    break;
                case CameraMode.FollowPlayer:
                    _transform =
                       Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                       Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));
                    break;
            }


            return _transform;
        }

        public bool IsVisible(Vector2 position)
        {
            return (position.X >= -(_viewport.Width * (1 / _zoom)) / 2f + _position.X - Engine.TileWidth && 
                    position.X <= (_viewport.Width * (1 / _zoom)) / 2f + _position.X + Engine.TileWidth &&
                    position.Y >= -(_viewport.Height * (1 / _zoom)) / 2f + _position.Y - Engine.TileHeight && 
                    position.Y <= (_viewport.Height * (1 / _zoom)) / 2f + _position.Y + Engine.TileHeight);
        }
    }
}
