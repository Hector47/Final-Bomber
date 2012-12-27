﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Final_Bomber.Sprites;

namespace Final_Bomber.Components
{
    public class Arrow : MapItem
    {
        #region Field Region
        public override Sprites.AnimatedSprite Sprite { get; protected set; }
        private FinalBomber gameRef;
        private bool isAlive;
        private LookDirection lookDirection;
        #endregion

        #region Property Region

        public bool IsAlive
        {
            get { return isAlive; }
        }

        #endregion

        #region Constructor Region
        public Arrow(FinalBomber game, Vector2 position, LookDirection initialLookDirection)
        {
            this.gameRef = game;

            int animationFramesPerSecond = 10;
            Dictionary<AnimationKey, Animation> animations = new Dictionary<AnimationKey, Animation>();

            Animation animation = new Animation(1, 32, 32, 0, 0, animationFramesPerSecond);
            animations.Add(AnimationKey.Down, animation);

            animation = new Animation(1, 32, 32, 0, 32, animationFramesPerSecond);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(1, 32, 32, 0, 64, animationFramesPerSecond);
            animations.Add(AnimationKey.Right, animation);

            animation = new Animation(1, 32, 32, 0, 96, animationFramesPerSecond);
            animations.Add(AnimationKey.Up, animation);

            Texture2D spriteTexture = gameRef.Content.Load<Texture2D>("Graphics/Characters/arrow");
            Sprite = new Sprites.AnimatedSprite(spriteTexture, animations, position);
            Sprite.IsAnimating = true;

            lookDirection = initialLookDirection;
            Sprite.CurrentAnimation = LookDirectionToAnimationKey(lookDirection);

            isAlive = true;
        }
        #endregion

        #region XNA Method Region

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Sprite.Draw(gameTime, gameRef.SpriteBatch);
        }

        #endregion

        #region Private Method Region

        private AnimationKey LookDirectionToAnimationKey(LookDirection lookDirection)
        {
            AnimationKey animationKey = AnimationKey.Up;
            switch (lookDirection)
            {
                case LookDirection.Up:
                    animationKey = AnimationKey.Up;
                    break;
                case LookDirection.Down:
                    animationKey = AnimationKey.Down;
                    break;
                case LookDirection.Right:
                    animationKey = AnimationKey.Right;
                    break;
                case LookDirection.Left:
                    animationKey = AnimationKey.Left;
                    break;
            }
            return animationKey;
        }

        #endregion

        #region Public Method Region

        public void ChangeDirection(Bomb bomb)
        {
            Point nextPosition = bomb.Sprite.CellPosition;
            switch (lookDirection)
            {
                case LookDirection.Up:
                    nextPosition.Y--;
                    break;
                case LookDirection.Down:
                    nextPosition.Y++;
                    break;
                case LookDirection.Left:
                    nextPosition.X--;
                    break;
                case LookDirection.Right:
                    nextPosition.X++;
                    break;
            }

            if (!gameRef.GamePlayScreen.World.Levels[gameRef.GamePlayScreen.World.CurrentLevel].
                CollisionLayer[nextPosition.X, nextPosition.Y])
            {
                bomb.ChangeDirection(lookDirection, -1);
                //bomb.ChangeSpeed(bomb.Sprite.Speed + Config.BombSpeedIncrementeur);
                bomb.ResetTimer();
            }
        }

        #endregion

        #region Override Method Region
        public override void Destroy()
        {
        }

        public override  void Remove()
        {
            this.isAlive = false;
        }
        #endregion
    }
}