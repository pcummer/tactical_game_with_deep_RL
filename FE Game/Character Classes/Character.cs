using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes
{
    class Character
    {
        // Basic Character Attributes
        public int MaxHealth;
        public int Health;
        public int Strength;
        public int Magic;
        public int Speed;
        public int Skill;
        public int Armor;
        public int Resistance;
        public int MovementRange;
        public int MaxMovementRange;

        // Advanced Character Attributes
        public string Name;
        public bool Alive;
        public bool Ally = true;

        public Character(string StartingName, int StartingHealth, int StartingStrength, int StartingMagic, int StartingSpeed, int StartingSkill, int StartingArmor, int StartingResistance)
        {
            MaxHealth = StartingHealth;
            Health = StartingHealth;
            Strength = StartingStrength;
            Magic = StartingMagic;
            Speed = StartingSpeed;
            Skill = StartingSkill;
            Armor = StartingArmor;
            Resistance = StartingResistance;
            Name = StartingName;
            MovementRange = 5;
            MaxMovementRange = 5;
            Alive = true;
        }

        public void TakePhysicalDamage(int Damage)
        {
            // Deal one damage if damage is less than armor
            if (!(Damage > Armor))
            {
                Damage = Armor + 1;
            }
            Health = Health - (Damage-Armor);

            if(!(Health > 0))
            {
                Death();
            }
        }

        public void TakeMagicalDamage(int Damage)
        {
            // Deal one damage if damage is less than resistance
            if (Damage < Resistance)
            {
                Damage = Resistance + 1;
            }
            Health = Health - (Damage - Resistance);

            if (!(Health > 0))
            {
                Death();
            }
        }

        protected void Death()
        {
            // Placeholder for death in child classes
            Alive = false;
        }
    }
}
