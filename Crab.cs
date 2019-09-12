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
    public class Crab : Enemy
    {
        public Crab(Tile loc, Game1 source, Color color) : base(loc,color)
        {
            Random random = new Random(GetHashCode());
            Blend = Color.Orange;
            Value = 1;

            if(random.Next(0,2) == 0)
            {
                Direction = Dir.left;
            }
            else
            {
                Direction = Dir.right;
            }
            Name = "Crab";
        }

        public override void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            base.OnArenaUpdate(sender, e);
            if (!IsDisposed)
            {
                Tile tile = null;

                if (Direction == Dir.left)
                {
                    tile = sender.GetTile(Pos + new Vector2(-1, 0));
                }
                else
                {
                    tile = sender.GetTile(Pos + new Vector2(+1, 0));
                }


                if (tile != null)
                {
                    if (tile.Surface != Block.floor | tile.Occupant != null && !(tile.Occupant is Player))
                    {
                        //bounce
                        if (Direction == Dir.left) Direction = Dir.right;
                        else Direction = Dir.left;

                        if (Direction == Dir.left)
                        {
                            tile = sender.Tiles[(int)Pos.X - 1, (int)Pos.Y];
                        }
                        else
                        {
                            tile = sender.Tiles[(int)Pos.X + 1, (int)Pos.Y];
                        }
                    }

                    var player = tile.Occupant as Player;
                    bool hitshield = false;
                    if (player != null)
                    {
                        hitshield = player.EqWeapon.Type == WeaponType.shield && player.Direction == Opposite(Direction);
                    }

                    if (tile.Surface == Block.floor && tile.Occupant == null & !Pushed)
                    {
                        Container.Occupant = null;
                        Container = tile;
                        Container.Occupant = this;
                    }
                    else if (tile.Surface == Block.floor && tile.Occupant is Player && !hitshield)
                    {
                        tile.Occupant.Health --;
                    }
                }


            }
        }
    }
}