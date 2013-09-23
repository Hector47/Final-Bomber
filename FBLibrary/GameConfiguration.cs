﻿using System;
using System.Collections.Generic;
using FBLibrary.Core;
using Microsoft.Xna.Framework;

namespace FBLibrary
{
    public static class GameConfiguration
    {
        // Invincibility
        public const bool Invincible = false;
        public static TimeSpan PlayerInvincibleTimer = TimeSpan.FromSeconds(3);

        // Base characteristics
        public const int BasePlayerBombPower = 1;
        public const float BasePlayerSpeed = 150f;
        public const float BaseBombSpeed = 3f;
        public const int BasePlayerBombAmount = 1;
        // Initially => 2
        public static TimeSpan BaseBombTimer = TimeSpan.FromSeconds(2);

        // Characteristics minimum and maximum
        public const float MaxSpeed = 300f;
        public const int MaxBombPower = 1;
        public const int MaxBombAmount = 1;

        public const float MinSpeed = 1f;
        public const int MinBombPower = 1;
        public const int MinBombAmount = 1;

        // Game info
        public static float PlayerSpeedIncrementeur = 0.25f;
        public static int WallPercentage = 100; // From 0% to 100%
        public static int PowerUpPercentage = 100;

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

        // TO DELETE
        public static int PlayerNumber = 1;
        public static string ServerIp = "localhost";
        public static string ServerPort = "2643";
    }
}