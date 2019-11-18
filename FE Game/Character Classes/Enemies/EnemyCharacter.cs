using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes.PlayerCharacter;

namespace FE_Game.Character_Classes.Enemies
{
    class EnemyCharacter:Character
    {
        public EnemyCharacter(EnemyClass enemyClass, int Level)
            :base(enemyClass.Name, enemyClass.Health, enemyClass.Strength, enemyClass.Magic, enemyClass.Speed, enemyClass.Skill, enemyClass.Armor, enemyClass.Resistance)
        {
            Ally = false;
            MovementRange = enemyClass.MovementRange;
            MaxMovementRange = MovementRange;
            EXPValue = (int)(enemyClass.EXPValue * Math.Pow(1.1,(double)Level));
            for(int i = 0; i< Level; i++)
            {
                GainLevel(enemyClass.levelUp);
            }
        }

        int EXPValue;

        protected void GainLevel(LevelUp LevelUp)
        {
            LevelUp.GainLevel();

            MaxHealth += LevelUp.HealthGrowth;
            Health += LevelUp.HealthGrowth;

            if (LevelUp.Strength)
            {
                Strength += 1;
            }

            if (LevelUp.Magic)
            {
                Magic += 1;
            }

            if (LevelUp.Speed)
            {
                Speed += 1;
            }

            if (LevelUp.Skill)
            {
                Skill += 1;
            }

            if (LevelUp.Armor)
            {
                Armor += 1;
            }

            if (LevelUp.Resistance)
            {
                Resistance += 1;
            }
        }
    }
}
