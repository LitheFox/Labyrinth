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
    public class Dummy : Enemy
    {
        public Dummy(Tile loc, Game1 source, Color color) : base(loc,color)
        {
            Random random = new Random(GetHashCode());
            Distance = 10;
            Name = "Dummy";
            IsStatic = true;
        }

        public float Distance { get; set; }

        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            
        }
    }
}