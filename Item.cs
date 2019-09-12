using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Labyrinth_Redux
{
    public class Item
    {
        public Color Blend { get; set; }
        public Tile Container { get; set; }
        public Vector2 Pos
        {
            get
            {
                return Container.Pos;
            }
            set
            {
                _pos = value;
            }
        }
        protected Vector2 _pos;

        public Item(Tile loc, Color color)
        {
            Blend = color;
            if(loc != null)
            {
                Container = loc;
                Pos = Container.Pos;
            }
            
        }
    }
    public class Coin : Item
    {
        public Coin(Tile loc, Color color) : base(loc, color)
        {
            Container = loc;
            Blend = color;
            Pos = Container.Pos;
        }
    }
    public class Weapon : Item
    {
        public WeaponType Type { get; set; }
        public Dir Direction { get; set; }
        public int Durability { get; set; }
        public int Moves { get; set; }
        public Weapon(Tile loc, Color color, WeaponType wt) :base(loc,color)
        {
            Container = loc;
            Blend = color;
            Pos = Container.Pos;
            Type = wt;
            Durability = -1;

            if (Type == WeaponType.mallet)
            {
                Durability = 10;
            }
            else if (Type == WeaponType.bowbomb)
            {
                Durability = 4;
            }
            
        }
    }
}
