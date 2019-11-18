using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes.PlayerCharacter;

namespace FE_Game.Character_Classes.Enemies
{
    class Militia:EnemyClass
    {
        public Militia()
        {
            levelUp = new LevelUp(2, 0.4, 0.1, 0.5, 0.3, 0.5, 0.3);
            Health = 4;
            Strength = 2;
            Magic = 1;
            Speed = 2;
            Skill = 2;
            Armor = 3;
            Resistance = 3;
            EXPValue = 20;
            Name = "Militia";
            MovementRange = 5;
        }
    }
}
