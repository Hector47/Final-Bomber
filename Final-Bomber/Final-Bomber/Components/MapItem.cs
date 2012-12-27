﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Final_Bomber.Sprites;
using Microsoft.Xna.Framework;

namespace Final_Bomber.Components
{
    public abstract class MapItem
    {
        public abstract AnimatedSprite Sprite { get; protected set; }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public abstract void Destroy();
        public abstract void Remove();
    }
}