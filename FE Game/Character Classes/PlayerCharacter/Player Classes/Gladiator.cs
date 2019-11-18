using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class Gladiator:PlayerClass
    {
        public Gladiator()
        {
            LevelUp = new LevelUp(3, 0.6, 0.2, 0.8, 0.7, 0.6, 0.4);
            MovementRange = 6;
            ExpPerLevel = 100;
            Promoted = false;
            Name = "Gladiator";
            PromotedClass = new Werewolf();
        }
    }
}
