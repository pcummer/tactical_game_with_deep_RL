using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class FleshAbomination:PlayerClass
    {
        public FleshAbomination()
        {
            LevelUp = new LevelUp(6, 1, 0.4, 0.6, 0.7, 0.8, 0.9);
            MovementRange = 5;
            ExpPerLevel = 500;
            Promoted = true;
            Name = "Flesh Abomination";
        }
    }
}
