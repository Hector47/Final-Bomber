﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Final_Bomber.Net.MainServer
{
    partial class MainServer
    {
        #region LoggedIn
        public delegate void LoggedInEventHandler(byte status);
        public event LoggedInEventHandler LoggedIn;
        protected virtual void OnLoggedIn(byte status)
        {
            if (LoggedIn != null)
                LoggedIn(status);
        }
        #endregion

        public void RecieveLoggedIn(byte status)
        {
            OnLoggedIn(status);
        }

        #region RecieveCreatedAccount
        public delegate void CreatedAccountEventHandler(byte status);
        public event CreatedAccountEventHandler CreatedAccount;
        protected virtual void OnCreatedAccount(byte status)
        {
            if (CreatedAccount != null)
                CreatedAccount(status);
        }
        #endregion

        public void RecieveCreatedAccount(byte status)
        {
            OnCreatedAccount(status);
        }

        #region HostedGames
        public delegate void HostedGamesEventHandler(string ip, string name, string map, float ping, int maxplayers, int players);
        public event HostedGamesEventHandler HostedGames;
        protected virtual void OnHostedGames(string ip, string name, string map, float ping, int maxplayers, int players)
        {
            if (HostedGames != null)
                HostedGames(ip, name, map, ping, maxplayers, players);
        }
        #endregion
        public void RecieveHostedGames(string game)
        {
            if (game != "END")
            {
                OnHostedGames(game, msgIn.ReadString(), msgIn.ReadString(), msgIn.ReadFloat(), msgIn.ReadInt32(), msgIn.ReadInt32());
            }
        }

        public void RecieveCurrentVersion(string version)
        {
            /*
            string VERSION = Program.VERSION;
            if (version != VERSION)
            {
                MessageBox.Show("A new version of Final-Bomber has been released, please download the latest patch:\nYour Patch Version: " + VERSION + "\nNew Patch Version: " + version);
                this.EndMainConnection("bye");
                Application.Exit();
            }
            */
        }

        #region Stats
        public delegate void StatsEventHandler(PlayerStatsEventArgs args);
        public event StatsEventHandler Stats;
        protected virtual void OnStats(PlayerStatsEventArgs args)
        {
            if (Stats != null)
                Stats(args);
        }
        #endregion
        public void RecieveStats()
        {
            PlayerStatsEventArgs args = new PlayerStatsEventArgs();
            args.Burns = msgIn.ReadInt32();
            args.Defeats = msgIn.ReadInt32();
            args.ExplodeHits = msgIn.ReadInt32();
            args.Kills = msgIn.ReadInt32();
            args.PowerupsPicked = msgIn.ReadInt32();
            args.SelfExplodeHits = msgIn.ReadInt32();
            args.SelfKills = msgIn.ReadInt32();
            args.TilesBlowned = msgIn.ReadInt32();
            args.TileWalkDistance = msgIn.ReadInt32();
            args.Wins = msgIn.ReadInt32();
            OnStats(args);
        }

        #region Ranking
        public delegate void RankingEventHandler(int ranknr, string username, int elo, bool clearRank);
        public event RankingEventHandler Ranking;
        protected virtual void OnRanking(int ranknr, string username, int elo, bool clearRank)
        {
            if (Ranking != null)
                Ranking(ranknr, username, elo, clearRank);
        }
        #endregion
        public void RecieveRanking()
        {
            OnRanking(0, "", 0, true);
            int ranknr = 1;
            int count = msgIn.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                OnRanking(ranknr, msgIn.ReadString(), msgIn.ReadInt32(), false);
                ranknr++;
            }
        }
    }

    public class PlayerStatsEventArgs
    {
        public int Kills, ExplodeHits, Burns, SelfExplodeHits, SelfKills, TilesBlowned, PowerupsPicked, TileWalkDistance, Wins, Defeats;
    }
}