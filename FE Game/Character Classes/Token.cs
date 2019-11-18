using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes.PlayerCharacter;

namespace FE_Game.Character_Classes
{
    class Token
    {
        public Character Character;
        public bool active;
        public System.Drawing.Color color = System.Drawing.Color.Silver;
        public int Score;
        public double Reward;

        public void Attack(Token Enemy)
        {
            Random random = new Random();

            if (Character.Skill-Enemy.Character.Skill + random.Next(0,100) > 5)
            {
                Enemy.Character.TakePhysicalDamage(Character.Strength);
                Reward += Character.Strength - Enemy.Character.Armor;
            }

            if(Enemy.Character.Alive == true & Enemy.Character.Skill - Character.Skill + 100 * random.Next(0, 100) > 5)
            {
                Character.TakePhysicalDamage(Enemy.Character.Strength);
            }

            if (Enemy.Character.Alive == true & Enemy.Character.Skill - Character.Skill + 100 * random.Next(0, 100) > 5 & Enemy.Character.Speed - Character.Speed > Character.Speed/2)
            {
                Character.TakePhysicalDamage(Enemy.Character.Strength);
            }

            if (Character.Alive == true & Character.Skill - Enemy.Character.Skill + 100 * random.Next(0, 100) > 5 & Character.Speed - Enemy.Character.Speed > Enemy.Character.Speed / 2)
            {
                Enemy.Character.TakePhysicalDamage(Character.Strength);
                Reward += Character.Strength - Enemy.Character.Armor;
            }

            active = false;

            if(Enemy.Character.Alive == false)
            {
                Reward += 10;
            }

            if(Character.Ally == true & Enemy.Character.Alive == false)
            {
                //gain experience here
            }
        }

        public void UpdateColor()
        {
            if(Character == null)
            {
                color = System.Drawing.Color.Silver;
            }
            else
            {
                if(active == true & Character.Ally == true)
                {
                    color = System.Drawing.Color.MediumSpringGreen;
                }
                else
                {
                    if (active == false & Character.Ally == true)
                    {
                        color = System.Drawing.Color.Violet;
                    }
                    else
                    {
                        color = System.Drawing.Color.Crimson;
                    }
                }
                if(Character.Alive == false)
                {
                    Character = null;
                    color = System.Drawing.Color.DarkSalmon;
                }
            }
        }

    }
}
