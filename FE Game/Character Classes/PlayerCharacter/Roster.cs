using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE_Game.Character_Classes.PlayerCharacter
{
    class Roster
    {
        Dictionary<string,Character> roster = new Dictionary<string, Character>();
        
        public void AddCharacter(Character character)
        {
            roster.Add(character.Name, character);
        }

        public Character[] ReturnCharacterArray()
        {
            return roster.Values.ToArray();
        }
    }
}
