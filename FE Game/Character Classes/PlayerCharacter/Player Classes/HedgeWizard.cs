using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class HedgeWizard: PlayerClass
    {
        public HedgeWizard()
        {
            LevelUp = new LevelUp(2, 0.2, 0.7, 0.5, 0.4, 0.4, 0.5);
            MovementRange = 6;
            ExpPerLevel = 100;
            Promoted = false;
            Name = "Hedge Wizard";
            PromotedClass = new WitchDoctor();
        }
    }
}
