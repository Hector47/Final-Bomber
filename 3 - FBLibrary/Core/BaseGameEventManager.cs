﻿
using System;
using FBLibrary.Core.BaseEntities;

namespace FBLibrary.Core
{
    /// <summary>
    /// Base class to manage all game events like game start, bomb planted, etc...
    /// </summary>
    public abstract class BaseGameEventManager
    {
        #region Events declarations

        public delegate void RoundEndEventHandler();
        public event RoundEndEventHandler RoundEnd;

        public delegate void PlayerDeathEventHandler(BasePlayer sender, EventArgs args);
        public event PlayerDeathEventHandler PlayerDeath;



        #endregion

        public virtual void Initialize()
        {
            // Bind events
            PlayerDeath += PlayerDeathAction;
            RoundEnd += RoundEndAction;
        }

        public virtual void Dispose()
        {
            // Unbind events
            PlayerDeath -= PlayerDeathAction;
            RoundEnd -= RoundEndAction;
        }

        #region Events invocators

        public void OnRoundEnd()
        {
            RoundEndEventHandler handler = RoundEnd;
            if (handler != null) handler();
        }

        public void OnPlayerDeath(BasePlayer player)
        {
            PlayerDeathEventHandler handler = PlayerDeath;
            if (handler != null) handler(player, EventArgs.Empty);
        }

        #endregion

        #region Events actions

        protected virtual void RoundEndAction()
        {
        }

        protected virtual void PlayerDeathAction(BasePlayer sender, EventArgs args)
        {
        }

        #endregion
    }
}
