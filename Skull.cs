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
    public class Skull : Enemy
    {
        public Skull(Tile loc, Color color) : base(loc, color)
        {
            Distance = 10;
            Name = "Skull";
            Value = 2;
        }

        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);
            Move();
            FindPath(sender, e);
                    
            Distance = Vector2.Distance(Pos, sender.PlayerSpace.Pos);
        }
    }

    public class ExSkull : Skull
    {
        public ExSkull(Tile loc, Color color) : base(loc, color)
        {
            Distance = 10;
            Name = "Bomb Skull";
            Value = 1;
            SafeZone = new List<Tile>();
            Path = new Stack<Tile>();
        }
    }
    public class StoneSkull : Stonework
    {
        public StoneSkull(Tile loc, Color color) : base(loc, color)
        {
            Distance = 10;
            Name = "Stone Skull";
            Value = 5;
        }
        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);
            FindPath(sender, e);
            if (!IsDisposed && Path.Count > 0)
            {
                var nextmove = Path.Peek();

                Vector2 move = nextmove.Pos - Pos;

                if (nextmove.Occupant is Player)
                {
                    var target = sender.Tiles[(int)Pos.X + (int)move.X * 2, (int)Pos.Y + (int)move.Y * 2];

                    if (target == null)
                    {
                        nextmove.Occupant.Dispose();
                    }
                    else
                    {
                        if (target.Surface != Block.floor)
                        {
                            nextmove.Occupant.Dispose();
                        }
                        else if (target.Occupant != null)
                        {
                            nextmove.Occupant.Dispose();
                        }
                        else
                        {
                            var player = nextmove.Occupant;

                            player.Container.Occupant = null;
                            player.Container = target;
                            player.Container.Occupant = player;
                        }
                    }

                    Container.Occupant = null;
                    Container = nextmove;
                    Container.Occupant = this;

                    Path.Pop();
                }
                else if (nextmove.Occupant == null)
                {
                    Container.Occupant = null;
                    Container = nextmove;
                    Container.Occupant = this;
                    Path.Pop();
                }
            }

            

            Distance = Vector2.Distance(Pos, sender.PlayerSpace.Pos);
        }
    }
}
