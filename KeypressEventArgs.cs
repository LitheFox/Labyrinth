using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Labyrinth_Redux
{
    public class KeypressEventArgs : EventArgs
    {
        public KeypressEventArgs(Keys[] keys, GamePadState controller, GamePadState oldcont)
        {
            GetKeys = keys;
            Controller = controller;
            oldController = oldcont;
        }

        public Keys[] GetKeys { get; set; }
        public GamePadState Controller { get; set; }
        public GamePadState oldController { get; set; }
    }
}
