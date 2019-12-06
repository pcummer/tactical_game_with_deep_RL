using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FE_Game
{
    public partial class MainWindow : Form
    {
        // The starting form from which the player begins the game
        TacticalMap tacticalMap = new TacticalMap();
        GameController gameController = new GameController();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameController.StartingRoster();
            gameController.ChangeActiveForm(this, tacticalMap);
            gameController.PassPlayerRoster(tacticalMap);
        }
    }
}
