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
    public class SpriteStrip
    {
        public Texture2D Sprite { get; set; }
        public Point Dim { get; set; }
        public Rectangle DrawReg { get => new Rectangle(new Point(Dim.X * CurFrame, 0), Dim); }
        public double AnimSpeed { get; set; }
        public double Timer { get; set; }
        public int Frames { get; set; }
        public int CurFrame { get; set; }
        public bool Playing
        {
            get => _playing;

            set
            {
                _playing = value;
                if(_playing)
                {
                    CurFrame = 0;
                }
                else
                {
                    CurFrame = Frames;
                }
            }
        }
        private bool _playing;
        public SpriteStrip(Texture2D spr, Point dim, float animspeed)
        {
            Dim = dim;
            Sprite = spr;
            AnimSpeed = animspeed;
            Frames = Sprite.Width / DrawReg.Width;
            CurFrame = Frames;
            Playing = false;
        }
        public void Play(double time)
        {
            Timer += time;

            if(Timer > AnimSpeed && Playing)
            {
                Timer = 0;
                CurFrame++;
            }

        }
    }
}
