using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class Knight:PlayerClass
    {
        public Knight()
        {
            LevelUp = new LevelUp(3, 0.7, 0.2, 0.4, 0.7, 0.8, 0.3);
            MovementRange = 4;
            ExpPerLevel = 100;
            Promoted = false;
            Name = "Knight";
            PromotedClass = new ForsakenChampion();
        }
    }
}
