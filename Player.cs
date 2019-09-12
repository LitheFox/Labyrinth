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
    public class Player : Piece
    {
        public delegate void OnPlayerUpdate(Player sender, PlayerEventArgs e);
        public event OnPlayerUpdate PlayerUpdateEvent;
        private bool IsOutOfBounds { get; set; }
        public bool HasUpdated { get; set; }       
        private bool ValidInput { get; set; }
        public int Angle { get; set; }
        public int Gold { get; set; }
        public Skill EqSkill { get; set; }
        public Skill LastSkill { get; set; }
        public bool EnoughEnergy { get; set; }
        public int RookCost = 5;
        public int KnightCost = 2;
        public Weapon EqWeapon { get; set; }
        public bool StoneBreaker { get; set; }
        public bool ShieldPush { get; set; }
        public float BowDist { get; set; }
        public int GetAngle()
        {
            return (int)Direction;
        }

        public Player(Tile loc,Color color) : base(loc,color)
        {
            Blend = Color.HotPink;

            EqSkill = Skill.none;

            EqWeapon = new Weapon(Container, Color.White, WeaponType.sword);

            Health = 1;
        }

        public void OnKeyPress(Game1 sender, KeypressEventArgs e)
        {
            if (!IsDisposed & Health > 0)
            {
                HasUpdated = false;
                ValidInput = false;
                LastSkill = Skill.none;

                int moveX = 0, moveY = 0;
                //keyboard
                if (e.GetKeys.Contains(Keys.A) | e.Controller.DPad.Left == ButtonState.Pressed)
                {
                    Direction = Dir.left;
                    ValidInput = true;
                    moveX = -1;
                }
                else if (e.GetKeys.Contains(Keys.D) | e.Controller.DPad.Right == ButtonState.Pressed)
                {
                    Direction = Dir.right;
                    ValidInput = true;
                    moveX = 1;
                }
                else if (e.GetKeys.Contains(Keys.S) | e.Controller.DPad.Down == ButtonState.Pressed)
                {
                    Direction = Dir.down;
                    ValidInput = true;
                    moveY = 1;
                }
                else if (e.GetKeys.Contains(Keys.W) | e.Controller.DPad.Up == ButtonState.Pressed)
                {
                    Direction = Dir.up;
                    ValidInput = true;
                    moveY = -1;
                }

                var atkregion = new List<Tile>();

                StoneBreaker = false;
                ShieldPush = false;

                if (EqWeapon != null)
                {
                    if (EqWeapon.Type == WeaponType.sword)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                    }
                    else if (EqWeapon.Type == WeaponType.axe)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));

                        if (Direction == Dir.left | Direction == Dir.right)
                        {
                            atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY - 1)));
                            atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY + 1)));
                        }
                        else
                        {
                            atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX - 1, moveY)));
                            atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX + 1, moveY)));
                        }
                    }
                    else if (EqWeapon.Type == WeaponType.spear)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX * 2, moveY * 2)));
                    }
                    else if (EqWeapon.Type == WeaponType.bow | EqWeapon.Type == WeaponType.bowbomb)
                    {
                        List<Tile> tiles = null;

                        if (moveX == 1)
                        {
                            tiles = sender.TheArena.GetRegion(Pos, Pos + new Vector2(moveX * 5, 0));
                        }
                        else if (moveX == -1)
                        {
                            tiles = sender.TheArena.GetRegion(Pos + new Vector2(moveX * 5, 0), Pos);
                        }
                        else if (moveY == -1)
                        {
                            tiles = sender.TheArena.GetRegion(Pos + new Vector2(0, moveY*5), Pos);
                        }
                        else if (moveY == 1)
                        {
                            tiles = sender.TheArena.GetRegion(Pos, Pos + new Vector2(0, moveY * 5));
                        }
                        if (tiles != null)
                        {
                            float r = 6;
                            Tile targ = null;
                            foreach (Tile t in tiles)
                            {
                                if (t != null && Vector2.Distance(t.Pos, Pos) < r && ((t.Occupant != null && !(t.Occupant is Player)) | t.Surface == Block.wall))
                                {
                                    r = Vector2.Distance(t.Pos, Pos);
                                    targ = t;
                                }
                            }
                            BowDist = r;
                            atkregion.Add(targ);
                        }

                        if (EqWeapon.Type == WeaponType.bowbomb) StoneBreaker = true;
                    }
                    else if (EqWeapon.Type == WeaponType.shield)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                        ShieldPush = true;
                    }
                    else if (EqWeapon.Type == WeaponType.mallet)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                        StoneBreaker = true;
                    }
                    else if (EqWeapon.Type == WeaponType.paired)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                    }
                    else if (EqWeapon.Type == WeaponType.harpoon)
                    {
                        atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                    }
                }
                else
                {
                    atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(moveX, moveY)));
                }

                bool isattack = false;

                foreach(Tile t in atkregion)
                {
                    if(t != null && t.Occupant is Piece && !(t.Occupant is Stonework & !(StoneBreaker | ShieldPush )))
                    {
                        isattack = true;
                    }
                }

                IsOutOfBounds = false;

                if (Pos.X + moveX >= sender.TheArena.Tiles.GetLength(0))
                {
                    IsOutOfBounds = true;
                }
                else if (Pos.Y + moveY >= sender.TheArena.Tiles.GetLength(1))
                {
                    IsOutOfBounds = true;
                }
                else if (Pos.X + moveX < 0)
                {
                    IsOutOfBounds = true;
                }
                else if (Pos.Y + moveY < 0)
                {
                    IsOutOfBounds = true;
                }

                var tile = sender.TheArena.Tiles[(int)Pos.X + moveX, (int)Pos.Y + moveY];

                if (!IsOutOfBounds && tile != null && ValidInput)
                {
                    if (tile.Occupant == null && tile.Surface == Block.floor && !isattack)
                    {
                        if (tile.Contents is Coin)
                        {
                            Gold++;
                            tile.Contents = null;
                        }
                        else if(tile.Contents is Weapon)
                        {
                            var weapon = tile.Contents as Weapon;

                            Weapon temp = null;
                            temp = EqWeapon;
                            EqWeapon = weapon;
                            tile.Contents = temp;
                            //swap weapons
                        }

                        //standard movement
                        sender.TheArena.LastPlayer = Container;
                        Container.Occupant = null;
                        Container = tile;
                        Container.Occupant = this;
                        HasUpdated = true;
                    }
                    else if (tile.Surface == Block.door)
                    {
                        //breaking door
                        sender.RoomNumber++;
                        //if (sender.RoomNumber > 24) sender.RoomNumber = 24; //clamp

                        Path.Clear();
                        sender.TheArena.Generate(new Vector2(Pos.X + moveX, Pos.Y + moveY), sender.RoomNumber, Direction, true, sender);
                        Container = Container;
                        sender.TheArena.Deekstrah(sender.FindDoor(), this);
                        Path = sender.TheArena.ConstructPath(Container, out float cost);

                        Vector2 oldPos = Pos;

                        sender.Cam1.Pos = new Vector2(oldPos.X * sender.Size, oldPos.Y * sender.Size);

                        HasUpdated = true;
                        Mask = Pos + new Vector2(moveX, moveY);
                        sender.DoorsKicked++;
                        sender.ShakeAlpha = 1;
                    }
                    else if (isattack)
                    {
                        foreach (SpriteStrip a in sender.Anims) a.Playing = true; //fix anims
                        bool noweapon = false;
                        if (EqWeapon == null) noweapon = true;
                        if (EqWeapon?.Type == WeaponType.paired)
                        {
                            atkregion.Add(sender.TheArena.GetTile(Pos + new Vector2(-moveX, -moveY))); 
                        }
                        else if(EqWeapon?.Type == WeaponType.bow | EqWeapon?.Type == WeaponType.bowbomb)
                        {
                            sender.ArrowAlpha = 1;
                        }
                        foreach (Tile t in atkregion)
                        {
                            if (t == null || t.Occupant == null) continue;
                            if(t.Occupant is Piece && !(t.Occupant is Stonework & !StoneBreaker) && !ShieldPush && !noweapon)
                            {
                                if (EqWeapon?.Type == WeaponType.bowbomb && t.Occupant is Stonework)
                                {
                                    sender.TheArena.Entities.Add(new ExPot(t, new Color(Color.White, 0f)));
                                    EqWeapon.Durability -= 1;
                                    if(EqWeapon.Durability > 0)
                                        sender.DuraAlpha = 1;
                                    else
                                        EqWeapon.Type = WeaponType.bow;
                                }
                                t.Occupant.Health--;
                                if (t.Occupant is ExPot | t.Occupant is ExSkull)
                                {
                                    t.Occupant.SafeZone.Add(Container);
                                }
                                HasUpdated = true;
                                Mask = Pos + new Vector2(moveX, moveY);
                                if(t.Occupant is Stonework && EqWeapon?.Type == WeaponType.mallet)
                                {
                                    EqWeapon.Durability -= 1;
                                    if (StoneBreaker) sender.DuraAlpha = 1;
                                    if (EqWeapon.Durability < 1) EqWeapon = null;
                                }
                            }
                            else if(ShieldPush && t.Occupant is Piece)
                            {
                                t.Occupant.Pushed = true;
                                Tile target = sender.TheArena.GetTile(Pos + new Vector2(moveX * 2, moveY * 2));

                                if(target != null && target.Occupant == null && target.Surface == Block.floor)
                                {
                                    var enemy = t.Occupant;

                                    enemy.Container.Occupant = null;
                                    enemy.Container = target;
                                    enemy.Container.Occupant = enemy;

                                    Container.Occupant = null;
                                    Container = t;
                                    Container.Occupant = this;
                                    HasUpdated = true;
                                    Mask = Pos + new Vector2(moveX, moveY);
                                }
                                else
                                {
                                    t.Occupant.Health--;
                                }
                                if(t.Occupant.Health < 1)
                                {
                                    sender.TurnHistory.Last().Kills.Add(t.Occupant);
                                    t.Occupant.Dispose();
                                    Container.Occupant = null;
                                    Container = t;
                                    Container.Occupant = this;
                                    HasUpdated = true;
                                    Mask = Pos + new Vector2(moveX, moveY);
                                }
                            }
                            else if(noweapon && t.Occupant is Pot | t.Occupant is ExPot)
                            {
                                t.Occupant.Health--;
                                if (t.Occupant is ExPot | t.Occupant is ExSkull)
                                {
                                    t.Occupant.SafeZone.Add(Container);
                                }
                                HasUpdated = true;
                                Mask = Pos + new Vector2(moveX, moveY);
                            }
                            sender.ShakeAlpha = 1;
                        }
                    }
                }
                else if (e.Controller.IsButtonDown(Buttons.A) && e.oldController.IsButtonUp(Buttons.A) && sender.NextEnemy != null && sender.NextEnemy.Occupant != null && sender.NextMove != null)
                {
                    //standard movement
                    sender.NextEnemy.Occupant.Health--;
                    if(sender.NextEnemy.Occupant.Health < 1)
                    {
                        sender.TurnHistory.Last().Kills.Add(sender.NextEnemy.Occupant);
                        sender.NextEnemy.Occupant.Dispose();
                    }

                    sender.TheArena.LastPlayer = Container;
                    if(Container != null) Container.Occupant = null;
                    Container = sender.NextMove;
                    Container.Occupant = this;
                    HasUpdated = true;

                    if (Container.Contents != null)
                    {
                        Gold++;
                        Container.Contents = null;
                    }
                    LastSkill = EqSkill;

                    sender.Energy -= sender.skillObjects[(int)EqSkill].Cost;
                }
                else if(e.Controller.IsButtonDown(Buttons.B) && e.oldController.IsButtonUp(Buttons.B) && EqWeapon != null)
                {
                    Tile attack = null;

                    if(EqWeapon.Type == WeaponType.shield)
                    {
                        attack = sender.TheArena.GetTile(DirToVec(Direction) + Pos);

                        if(attack != null && attack.Occupant != null)
                        {
                            var enemy = attack.Occupant;

                            Tile moveto = sender.TheArena.GetTile(Pos + DirToVec(Direction) * 2);

                            if(moveto != null && moveto.Occupant == null && moveto.Surface == Block.floor)
                            {
                                moveto = sender.TheArena.GetTile(Pos + DirToVec(Direction) * 3);
                            }

                            if (moveto == null || moveto.Occupant != null || moveto.Surface != Block.floor)
                            {
                                enemy.Health--;
                            }

                            enemy.Pushed = true;
                            enemy.Container.Occupant = null;
                            enemy.Container = moveto;
                            moveto.Occupant = enemy;

                            if (enemy.Health < 1)
                            {
                                //sender.TurnHistory.Last().Kills.Add(enemy);
                            }

                            HasUpdated = true;
                            Mask = Pos + new Vector2(moveX, moveY);
                        }
                    }
                    else if(EqWeapon.Type == WeaponType.harpoon)
                    {
                        List<Tile> tiles = null;

                        if (Direction == Dir.right)
                        {
                            tiles = sender.TheArena.GetRegion(Pos, Pos + new Vector2(DirToVec(Direction).X * 5, 0));
                        }
                        else if (Direction == Dir.left)
                        {
                            tiles = sender.TheArena.GetRegion(Pos + new Vector2(DirToVec(Direction).X * 5, 0), Pos);
                        }
                        else if (Direction == Dir.up)
                        {
                            tiles = sender.TheArena.GetRegion(Pos + new Vector2(0, DirToVec(Direction).Y * 5), Pos);
                        }
                        else if (Direction == Dir.down)
                        {
                            tiles = sender.TheArena.GetRegion(Pos, Pos + new Vector2(0, DirToVec(Direction).Y * 5));
                        }
                        if (tiles != null)
                        {
                            Tile targ = null;

                            foreach(Tile t in tiles)
                            {
                                t.Distance = Vector2.Distance(t.Pos, Pos);
                            }

                            tiles.Sort(delegate (Tile a, Tile b)
                            {
                                return a.Distance.CompareTo(b.Distance);
                            });

                            foreach (var item in tiles)
                            {
                                Console.WriteLine(item.Distance);
                            }

                            foreach (Tile t in tiles)
                            {
                                if(t != null && t.Occupant != null && !(t.Occupant is Player) && !(t.Occupant is Stonework))
                                {
                                    t.Occupant.Health--;
                                }
                                if(t != null && t.Surface == Block.floor && !(t.Occupant is Stonework))
                                {
                                    targ = t;
                                }
                                if(t != null && t.Surface == Block.wall | t.Occupant is Stonework)
                                {
                                    break;
                                }
                                if(t == null)
                                {
                                    break;
                                }
                            }
                            BowDist = Vector2.Distance(Pos, targ.Pos);
                            sender.ArrowAlpha = 1;
                            targ.Contents = EqWeapon;
                            EqWeapon = null;
                            HasUpdated = true;
                            Mask = Pos + new Vector2(moveX, moveY);
                        }
                    }

                }
            }
            sender.TheArena.PlayerSpace = Container;
        }
    }
}