using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_Redux
{

    public enum Block
    {
        floor, wall, door
    }
    public enum Dir 
    {
        up, right, down, left, none
    }
    
    public enum Skill
    {
        none, en_passant, rook, knight
    }
    public enum WeaponType
    {
        none, sword, axe, spear, harpoon, shield, paired, mallet, bow, shieldnew, bowbomb
    }
    
    public enum Anim
    {
        axe, sword, spear, hammer
    }

}
