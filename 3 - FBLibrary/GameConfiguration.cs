﻿using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Microsoft.Xna.Framework;

namespace FBLibrary
{
    public static class GameConfiguration
    {
        // Random
        public static readonly Random Random = new Random(Environment.TickCount);

        // Invincibility
        public const bool Invincible = false;
        public static TimeSpan PlayerInvincibleTimer = TimeSpan.FromSeconds(3);

        // Base characteristics
        public const int BasePlayerBombPower = 1;
        public const float BasePlayerSpeed = 150;
        public const float BaseBombSpeed = 3f;
        public const int BasePlayerBombAmount = 1;
        // Initially => 2
        public static TimeSpan BaseBombTimer = TimeSpan.FromSeconds(2);

        // Characteristics minimum and maximum
        public const float MaxSpeed = 300f;
        public const int MaxBombPower = 30;
        public const int MaxBombAmount = 30;

        public const float MinSpeed = 1f;
        public const int MinBombPower = 1;
        public const int MinBombAmount = 1;

        // Bad Item
        public const int BadItemTimerMin = 10; // Seconds
        public const int BadItemTimerMax = 30; // Seconds
        public const int BadItemTimerChangedMin = 3; // Seconds
        public const int BadItemTimerChangedMax = 8; // Seconds

        // Sudden Death
        public static bool ActiveSuddenDeath = true;
        public static TimeSpan SuddenDeathTimer = TimeSpan.FromSeconds(5);
        public static SuddenDeathTypeEnum SuddenDeathType = SuddenDeathTypeEnum.OnlyWall;
        public static float SuddenDeathCounterBombs = 0.3f;
        public static float SuddenDeathCounterWalls = 0.3f;
        public static float SuddenDeathWallSpeed = (float)Math.Round(0.25f, 2);
        public const float SuddenDeathMaxWallSpeed = 1f;


        // Game info
        public const float PlayerSpeedIncrementeurPercentage = 5; // Percentage of base player speed
        public static int WallPercentage = 0; // From 0% to 100%
        public const int PowerUpPercentage = 100;

        public static TimeSpan BombDestructionTime = TimeSpan.FromMilliseconds(350);
        public static TimeSpan WallDestructionTime = TimeSpan.FromMilliseconds(300);
        public static TimeSpan PowerUpDestructionTime = TimeSpan.FromMilliseconds(750);
        public static TimeSpan PlayerDestructionTime = TimeSpan.FromMilliseconds(1750);

        // Score
        public const int ScoreByKill = 3;
        public const int ScoreBySuicide = 1;

        // Power up
        public static readonly List<PowerUpType> PowerUpTypeAvailable = new List<PowerUpType>() 
        { 
            PowerUpType.Power,
            PowerUpType.Bomb,
            PowerUpType.Speed,
            //PowerUpType.BadItem,
            PowerUpType.Score 
        };

        public static readonly List<BadEffect> BadEffectList = new List<BadEffect>()
        {
            BadEffect.BombDrop,
            BadEffect.BombTimerChanged,
            BadEffect.KeysInversion,
            BadEffect.NoBomb,
            BadEffect.TooSlow,
            BadEffect.TooSpeed
        };

        // World
        public static Point BaseTileSize = new Point(32, 32);

        // Time
        public static long DeltaTime; // Ticks

#if DEBUG
        public const int MinimumPlayerNumber = 1;
#else
        public const int MinimumPlayerNumber = 2;
#endif

        public const string NetworkAppIdentifier = "Final-Bomber";

        // TO DELETE
        public const int PlayerNumber = 5;
        public const string ServerPort = "2643";
        public const int AlivePlayerRemaining = PlayerNumber - 1;
        public const float SimulatedMinimumLatency = 0.05f; // 0.05f for a ping of 50 ms
    }
}
