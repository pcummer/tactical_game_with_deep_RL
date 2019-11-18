using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes.PlayerCharacter;

namespace FE_Game.Character_Classes.Enemies
{
    class Exorcist:EnemyClass
    {
        public Exorcist()
        {
            levelUp = new LevelUp(1, 0.1, 0.4, 0.4, 0.5, 0.1, 0.4);
            Health = 3;
            Strength = 1;
            Magic = 3;
            Speed = 2;
            Skill = 2;
            Armor = 1;
            Resistance = 3;
            EXPValue = 25;
            Name = "Exorcist";
            MovementRange = 4;
        }
    }
}
