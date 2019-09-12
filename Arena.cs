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
    public class Arena
    {
        public Tile[,] Tiles { get; set; }
        private Random Rand { get; set; }
        
        public Tile PlayerSpace { get; set; }
        public Tile LastPlayer { get; set; }
        public List<Piece> Entities { get; set; }
        public bool EnemiesSpawned { get; set; }

        public Arena(Tile[,] tiles, Game1 source)
        {
            Entities = new List<Piece>();
            Tiles = tiles;
            Rand = new Random();

            for (int y = 1; y < 12; y++)
            {
                for (int x = 1; x < 12; x++)
                {
                    Tiles[x, y] = new Tile(Block.floor, new Vector2(x, y)) { Age = 1 };
                }
            }

            Tiles[5, 12] = new Tile(Block.door, new Vector2(5,10));

            AddWalls();

            Entities.Add(new ExPot(tiles[11, 11],  Color.SaddleBrown));
            Entities.Add(new ExPot(tiles[11, 10], Color.SaddleBrown));

            Entities.Add(new Pot(tiles[1, 11], Color.SaddleBrown));
            Entities.Add(new Pot(tiles[2, 11], Color.SaddleBrown));

            Tiles[2, 3].Contents = new Weapon(Tiles[2, 3], Color.White, WeaponType.axe);
            Tiles[3, 3].Contents = new Weapon(Tiles[3, 3], Color.White, WeaponType.paired);
            Tiles[4, 3].Contents = new Weapon(Tiles[4, 3], Color.White, WeaponType.spear);
            Tiles[5, 3].Contents = new Weapon(Tiles[5, 3], Color.White, WeaponType.harpoon);
            Tiles[6, 3].Contents = new Weapon(Tiles[6, 3], Color.White, WeaponType.mallet);
            Tiles[7, 3].Contents = new Weapon(Tiles[7, 3], Color.White, WeaponType.bow);
            Tiles[8, 3].Contents = new Weapon(Tiles[8, 3], Color.White, WeaponType.bowbomb);

        }

        public Stack<Tile> ConstructPath(Tile target, out float cost)
        {
            Stack<Tile> S = new Stack<Tile>();
            cost = 0;

            if (target.Previous != null)
            {
                while (target != null)
                {
                    cost += target.Distance;
                    S.Push(target);
                    target = target.Previous;
                }
            }
            
            var chegg = new Stack<Tile>();
            
            if (S.Count != 0)
            {
                //invert the path because .net is bad
                while (S.Count != 0)
                {
                    chegg.Push(S.Pop());
                }

                chegg.Pop();
            }

            return chegg;
        }

        public int Deekstrah(Tile start, Piece source)
        {// this function works out the shortest path from all Tiles to the start Tile
            if (start == null) return -1;

            List<Tile> Unvisited = new List<Tile>();
            //create empty list of unvisited Tiles
            foreach (Tile tile in Tiles)
            {
                if(tile != null && Vector2.Distance(tile.Pos, PlayerSpace.Pos) <= Piece.Range)
                {
                    tile.Distance = 10000; //set the distance to unknown
                    tile.Previous = null; //set the previous Tile to unknown
                    Unvisited.Add(tile); //add each Tile to the unvisited list
                }
            }

            start.Distance = 0; //set the starting Tile's disctance from itself to 0

            while (Unvisited.Count != 0)
            {//while there are still unvisited Tiles
                double Dist = 10000; //distance is unknown
                Tile u = null; //u = vertex in unvisited with the least distance from the start Tile

                foreach (Tile Tile in Unvisited)
                {
                    if (Tile.Distance < Dist)
                    {
                        u = Tile;
                        Dist = Tile.Distance;
                    }
                }//this loop finds the vertex closest to the start Tile

                Unvisited.Remove(u); //remove the visited Tile from the list of unvisited Tiles
                if (u == null) break;
                else if (u.Occupant == source)
                {
                    //Unvisited.Clear();
                }

                if (GetNeighbours(u) != null)
                {
                    foreach (Tile Tile in GetNeighbours(u))
                    {//for each of the neighboring Tiles of the current Tile
                        if (Unvisited.Contains(Tile)) //if the Tile has not been visited
                        {
                            if (Tile.Surface != Block.floor) continue;
                            //if (Tile.Occupant is Pot | Tile.Occupant is ExPot) continue;
                            
                            float alt = u.Distance + 1; //alternate possible path is the current Tile's 

                            if (Tile.Occupant != null && Tile.Occupant != source)
                            {
                                alt += 100;
                            }

                            if (alt < Tile.Distance)  //if this new distance is less than the existing one then we have found a shorter path
                            {
                                Tile.Distance = alt; //update the Tile with the new distance
                                Tile.Previous = u; //update the Tile with the new previous Tile

                                if (u.Occupant != null && u.Occupant == source) Unvisited.Clear();
                            }
                        }
                    }
                }
            }

            return 0;
        }
        public int Pathfind(Tile start, Piece source)
        {// this function works out the shortest path from all Tiles to the start Tile
            if (start == null) return -1;

            List<Tile> Unvisited = new List<Tile>();
            //create empty list of unvisited Tiles
            foreach (Tile tile in Tiles)
            {
                if (tile != null && Vector2.Distance(tile.Pos, PlayerSpace.Pos) <= Piece.Range)
                {
                    tile.Distance = 10000; //set the distance to unknown
                    tile.Previous = null; //set the previous Tile to unknown
                    tile.Cost = 1; //reset tile's cost
                    Unvisited.Add(tile); //add each Tile to the unvisited list
                }
            }

            start.Distance = 0; //set the starting Tile's disctance from itself to 0
            //find preferred tile
            if(PlayerSpace.Occupant is Player)
            {
                var revplayerdir = Piece.Opposite(PlayerSpace.Occupant.Direction);
                Tile pref = GetTile(Piece.DirToVec(revplayerdir) + PlayerSpace.Pos);
                if (pref != null) pref.Cost = 0;
            }
           
            while (Unvisited.Count != 0)
            {//while there are still unvisited Tiles
                double Dist = 10000; //distance is unknown
                Tile u = null; //u = vertex in unvisited with the least distance from the start Tile

                foreach (Tile Tile in Unvisited)
                {
                    if (Tile.Distance < Dist)
                    {
                        u = Tile;
                        Dist = Tile.Distance;
                    }
                }//this loop finds the vertex closest to the start Tile

                Unvisited.Remove(u); //remove the visited Tile from the list of unvisited Tiles
                if (u == null) break;
                else if (u.Occupant == source)
                {
                    //Unvisited.Clear();
                }

                if (GetNeighbours(u) != null)
                {
                    foreach (Tile Tile in GetNeighbours(u))
                    {//for each of the neighboring Tiles of the current Tile
                        if (Unvisited.Contains(Tile)) //if the Tile has not been visited
                        {
                            if (Tile.Surface != Block.floor) continue;
                            //if (Tile.Occupant is Pot | Tile.Occupant is ExPot) continue;

                            float alt = u.Distance + Tile.Cost; //alternate possible path is the current Tile's 

                            if (Tile.Occupant != null && Tile.Occupant != source && Tile.Occupant != PlayerSpace?.Occupant && Tile.Occupant.IsStatic == true)
                            {
                                alt += 100;
                            }
                            else if (Tile.Occupant != null && Tile.Occupant != source && Tile.Occupant != PlayerSpace?.Occupant)
                            {
                                alt += 3;
                            }

                            if (alt < Tile.Distance)  //if this new distance is less than the existing one then we have found a shorter path
                            {
                                Tile.Distance = alt; //update the Tile with the new distance
                                Tile.Previous = u; //update the Tile with the new previous Tile

                                //if (u.Occupant != null && u.Occupant == source) Unvisited.Clear();
                            }
                        }
                    }
                }
            }

            return 0;
        }
        public void NoEntSearch(Tile start, Piece source)
        {// this function works out the shortest path from all Tiles to the start Tile
            List<Tile> Unvisited = new List<Tile>();
            //create empty list of unvisited Tiles
            foreach (Tile tile in Tiles)
            {
                if (tile != null)
                {
                    tile.Distance = 200; //set the distance to unknown
                    tile.Previous = null; //set the previous Tile to unknown
                    Unvisited.Add(tile); //add each Tile to the unvisited list
                }
            }

            start.Distance = 0; //set the starting Tile's disctance from itself to 0

            while (Unvisited.Count != 0)
            {//while there are still unvisited Tiles
                double Dist = 999; //distance is unknown
                Tile u = null; //u = vertex in unvisited with the least distance from the start Tile

                foreach (Tile Tile in Unvisited)
                {
                    if (Tile.Distance < Dist)
                    {
                        u = Tile;
                        Dist = Tile.Distance;
                    }
                }//this loop finds the vertex closest to the start Tile

                Unvisited.Remove(u); //remove the visited Tile from the list of unvisited Tiles
                if (u == null) break;
                else if (u.Occupant == source)
                {
                    //Unvisited.Clear();
                }

                if (GetNeighbours(u) != null)
                {
                    foreach (Tile Tile in GetNeighbours(u))
                    {//for each of the neighboring Tiles of the current Tile
                        if (Unvisited.Contains(Tile)) //if the Tile has not been visited
                        {
                            if (Tile.Surface == Block.wall) continue;

                            float alt = u.Distance + 1; //alternate possible path is the current Tile's 
                            if (alt < Tile.Distance)  //if this new distance is less than the existing one then we have found a shorter path
                            {
                                Tile.Distance = alt; //update the Tile with the new distance
                                Tile.Previous = u; //update the Tile with the new previous Tile

                                if (u.Occupant != null && u.Occupant == source) Unvisited.Clear();
                            }
                        }
                    }
                }
            }
        }

        public List<Tile> GetNeighbours(Tile tile)
        {
            List<Tile> Neighbours = new List<Tile>();

            if (tile.Pos.X != 0) 
                Neighbours.Add(Tiles[(int)tile.Pos.X - 1, (int)tile.Pos.Y]);

            if (tile.Pos.X != Tiles.GetLength(0) - 1) 
                Neighbours.Add(Tiles[(int)tile.Pos.X + 1, (int)tile.Pos.Y]);

            if (tile.Pos.Y != 0) 
                Neighbours.Add(Tiles[(int)tile.Pos.X, (int)tile.Pos.Y - 1]);

            if (tile.Pos.Y != Tiles.GetLength(1) - 1) 
                Neighbours.Add(Tiles[(int)tile.Pos.X, (int)tile.Pos.Y + 1]);

            return Neighbours;
        }

        public void Generate(Vector2 start, int width, Dir opendir, bool ShouldAge, Game1 source)
        {
            Tiles[(int)start.X, (int)start.Y].Surface = Block.floor;

            width = width / 2;
            Vector2 InitialSize = new Vector2(Tiles.GetLength(0), Tiles.GetLength(1));
            int moves = width * width;
            var temp = Tiles;
            int minX = 0, maxX = 0, minY = 0, maxY = 0;
            Vector2 bot = start;
            int offset = width / 10;

            Tiles = new Tile[temp.GetLength(0) + moves, temp.GetLength(1) + moves];

            for (int y = 0; y < temp.GetLength(1); y++)
            {
                for (int x = 0; x < temp.GetLength(0); x++)
                {
                    if(temp[x, y] != null) temp[x, y].Age += 1;

                    if (temp[x, y] != null && temp[x, y].Age > 2)
                    {
                        if (temp[x, y].Occupant != null)
                        {
                            Entities.Remove(temp[x, y].Occupant);
                            temp[x, y].Occupant.Dispose();
                        }
                        temp[x, y] = null;
                    }

                    Tiles[x + moves/2, y + moves/2] = temp[x, y];
                }
            }

            minX = 0;
            maxX = Tiles.GetLength(0);
            minY = 0;
            maxY = Tiles.GetLength(1);
            bot += new Vector2(moves / 2, moves / 2);
            Vector2 origin = bot;
            
            if(opendir == Dir.right)
            {
                Tiles[(int)origin.X + 1, (int)origin.Y] = new Tile(Block.floor, origin + new Vector2(1, 0));
                Tiles[(int)origin.X + 2, (int)origin.Y] = new Tile(Block.floor, origin + new Vector2(2, 0));
            }
            if (opendir == Dir.left)
            {
                Tiles[(int)origin.X - 1, (int)origin.Y] = new Tile(Block.floor, origin + new Vector2(-1, 0));
                Tiles[(int)origin.X - 2, (int)origin.Y] = new Tile(Block.floor, origin + new Vector2(-2, 0));
            }
            if (opendir == Dir.up)
            {
                Tiles[(int)origin.X, (int)origin.Y-1] = new Tile(Block.floor, origin + new Vector2(0, -1));
                Tiles[(int)origin.X, (int)origin.Y - 2] = new Tile(Block.floor, origin + new Vector2(0, -2));
            }
            if (opendir == Dir.down)
            {
                Tiles[(int)origin.X, (int)origin.Y + 1] = new Tile(Block.floor, origin + new Vector2(0, 1));
                Tiles[(int)origin.X, (int)origin.Y + 2] = new Tile(Block.floor, origin + new Vector2(0, 2));
            }

            /*
            try
            {
                Tiles[(int)bot.X, (int)bot.Y].Surface = Block.floor;
                Tiles[(int)bot.X - 1, (int)bot.Y].Surface = Block.floor;
                Tiles[(int)bot.X + 1, (int)bot.Y].Surface = Block.floor;
                Tiles[(int)bot.X, (int)bot.Y + 1].Surface = Block.floor;
                Tiles[(int)bot.X, (int)bot.Y - 1].Surface = Block.floor;
            }
            catch (Exception)
            {   }
            */

            var startingmoves = moves;

            while (moves != 0)
            {
                Vector2 botold = bot;

                outofbounds:

                var botdir = (Dir)Rand.Next(0, 4);

                switch (botdir)
                {
                    case Dir.up: bot += new Vector2(0, -1); break;
                    case Dir.right: bot += new Vector2(1, 0); break;
                    case Dir.down: bot += new Vector2(0, 1); break;
                    case Dir.left: bot += new Vector2(-1, 0); break;
                }

                if(bot.Y <= origin.Y && opendir == Dir.down)
                {
                    bot = botold;
                    goto outofbounds;
                }
                if (bot.Y >= origin.Y && opendir == Dir.up)
                {
                    bot = botold;
                    goto outofbounds;
                }
                if (bot.X <= origin.X && opendir == Dir.right)
                {
                    bot = botold;
                    goto outofbounds;
                }
                if (bot.X >= origin.X && opendir == Dir.left)
                {
                    bot = botold;
                    goto outofbounds;
                }

                if(bot.X < 0 | bot.X > Tiles.GetLength(0)-1 | bot.Y < 0 | bot.Y > Tiles.GetLength(1) - 1)
                {
                    bot = botold;
                    goto outofbounds;
                }

                if (Tiles[(int)bot.X, (int)bot.Y] != null && Tiles[(int)bot.X, (int)bot.Y].Age == 0)
                {
                    Tiles[(int)bot.X, (int)bot.Y] = new Tile(Block.floor, bot);                    
                }
                else
                {
                    Tiles[(int)bot.X, (int)bot.Y] = new Tile(Block.floor, bot);
                    moves--;
                }

                if (bot.X < Tiles.GetLength(0) - 2)
                {
                    if (Tiles[(int)bot.X + 1, (int)bot.Y] == null)
                    {
                        Tiles[(int)bot.X + 1, (int)bot.Y] = new Tile(Block.floor, bot + new Vector2(1, 0));
                    }
                }
                if (bot.Y < Tiles.GetLength(1) - 2)
                {
                    if (Tiles[(int)bot.X, (int)bot.Y + 1] == null)
                    {
                        Tiles[(int)bot.X, (int)bot.Y + 1] = new Tile(Block.floor, bot + new Vector2(0, 1));
                    }
                }
                if (bot.Y < Tiles.GetLength(1) - 2 && bot.X < Tiles.GetLength(0) - 2)
                {
                    if (Tiles[(int)bot.X + 1, (int)bot.Y + 1] == null)
                    {
                        Tiles[(int)bot.X + 1, (int)bot.Y + 1] = new Tile(Block.floor, bot + new Vector2(1, 1));
                    }
                }

            }

            AddWalls();

            float current = 0f;
            Tile door = null;

            for (int y = 1; y < Tiles.GetLength(1)-1; y++)
            {
                for (int x = 1; x < Tiles.GetLength(0)-1; x++)
                {
                    if(Tiles[x,y] != null)
                    {
                        var n = GetNeighbours(Tiles[x, y]);

                        bool goodspace = false;

                        foreach(Tile t in n)
                        {
                            if(t != null && t.Surface == Block.floor)
                            {
                                goodspace = true;
                            }
                        }

                        if (goodspace && Tiles[x,y].Surface == Block.wall && Tiles[x, y].Age == 0)
                        {
                            if (Vector2.Distance(Tiles[x, y].Pos, origin) >= current)
                            {
                                current = Vector2.Distance(Tiles[x, y].Pos, origin);
                                door = Tiles[x, y];
                            }
                        }
                    }
                }
            }

            door.Surface = Block.door;
            //door done

            int chopX = -1, chopY = -1, chopX2 = -1, chopY2 = -1;

            for (int y = Tiles.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = Tiles.GetLength(0) - 1; x >= 0; x--)
                {
                    if (Tiles[x, y] != null) {chopY = y+1; break; }
                }
                if (chopY != -1) break;//goog
            }

            for (int x = Tiles.GetLength(0) - 1; x >= 0; x--)
            {
                for (int y = Tiles.GetLength(1) - 1; y >= 0; y--)
                {
                    if (Tiles[x, y] != null) { chopX = x+1; break; }
                }
                if (chopX != -1) break;//goog
            }

            for (int x = 0; x < Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < Tiles.GetLength(1); y++)
                {
                    if (Tiles[x, y] != null) { chopX2 = x; break; }
                }
                if (chopX2 != -1) break;//goog
            }

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Tiles[x, y] != null) { chopY2 = y; break; }
                }
                if (chopY2 != -1) break;
            }
            
            temp = Tiles;

            Tiles = new Tile[chopX - chopX2 , chopY - chopY2];

            Vector2 NewSize = new Vector2(Tiles.GetLength(0), Tiles.GetLength(1));

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    Tiles[x, y] = temp[x + chopX2, y + chopY2];
                    if(Tiles[x,y] != null)
                    {
                        Tiles[x, y].Pos = new Vector2(x, y);
                        if (Tiles[x, y].Occupant != null) Tiles[x, y].Occupant.Pos = Tiles[x, y].Pos;
                        //update coordinates and remove null space
                    }
                }
            }

            //add walls in room
            var player = source.ThePlayer;
            NoEntSearch(player.Container, null);
            var path = ConstructPath(door, out float cost);

            List<Tile> walls = new List<Tile>();

            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Rand.Next(0, 16) == 0)
                    {
                        var t = Tiles[x, y];

                        if (t == null || t.Age > 0) continue;

                        bot = new Vector2(x, y);

                        int length = Rand.Next(1, 9);

                        for (int i = 0; i < length; i++)
                        {
                            if (Tiles[(int)bot.X, (int)bot.Y] != null && Tiles[(int)bot.X, (int)bot.Y].Surface != Block.door && Tiles[(int)bot.X, (int)bot.Y].Age == 0)
                            {
                                Tiles[(int)bot.X, (int)bot.Y].Surface = Block.wall;
                                walls.Add(Tiles[(int)bot.X, (int)bot.Y]);
                            }

                            var botdir = (Dir)Rand.Next(0, 4);

                            var oldbot = bot;

                            switch (botdir)
                            {
                                case Dir.up: bot += new Vector2(0, -1); break;
                                case Dir.right: bot += new Vector2(1, 0); break;
                                case Dir.down: bot += new Vector2(0, 1); break;
                                case Dir.left: bot += new Vector2(-1, 0); break;
                            }

                            if (bot.X < 0 | bot.X > Tiles.GetLength(0) - 1 | bot.Y < 0 | bot.Y > Tiles.GetLength(1) - 1)
                            {
                                bot = oldbot;
                            }
                        }
                    }
                }
            }

            foreach(Tile t in path)
            {
                if (t == null) continue;

                if(t.Surface == Block.wall)
                {
                    t.Surface = Block.floor;
                }
            }

            //add walls in the room

            SpawnEnemies(source);

            foreach(Piece e in Entities)
            {
                e.Mask = e.Pos;
            }
        }

        void PlaceWall(int x, int y, Tile door, Game1 source)
        {
            
        }

        public void SpawnEnemies(Game1 source)
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Tiles[x,y] != null && Tiles[x,y].Age == 0 && Tiles[x, y].Surface == Block.floor && Tiles[x, y].Occupant == null && Vector2.Distance(new Vector2(x,y),PlayerSpace.Pos) > Piece.Range/2)
                    {
                        if (Rand.Next(0, 40) == 0)
                        {
                            Entities.Add(new Slime(Tiles[x, y], source, Color.LawnGreen));
                            continue;
                        }
                        if (Rand.Next(0, 80) == 0)
                        {
                            Entities.Add(new Pot(Tiles[x, y], Color.SaddleBrown));
                            continue;
                        }
                        if (Rand.Next(0, 75) == 0)
                        {
                            Entities.Add(new Skull(Tiles[x, y],  Color.BlanchedAlmond));
                            continue;
                        }
                        if (Rand.Next(0, 60) == 0)
                        {
                            Entities.Add(new Crab(Tiles[x, y], source, Color.Orange));
                            continue;
                        }
                        if (Rand.Next(0, 70) == 0)
                        {
                            Entities.Add(new Spinner(Tiles[x, y], source, Color.Yellow));
                            continue;
                        }
                        if (Rand.Next(0, 40) == 0)
                        {
                            Entities.Add(new ExPot(Tiles[x, y], Color.SaddleBrown));
                            continue;
                        }
                        if (Rand.Next(0, 290) == 0)
                        {
                            Entities.Add(new StoneSpinner(Tiles[x, y], source, Color.Violet));
                            continue;
                        }
                        if (Rand.Next(0, 300) == 0)
                        {
                            Entities.Add(new StoneSkull(Tiles[x, y],  Color.Violet));
                            continue;
                        }
                        if (Rand.Next(0, 100) == 0)
                        {
                            Entities.Add(new ExSkull(Tiles[x, y], Color.Crimson));
                            continue;
                        }
                        if (Rand.Next(0, 110) == 0)
                        {
                            Entities.Add(new Chest(Tiles[x, y], Color.White));
                            continue;
                        }
                    }
                }
            }

            EnemiesSpawned = true;
        }

        private void AddWalls()
        {
            for (int y = 0; y < Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < Tiles.GetLength(0); x++)
                {
                    if (Tiles[x, y] != null && Tiles[x, y].Surface == Block.floor)
                    {
                        if(x == 0)
                        {
                            Tiles[x, y].Surface = Block.wall;
                        }
                        if(x == Tiles.GetLength(0)-1)
                        {
                            Tiles[x, y].Surface = Block.wall;
                        }
                        if(y == 0)
                        {
                            Tiles[x, y].Surface = Block.wall;
                        }
                        if(y == Tiles.GetLength(1)-1)
                        {
                            Tiles[x, y].Surface = Block.wall;
                        }

                        if (x > 0 && Tiles[x - 1, y] == null)
                        {
                            Tiles[x - 1, y] = new Tile(Block.wall, new Vector2(x - 1, y)) { Age = Tiles[x, y].Age };
                        }
                        if (x < Tiles.GetLength(0)-1 && Tiles[x + 1, y] == null)
                        {
                            Tiles[x + 1, y] = new Tile(Block.wall, new Vector2(x + 1, y)) { Age = Tiles[x, y].Age };
                        }
                        if (y < Tiles.GetLength(1)-1 && Tiles[x, y + 1] == null)
                        {
                            Tiles[x, y + 1] = new Tile(Block.wall, new Vector2(x, y + 1)) { Age = Tiles[x, y].Age };
                        }
                        if (y > 0 && Tiles[x, y - 1] == null)
                        {
                            Tiles[x, y - 1] = new Tile(Block.wall, new Vector2(x, y - 1)) { Age = Tiles[x, y].Age };
                        }

                    }
                }
            }
        }

        public Tile GetTile(Vector2 pos)
        {
            if (pos.X < 0 | pos.X > Tiles.GetLength(0) - 1 | pos.Y < 0 | pos.Y > Tiles.GetLength(1) - 1)
            {
                return null; //out of bounds
            }

            var point = pos.ToPoint();

            return Tiles[point.X, point.Y];
        }
        public Tile GetTile(Point pos)
        {
            if (pos.X < 0 | pos.X > Tiles.GetLength(0) - 1 | pos.Y < 0 | pos.Y > Tiles.GetLength(1) - 1)
            {
                return null; //out of bounds
            }

            return Tiles[pos.X, pos.Y];
        }

        public List<Tile> GetRegion(Point topleft, Point botright)
        {
            List<Tile> reg = new List<Tile>();


            for (int y = topleft.Y; y <= botright.Y; y++)
            {
                for (int x = topleft.X; x <= botright.X; x++)
                {
                    if(x < 0 | x > Tiles.GetLength(0)-1 | y < 0 | y > Tiles.GetLength(1) - 1)
                    {
                        continue; //out of bounds
                    }

                    var tile = Tiles[x, y];

                    if (tile == null)
                        continue;

                    reg.Add(tile);
                }
            }

            return reg;
        }

        public List<Tile> GetRegion(Vector2 topleftv, Vector2 botrightv)
        {
            Point topleft = topleftv.ToPoint();
            Point botright = botrightv.ToPoint();

            List<Tile> reg = new List<Tile>();

            for (int y = topleft.Y; y <= botright.Y; y++)
            {
                for (int x = topleft.X; x <= botright.X; x++)
                {
                    if (x < 0 | x > Tiles.GetLength(0) - 1 | y < 0 | y > Tiles.GetLength(1) - 1)
                    {
                        continue; //out of bounds
                    }

                    var tile = Tiles[x, y];

                    if (tile == null)
                        continue;

                    reg.Add(tile);
                }
            }

            return reg;
        }
    }
}