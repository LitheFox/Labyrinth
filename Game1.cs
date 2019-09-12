using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Labyrinth_Redux
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics { get; set; }
        public SpriteBatch DrawBatch { get; set; }
        public SpriteBatch GUI { get; set; }
        private Texture2D Dot { get; set; }
        public Camera Cam1 { get; set; }
        private SpriteFont Calibri { get; set; }
        public Player ThePlayer { get; set; }
        public double Cooldown { get; set; }
        public int RoomNumber { get; set; }
        public int Size { get; set; }
        public Tile NextDoor { get; set; }
        private double KeyHeldTimer { get; set; }
        public Thread Pathfinding { get; set; }
        private bool TurnComplete { get; set; }
        public double LastElapsed { get; set; }
        public Stopwatch PathgenTimer { get; set; }
        public Stopwatch Lastpathtimer { get; set; }
        public double Lastpathelapsed { get; set; }
        public double EnemiesDone { get; set; }
        private MouseState NewMouse { get; set; }
        private const float PI = MathHelper.Pi;

        public Arena TheArena { get; set; }
        public delegate void OnKeyPressed(Game1 sender, KeypressEventArgs e);
        public event OnKeyPressed KeyPressEvent;
        public delegate void OnArenaUpdate(Arena sender, ArenaEventArgs e);
        public event OnArenaUpdate ArenaUpdateEvent;

        private SoundEffect[] Footsteps;
        public static Random rnd = new Random();
        Texture2D Arrow;
        Texture2D Explosion;
        Texture2D Box;
        Texture2D Spook;
        Texture2D Fox;
        Texture2D Mask;
        Texture2D Crab1;
        Texture2D Goop;
        Texture2D Sword;
        Texture2D Axe;
        Texture2D Paired;
        Texture2D Spear;
        Texture2D Harpoon;
        Texture2D Boomerang;
        Texture2D Shield;
        Texture2D Mallet;
        Texture2D Bow;
        Texture2D BowBomb;
        Texture2D SpookBomb;
        Texture2D SpookStone;
        Texture2D ShopKeeper;
        Texture2D ChestClose;
        Texture2D ChestMouth;
        Texture2D ArrowShot;
        public float ArrowAlpha; 

        public SpriteStrip[] Anims;

        List<Tile> TileTargets;
        List<Tile> EnemyTargets;
        public Tile NextMove;
        public Tile NextEnemy;
        public int Energy;

        Vector2 debugpos;

        GamePadState Controller;
        GamePadState oldController;
        public int TurnNumber = 0;
        public List<TurnInfo> TurnHistory;
        string ComboText;
        int ComboCount;
        int ComboIndex;
        float ComboLerp;
        float ComboAlpha;
        SpriteFont Consolas;
        MouseState oldmouse;
        float MenuAlpha;
        bool ComboExpire;
        Texture2D[] SkillIcons;
        Vector2[] skillSelect;
        int skillLength = Enum.GetNames(typeof(Skill)).Length;
        public Skills[] skillObjects;
        
        public int DoorsKicked;
        Texture2D ShieldTile;
        Vector2 ArrowPos;
        Dir ArrowDir;
        float ArrowLeng;
        public float DuraAlpha;
        public float ShakeAlpha;
        int HighestCombo;
        int Kills;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(10);

            Size = 25;
            Cam1 = new Camera(this);
            //RoomNumber = 15;
            RoomNumber = 14;
            Window.AllowUserResizing = true;
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1000;
            Graphics.ApplyChanges();

            Dot = new Texture2D(GraphicsDevice, 1, 1);
            Dot.SetData(new Color[1] { Color.White });

            TheArena = new Arena(new Tile[40, 40], this);
            
            ThePlayer = new Player(TheArena.Tiles[6, 6],Color.HotPink);

            TheArena.Entities.Add(ThePlayer);

            KeyPressEvent += ThePlayer.OnKeyPress;
            KeyPressEvent += OnKeyPress;

            TheArena.PlayerSpace = ThePlayer.Container;
            TheArena.LastPlayer = ThePlayer.Container;

            TurnComplete = true;
            //ThePlayer.PlayerUpdateEvent += OnPlayerUpdate;
            PathgenTimer = new Stopwatch();
            Lastpathtimer = new Stopwatch();

            //Mouse.SetPosition(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
            mouseorigin = Mouse.GetState();

            TileTargets = new List<Tile>();
            EnemyTargets = new List<Tile>();

            TurnHistory = new List<TurnInfo>();
            TurnHistory.Add(new TurnInfo());

            ComboText = "";
        }
        protected override void LoadContent()
        {
            DrawBatch = new SpriteBatch(GraphicsDevice);
            GUI = new SpriteBatch(GraphicsDevice);
            Calibri = Content.Load<SpriteFont>("Calibri");

            Footsteps = new SoundEffect[5];
            Footsteps[0] = Content.Load<SoundEffect>("run1");
            Footsteps[1] = Content.Load<SoundEffect>("run2");
            Footsteps[2] = Content.Load<SoundEffect>("run3");
            Footsteps[3] = Content.Load<SoundEffect>("run4");
            Footsteps[4] = Content.Load<SoundEffect>("run5");

            Arrow = Content.Load<Texture2D>("arrow3");
            Explosion = Content.Load<Texture2D>("exbox3");
            Spook = Content.Load<Texture2D>("skull3");
            Fox = Content.Load<Texture2D>("fox1");
            Mask = Content.Load<Texture2D>("mask1");
            Crab1 = Content.Load<Texture2D>("crab1");
            Goop = Content.Load<Texture2D>("slime1");
            Box = Content.Load<Texture2D>("box1");
            Consolas = Content.Load<SpriteFont>("consolas1");

            SkillIcons = new Texture2D[skillLength];

            SkillIcons[0] = Content.Load<Texture2D>("none1");
            SkillIcons[1] = Content.Load<Texture2D>("pawnup1");
            SkillIcons[2] = Content.Load<Texture2D>("rookup1");
            SkillIcons[3] = Content.Load<Texture2D>("horse1");
            
            skillSelect = new Vector2[skillLength];

            for (float i = 0; i < skillLength; i++)
            {
                var a = ((PI * 2f) / skillLength) * (i) - MathHelper.PiOver2;

                skillSelect[(int)i] = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
            }

            skillObjects = new Skills[skillLength];

            skillObjects[0] = new Skills(0);
            skillObjects[1] = new Skills(0);
            skillObjects[2] = new Skills(8);
            skillObjects[3] = new Skills(2);

            Axe = Content.Load<Texture2D>("axe1");
            Sword = Content.Load<Texture2D>("sword1");
            Shield = Content.Load<Texture2D>("shield1");
            Boomerang = Content.Load<Texture2D>("boomer1");
            Spear = Content.Load<Texture2D>("spear1");
            Harpoon = Content.Load<Texture2D>("harpoon1");
            Mallet = Content.Load<Texture2D>("mallet1");
            Paired = Content.Load<Texture2D>("paired1");
            Bow = Content.Load<Texture2D>("bow1");
            BowBomb = Content.Load<Texture2D>("bombbow");
            ArrowShot = Content.Load<Texture2D>("arrowshot1");
            SpookStone = Content.Load<Texture2D>("skullstone1");
            ShopKeeper = Content.Load<Texture2D>("shopkeep1");
            ChestClose = Content.Load<Texture2D>("chest1");
            ChestMouth = Content.Load<Texture2D>("chestmimic1");
            SpookBomb = Content.Load<Texture2D>("skullbomb2");

            /*
             SpookBomb;
        Texture2D SpookStone;
        Texture2D ShopKeeper;
        Texture2D ChestClose;
        Texture2D ChestMouth;
             * */

            Anims = new SpriteStrip[4]
            {
                new SpriteStrip(Content.Load<Texture2D>("axe_slash2"), new Point(75,75), 30),
                new SpriteStrip(Content.Load<Texture2D>("sword_slash3"), new Point(25, 50), 40),
                new SpriteStrip(Content.Load<Texture2D>("spear_slash"), new Point(25, 75), 40),
                new SpriteStrip(Content.Load<Texture2D>("hammerbonk"), new Point(25, 50), 30)
        };

            ShieldTile = Content.Load<Texture2D>("shieldtile"); 
        }
        

        public void OnKeyPress(Game1 sender, KeypressEventArgs e)
        {            
            if (Controller.ThumbSticks.Right.Length() > .05f)
            {
                MenuAlpha = 1f;

                var select = new Vector2(Controller.ThumbSticks.Right.X, -Controller.ThumbSticks.Right.Y);
                float dist = 20;

                for(var i = 0; i < skillLength; i++)
                {
                    var vec = skillSelect[i];

                    if (Vector2.Distance(vec, select) < dist)
                    {
                        dist = Vector2.Distance(vec, select);
                        ThePlayer.EqSkill = (Skill)i;
                    }
                }

                NextEnemy = null;
                NextMove = null;
                TileTargets.Clear();
                EnemyTargets.Clear();
            }
            if (e.Controller.Triggers.Left > .1f)
            {
                if(!ThePlayer.IsDisposed)
                {
                    if(ThePlayer.EqSkill == Skill.en_passant)
                    {
                        if (TileTargets.Count == 0)
                        {
                            var left = TheArena.GetTile(ThePlayer.Pos + new Vector2(-1, 0));
                            var right = TheArena.GetTile(ThePlayer.Pos + new Vector2(1, 0));

                            var up = TheArena.GetTile(ThePlayer.Pos + new Vector2(0, -1));
                            var down = TheArena.GetTile(ThePlayer.Pos + new Vector2(0, 1));

                            var next = left;
                            if(next != null && next.Occupant != null && next.Occupant is Enemy && !(next.Occupant is Stonework))
                            {
                                var a = TheArena.GetTile(ThePlayer.Pos + new Vector2(-1, -1));
                                var b = TheArena.GetTile(ThePlayer.Pos + new Vector2(-1, +1));

                                if((a != null && a.Occupant == null) | (b != null && b.Occupant == null))
                                {
                                    TileTargets.Add(a);
                                    TileTargets.Add(b);
                                    EnemyTargets.Add(TheArena.GetTile(ThePlayer.Pos + new Vector2(-1, 0)));
                                }
                            }
                            next = right;
                            if (next != null && next.Occupant != null && next.Occupant is Enemy && !(next.Occupant is Stonework))
                            {
                                var a = TheArena.GetTile(ThePlayer.Pos + new Vector2(+1, -1));
                                var b = TheArena.GetTile(ThePlayer.Pos + new Vector2(+1, +1));

                                if ((a != null && a.Occupant == null) | (b != null && b.Occupant == null))
                                {
                                    TileTargets.Add(a);
                                    TileTargets.Add(b);
                                    EnemyTargets.Add(TheArena.GetTile(ThePlayer.Pos + new Vector2(+1, 0)));
                                }
                            }
                            next = up;
                            if (next != null && next.Occupant != null && next.Occupant is Enemy && !(next.Occupant is Stonework))
                            {
                                var a = TheArena.GetTile(ThePlayer.Pos + new Vector2(-1, -1));
                                var b = TheArena.GetTile(ThePlayer.Pos + new Vector2(+1, -1));

                                if ((a != null && a.Occupant == null) | (b != null && b.Occupant == null))
                                {
                                    TileTargets.Add(a);
                                    TileTargets.Add(b);
                                    EnemyTargets.Add(TheArena.GetTile(ThePlayer.Pos + new Vector2(0, -1)));
                                }
                            }
                            next = down;
                            if (next != null && next.Occupant != null && next.Occupant is Enemy && !(next.Occupant is Stonework))
                            {
                                var a = TheArena.GetTile(ThePlayer.Pos + new Vector2(-1, +1));
                                var b = TheArena.GetTile(ThePlayer.Pos + new Vector2(+1, +1));

                                if ((a != null && a.Occupant == null) | (b != null && b.Occupant == null))
                                {
                                    TileTargets.Add(a);
                                    TileTargets.Add(b);
                                    EnemyTargets.Add(TheArena.GetTile(ThePlayer.Pos + new Vector2(0, +1)));
                                }
                            }

                            TileTargets = TileTargets.Distinct().ToList();

                            List<Tile> toremove = new List<Tile>();

                            foreach(Tile t in TileTargets)
                            { 
                                if(t.Surface != Block.floor | t.Occupant != null)
                                {
                                    toremove.Add(t);
                                }
                            }

                            foreach(Tile t in toremove)
                            {
                                TileTargets.Remove(t);
                            }
                        }
                        else
                        {
                            if(Controller.ThumbSticks.Left.Length() > .05f)
                            {
                                var select = ThePlayer.Pos + new Vector2( Controller.ThumbSticks.Left.X,-Controller.ThumbSticks.Left.Y);
                                float dist = 10;

                                debugpos = select;

                                foreach (Tile t in TileTargets)
                                {
                                    if (Vector2.Distance(t.Pos, select) < dist)
                                    {
                                        dist = Vector2.Distance(t.Pos, select);
                                        NextMove = t;
                                    }
                                }

                                dist = 10;

                                foreach (Tile t in EnemyTargets)
                                {
                                    if (Vector2.Distance(t.Pos, select) < dist)
                                    {
                                        dist = Vector2.Distance(t.Pos, select);
                                        NextEnemy = t;
                                    }
                                }
                            }
                            else
                            {
                                NextMove = null;
                                NextEnemy = null;
                            }
                        }
                    }
                    else if(ThePlayer.EqSkill == Skill.rook && Energy >= skillObjects[(int)Skill.rook].Cost)
                    {
                        if (TileTargets.Count == 0)
                        {
                            Tile left = null, right = null, up = null, down = null;

                            var range = 9;

                            for (int x = 1; x < range; x++)
                            {
                                var t = TheArena.GetTile(ThePlayer.Pos - new Vector2(x, 0));

                                if (t == null || t.Surface != Block.floor)
                                    break;

                                if(t.Occupant is Enemy && !(t.Occupant is Stonework))
                                {
                                    left = t;
                                    break;
                                }
                                else if(t.Occupant != null)
                                {
                                    break;
                                }
                            }
                            for (int x = 1; x < range; x++)
                            {
                                var t = TheArena.GetTile(ThePlayer.Pos + new Vector2(x, 0));

                                if (t == null || t.Surface != Block.floor)
                                    break;

                                if (t.Occupant is Enemy && !(t.Occupant is Stonework))
                                {
                                    right = t;
                                    break;
                                }
                                else if(t.Occupant != null)
                                {
                                    break;
                                }
                            }
                            for (int y = 1; y < range; y++)
                            {
                                var t = TheArena.GetTile(ThePlayer.Pos - new Vector2(0, y));

                                if (t == null || t.Surface != Block.floor)
                                    break;

                                if (t.Occupant is Enemy && !(t.Occupant is Stonework))
                                {
                                    up = t;
                                    break;
                                }
                                else if (t.Occupant != null)
                                {
                                    break;
                                }
                            }
                            for (int y = 1; y < range; y++)
                            {
                                var t = TheArena.GetTile(ThePlayer.Pos + new Vector2(0, y));

                                if (t == null || t.Surface != Block.floor)
                                    break;

                                if (t.Occupant is Enemy && !(t.Occupant is Stonework))
                                {
                                    down = t;
                                    break;
                                }
                                else if (t.Occupant != null)
                                {
                                    break;
                                }
                            }

                            TileTargets.Add(left);
                            TileTargets.Add(right);
                            TileTargets.Add(up);
                            TileTargets.Add(down);
                        }
                        else
                        {
                            if (Controller.ThumbSticks.Left.Length() > .05f)
                            {
                                var select = new Vector2(Controller.ThumbSticks.Left.X, -Controller.ThumbSticks.Left.Y);
                                float dist = 20;

                                foreach (Tile t in TileTargets)
                                {
                                    if (t == null) continue;

                                    var sx = Math.Sign(t.Pos.X - ThePlayer.Pos.X);
                                    var sy = Math.Sign(t.Pos.Y - ThePlayer.Pos.Y);

                                    var v = new Vector2(sx, sy);

                                    if (Vector2.Distance(v, select) < dist)
                                    {
                                        dist = Vector2.Distance(v, select);
                                        NextMove = t;
                                    }
                                }

                                NextEnemy = NextMove;
                            }
                            else
                            {
                                NextMove = null;
                                NextEnemy = null;
                            }
                        }
                    }
                    else if(ThePlayer.EqSkill == Skill.knight && Energy >= skillObjects[(int)Skill.knight].Cost)
                    {
                        if (TileTargets.Count == 0)
                        {
                            Tile l1 = null, l2 = null, r1 = null, r2 = null, u1 = null, u2 = null, d1 = null, d2 = null;

                            var p = ThePlayer.Pos;

                            l1 = TheArena.GetTile(p + new Vector2(-2, -1));
                            l2 = TheArena.GetTile(p + new Vector2(-2, +1));

                            r1 = TheArena.GetTile(p + new Vector2(+2, -1));
                            r2 = TheArena.GetTile(p + new Vector2(+2, +1));

                            u1 = TheArena.GetTile(p + new Vector2(-1, -2));
                            u2 = TheArena.GetTile(p + new Vector2(+1, -2));

                            d1 = TheArena.GetTile(p + new Vector2(-1, +2));
                            d2 = TheArena.GetTile(p + new Vector2(+1, +2));

                            var temp = new List<Tile>() { l1, l2, r1, r2, u1, u2, d1, d2 };

                            foreach(Tile t in temp)
                            {
                                if (t == null) continue;

                                if(t.Occupant is Enemy && !(t.Occupant is Stonework))
                                {
                                    TileTargets.Add(t);
                                }
                            }
                        }
                        else
                        {
                            if (Controller.ThumbSticks.Left.Length() > .05f)
                            {
                                var select = new Vector2(Controller.ThumbSticks.Left.X, -Controller.ThumbSticks.Left.Y);
                                float dist = 20;

                                foreach (Tile t in TileTargets)
                                {
                                    if (t == null) continue;

                                    var sx = Math.Sign(t.Pos.X - ThePlayer.Pos.X);
                                    var sy = Math.Sign(t.Pos.Y - ThePlayer.Pos.Y);

                                    var v = Vector2.Normalize(t.Pos - ThePlayer.Pos);

                                    if (Vector2.Distance(v, select) < dist)
                                    {
                                        dist = Vector2.Distance(v, select);
                                        NextMove = t;
                                    }
                                }

                                NextEnemy = NextMove;
                            }
                            else
                            {
                                NextMove = null;
                                NextEnemy = null;
                            }
                        }
                    }

                }
            }

        }

        public Tile FindDoor()
        {
            foreach(Tile x in TheArena.Tiles)
            {
                if (x != null && x.Surface == Block.door) return x;
            }

            return null;
        }

        public void OnPlayerUpdate() 
        {
            TurnNumber++;
            var currentturn = TurnHistory.Last();
            currentturn.UsedSkill = ThePlayer.LastSkill;

            if (ThePlayer.Health < 0) ThePlayer.Dispose();

            PathgenTimer.Restart();

            Cooldown = 20;

            EnemiesDone = 0;
             
            List<Piece> ToDispose = new List<Piece>();

            for (int i = 0; i < 2; i++)
            {
                foreach (Piece x in TheArena.Entities)
                {
                    Piece pot = null;

                    if(x is ExSkull)
                    {
                        pot = x as ExSkull;
                    }
                    if (x is ExPot)
                    {
                        pot = x as ExPot;
                    }
                    if (pot != null)
                    {
                        if (pot.Container != null)
                        {
                            if (!pot.IsDisposed && pot.Container.Blast)
                            {
                                pot.Health--;
                            }
                            if (pot.Health < 1 && !pot.IsDisposed)
                            {
                                currentturn.Kills.Add(pot);
                                pot.Dispose();
                            }
                            if (pot.IsDisposed)
                            {
                                ToDispose.Add(pot);

                                var blast = TheArena.GetRegion((pot.Pos - Vector2.One).ToPoint(), (pot.Pos + Vector2.One).ToPoint());

                                if (blast != null)
                                {
                                    foreach (Tile t in blast)
                                    {
                                        if (t != null && !pot.SafeZone.Contains(t))
                                        {
                                            t.Blast = true;
                                            t.BlastAlpha = 1f;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach(Piece x in TheArena.Entities)
            {
                if (!x.IsDisposed)
                {
                    if (x.Container.Blast)
                    {
                        x.Health--;
                        currentturn.PyroCount++;
                    }
                    if(x.Health < 1)
                    {
                        x.Dispose();
                        currentturn.Kills.Add(x);
                    }
                    if (x.IsDisposed)
                    {
                        ToDispose.Add(x);
                    }
                }
                else
                {
                    ToDispose.Add(x);
                }
            }

            foreach (Piece x in ToDispose)
            {
                Energy += x.Value;
                TheArena.Entities.Remove(x);
            }

            ToDispose.Clear();
            //make the stonework last in initative to allow pushing

            foreach (Piece x in TheArena.Entities)
            {
                if (x is Stonework)
                    continue;

                var e = x as Enemy;

                if (e == null)
                    continue;

                if (e.Pushed)
                {
                    e.Pushed = false;
                }
                else
                {
                    e.OnArenaUpdate(TheArena, new ArenaEventArgs());
                }
            }

            foreach (Piece x in TheArena.Entities)
            {
                var e = x as Stonework;

                if (e == null)
                    continue;

                if (e.Pushed)
                {
                    e.Pushed = false;
                }
                else
                {
                    e.OnArenaUpdate(TheArena, new ArenaEventArgs());
                }
            }

            if (ThePlayer.Container != null && ThePlayer.Container.Blast)
            {
                ThePlayer.Health--;
            }

            foreach(Tile t in TheArena.Tiles)
            {
                if (t == null) continue;

                if (t.Blast) t.Blast = false;
            }

            TileTargets.Clear();
            EnemyTargets.Clear();
            NextEnemy = null;
            NextMove = null;

            TurnComplete = true;

            PathgenTimer.Stop();

            LastElapsed = PathgenTimer.ElapsedMilliseconds;

            TheArena.EnemiesSpawned = false;

            if(ComboCount == 0 && TurnHistory.Last().Kills.Count > 0)
            {
                ComboIndex = TurnHistory.Count-1;
            }
            
            if (ComboExpire && TurnHistory.Last().Kills.Count == 0)
            {
                ComboCount = 0;
                ComboExpire = false;
            }
            else if (TurnHistory.Last().Kills.Count == 0)
            {
                ComboExpire = true;
            }
            else
            {
                ComboExpire = false;

                ComboCount += TurnHistory.Last().Kills.Count;

                bool sametype = true;
                bool sameskill = true;

                var starter = TurnHistory[ComboIndex].Kills[0];
                var firstskill = TurnHistory[ComboIndex].UsedSkill;

                for (int i = ComboIndex; i < TurnHistory.Count; i++)
                {
                    var t = TurnHistory[i];

                    if (t.Kills.Count == 0) continue;

                    foreach(Piece e in t.Kills)
                    {
                        if(e.GetType() != starter.GetType())
                        {
                            sametype = false;
                        }
                    }

                    if (t.UsedSkill != firstskill) sameskill = false;
                }

                string name = "", move = "";

                if (sametype)
                {
                    name = starter.Name;
                }
                if(sameskill && firstskill != Skill.none)
                {
                    move = firstskill.ToString();
                }

                if(ComboCount > 1)
                {
                    ComboText = $"{move} {name} Combo: {ComboCount}!";
                    Energy += ComboCount-1;
                    if (ComboCount > HighestCombo) HighestCombo = ComboCount;
                }
            }

            var k = TurnHistory.Last().Kills;

            foreach(var e in k)
            {
                if (e is Enemy) Kills += 1;
            }

            TurnHistory.Add(new TurnInfo());

            if (Energy > 10) Energy = 10;

            if (ThePlayer.IsDisposed)
            {
                //Thread.Sleep(1000);
               // Exit();
            }
        }

        private MouseState mousecurrent;
        private MouseState mouseorigin;
        private float xChange;
        private float yChange;

        protected override void Update(GameTime gameTime)
        {
            mousecurrent = Mouse.GetState();
            Controller = GamePad.GetState(0);

            bool movehold = false;
            bool dpadpressed = false;
            bool controllerpressed = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) | Controller.IsButtonDown(Buttons.Back))
            {
                Exit();
                Program.shouldrestart = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) | Controller.IsButtonDown(Buttons.Start))
            {
                Exit();
            }

            foreach (Keys k in Keyboard.GetState().GetPressedKeys())
            {
                if (k == Keys.A | k == Keys.S | k == Keys.D | k == Keys.W)
                    movehold = true;
            }

            if (Controller.DPad.Left == ButtonState.Pressed |
                Controller.DPad.Right == ButtonState.Pressed |
                Controller.DPad.Up == ButtonState.Pressed |
                Controller.DPad.Down == ButtonState.Pressed)
            {
                movehold = true;
                dpadpressed = true;
            }

            if (Controller.Triggers.Left > .1f | Controller.ThumbSticks.Right.Length() > .05f)
            {
                controllerpressed = true;
            }
            else
            {
                TileTargets.Clear();
                EnemyTargets.Clear();
                NextEnemy = null;
                NextMove = null;
            }

            if (Controller.IsButtonDown(Buttons.LeftShoulder) | Controller.IsButtonDown(Buttons.RightShoulder) | (Controller.IsButtonDown(Buttons.B)))
            {
                controllerpressed = true;
            }

            controllerpressed = true;

            if (movehold)
            {
                KeyHeldTimer += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                KeyHeldTimer = 0;
            }

            if ((movehold | controllerpressed) && TurnComplete && Cooldown <= 0 && !ThePlayer.IsDisposed)
            {
                KeyPressEvent?.Invoke(this, new KeypressEventArgs(Keyboard.GetState().GetPressedKeys(), Controller, oldController));

                if (ThePlayer.HasUpdated)
                {
                    TurnComplete = false;
                    ThreadStart child = new ThreadStart(OnPlayerUpdate);
                    Pathfinding = new Thread(child);
                    Pathfinding.Start();
                }
            }

            if (Cooldown > 0 && !movehold)
            {
                Cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (TurnComplete && KeyHeldTimer > 200 && !ThePlayer.IsDisposed)
            {
                KeyPressEvent?.Invoke(this, new KeypressEventArgs(Keyboard.GetState().GetPressedKeys(), Controller, oldController));

                if (ThePlayer.HasUpdated)
                {
                    TurnComplete = false;
                    ThreadStart child = new ThreadStart(OnPlayerUpdate);
                    Pathfinding = new Thread(child);
                    Pathfinding.Start();
                }
                KeyHeldTimer = 100;
            }

            Cam1.Pos = Vector2.Lerp(Cam1.Pos, new Vector2(ThePlayer.Pos.X * Size - 0, ThePlayer.Pos.Y * Size - 0), 0.1f);

            if (ShakeAlpha > 0)
            {
                Cam1.Pos = Vector2.Lerp(Cam1.Pos, Cam1.Pos + new Vector2(rnd.Next(-100, 100) * ShakeAlpha/2, rnd.Next(-100, 100) * ShakeAlpha/2), .08f);

                ShakeAlpha -= .05f;
            }

            if (TurnComplete)
            {
                for (int i = 0; i < TheArena.Entities.Count; i++)
                {
                    var e = TheArena.Entities[i];

                    if (e.IsStatic)
                        e.Mask = Vector2.Lerp(e.Mask, e.Pos, .3f);
                    else
                        e.Mask = Vector2.Lerp(e.Mask, e.Pos + new Vector2(0, (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / (400f + e.BobPos)) / 10f), .3f);

                    if (e is Slime)
                    {
                        var enemy = e as Slime;

                        if (enemy.IsCoolingDown)
                        {
                            e.Blend = Color.DarkGreen * .6f;
                        }
                        else
                        {
                            e.Blend = Color.LawnGreen * .6f;
                        }
                    }
                    if (e is Spinner)
                    {
                        var s = e as Spinner;

                        s.MaskAngle = LerpRads(s.MaskAngle, (int)e.Direction * MathHelper.PiOver2 - MathHelper.PiOver2, .3f);
                    }
                    if (e is StoneSpinner)
                    {
                        var s = e as StoneSpinner;

                        s.MaskAngle = LerpRads(s.MaskAngle, (int)e.Direction * MathHelper.PiOver2 - MathHelper.PiOver2, .3f);
                    }
                }
            }
            var mousetrans = Mouse.GetState().Position.ToVector2();
            mousetrans = Vector2.Transform(-mousetrans, Cam1._transform );
            mousetrans *= -1;
            mousetrans /= Size;

            //debug mouse placement
            var tile = TheArena.GetTile(mousetrans);
            if (tile != null && tile.Occupant == null && mousecurrent.LeftButton == ButtonState.Pressed)
            {
                TheArena.Entities.Add( new Dummy(tile, this, Color.Orange));
            }
            if (tile != null && tile.Occupant == null && mousecurrent.RightButton == ButtonState.Pressed)
            {
                TheArena.Entities.Add(new StoneSpinner(tile,this, Color.Purple));
            }
            if (tile != null && tile.Occupant == null && mousecurrent.XButton1 == ButtonState.Pressed)
            {
                TheArena.Entities.Add(new ExSkull(tile, Color.Purple));
                tile.Occupant.Health = 1;
            }

            oldController = Controller;
            oldmouse = mousecurrent;

            for (int i = 0; i < Anims.Length; i++)
            {
                Anims[i].Play(gameTime.ElapsedGameTime.TotalMilliseconds);
            }

            shieldangle = LerpRads(shieldangle, (int)ThePlayer.Direction * MathHelper.PiOver2 , .3f);

            base.Update(gameTime);
        }
        float shieldangle;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(20,20,20));

            SamplerState ss = new SamplerState() { Filter = TextureFilter.Point };

            DrawBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, ss, null, RasterizerState.CullNone, null, Cam1.get_transformation(GraphicsDevice));

            for (int y = 0; y < TheArena.Tiles.GetLength(1); y++)
            {
                for (int x = 0; x < TheArena.Tiles.GetLength(0); x++)
                {
                    var tile = TheArena.Tiles[x, y];

                    if (tile != null)
                    {
                        float alpha = 1 - Vector2.Distance(tile.Pos, ThePlayer.Pos) / 12f;

                        if (alpha >= tile.Alpha) tile.Alpha = alpha;

                        if (tile.Occupant != null) tile.Occupant.Alpha = tile.Alpha;

                        //draw tile
                        DrawBatch.Draw(Dot, new Vector2(x, y) * Size, new Rectangle(Point.Zero, new Point(Size)), new Color(tile.Blend, tile.Alpha));

                        if(tile.Contents is Coin)
                        {
                            DrawBatch.Draw(Dot, new Vector2(x, y) * Size + new Vector2(Size/4f), new Rectangle(Point.Zero, new Point(Size/2)), new Color(tile.Contents.Blend, tile.Alpha ));
                        }
                        else if(tile.Contents is Weapon)
                        {
                            Texture2D spr = null;

                            var w = tile.Contents as Weapon;

                            if(w.Type == WeaponType.axe)
                            {
                                spr = Axe;
                            }
                            else if (w.Type == WeaponType.sword)
                            {
                                spr = Sword;
                            }
                            else if (w.Type == WeaponType.spear)
                            {
                                spr = Spear;
                            }
                            else if (w.Type == WeaponType.shield)
                            {
                                spr = Shield;
                            }
                            else if (w.Type == WeaponType.harpoon)
                            {
                                spr = Harpoon;
                            }
                            else if (w.Type == WeaponType.mallet)
                            {
                                spr = Mallet;
                            }
                            else if (w.Type == WeaponType.paired)
                            {
                                spr = Paired;
                            }
                            else if (w.Type == WeaponType.bow)
                            {
                                spr = Bow;
                            }
                            else if (w.Type == WeaponType.bowbomb)
                            {
                                spr = BowBomb;
                            }

                            DrawBatch.Draw(spr, new Vector2(x, y) * Size, new Color(tile.Contents.Blend, tile.Alpha));
                        }

                        if (tile.BlastAlpha > 0)
                        {
                            DrawBatch.Draw(Dot, new Vector2(x, y) * Size, new Rectangle(Point.Zero, new Point(Size)), new Color(Color.OrangeRed, tile.BlastAlpha));
                            tile.BlastAlpha -= .04f;
                        }
                    }

                }
            }

            for (int i = 0; i < TheArena.Entities.Count; i++)
            {
                var e = TheArena.Entities[i];

                if (e == null) continue;

                if(e is Spinner)
                {
                    var s = e as Spinner;
                    DrawBatch.Draw(Arrow, s.Mask * Size + new Vector2(Size/2f), null, null, new Vector2(25/2f), s.MaskAngle, null, new Color(s.Blend, s.Alpha),SpriteEffects.None,0f);
                }
                else if (e is StoneSpinner)
                {
                    var s = e as StoneSpinner;
                    DrawBatch.Draw(Arrow, s.Mask * Size + new Vector2(Size / 2f), null, null, new Vector2(25 / 2f), s.MaskAngle, null, new Color(s.Blend, s.Alpha), SpriteEffects.None, 0f);
                }
                else if (e is Pot)
                {
                    DrawBatch.Draw(Box, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is ExPot)
                {
                    DrawBatch.Draw(Explosion, e.Mask * Size,  new Color(e.Blend, e.Alpha));
                }
                else if (e is Crab)
                {
                    DrawBatch.Draw(Crab1, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is Slime)
                {
                    DrawBatch.Draw(Goop, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is ExSkull)
                {
                    DrawBatch.Draw(SpookBomb, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is Skull)
                {
                    DrawBatch.Draw(Spook, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is StoneSkull)
                {
                    DrawBatch.Draw(SpookStone, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is Chest)
                {
                    var c = e as Chest;

                    if(c.IsMimic && !c.IsHidden)
                        DrawBatch.Draw(ChestMouth, e.Mask * Size, new Color(e.Blend, e.Alpha));
                    else
                        DrawBatch.Draw(ChestClose, e.Mask * Size, new Color(e.Blend, e.Alpha));
                }
                else if (e is Player)
                {
                    DrawBatch.Draw(Mask, e.Mask * Size, new Color(e.Blend, e.Alpha));

                    if(ThePlayer.EqWeapon != null && ThePlayer.EqWeapon.Type == WeaponType.shield)
                    {
                        DrawBatch.Draw(ShieldTile, ThePlayer.Mask*Size + new Vector2(Size/2),null, Color.White, shieldangle, new Vector2(25/2),1f, SpriteEffects.None, 0f);
                    }
                }
                else
                {
                    DrawBatch.Draw(Dot, new Rectangle((e.Mask * Size).ToPoint(), new Point(Size)), new Color(e.Blend, e.Alpha));
                }

                if (false)//debug path
                {
                    if (e.Path != null && TurnComplete)
                    {//debugd
                        foreach (Tile t in e.Path)
                        {
                            DrawBatch.Draw(Dot, t.Pos * Size + new Vector2(Size / 4f), new Rectangle(Point.Zero, new Point(Size / 2)), new Color(Color.Black, .5f));
                        }
                    }
                }
            }

            //draw swoosh swoosh
            if (Anims[(int)Anim.axe].CurFrame < Anims[(int)Anim.axe].Frames && ThePlayer.EqWeapon?.Type == WeaponType.axe)
            {//axe
                DrawBatch.Draw(Anims[(int)Anim.axe].Sprite, ThePlayer.Mask * Size + new Vector2(25 / 2), Anims[(int)Anim.axe].DrawReg, Color.White, (float)ThePlayer.Direction * MathHelper.PiOver2, new Vector2(75 / 2), Vector2.One, SpriteEffects.None, 0f);
            }
            else if (Anims[(int)Anim.sword].CurFrame < Anims[(int)Anim.sword].Frames && (ThePlayer.EqWeapon?.Type == WeaponType.sword | ThePlayer.EqWeapon?.Type == WeaponType.harpoon))
            {//sword / harpoon
                DrawBatch.Draw(Anims[(int)Anim.sword].Sprite, ThePlayer.Mask * Size + new Vector2(25 / 2), Anims[(int)Anim.sword].DrawReg, Color.White, (float)ThePlayer.Direction * MathHelper.PiOver2, new Vector2(25 / 2, (25 / 2) + 25), Vector2.One, SpriteEffects.None, 0f);
            }
            else if (Anims[(int)Anim.spear].CurFrame < Anims[(int)Anim.spear].Frames && ThePlayer.EqWeapon?.Type == WeaponType.spear)
            {//spear
                DrawBatch.Draw(Anims[(int)Anim.spear].Sprite, ThePlayer.Mask * Size + new Vector2(25 / 2), Anims[(int)Anim.spear].DrawReg, Color.White, (float)ThePlayer.Direction * MathHelper.PiOver2, new Vector2(25 / 2, (25 / 2) + 50), Vector2.One, SpriteEffects.None, 0f);
            }
            else if (Anims[(int)Anim.sword].CurFrame < Anims[(int)Anim.sword].Frames && ThePlayer.EqWeapon?.Type == WeaponType.paired)
            {//paired blades
                DrawBatch.Draw(Anims[(int)Anim.sword].Sprite, ThePlayer.Mask * Size + new Vector2(25 / 2), Anims[(int)Anim.sword].DrawReg, Color.White, (float)ThePlayer.Direction * MathHelper.PiOver2, new Vector2(25 / 2, (25 / 2) + 25), Vector2.One, SpriteEffects.None, 0f);
                DrawBatch.Draw(Anims[(int)Anim.sword].Sprite, ThePlayer.Mask * Size + new Vector2(25 / 2), Anims[(int)Anim.sword].DrawReg, Color.White, (float)Piece.Opposite(ThePlayer.Direction) * MathHelper.PiOver2, new Vector2(25 / 2, (25 / 2) + 25), Vector2.One, SpriteEffects.None, 0f);
            }
            else if (Anims[(int)Anim.hammer].CurFrame < Anims[(int)Anim.hammer].Frames && ThePlayer.EqWeapon?.Type == WeaponType.mallet)
            {//hammer
                DrawBatch.Draw(Anims[(int)Anim.hammer].Sprite, ThePlayer.Pos * Size + new Vector2(25 / 2), Anims[(int)Anim.hammer].DrawReg, Color.White, (float)ThePlayer.Direction * MathHelper.PiOver2, new Vector2(25 / 2, (25 / 2) + 25), Vector2.One, SpriteEffects.None, 0f);
            }
            else if (ArrowAlpha > 0)
            {//arrow / thrown
                if(ArrowAlpha == 1)
                {
                    ArrowPos = ThePlayer.Pos;
                    ArrowDir = ThePlayer.Direction;
                    ArrowLeng = ThePlayer.BowDist;
                }
                Rectangle drawreg = new Rectangle(Point.Zero, new Point(25, 25 * (int)ArrowLeng));
                DrawBatch.Draw(ArrowShot, ArrowPos * Size + new Vector2(25 / 2), drawreg, new Color(Color.White, ArrowAlpha), (float)ArrowDir * MathHelper.PiOver2, new Vector2(25 / 2, (25 / 2) + 25*ArrowLeng), Vector2.One, SpriteEffects.None, 0f);
                ArrowAlpha -= .05f;
            }

            for (int i = 0; i < TileTargets.Count; i++)
            {
                var t = TileTargets[i];

                if (t == null) continue;

                if(t == NextMove)
                {
                    DrawBatch.Draw(Dot, new Rectangle((t.Pos * Size).ToPoint(), new Point(Size)), new Color(Color.White, .5f));
                }
                else
                {
                    DrawBatch.Draw(Dot, new Rectangle((t.Pos * Size).ToPoint(), new Point(Size)), new Color(Color.Black, .5f));
                }
            }

            for (int i = 0; i < EnemyTargets.Count; i++)
            {
                var t = EnemyTargets[i];

                if (t == null) continue;

                if (t == NextEnemy)
                {
                    DrawBatch.Draw(Dot, new Rectangle((t.Pos * Size).ToPoint(), new Point(Size)), new Color(Color.White, .5f));
                }
                else
                {
                    DrawBatch.Draw(Dot, new Rectangle((t.Pos * Size).ToPoint(), new Point(Size)), new Color(Color.Black, .5f));
                }
            }

            float rot = (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 100f) /10f;

            Vector2 rand = new Vector2(rot, rot)*110f;

            if (ComboCount > 1)
            {
                ComboAlpha += .09f;
                if (ComboAlpha > 1) ComboAlpha = 1;
            }
            else
            {
                ComboAlpha -= .05f;
                if (ComboAlpha < 0) ComboAlpha = 0;
            }
            ComboLerp = MathHelper.Lerp(ComboLerp, ComboCount, .05f);

            DrawBatch.DrawString(Consolas, ComboText, ThePlayer.Pos * Size + new Vector2(50, 200) + rand, new Color(Color.HotPink, ComboAlpha), rot, new Vector2(ComboText.Length * 10, 10), .5f + ComboLerp * .3f, SpriteEffects.None, 0f);

            //DrawBatch.Draw(Dot, new Rectangle((debugpos * Size).ToPoint(), new Point(6)), new Color(Color.Red, 1f));

            if (MenuAlpha > 0)
            {
                MenuAlpha -= .01f;

                Vector2 pos = ThePlayer.Mask * (Size) + new Vector2(Size/2); //+ new Vector2(-15, -70);

                var skill = ThePlayer.EqSkill;
                Texture2D spr = null;

                spr = SkillIcons[(int)skill];

                for (int i = 0; i < skillLength; i++)
                {
                    spr = SkillIcons[i];
                    Color color = Color.White;

                    if(Energy < skillObjects[i].Cost)
                    {
                        color = Color.DarkRed;
                    }

                    if((Skill)i == skill)
                    {
                        DrawBatch.Draw(spr, pos + skillSelect[i] * 70f, null, new Color(color, MenuAlpha), 0f, new Vector2(25/2f), 2f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        DrawBatch.Draw(spr, pos + skillSelect[i] * 70f, null, new Color(color, MenuAlpha / 2f), 0f, new Vector2(25 / 2f), 1.5f, SpriteEffects.None, 0f);
                    }

                    DrawBatch.DrawString(Consolas, skillObjects[i].Cost.ToString() ,pos + skillSelect[i] * 70f, new Color(color, MenuAlpha), 0f, new Vector2(25 / 2f), 1f, SpriteEffects.None, 0f);
                }
                //drawing the skills
                
                if(ThePlayer?.EqWeapon?.Durability > 0)
                {
                    DrawBatch.DrawString(Consolas, $"{ThePlayer.EqWeapon.Type.ToString()}: {ThePlayer.EqWeapon.Durability}", pos + new Vector2(-40, -35), new Color(Color.White, MenuAlpha), 0f, Vector2.Zero, .65f, SpriteEffects.None, 0f);
                }
            }

            if(true)
            {
                DrawBatch.DrawString(Consolas, Energy.ToString(), ThePlayer.Mask * Size + new Vector2(0, 25), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
            }

            if(DuraAlpha > 0)
            {
                string text = "";

                if (ThePlayer.EqWeapon?.Durability > 0) text = ThePlayer.EqWeapon.Durability.ToString();
                else if(ThePlayer.EqWeapon?.Type != WeaponType.bowbomb) text = "Broken!";
                if (ThePlayer.EqWeapon?.Durability == 1 && ThePlayer.EqWeapon?.Type == WeaponType.mallet) text = "weapon @ risk!";

                DrawBatch.DrawString(Consolas, text, ThePlayer.Mask * Size + new Vector2(5), new Color(Color.White, DuraAlpha), 0f, new Vector2(10), 1 + DuraAlpha, SpriteEffects.None, 0f);
                DuraAlpha -= .01f;
            }

            DrawBatch.End();
            
            GUI.Begin();
            GUI.DrawString(Calibri, "Gold: " + ThePlayer.Gold, Vector2.Zero, Color.White);
            GUI.DrawString(Consolas, DoorsKicked.ToString(), new Vector2(Graphics.PreferredBackBufferWidth / 2f, 20), Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            if (ThePlayer.IsDisposed)
            {
                var text =
                    $"You died.\n\nDoors kicked: {DoorsKicked}\nKills: {Kills}\nBest combo: {HighestCombo}\n\nStart to retry\nBack to exit.";

                GUI.DrawString(Consolas, text, new Vector2(100), Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            }

            /*
            //GUI.DrawString(Calibri, RoomNumber.ToString(), new Vector2(10), Color.Black);
            GUI.DrawString(Calibri, LastElapsed.ToString(), new Vector2(10), Color.Black);

            //GUI.Draw(Dot, new Rectangle(new Point(960, 540), new Point(10)), Color.Black);

            GUI.Draw(Dot, new Rectangle(new Point(10, 50), new Point(110, 30)), Color.White);

            if(PathgenTimer.ElapsedMilliseconds > 10)
            {
                double max = TheArena.Enemies.Count;

                double temp = EnemiesDone / max;

                temp = temp * 100;

                GUI.Draw(Dot, new Rectangle(new Point(15, 55), new Point((int)temp, 20)), Color.Black);
            }
            */
            GUI.End();
            
            base.Draw(gameTime);
        }

        public static float LerpRads(float start, float end, float amount)
        {
            float difference = Math.Abs(end - start);
            if (difference > MathHelper.Pi)
            {
                // We need to add on to one of the values.
                if (end > start)
                {
                    // We'll add it on to start...
                    start += MathHelper.TwoPi;
                }
                else
                {
                    // Add it on to end.
                    end += MathHelper.TwoPi;
                }
            }

            // Interpolate it.
            float value = (start + ((end - start) * amount));

            // Wrap it..
            float rangeZero = MathHelper.TwoPi;

            if (value >= 0 && value <= MathHelper.TwoPi)
                return value;

            return (value % rangeZero);
        }
    }
}