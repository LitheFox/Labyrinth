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
    public class Chest : Enemy
    {
        public Chest(Tile loc, Color color) : base(loc, color)
        {
            IsMimic = Convert.ToBoolean(rnd.Next(0, 2));
            if (IsMimic)
            {
                Name = "Mimic";
                Value = 2;
            }
            else
            {
                Name = "Chest";
                Value = 0;
                IsStatic = true;
            }
            Distance = Range;
            IsHidden = true;
            Blend = Color.White;//hover
        }
        public bool IsMimic { get; set; }
        public bool IsHidden { get; set; }
        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);
            if (!IsHidden && IsMimic)
            {
                Move();
                FindPath(sender, e);
            }
            
            Distance = Vector2.Distance(Pos, sender.PlayerSpace.Pos);

            if (Distance <= 2 && IsMimic && IsHidden)
            {
                IsHidden = false;
                FindPath(sender, e);
            }
        }
    }

    
}
