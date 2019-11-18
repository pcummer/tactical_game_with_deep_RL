using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class ForsakenChampion:PlayerClass
    {
        public ForsakenChampion()
        {
            LevelUp = new LevelUp(4, 0.8, 0.7, 0.5, 0.8, 1.0, 0.8);
            MovementRange = 6;
            ExpPerLevel = 600;
            Promoted = true;
            Name = "Forsaken Champion";
        }
    }
}
