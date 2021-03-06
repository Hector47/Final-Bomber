﻿using FBServer.Core.Entities;
using System;

namespace FBServer.Host
{
    public class HostGame
    {
        public HostGame()
        {
        }

        public void Initialize()
        {
            // Events
            GameServer.Instance.ConnectedClient += GameServer_ConnectedClient;
            GameServer.Instance.DisconnectedClient += GameServer_DisconnectedClient;
            GameServer.Instance.BombPlacing += GameServer_BombPlacing;

            GameServer.Instance.StartServer();
        }

        public void Update()
        {
            GameServer.Instance.Update();
        }

        public void Dispose()
        {
            GameServer.Instance.EndServer("Ending game");
        }

        #region Server events

        private void GameServer_ConnectedClient(Client client, EventArgs e)
        {
            if (true) // TODO: Check that the server is not full
            {
                // Add client to the list
                GameServer.Instance.Clients.AddClient(client);

                //MainServer.SendCurrentPlayerAmount();
            }
            else
            {
                client.ClientConnection.Disconnect("Full Server");

                Program.Log.Info("[FULLGAME] Client tried to connect but server is full !");
            }
        }

        private void GameServer_DisconnectedClient(Client sender, EventArgs e)
        {
            if (GameServer.Instance.GameManager.GameInitialized)
            {
                sender.Player.IsAlive = false;
                GameServer.Instance.SendRemovePlayer(sender.Player);
            }
            //MainServer.SendCurrentPlayerAmount();
        }

        // An evil player wants to plant a bomb
        private void GameServer_BombPlacing(Client sender)
        {
            if (GameServer.Instance.GameManager.GameHasBegun)
            {
                var player = sender.Player;

                if (player.CurrentBombAmount > 0)
                {
                    var bo = GameServer.Instance.GameManager.BombList.Find(b => b.CellPosition == player.CellPosition);

                    if (bo != null) return;

                    var bomb = new Bomb(player.Id, player.CellPosition, player.BombPower, player.BombTimer,
                        player.Speed);

                    bomb.Initialize(GameServer.Instance.GameManager.CurrentMap.Size, 
                                    GameServer.Instance.GameManager.CurrentMap.CollisionLayer, 
                                    GameServer.Instance.GameManager.HazardMap);

                    GameServer.Instance.GameManager.CurrentMap.Board[bomb.CellPosition.X, bomb.CellPosition.Y] = bomb;
                    GameServer.Instance.GameManager.CurrentMap.CollisionLayer[bomb.CellPosition.X, bomb.CellPosition.Y] = true;

                    GameServer.Instance.GameManager.AddBomb(bomb);
                    player.CurrentBombAmount--;

                    GameServer.Instance.SendPlayerPlacingBomb(sender, bomb.CellPosition);
                }
            }
        }

        private void Bomb_IsExploded(Bomb bomb)
        {
            /*
            //_gameManager.CurrentMap.CalcBombExplosion(bomb);
            GameServer.Instance.SendBombExploded(bomb);
            bomb.Position.Bombed = null;

            //Kollar om bomben exploderar en annan bomb
            if (GameSettings.BombsExplodeBombs)
            {
                foreach (Explosion ex in bomb.Explosion)
                {
                    foreach (Bomb b in GameManager.BombList)
                    {
                        if (b.Position == ex.Position)
                        {
                            if (b.Exploded == false)
                                b.Explode();
                        }
                    }
                }
            }
            */
        }

        #endregion
    }
}
