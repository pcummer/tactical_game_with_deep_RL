using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class WitchDoctor:PlayerClass
    {
        public WitchDoctor()
        {
            LevelUp = new LevelUp(3, 0.2, 0.95, 0.6, 0.7, 0.6, 0.6);
            MovementRange = 7;
            ExpPerLevel = 500;
            Promoted = true;
            Name = "Witch Doctor";
        }
    }
}
