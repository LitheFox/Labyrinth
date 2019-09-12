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
    public class Spinner : Enemy
    {
        public Spinner(Tile loc, Game1 source, Color color) : base(loc, color)
        {
            Distance = 10;
            Name = "Spinner";
            Value = 1;
        }

        public float MaskAngle { get; set; }

        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);
            FindPath(sender, e);

            if (!IsDisposed && Path.Count > 0)
            {
                var nextmove = Path.Peek();
                if (Vector2.Distance(nextmove.Pos, Pos) <= 1)
                {
                    Vector2 move = nextmove.Pos - Pos;

                    var rand = 0;

                    while (rand == 0)
                    {
                        rand = rnd.Next(-1, 2);
                    }

                    if (move.X == 1 && Direction != Dir.right)
                    {
                        if (Direction == Dir.left)
                        {
                            Direction += rand;
                        }
                        else
                        {
                            Direction = Dir.right;
                        }
                    }
                    else if (move.X == -1 && Direction != Dir.left)
                    {
                        if (Direction == Dir.right)
                        {
                            Direction += rand;
                        }
                        else
                        {
                            Direction = Dir.left;
                        }
                    }
                    else if (move.Y == 1 && Direction != Dir.down)
                    {
                        if (Direction == Dir.up)
                        {
                            Direction += rand;
                        }
                        else
                        {
                            Direction = Dir.down;
                        }
                    }
                    else if (move.Y == -1 && Direction != Dir.up)
                    {
                        if (Direction == Dir.down)
                        {
                            Direction += rand;
                        }
                        else
                        {
                            Direction = Dir.up;
                        }
                    }
                    else
                    {
                        var player = nextmove.Occupant as Player;

                        if (player != null && !(player.EqWeapon.Type == WeaponType.shield && player.Direction == Opposite(Direction)))
                        {
                            nextmove.Occupant.Health--;
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
                }
            }

            

            Distance = Vector2.Distance(Pos, sender.PlayerSpace.Pos);

        } 
    }
}
