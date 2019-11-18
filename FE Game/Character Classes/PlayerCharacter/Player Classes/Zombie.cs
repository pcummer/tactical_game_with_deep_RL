using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class Zombie:PlayerClass
    {
        public Zombie()
        {
            LevelUp = new LevelUp(3, 0.9, 0.3, 0.5, 0.6, 0.6, 0.8);
            MovementRange = 5;
            ExpPerLevel = 70;
            Promoted = false;
            Name = "Zombie";
            PromotedClass = new FleshAbomination();
        }
    }
}
