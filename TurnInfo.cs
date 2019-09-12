using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_Redux
{
    public class TurnInfo
    {
        public List<Piece> Kills { get; set; }
        public Skill UsedSkill { get; set; }
        public int PyroCount { get; set; }
        public TurnInfo()
        {
            Kills = new List<Piece>();
        }
        
    }
}
