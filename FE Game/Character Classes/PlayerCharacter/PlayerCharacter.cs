using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter
{
    class PlayerCharacter:Character
    {
        public PlayerCharacter(string StartingName, PlayerClass StartingClass, int StartingHealth, int StartingStrength, int StartingMagic, int StartingSpeed, int StartingSkill, int StartingArmor, int StartingResistance)
            :base(StartingName, StartingHealth, StartingStrength, StartingMagic, StartingSpeed, StartingSkill, StartingArmor, StartingResistance)
        {
            Exp = 0;
            Level = 1;
            MovementRange = StartingClass.MovementRange;
            MaxMovementRange = MovementRange;
            Class = StartingClass;
            LevelUp = Class.LevelUp;
            ExpPerLevel = Class.ExpPerLevel;
            Ally = true;
        }

        int ExpPerLevel;
        int Exp;
        int Level;
        LevelUp LevelUp;
        PlayerClass Class;

        protected void GainExperience(int GainedExp)
        {
            Exp += GainedExp;

            if (Exp > ExpPerLevel)
            {
                GainLevel();
                Exp = 0;
                ExpPerLevel = (int)Math.Round(ExpPerLevel * 1.08);
                Level += 1;

                if(Level == 20 & Class.Promoted != true)
                {
                    Promote();
                }
            }
        }

        protected void GainLevel()
        {
            LevelUp.GainLevel();

            MaxHealth += LevelUp.HealthGrowth;
            Health += LevelUp.HealthGrowth;

            if(LevelUp.Strength)
            {
                Strength += 1;
            }

            if(LevelUp.Magic)
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

            if(LevelUp.Armor)
            {
                Armor +=1;
            }

            if(LevelUp.Resistance)
            {
                Resistance += 1;
            }
        }

        protected void Promote()
        {
            Class = Class.PromotedClass;
            LevelUp = Class.LevelUp;
            for(int i = 0; i < 3; i++)
            { GainLevel(); }
            ExpPerLevel = Class.ExpPerLevel;
            Level = 1;
        }
    }
}
