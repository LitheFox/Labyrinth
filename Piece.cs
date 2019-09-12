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
    public abstract class Piece : IDisposable
    {
        public Vector2 Pos { get; set; }    
        public int Health
        {
            get { return _health; }

            set
            {
                _health = value;
            }
        }
        private int _health;
        public Tile Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
                if (_container == null)
                {
                    Dispose();
                }
                else
                {
                    Pos = Container.Pos;
                }
                
            }
        }
        private Tile _container;
        public Dir Direction 
        {
            get { return _direction; }
            
            set
            {
                if ((int)value > 3) 
                    _direction = Dir.up;
                else if ((int)value < 0) 
                    _direction = Dir.left;
                else
                _direction = value;
            }
        }
        protected Dir _direction;
        public Stack<Tile> Path { get; set; }
        public bool IsDisposed { get; set; }
        public Color Blend { get; set; }
        public Vector2 Mask { get; set; }
        public float Alpha { get; set; }
        public int Value { get; set; }
        public bool IsStatic { get; set; }
        public float BobPos { get; set; }
        public static Dir Opposite(Dir dir)
        {
            if (dir == Dir.left) return Dir.right;
            if (dir == Dir.right) return Dir.left;
            if (dir == Dir.up) return Dir.down;
            if (dir == Dir.down) return Dir.up;

            return Dir.none;
        }
        protected static Vector2 MoveMod(Vector2 current, Vector2 next)
        {
            return next - current;
        }
        protected static Dir MoveDir(Vector2 current, Vector2 next)
        {
            var mod = next - current;

            if(mod.X == -1)
            {
                return Dir.left;
            }
            else if (mod.X == 1)
            {
                return Dir.right;
            }
            else if (mod.Y == -1)
            {
                return Dir.up;
            }
            else if (mod.Y == 1)
            {
                return Dir.down;
            }

            return Dir.none;
        }
        public static Vector2 DirToVec(Dir dir)
        {
            if(dir == Dir.left)
            {
                return new Vector2(-1, 0);
            }
            if (dir == Dir.right)
            {
                return new Vector2(1, 0);
            }
            if (dir == Dir.up)
            {
                return new Vector2(0, -1);
            }
            if (dir == Dir.down)
            {
                return new Vector2(0, 1);
            }

            return Vector2.Zero;
        }

        public static float Range = 11f;
        protected static Random rnd = new Random();
        public string Name { get; set; }
        public bool Pushed { get; set; }
        public List<Tile> SafeZone { get; set; }

        public Piece(Tile loc, Color color)  
        {
            Blend = color;
            loc.Occupant = this;
            Container = loc;
            Path = new Stack<Tile>();
            Health = 1;
            IsDisposed = false;
            Mask = Pos;
            Direction = (Dir)rnd.Next(0, 4);
            Name = "";
            BobPos = (float)rnd.NextDouble() * 100;
        }

        public virtual void Dispose()
        {
            if(_container != null)
            {
                _container.Occupant = null;
                _container = null;
            }

            Path = null;
            IsDisposed = true;
        }
    }

    public abstract class Enemy : Piece
    {
        public Enemy(Tile loc, Color color) : base(loc, color)
        {

        }
        public float Distance { get; set; }
        public float PathCost { get; set; }
        public Tile LastMove { get; set; }
        public virtual void OnArenaUpdate(Arena sender, ArenaEventArgs e)
        {
            if (Container?.Surface != Block.floor) Dispose();
        }
        public virtual void FindPath(Arena sender, ArenaEventArgs e)
        {
            if(!IsDisposed && Distance < Range)
            {
                sender.Pathfind(sender.PlayerSpace, this);
                var newpath = sender.ConstructPath(Container, out float newcost);
                Path = newpath;
            }
        }
        public virtual void FindPathold(Arena sender, ArenaEventArgs e)
        {
            if (!IsDisposed && Distance < Range)
            {
                if (Distance <= Vector2.Distance(Pos, sender.PlayerSpace.Pos))
                {
                    sender.Deekstrah(sender.LastPlayer, this);
                    //if distance to player is less than last time move towards the players last pos
                }
                else
                {
                    sender.Deekstrah(sender.PlayerSpace, this);
                    //if the distance to the player is the same or more then go towards their current position
                }

                var newpath = sender.ConstructPath(Container, out float newcost);
                //find a new path

                if (newcost < PathCost)
                {
                    Path = newpath;
                    PathCost = newcost;
                }
                if (Path.Count > 0 && Path.Peek().Occupant != null && !(Path.Peek().Occupant is Player))
                {
                    Path = newpath;
                    PathCost = newcost;
                }
                if (newpath.Count < Path.Count)
                {
                    Path = newpath;
                    //if the new path is shorter than or equivalent to the last one it is prefered
                }
                else if (Path.Count == 0)
                {
                    Path = newpath;
                    //if the path is empty set it to the new path
                }
            }
        }
        public virtual void Move()
        {
            if(!IsDisposed && Path.Count > 0)
            {
                var nextmove = Path.Peek();

                if (Vector2.Distance(nextmove.Pos, Pos) <= 1)
                {
                    Direction = MoveDir(Pos, nextmove.Pos);

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
        public virtual void Moveold()
        {
            if (Path.Count > 0)
            {//if there is a path
                var nextmove = Path.Peek();

                if (Vector2.Distance(nextmove.Pos, Pos) < 2)
                {
                    Direction = MoveDir(Pos, nextmove.Pos);

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
                    else if (nextmove.Occupant != null)
                    {
                        Path.Clear();
                    }
                }
                else
                {
                    Path.Clear();
                }
            }
        }
    }

    public abstract class Stonework : Enemy
    {
        public Stonework(Tile loc, Color color) : base(loc, color)
        {

        }
    }
}