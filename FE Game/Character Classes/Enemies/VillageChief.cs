using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes.PlayerCharacter;

namespace FE_Game.Character_Classes.Enemies
{
    class VillageChief:EnemyClass
    {
        public VillageChief()
        {
            levelUp = new LevelUp(3, 0.7, 0.4, 0.5, 0.5, 0.5, 0.7);
            Health = 8;
            Strength = 5;
            Magic = 3;
            Speed = 4;
            Skill = 3;
            Armor = 2;
            Resistance = 3;
            EXPValue = 80;
            Name = "Village Chief";
            MovementRange = 4;
        }
    }
}
