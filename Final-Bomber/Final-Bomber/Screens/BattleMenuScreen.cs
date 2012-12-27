﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Final_Bomber.Components;
using Final_Bomber.Controls;
using Microsoft.Xna.Framework.Input;

namespace Final_Bomber.Screens
{
    public class BattleMenuScreen : BaseGameState
    {
        #region Field Region
        string[] menuString;
        int indexMenu;
        Vector2 menuPosition;
        bool disabledArrows;
        #endregion

        #region Property Region

        #endregion

        #region Constructor Region

        public BattleMenuScreen(Game game, GameStateManager manager)
            : base(game, manager)
        {
            menuString = new string[] 
            { 
                "Nombre de joueur", 
                "Taille", 
                Config.MapSize.X.ToString(), 
                Config.MapSize.Y.ToString(), 
                "Téléporteurs",
                "Flèches",
                "Nombre de murs",
                "Nombre d'objets",
                "Mort Subite", 
                "Objets", 
                "Lancer la partie !" };
            indexMenu = 0;

            disabledArrows = false;
        }

        #endregion

        #region XNA Method Region

        public override void Initialize()
        {
            menuPosition = new Vector2(Config.Resolutions[Config.IndexResolution, 0] / 2, Config.Resolutions[Config.IndexResolution, 1] / 4);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            ControlManager.Update(gameTime, PlayerIndex.One);

            if (InputHandler.KeyPressed(Keys.Escape))
                StateManager.PushState(GameRef.TitleScreen);

            switch (indexMenu)
            {
                case 0:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.PlayersNumber <= 1)
                            Config.PlayersNumber = 5;
                        else
                            Config.PlayersNumber--;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.PlayersNumber < 5)
                            Config.PlayersNumber++;
                        else
                            Config.PlayersNumber = 1;
                    }
                    break;
                case 1:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.MapSize.X <= Config.MinimumMapSize.X || Config.MapSize.Y <= Config.MinimumMapSize.Y)
                        {
                            Config.MapSize = Config.MaximumMapSize[Config.IndexResolution];
                        }
                        else
                        {
                            Config.MapSize = new Point(Config.MapSize.X - 2, Config.MapSize.Y - 2);
                        }
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.MapSize.X == Config.MaximumMapSize[Config.IndexResolution].X ||
                            Config.MapSize.Y == Config.MaximumMapSize[Config.IndexResolution].Y)
                        {
                            Config.MapSize = Config.MinimumMapSize;
                        }
                        else
                        {
                            Config.MapSize = new Point(Config.MapSize.X + 2, Config.MapSize.Y + 2);
                        }
                    }
                    menuString[2] = Config.MapSize.X.ToString();
                    menuString[3] = Config.MapSize.Y.ToString();

                    if (Config.MapSize.X < 17 || Config.MapSize.Y < 17)
                    {
                        disabledArrows = true;
                        Config.ActiveArrows = false;
                    }
                    else
                        disabledArrows = false;
                    break;
                case 2:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.MapSize.X <= Config.MinimumMapSize.X)
                            Config.MapSize.X = Config.MaximumMapSize[Config.IndexResolution].X;
                        else
                            Config.MapSize.X -= 2;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.MapSize.X == Config.MaximumMapSize[Config.IndexResolution].X)
                            Config.MapSize.X = Config.MinimumMapSize.X;
                        else
                            Config.MapSize.X += 2;
                    }
                    menuString[2] = Config.MapSize.X.ToString();
                    if (Config.MapSize.X < 17)
                    {
                        disabledArrows = true;
                        Config.ActiveArrows = false;
                    }
                    else if (Config.MapSize.Y >= 17)
                        disabledArrows = false;   
                    break;
                case 3:
                    if (InputHandler.KeyPressed(Keys.Left))
                    {
                        if (Config.MapSize.Y <= Config.MinimumMapSize.Y)
                            Config.MapSize.Y = Config.MaximumMapSize[Config.IndexResolution].Y;
                        else
                            Config.MapSize.Y -= 2;
                    }
                    else if (InputHandler.KeyPressed(Keys.Right))
                    {
                        if (Config.MapSize.Y == Config.MaximumMapSize[Config.IndexResolution].Y)
                            Config.MapSize.Y = Config.MinimumMapSize.Y;
                        else
                            Config.MapSize.Y += 2;
                    }
                    menuString[3] = Config.MapSize.Y.ToString();
                    if (Config.MapSize.Y < 17)
                    {
                        disabledArrows = true;
                        Config.ActiveArrows = false;
                    }
                    else if(Config.MapSize.X >= 17)
                        disabledArrows = false;                    
                    break;
                case 4:
                    if(InputHandler.KeyPressed(Keys.Right) || InputHandler.KeyPressed(Keys.Left) || InputHandler.KeyPressed(Keys.Enter))
                        Config.ActiveTeleporters = !Config.ActiveTeleporters;
                    break;
                case 5:
                    if(InputHandler.KeyPressed(Keys.Right) || InputHandler.KeyPressed(Keys.Left) || InputHandler.KeyPressed(Keys.Enter))
                        Config.ActiveArrows = !Config.ActiveArrows;
                    break;
                case 6:
                    if (InputHandler.KeyDown(Keys.Right) || InputHandler.KeyPressed(Keys.Enter))
                        Config.WallNumber = (Config.WallNumber + 1) % 101;
                    else if (InputHandler.KeyDown(Keys.Left))
                    {
                        if (Config.WallNumber <= 0)
                            Config.WallNumber = 100;
                        else
                            Config.WallNumber--;
                    }
                    break;
                case 7:
                    if (InputHandler.KeyDown(Keys.Right) || InputHandler.KeyPressed(Keys.Enter))
                        Config.ItemNumber = (Config.ItemNumber + 1) % 101;
                    else if (InputHandler.KeyDown(Keys.Left))
                    {
                        if (Config.ItemNumber <= 0)
                            Config.ItemNumber = 100;
                        else
                            Config.ItemNumber--;
                    }
                    break;
                case 8:
                    if(InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(GameRef.SuddenDeathMenuScreen);
                    break;
                case 9:
                    if (InputHandler.KeyPressed(Keys.Enter))
                        StateManager.ChangeState(GameRef.ItemMenuScreen);
                    break;
                case 10:
                    if (InputHandler.KeyPressed(Keys.Enter))
                    {
                        Config.PlayersPositions = new Point[]
                        {
                            new Point(1, 1),
                            new Point(Config.MapSize.X - 2, Config.MapSize.Y - 2),
                            new Point(1, Config.MapSize.Y - 2),
                            new Point(Config.MapSize.X - 2, 1),
                            new Point((int)Math.Ceiling((double)(Config.MapSize.X - 2)/(double)2), 
                                (int)Math.Ceiling((double)(Config.MapSize.Y - 2)/(double)2))
                        };
                        StateManager.ChangeState(GameRef.GamePlayScreen);
                    }
                    break;
            }

            if (InputHandler.KeyPressed(Keys.Up))
            {
                if (indexMenu == 6 && disabledArrows)
                    indexMenu = 4;
                else
                {
                    if (indexMenu <= 0)
                        indexMenu = menuString.Length - 1;
                    else
                        indexMenu--;
                }
            }
            else if (InputHandler.KeyPressed(Keys.Down))
            {
                if (indexMenu == 4 && disabledArrows)
                    indexMenu = 6;
                else
                    indexMenu = (indexMenu + 1) % menuString.Length;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameRef.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.Identity);

            base.Draw(gameTime);

            ControlManager.Draw(GameRef.SpriteBatch);

            string text;
            int xLag = 0;
            int yLag = 0;
            for (int i = 0; i < menuString.Length; i++)
            {
                if (i < 2)
                    yLag = i;
                else if (i >= 2 && i <= 3)
                {
                    if (Config.MapSize.X >= 10)
                        xLag = 70;
                    else
                        xLag = 60;
                    if (i == 3)
                    {
                        if (Config.MapSize.Y >= 10)
                        {
                            if (Config.MapSize.X >= 10)
                                xLag += 70;
                            else
                                xLag += 60;
                        }
                        else
                        {
                            if (Config.MapSize.X >= 10)
                                xLag += 60;
                            else
                                xLag += 50;
                        }
                    }
                }
                else if (i > 3)
                {
                    yLag = i - 2;
                    xLag = 0;
                }

                if (i >= 1 && i <= 3)
                    xLag -= 50;
                else if (i >= 4 && i <= 7)
                    xLag -= 50;
                else if (i == 0)
                    xLag -= 30;

                text = menuString[i];

                Color textColor = Color.Black;
                if (i == indexMenu)
                    textColor = Color.Green;
                else if (i == 5 && disabledArrows)
                    textColor = Color.Gray;

                GameRef.SpriteBatch.DrawString(this.BigFont, text,
                    new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag,
                        menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), textColor);
                
                switch(i)
                {
                    case 0:
                        GameRef.SpriteBatch.DrawString(this.BigFont, ": " + Config.PlayersNumber,
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 1:
                        GameRef.SpriteBatch.DrawString(this.BigFont, ": ",
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 2:
                        GameRef.SpriteBatch.DrawString(this.BigFont, "x",
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 4:
                        GameRef.SpriteBatch.DrawString(this.BigFont, ": " + Config.ActiveTeleporters,
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 5:
                        if(!disabledArrows)
                            textColor = Color.Black;
                        GameRef.SpriteBatch.DrawString(this.BigFont, ": " + Config.ActiveArrows,
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), textColor);
                        break;
                    case 6:
                        GameRef.SpriteBatch.DrawString(this.BigFont, ": " + Config.WallNumber + "%",
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                    case 7:
                        GameRef.SpriteBatch.DrawString(this.BigFont, ": " + Config.ItemNumber + "%",
                        new Vector2(menuPosition.X - this.BigFont.MeasureString(text).X / 2 + xLag + this.BigFont.MeasureString(text).X,
                            menuPosition.Y + this.BigFont.MeasureString(text).Y * yLag - this.BigFont.MeasureString(text).Y / 2), Color.Black);
                        break;
                }
            }

            GameRef.SpriteBatch.End();
        }

        #endregion

        #region Abstract Method Region
        #endregion

        #region Method Region
        #endregion
    }
}