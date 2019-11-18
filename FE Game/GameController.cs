using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE_Game.Character_Classes;
using FE_Game.Character_Classes.PlayerCharacter;
using FE_Game.Character_Classes.PlayerCharacter.Player_Classes;


namespace FE_Game
{
    class GameController
    {
        Roster playerroster = new Roster();

        public void StartingRoster()
        {
            playerroster.AddCharacter(new PlayerCharacter("Starting Zombie",new Zombie(), 9, 6, 2, 3, 4, 3, 3));
            playerroster.AddCharacter(new PlayerCharacter("Starting Gladiator", new Gladiator(), 8, 4, 1, 5, 5, 4, 2));
            playerroster.AddCharacter(new PlayerCharacter("Starting Hedge Wizard", new HedgeWizard(), 7, 2, 5, 4, 4, 1, 3));
            playerroster.AddCharacter(new PlayerCharacter("Starting Witch Doctor", new HedgeWizard(), 5, 3, 4, 5, 5, 8, 1));
        }

        public void ChangeActiveForm(System.Windows.Forms.Form formA, System.Windows.Forms.Form formB)
        {
            formA.Hide();
            formB.Show();
        }

        public void PassPlayerRoster(TacticalMap map)
        {
            map.PlacePlayerRoster(playerroster);
        }

        public void ReceivePlayerRoster(Character[] roster)
        {
            for(int i = 0; i < roster.Length; i++)
            {
                playerroster.AddCharacter(roster[i]);
            }
        }
    }
}
