using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes.PlayerCharacter;

namespace FE_Game.Character_Classes.Enemies
{
    class Scout:EnemyClass
    {
        public Scout()
        {
            levelUp = new LevelUp(1, 0.3, 0.1, 0.7, 0.5, 0.3, 0.3);
            Health = 5;
            Strength = 2;
            Magic = 1;
            Speed = 3;
            Skill = 3;
            Armor = 2;
            Resistance = 2;
            EXPValue = 30;
            Name = "Scout";
            MovementRange = 6;
        }
    }
}
