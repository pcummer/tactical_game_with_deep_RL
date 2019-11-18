using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter.Player_Classes
{
    class Werewolf:PlayerClass
    {
        public Werewolf()
        {
            LevelUp = new LevelUp(4, 0.8, 0.4, 0.9, 0.8, 0.6, 0.7);
            MovementRange = 7;
            ExpPerLevel = 800;
            Promoted = true;
            Name = "Werewolf";
        }
    }
}
