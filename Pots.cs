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
    public class Pot : Piece
    {
        public Pot(Tile loc, Color color) : base(loc,color)
        {
            IsStatic = true;
        }

        public override void Dispose()
        {
            if (Game1.rnd.Next(0, 5) == 0) Container.Contents = new Item(Container, Color.LightGoldenrodYellow);
            if (Game1.rnd.Next(0, 4) == 0)
            {
                Container.Contents = new Weapon(Container, Color.LightGoldenrodYellow, (WeaponType)Game1.rnd.Next(1, 9));

            }
            base.Dispose();
        }
    }

    public class ExPot : Piece
    {
        public ExPot(Tile loc, Color color) : base(loc, color)
        {
            _direction = Dir.none;
            SafeZone = new List<Tile>();
            IsStatic = true;
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
