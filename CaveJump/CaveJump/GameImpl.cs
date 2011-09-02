using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Juicy.Engine;
using Cavejump.Screens;

namespace Cavejump
{
    public class GameImpl : JuicyGame
    {
        public GameImpl(Game game)
            : base(game)
        {

        }

        protected override void AddScreens()
        {
            AddScreen(1, new PlayScreen2());
        }

        public override void LoadContent()
        {
            base.LoadContent();
            GameConfig.initConfig(this);
        }
    }
}
