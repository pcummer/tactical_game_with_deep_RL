using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter
{
    class LevelUp
    {
        public int HealthGrowth;
        double StrengthProbability;
        double MagicProbability;
        double SpeedProbability;
        double SkillProbability;
        double ArmorProbability;
        double ResistanceProbability;
        public bool Strength;
        public bool Magic;
        public bool Speed;
        public bool Skill;
        public bool Armor;
        public bool Resistance;
        bool BadLevelUp;
        Random Random = new Random();

        public LevelUp(int Health, double Strength, double Magic, double Speed, double Skill, double Armor, double Resistance)
        {
            HealthGrowth = Health;
            StrengthProbability = Strength;
            MagicProbability = Magic;
            SpeedProbability = Speed;
            SkillProbability = Skill;
            ArmorProbability = Armor;
            ResistanceProbability = Resistance;
        }

        public void GainLevel()
        {
            Strength = false;
            Magic = false;
            Speed = false;
            Skill = false;
            Armor = false;
            Resistance = false;
            BadLevelUp = false;
            int LevelUpQuality = 0;

            if (Random.NextDouble() < StrengthProbability)
            {
                Strength = true;
                LevelUpQuality += 1;
            }

            if (Random.NextDouble() < MagicProbability)
            {
                Magic = true;
                LevelUpQuality += 1;
            }

            if (Random.NextDouble() < SpeedProbability)
            {
                Speed = true;
                LevelUpQuality += 1;
            }

            if (Random.NextDouble() < SkillProbability)
            {
                Skill = true;
                LevelUpQuality += 1;
            }

            if (Random.NextDouble() < ArmorProbability)
            {
                Armor = true;
                LevelUpQuality += 1;
            }

            if (Random.NextDouble() < ResistanceProbability)
            {
                Resistance = true;
                LevelUpQuality += 1;
            }

            if(LevelUpQuality < (StrengthProbability + MagicProbability + SpeedProbability + SkillProbability + ArmorProbability + ResistanceProbability)/2)
            {
                GainLevel();
            }

        }

    }
}
