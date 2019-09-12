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
    public class Slime : Enemy
    {
        public Slime(Tile loc, Game1 source, Color color) : base(loc,color)
        {
            Random random = new Random(GetHashCode());
            Distance = 10;
            IsCoolingDown = false;
            IsNew = true;
            Value = 1;

            if (random.Next(0, 2) == 0)
            {
                IsCoolingDown = false;
            }
            else
            {
                IsCoolingDown = true;
            }

            Name = "Slime";
        }

        public bool IsCoolingDown { get; set; }
        public bool IsNew { get; set; }

        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);

            if (!IsNew && (IsCoolingDown || IsDisposed) && Distance < Range)
            {
                IsCoolingDown = false;
            }
            else
            {
                Move();
                FindPath(sender, e);
                Distance = Vector2.Distance(Pos, sender.PlayerSpace.Pos);

                if (!IsNew) IsCoolingDown = true;
                IsNew = false;
            }
        }
    }
}