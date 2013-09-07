﻿using System;
using System.Collections.Generic;
using Final_Bomber.Components.AI;
using Final_Bomber.Screens;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Final_Bomber.Controls;
using Final_Bomber.TileEngine;
using Final_Bomber.WorldEngine;
using Microsoft.Xna.Framework.Input;
using Final_Bomber.Components;

namespace Final_Bomber.Components
{
    public enum LookDirection { Down, Left, Right, Up, Idle }

    public abstract class Player : Entity
    {
        #region Field Region

        public override sealed AnimatedSprite Sprite { get; protected set; }

        private readonly AnimatedSprite _playerDeathAnimation;

        protected bool IsMoving;

        private TimeSpan _invincibleTimer;
        private TimeSpan _invincibleBlinkTimer;
        private float _invincibleBlinkFrequency;
        
        private Point _previousCellPosition;
        private bool _cellChanging;
        private bool _cellTeleporting;

        protected LookDirection LookDirection;

        protected readonly FinalBomber GameRef;
        protected Point MapSize;

        // Characteristics

        // Bad item
        protected TimeSpan BombTimerSaved;
        protected float SpeedSaved;

        #endregion

        #region Property Region

        public int Id { get; private set; }

        public Camera Camera { get; private set; }

        public bool IsAlive { get; private set; }

        public bool InDestruction { get; private set; }

        public bool IsInvincible { get; private set; }

        public int Power { get; private set; }

        public int CurrentBombNumber { get; set; }

        public int TotalBombNumber { get; private set; }

        public bool OnEdge { get; set; }

        public bool HasBadItemEffect { get; private set; }

        public BadItemEffect BadItemEffect { get; private set; }

        public TimeSpan BadItemTimer { get; private set; }

        public TimeSpan BadItemTimerLenght { get; private set; }

        public TimeSpan BombTimer { get; protected set; }

        #endregion

        #region Constructor Region

        protected Player(int id, FinalBomber game, Vector2 position)
        {
            this.Id = id;
            this.GameRef = game;

            this.MapSize = Config.MapSize;
            //this.MapSize = game.GamePlayScreen.World.Levels[game.GamePlayScreen.World.CurrentLevel].Size;

            this.Camera = new Camera(GameRef.ScreenRectangle);

            const int animationFramesPerSecond = 10;
            var animations = new Dictionary<AnimationKey, Animation>();

            var animation = new Animation(4, 23, 23, 0, 0, animationFramesPerSecond);
            animations.Add(AnimationKey.Down, animation);

            animation = new Animation(4, 23, 23, 0, 23, animationFramesPerSecond);
            animations.Add(AnimationKey.Left, animation);

            animation = new Animation(4, 23, 23, 0, 46, animationFramesPerSecond);
            animations.Add(AnimationKey.Right, animation);

            animation = new Animation(4, 23, 23, 0, 69, animationFramesPerSecond);
            animations.Add(AnimationKey.Up, animation);

            var spriteTexture = GameRef.Content.Load<Texture2D>("Graphics/Characters/player1");
            
            this.Sprite = new AnimatedSprite(spriteTexture, animations, position);
            this.Sprite.ChangeFramesPerSecond(10);
            this.Sprite.Speed = Config.BasePlayerSpeed;

            this._previousCellPosition = this.Sprite.CellPosition; 

            var playerDeathTexture = GameRef.Content.Load<Texture2D>("Graphics/Characters/player1Death");
            animation = new Animation(8, 23, 23, 0, 0, 4);
            _playerDeathAnimation = new AnimatedSprite(playerDeathTexture, animation, Sprite.Position)
                {
                    IsAnimating = false
                };

            this.IsMoving = false;
            this.IsAlive = true;
            this.InDestruction = false;
            this.OnEdge = false;
            this.IsInvincible = true;

            this._invincibleTimer = Config.PlayerInvincibleTimer;
            this._invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            this._invincibleBlinkTimer = TimeSpan.FromSeconds(this._invincibleBlinkFrequency);

            Power = Config.BasePlayerBombPower;
            TotalBombNumber = Config.BasePlayerBombNumber;
            CurrentBombNumber = TotalBombNumber;

            BombTimer = Config.BombTimer;

            LookDirection = LookDirection.Down;

            // Bad item
            HasBadItemEffect = false;
            BadItemTimer = TimeSpan.Zero;
            BadItemTimerLenght = TimeSpan.Zero;
        }
        #endregion

        #region XNA Method Region
        public override void Update(GameTime gameTime)
        {
            if (IsAlive && !InDestruction)
            {
                Sprite.Update(gameTime);

                #region Invincibility

                if (!Config.Invincible && IsInvincible)
                {
                    if (_invincibleTimer >= TimeSpan.Zero)
                    {
                        _invincibleTimer -= gameTime.ElapsedGameTime;
                        if (_invincibleBlinkTimer >= TimeSpan.Zero)
                            _invincibleBlinkTimer -= gameTime.ElapsedGameTime;
                        else
                            _invincibleBlinkTimer = TimeSpan.FromSeconds(_invincibleBlinkFrequency);
                    }
                    else
                    {
                        _invincibleTimer = Config.PlayerInvincibleTimer;
                        IsInvincible = false;
                    }
                }

                #endregion

                #region Moving

                _cellChanging = _previousCellPosition != Sprite.CellPosition;

                Move();

                #endregion

                #region Bomb

                #region Push a bomb

                if (Config.PlayerCanPush)
                {
                    if (LookDirection != LookDirection.Idle)
                    {
                        Point direction = Point.Zero;
                        switch (LookDirection)
                        {
                            case LookDirection.Up:
                                direction = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y - 1);
                                break;
                            case LookDirection.Down:
                                direction = new Point(Sprite.CellPosition.X, Sprite.CellPosition.Y + 1);
                                break;
                            case LookDirection.Left:
                                direction = new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y);
                                break;
                            case LookDirection.Right:
                                direction = new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y);
                                break;
                        }
                        Bomb bomb = BombAt(direction);
                        if (bomb != null)
                            bomb.ChangeDirection(LookDirection, this.Id);
                    }
                }

                #endregion

                #endregion

                #region Item

                /*
                if (GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Item)
                {
                    var item = (Item)(GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                        Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);
                    if (!item.InDestruction)
                    {
                        if (!HasBadItemEffect || (HasBadItemEffect && item.Type != ItemType.BadItem))
                        {
                            item.ApplyItem(this);
                            GameRef.GamePlayScreen.ItemPickUpSound.Play();
                            item.Remove();
                        }
                    }
                }
                */
                // Have caught a bad item
                if (HasBadItemEffect)
                {
                    BadItemTimer += gameTime.ElapsedGameTime;
                    if (BadItemTimer >= BadItemTimerLenght)
                    {
                        RemoveBadItem();
                    }
                }

                #endregion

                #region Teleporter

                /*
                if (!_cellTeleporting && GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] is Teleporter)
                {
                    var teleporter = (Teleporter)(GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                        Map[Sprite.CellPosition.X, Sprite.CellPosition.Y]);

                    teleporter.ChangePosition(this);
                    _cellTeleporting = true;
                }
                */
                #endregion
            }

            #region Death
            else if(_playerDeathAnimation.IsAnimating)
            {
                _playerDeathAnimation.Update(gameTime);

                if (_playerDeathAnimation.Animation.CurrentFrame == _playerDeathAnimation.Animation.FrameCount - 1)
                    Remove();
            }
            #endregion

            #region Edge wall gameplay
            else if (OnEdge && (!Config.ActiveSuddenDeath || (Config.ActiveSuddenDeath && !GameRef.GamePlayScreen.SuddenDeath.HasStarted)))
            {
                Sprite.Update(gameTime);

                MoveFromEdgeWall();
            }
            #endregion

            #region Camera
            Camera.Update(gameTime);

            if (Config.Debug)
            {
                if (InputHandler.KeyDown(Microsoft.Xna.Framework.Input.Keys.PageUp) ||
                    InputHandler.ButtonReleased(Buttons.LeftShoulder, PlayerIndex.One))
                {
                    Camera.ZoomIn();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }
                else if (InputHandler.KeyDown(Microsoft.Xna.Framework.Input.Keys.PageDown))
                {
                    Camera.ZoomOut();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }
                else if (InputHandler.KeyDown(Microsoft.Xna.Framework.Input.Keys.End))
                {
                    Camera.ZoomReset();
                }

                if (InputHandler.KeyReleased(Microsoft.Xna.Framework.Input.Keys.F))
                {
                    Camera.ToggleCameraMode();
                    if (Camera.CameraMode == CameraMode.Follow)
                        Camera.LockToSprite(Sprite);
                }

                if (Camera.CameraMode != CameraMode.Follow)
                {
                    if (InputHandler.KeyReleased(Microsoft.Xna.Framework.Input.Keys.L))
                    {
                        Camera.LockToSprite(Sprite);
                    }
                }
            }

            if (IsMoving && Camera.CameraMode == CameraMode.Follow)
                Camera.LockToSprite(Sprite);
            #endregion

            _previousCellPosition = Sprite.CellPosition;
        }

        public override void Draw(GameTime gameTime)
        {
            var playerNamePosition = new Vector2(
                Sprite.Position.X + Engine.Origin.X + (int)(Sprite.Width/2) - 
                ControlManager.SpriteFont.MeasureString(Config.PlayersName[Id - 1]).X / 2 + 5,
                Sprite.Position.Y + Engine.Origin.Y - 25 - 
                ControlManager.SpriteFont.MeasureString(Config.PlayersName[Id - 1]).Y / 2);

            if ((IsAlive && !InDestruction) || OnEdge)
            {
                if (IsInvincible)
                {
                    if (_invincibleBlinkTimer > TimeSpan.FromSeconds(_invincibleBlinkFrequency * 0.5f))
                    {
                        Sprite.Draw(gameTime, GameRef.SpriteBatch);
                        GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[Id - 1], playerNamePosition, Color.Black);
                    }
                }
                else
                {
                    Sprite.Draw(gameTime, GameRef.SpriteBatch);
                    GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[Id - 1], playerNamePosition, Color.Black);
                }
            }
            else
            {
                _playerDeathAnimation.Draw(gameTime, GameRef.SpriteBatch);
                GameRef.SpriteBatch.DrawString(ControlManager.SpriteFont, Config.PlayersName[Id - 1], playerNamePosition, Color.Black);
            }
        }
        #endregion

        #region Method Region

        #region Private Method Region

        private Bomb BombAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].Size.X &&
                cell.Y < GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].Size.Y)
            {
                Bomb bomb = GameRef.GamePlayScreen.BombList.Find(b => b.Sprite.CellPosition == cell);
                return (bomb);
            }
            else
                return null;
        }

        private void Invincibility()
        {
            this.IsInvincible = true;
            this._invincibleTimer = Config.PlayerInvincibleTimer;
            this._invincibleBlinkFrequency = Config.InvincibleBlinkFrequency;
            this._invincibleBlinkTimer = TimeSpan.FromSeconds(this._invincibleBlinkFrequency);
        }

        #endregion

        #region Protected Method Region
        protected bool WallAt(Point cell)
        {
            if (cell.X >= 0 && cell.Y >= 0 && cell.X < GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].Size.X &&
                cell.Y < GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].Size.Y)
                return (this.GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].CollisionLayer[cell.X, cell.Y]);
            else
                return false;
        }

        protected bool IsMoreTopSide()
        {
            return this.Sprite.Position.Y < ((this.Sprite.CellPosition.Y * Engine.TileHeight) - (this.Sprite.Speed / 2));
        }

        protected bool IsMoreBottomSide()
        {
            return this.Sprite.Position.Y > ((Sprite.CellPosition.Y * Engine.TileHeight) + (Sprite.Speed / 2));
        }

        protected bool IsMoreLeftSide()
        {
            return this.Sprite.Position.X < ((this.Sprite.CellPosition.X * Engine.TileWidth) - (this.Sprite.Speed / 2));
        }

        protected bool IsMoreRightSide()
        {
            return this.Sprite.Position.X > ((this.Sprite.CellPosition.X * Engine.TileWidth) + (this.Sprite.Speed / 2));
        }


        protected virtual void Move() 
        { 
        }

        protected void ComputeWallCollision()
        {
            #region Wall collisions
            Sprite.LockToMap();

            // -- Vertical check -- //
            // Is there a wall on the top ?
            if (WallAt(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y - 1)))
            {
                // Is there a wall on the bottom ?
                if (WallAt(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y + 1)))
                {
                    // Top collision and Bottom collision
                    if ((LookDirection == LookDirection.Up && IsMoreTopSide()) || (LookDirection == LookDirection.Down && IsMoreBottomSide()))
                        this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                }
                // No wall at the bottom
                else
                {
                    // Top collision
                    if (LookDirection == LookDirection.Up && IsMoreTopSide())
                        this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                }
            }
            // Wall only at the bottom
            else if (WallAt(new Point(this.Sprite.CellPosition.X, this.Sprite.CellPosition.Y + 1)))
            {
                // Bottom collision
                if (LookDirection == LookDirection.Down && IsMoreBottomSide())
                    this.Sprite.PositionY = this.Sprite.CellPosition.Y * Engine.TileHeight;
                // To lag him
                else if (LookDirection == LookDirection.Down)
                {
                    if (IsMoreLeftSide())
                        this.Sprite.PositionX += this.Sprite.Speed;
                    else if (IsMoreRightSide())
                        this.Sprite.PositionX -= this.Sprite.Speed;
                }
            }

            // -- Horizontal check -- //
            // Is there a wall on the left ?
            if (WallAt(new Point(Sprite.CellPosition.X - 1, Sprite.CellPosition.Y)))
            {
                // Is there a wall on the right ?
                if (WallAt(new Point(Sprite.CellPosition.X + 1, Sprite.CellPosition.Y)))
                {
                    // Left and right collisions
                    if ((this.LookDirection == LookDirection.Left && IsMoreLeftSide()) || (this.LookDirection == LookDirection.Right && IsMoreRightSide()))
                        this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                }
                // Wall only at the left
                else
                {
                    // Left collision
                    if (this.LookDirection == LookDirection.Left && IsMoreLeftSide())
                        this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
                }
            }
            // Wall only at the right
            else if (WallAt(new Point(this.Sprite.CellPosition.X + 1, this.Sprite.CellPosition.Y)))
            {
                // Right collision
                if (this.LookDirection == LookDirection.Right && IsMoreRightSide())
                    this.Sprite.PositionX = this.Sprite.CellPosition.X * Engine.TileWidth - Engine.TileWidth / 2 + Engine.TileWidth / 2;
            }


            // The player must stay in the map
            this.Sprite.PositionX = MathHelper.Clamp(this.Sprite.Position.X, Engine.TileWidth,
                (GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].Size.X * Engine.TileWidth) - 2 * Engine.TileWidth);
            this.Sprite.PositionY = MathHelper.Clamp(this.Sprite.Position.Y, Engine.TileHeight,
                (GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].Size.Y * Engine.TileHeight) - 2 * Engine.TileHeight);

            #endregion
        }

        protected void UpdatePlayerPosition()
        {
            #region Update player's position
            if (_cellChanging)
            {
                if (GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                   Map[_previousCellPosition.X, _previousCellPosition.Y] == this)
                {
                    GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                        Map[_previousCellPosition.X, _previousCellPosition.Y] = null;
                }

                if (_cellTeleporting)
                    _cellTeleporting = false;
            }
            else
            {
                if (GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                    Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] == null)
                {
                    GameRef.GamePlayScreen.World.Levels[GameRef.GamePlayScreen.World.CurrentLevel].
                        Map[Sprite.CellPosition.X, Sprite.CellPosition.Y] = this;
                }
            }
            #endregion
        }

        #endregion

        #region Public Method Region

        public void IncreaseTotalBombNumber(int incr)
        {
            if (this.TotalBombNumber + incr > Config.MaxBombNumber)
            {
                this.TotalBombNumber = Config.MaxBombNumber;
                this.CurrentBombNumber = TotalBombNumber;
            }
            else if (this.TotalBombNumber + incr < Config.MinBombNumber)
            {
                this.TotalBombNumber = Config.MinBombNumber;
                this.CurrentBombNumber = TotalBombNumber;
            }
            else
            {
                this.TotalBombNumber += incr;
                this.CurrentBombNumber += incr;
            }
        }

        public void IncreasePower(int incr)
        {
            if (this.Power + incr > Config.MaxBombPower)
                this.Power = Config.MaxBombPower;
            else if (this.Power + incr < Config.MinBombPower)
                this.Power = Config.MinBombPower;
            else
                this.Power += incr;
        }

        public void IncreaseSpeed(float incr)
        {
            this.Sprite.Speed += incr;
        }

        public virtual void ApplyBadItem(BadItemEffect effect)
        {
            this.HasBadItemEffect = true;
            this.BadItemEffect = effect;
            this.BadItemTimerLenght = TimeSpan.FromSeconds(GamePlayScreen.Random.Next(Config.BadItemTimerMin, Config.BadItemTimerMax));
        }

        protected virtual void RemoveBadItem()
        {
            HasBadItemEffect = false;
            BadItemTimer = TimeSpan.Zero;
            BadItemTimerLenght = TimeSpan.Zero;
        }

        public void Rebirth(Vector2 position)
        {
            this.IsAlive = true;
            this.Sprite.IsAnimating = true;
            this.InDestruction = false;
            this.Sprite.Position = position;
            this.Sprite.CurrentAnimation = AnimationKey.Down;
            this._playerDeathAnimation.IsAnimating = false;
            
            Invincibility();
        }
        
        #endregion

        #region Override Method Region

        public override void Destroy()
        {
            if (!this.InDestruction)
            {
                this.Sprite.IsAnimating = false;
                this.InDestruction = true;
                GameRef.GamePlayScreen.PlayerDeathSound.Play();
                this._playerDeathAnimation.Position = this.Sprite.Position;
                this._playerDeathAnimation.IsAnimating = true;
            }
        }

        public override void Remove()
        {
            this._playerDeathAnimation.IsAnimating = false;
            this.InDestruction = false;
            this.IsAlive = false;

            // Replacing for the gameplay on the edges
            // Right side
            if (MapSize.X - this.Sprite.CellPosition.X < MapSize.X / 2)
            {
                this.Sprite.CurrentAnimation = AnimationKey.Left;
                this.Sprite.Position = new Vector2((MapSize.X * Engine.TileWidth) - Engine.TileWidth, this.Sprite.Position.Y);
            }
            // Left side
            else
            {
                this.Sprite.CurrentAnimation = AnimationKey.Right;
                this.Sprite.Position = new Vector2(0, this.Sprite.Position.Y);
            }
        }

        protected virtual void MoveFromEdgeWall()
        {
            
        }

        #endregion

        #endregion
    }
}

class PlayerCollection : List<Player>
{
    public Player GetPlayerByID(int playerID)
    {
        foreach (Player player in this)
        {
            if (player.Id == playerID)
                return player;
        }
        return null;
    }
}
