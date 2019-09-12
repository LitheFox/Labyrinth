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
    public class Tile
    {
        public Vector2 Pos
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
            }
        }
        private Vector2 _pos;
        public float Alpha { get; set; }
        
        public Block Surface
        {
            get { return _surface; }

            set
            {
                _surface = value;

                switch (value)
                {
                    case Block.door: Blend = Color.Gold; break;
                    case Block.wall: Blend = Color.DarkGray; break;
                    case Block.floor: Blend = Color.DimGray; break;
                }
            }
        }
        private Block _surface;
        public Tile(Block surface, Vector2 pos)
        {
            Surface = surface;
            Pos = pos;
            Age = 0;
            Cost = 1;
        }

        public int Age { get; set; }
        public Piece Occupant { get; set; }
        public Tile Previous { get; set; }
        public float Distance { get; set; }
        public Item Contents { get; set; }
        public Color Blend { get; set; }
        public Color SelectColor { get; set; }
        public bool Blast { get; set; }
        public float BlastAlpha { get; set; }
        public float Cost { get; set; }
    }
}