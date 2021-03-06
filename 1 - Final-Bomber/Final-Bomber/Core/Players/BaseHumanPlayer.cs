﻿using System;
using FBLibrary;
using FBLibrary.Core;
using FBClient.Controls;
using FBClient.Entities;
using FBClient.Sprites;
using FBClient.WorldEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FBClient.Core.Players
{
    public abstract class BaseHumanPlayer : Player
    {
        public int ControlSettingsId;
        protected Keys[] KeysSaved;
        private Vector2 _motionVector;

        protected Keys[] Keys { get; private set; }
        protected Buttons[] Buttons { get; private set; }

        protected BaseHumanPlayer(int id, int controlSettingsId) : base(id)
        {
            Initialize(controlSettingsId);
        }

        protected BaseHumanPlayer(int id, int controlSettingsId, PlayerStats stats)
            : base(id, stats)
        {
            Initialize(controlSettingsId);
        }

        private void Initialize(int controlSettingsId)
        {
            Keys = Config.PlayersKeys[controlSettingsId];
            Buttons = Config.PlayersButtons[controlSettingsId];
            _motionVector = Vector2.Zero;
        }

        protected override void Move(GameTime gameTime, Map map, int[,] hazardMap)
        {
            #region Moving input

            _motionVector = Vector2.Zero;

            // Up
            if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[0], PlayerIndex.One)) ||
                InputHandler.KeyDown(Keys[0]))
            {
                Sprite.CurrentAnimation = AnimationKey.Up;
                CurrentDirection = LookDirection.Up;
                _motionVector.Y = -1;
            }
            // Down
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[1], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[1]))
            {
                Sprite.CurrentAnimation = AnimationKey.Down;
                CurrentDirection = LookDirection.Down;
                _motionVector.Y = 1;
            }
            // Left
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[2], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[2]))
            {
                Sprite.CurrentAnimation = AnimationKey.Left;
                CurrentDirection = LookDirection.Left;
                _motionVector.X = -1;
            }
            // Right
            else if ((Config.PlayersUsingController[Id] && InputHandler.ButtonDown(Buttons[3], PlayerIndex.One)) ||
                     InputHandler.KeyDown(Keys[3]))
            {
                Sprite.CurrentAnimation = AnimationKey.Right;
                CurrentDirection = LookDirection.Right;
                _motionVector.X = 1;
            }
            else
            {
                CurrentDirection = LookDirection.Idle;
            }

            #endregion

            if (_motionVector != Vector2.Zero)
            {
                IsMoving = true;

                Position += _motionVector * GetMovementSpeed();
            }
            else
            {
                IsMoving = false;
            }

            Sprite.IsAnimating = IsMoving;

            base.Move(gameTime, map, hazardMap);
        }

        public override void ApplyBadItem(BadEffect effect)
        {
            base.ApplyBadItem(effect);

            switch (effect)
            {
                case BadEffect.TooSlow:
                    SpeedSaved = Speed;
                    Speed = Config.MinSpeed;
                    break;
                case BadEffect.TooSpeed:
                    SpeedSaved = Speed;
                    Speed = Config.MaxSpeed;
                    break;
                case BadEffect.KeysInversion:
                    break;
                case BadEffect.BombTimerChanged:
                    BombTimerSaved = BombTimer;
                    int randomBombTimer = GameConfiguration.Random.Next(
                        GameConfiguration.BadItemTimerChangedMin,
                        GameConfiguration.BadItemTimerChangedMax);
                    BombTimer = TimeSpan.FromSeconds(randomBombTimer);
                    break;
            }
        }

        protected override void RemoveBadItem()
        {
            base.RemoveBadItem();

            switch (BadEffect)
            {
                case BadEffect.TooSlow:
                    Speed = SpeedSaved;
                    break;
                case BadEffect.TooSpeed:
                    Speed = SpeedSaved;
                    break;
                case BadEffect.KeysInversion:
                    break;
                case BadEffect.BombTimerChanged:
                    BombTimer = BombTimerSaved;
                    break;
            }
        }
    }
}
