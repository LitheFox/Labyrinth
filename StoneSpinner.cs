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
    public class StoneSpinner : Stonework
    {
        public StoneSpinner(Tile loc, Game1 source, Color color) : base(loc, color)
        {
            Distance = 10;
            Name = "Stone Spinner";
            Value = 5;
        }

        public float Distance { get; set; }
        public float MaskAngle { get; set; }

        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);

            FindPath(sender, e);

            if (!IsDisposed && Path.Count > 0)
            {//if there is a path

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
                        if (nextmove.Occupant is Player)
                        {
                            var target = sender.Tiles[(int)Pos.X + (int)move.X * 2, (int)Pos.Y + (int)move.Y * 2];

                            Piece player = nextmove.Occupant;

                            bool squash = false;

                            if (target == null)
                            {
                                nextmove.Occupant.Health--;
                                squash = true;
                            }
                            else
                            {
                                if (target.Surface != Block.floor)
                                {
                                    nextmove.Occupant.Health--;
                                    squash = true;
                                }
                                else if (target.Occupant != null)
                                {
                                    nextmove.Occupant.Health--;
                                    squash = true;
                                }
                                else
                                {
                                    player.Container.Occupant = null;
                                    player.Container = target;
                                    player.Container.Occupant = player;
                                }
                            }

                            if(!squash && nextmove.Occupant?.Health < 1)
                            {
                                
                            }
                            else
                            {
                                Container.Occupant = null;
                                Container = nextmove;
                                Container.Occupant = this;
                            }

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
