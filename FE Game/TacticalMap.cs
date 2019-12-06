using FE_Game.Character_Classes;
using FE_Game.Character_Classes.PlayerCharacter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FE_Game.Character_Classes.Enemies;
using System.Diagnostics;
using System.Net.Http;
using CsvHelper;
using FE_Game.Character_Classes.PlayerCharacter.Player_Classes;

namespace FE_Game
{
    public partial class TacticalMap : Form
    {
        // The vast majority of the actual gameplay takes place within this class. It is primarily associated with the movement of
        // characters around the board, including API calls to a tensorflow model for the AI and a UI for the player actions
        // It can either be played as a human using the end turn button to call the AI to take its turn or it can be used to train
        // the AI by playing it against itself

        // Our core construction is a 2D array of token objects corresponding to a 2D array of picture boxes
        // This tracks all the action on the backend by updating token properties and displays it on the UI by updating the picture boxes
        // Note that the tokens do not move in the array, but rather swap properties to recreate the motion of pieces on the board
        Token[,] Map;
        Dictionary<Token, PictureBox> TokenTileCorrespondence = new Dictionary<Token, PictureBox>();
        Dictionary<PictureBox, Token> ReverseTokenTileCorrespondence = new Dictionary<PictureBox, Token>();

        // Define the map shape and display size
        int MapHeight = 5;
        int MapWidth = 5;
        int GridSize = 20;
        int GridSpacing = 30;

        // The selected token is that token which has the focus of the player or the AI
        Token SelectedToken;

        // Track rewards for debugging AI
        double AlliedReward = 0;
        double EnemyReward = 0;

        // Random variable used for randomly generating new board states
        Random RandomGlobal = new Random();

        // Variables for encoding the game state from the perspective of the selected token; passed to the ai for decision making
        int[] GameState;
        int StateLength = 10;
        int VisionLength = 7;

        // Prediction from the AI
        int Target;

        private static readonly HttpClient client = new HttpClient();
        
        public TacticalMap()
        {
            InitializeComponent();
        }

        private void TacticalMap_Load(object sender, EventArgs e)
        {
            InitializeMap();
            PlaceEnemyCharacters();
            UpdateColors();
        }

        public void PlacePlayerRoster(object roster)
        {
            // Place a basic team of player characters
            // To do: add support for permanent player teams
            Roster dummyroster = (Roster)roster;
            for (int i =0 ; i < dummyroster.ReturnCharacterArray().Length; i++)
            {
                Map[i, 0].Character = dummyroster.ReturnCharacterArray()[i];
                Map[i, 0].active = true;
                UpdateColors();
            }
        }

        private void InitializeMap()
        {
            // create the corresponding arrays of tokens and picture boxes
            Map = new Token[MapWidth, MapHeight];
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    Map[i, j] = new Token();
                    AddPictureBox(i, j, Map[i, j]);
                }
            }

            // add buttons and displays
            InitializeTextDisplay();
            InitializeEndTurn();
            InitializePlaySelf();
            InitializeRewardDisplay();
        }

        private void InitializeTextDisplay()
        {
            var DisplayBox = new Label();
            DisplayBox.Name = "CharacterDisplay";
            DisplayBox.Location = new Point((MapWidth * GridSpacing + GridSize), (GridSize));
            DisplayBox.AutoSize = true;
            DisplayBox.Font = new Font("Arial",22);
            this.Controls.Add(DisplayBox);
        }

        private void InitializeRewardDisplay()
        {
            var DisplayBox = new Label();
            DisplayBox.Name = "RewardDisplay";
            DisplayBox.Location = new Point((MapWidth * GridSpacing + 4 * GridSize), (GridSize));
            DisplayBox.AutoSize = true;
            DisplayBox.Font = new Font("Arial", 22);
            this.Controls.Add(DisplayBox);
        }

        private void InitializeEndTurn()
        {
            var EndTurn = new Button();
            EndTurn.Name = "EndTurnButton";
            EndTurn.Location = new Point((MapWidth * GridSpacing + GridSize), ((MapHeight-2)*GridSpacing+GridSize));
            EndTurn.Font = new Font("Arial", 12);
            EndTurn.Text = "End Turn";
            EndTurn.Size = new Size(3* GridSpacing, GridSize + GridSpacing);
            EndTurn.Click += new EventHandler(endturn_clickAsync);
            this.Controls.Add(EndTurn);
        }

        private void InitializePlaySelf()
        {
            var playself = new Button();
            playself.Name = "PlaySelfButton";
            playself.Location = new Point((MapWidth * GridSpacing + 7*GridSize), ((MapHeight - 2) * GridSpacing + 1*GridSize));
            playself.Font = new Font("Arial", 12);
            playself.Text = "Train";
            playself.Size = new Size(3 * GridSpacing, GridSize + GridSpacing);
            playself.Click += new EventHandler(playself_clickAsync);
            this.Controls.Add(playself);
        }

        private void PlaceEnemyCharacters()
        {
            // place basic team of opponents
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new EnemyCharacter(new Exorcist(),1);
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new EnemyCharacter(new VillageChief(), 1);
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new EnemyCharacter(new Scout(), 1);
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new EnemyCharacter(new Scout(), 1);
        }

        private void PlaceTrainPlayerTeam()
        {
            // random initialization of player characters for AI training
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new PlayerCharacter("Starting Zombie", new Zombie(), 9, 6, 2, 3, 4, 3, 3);
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new PlayerCharacter("Starting Gladiator", new Gladiator(), 8, 4, 1, 5, 5, 4, 2);
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new PlayerCharacter("Starting Hedge Wizard", new HedgeWizard(), 7, 2, 5, 4, 4, 1, 3);
            Map[RandomGlobal.Next(0, MapWidth), RandomGlobal.Next(0, MapHeight)].Character = new PlayerCharacter("Starting Witch Doctor", new HedgeWizard(), 5, 3, 4, 5, 5, 8, 1);
        }

        private void AddPictureBox(int i, int j, Token token)
        {
            // creates a new picture box, adds it to visual array on form, tracks its correspondence to a particular token on the backend
            var picturebox = new PictureBox();
            picturebox.Name = (String.Concat("picturebox", i, j));
            picturebox.Location = new Point(GridSize+i * GridSpacing,GridSize+ j * GridSpacing);
            picturebox.Size = new Size(GridSize, GridSize);
            picturebox.Click += new EventHandler(picturebox_click);
            picturebox.MouseHover += new EventHandler(picturebox_hover);
            picturebox.BackColor = token.color;
            TokenTileCorrespondence.Add(token, picturebox);
            ReverseTokenTileCorrespondence.Add(picturebox, token);
            this.Controls.Add(picturebox);
        }
        
        private void MoveCharacter(Token leaving, Token arriving)
        {
            // Swap characters between two tokens; this is the implementation of moving a character on the board
            Character temp_character = arriving.Character;
            bool temp_active = arriving.active;
            double temp_reward = arriving.Reward;
            arriving.Character = leaving.Character;
            arriving.active = true;
            arriving.Reward = leaving.Reward;
            leaving.Character = temp_character;
            leaving.active = temp_active;
            leaving.Reward = temp_reward;
            SelectedToken = arriving;
        }

        private void UpdateColors()
        {
            // update picture box colors to display current board state
            for(int i = 0; i < MapWidth; i++)
            {
                for(int j = 0; j< MapHeight; j++)
                {
                    Map[i, j].UpdateColor();
                    TokenTileCorrespondence[Map[i, j]].BackColor = Map[i, j].color;
                }
            }
        }
        
        private void picturebox_click(object sender, EventArgs e)
        {
            // allow player to select token if none is selected, or move/attack if a token is already selected
            // subject to constraints to ensure it is a valid move according to game rules
            PictureBox picture = (PictureBox)sender;
            if (SelectedToken == null)
            {
                SelectToken(picture);
            }
            else
            {
                if (SelectedToken == ReverseTokenTileCorrespondence[picture])
                {
                    SelectedToken = null;
                    UpdateColors();
                }
                else
                {
                    MoveOrAttack(picture);
                    if (ReverseTokenTileCorrespondence[picture].Character != null)
                    {
                        DisplayText(CharacterInformation(ReverseTokenTileCorrespondence[picture]));
                    }
                    else
                    {
                        DisplayText(CharacterInformation(SelectedToken));
                    }
                    SelectedToken = null;
                    UpdateColors();
                }
            }
            
        }

        private void picturebox_hover(object sender, EventArgs e)
        {
            // shows character stats on hover over corresponding picture box
            var picture = (PictureBox)sender;
            var token = (Token)ReverseTokenTileCorrespondence[picture];
            if (!(token.Character == null))
            {
                if (token.Character.Alive == true)
                {
                    DisplayText(CharacterInformation(token));
                }
            }
            else
            {
                DisplayText("");
            }
        }

        private void DisplayText(string text)
        {
            var display = this.Controls.Find("CharacterDisplay", true).FirstOrDefault();
            display.Text = text;
        }

        private void DisplayReward(string text)
        {
            var display = this.Controls.Find("RewardDisplay", true).FirstOrDefault();
            display.Text = text;
        }

        private async void endturn_clickAsync(object sender, EventArgs e)
        {
            // reset movement ranges, call AI turn
            for(int i = 0; i < MapWidth; i++)
            {
                for(int j = 0; j<MapHeight; j++)
                {
                    if(Map[i,j].Character != null)
                    {
                        Map[i, j].Character.MovementRange = Map[i, j].Character.MaxMovementRange;
                        Map[i, j].active = true;
                    }
                }
            }
            UpdateColors();
            await AITurnAsync(false);
            UpdateColors();
            SelectedToken = null;
        }

        private void ReinitializeMap()
        {
            // create fresh game state for training
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    Map[i, j].Character = null;
                }
            }
            PlaceEnemyCharacters();
            PlaceTrainPlayerTeam();                            
        }

        private async void playself_clickAsync(object sender, EventArgs e)
        {
            // Set AI to play both teams for many games against itself
            // Games end on one team being eliminated or max turn timer
            // Model trains between games
            int time_counter = 0;
            Random random = new Random();

            for (int count = 0; count < 1000; count++){
                // AI turn for playing as player team
                await AITurnAsync(true);

                // Normal end turn updates along with check that both teams still have at least one player
                int count_enemies = 0;
                int count_allies = 0;
                time_counter++;
                for (int i = 0; i < MapWidth; i++)
                {
                    for (int j = 0; j < MapHeight; j++)
                    {
                        if (Map[i, j].Character != null)
                        {
                            // reset movement ranges
                            Map[i, j].Character.MovementRange = Map[i, j].Character.MaxMovementRange;
                            Map[i, j].active = true;

                            // count characters on each team
                            if (Map[i, j].Character.Ally == true & Map[i, j].Character.Alive == true)
                            {
                                count_allies += 1;
                            }
                            else
                            if (Map[i, j].Character.Alive == true)
                            {
                                {
                                    count_enemies += 1;
                                }
                            }
                        }
                    }
                }
                UpdateColors();

                // End game if one team is eliminated
                if(count_allies == 0 | count_enemies == 0)
                {
                    await SendGameOutcome();
                    await SendTrainCommand(1);
                    time_counter = 0;
                    AlliedReward = 0;
                    EnemyReward = 0;
                    ReinitializeMap();
                    UpdateColors();
                }

                // End game if it has gone on too long
                if (time_counter > 30)
                {
                    await SendTrainCommand(1);
                    time_counter = 0;
                    AlliedReward = 0;
                    EnemyReward = 0;
                    ReinitializeMap();
                    UpdateColors();
                }

                // Call AI to play as enemy turn
                await AITurnAsync(false);
                UpdateColors();
                SelectedToken = null;

                // Show total rewards for each team
                DisplayReward("Ally: " + AlliedReward.ToString() + System.Environment.NewLine + "Enemy: " + EnemyReward.ToString());
            }
        }

        private async Task SendGameOutcome()
        {
            // Sends a large reward along with a random action if the team has won the game
            // This reward will propogate to earlier state-action pairs via q-learning
            Random random = new Random();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (Map[i, j].Character != null)
                    {
                        Character character = Map[i, j].Character;
                        SelectedToken = Map[i, j];
                        GenerateFeaturesForModel();

                        if (CheckTeamAlive(!character.Ally) == false)
                        {
                            await SendRewardAsync(random.Next(0, 5), 20);
                        }
                        else
                        {
                            if (CheckTeamAlive(character.Ally) == false)
                            {
                                await SendRewardAsync(random.Next(0, 5), -20);
                            }
                        }
                    }
                }
            }
        }

        private bool CheckTeamAlive(bool ally)
        {
            int count_team = 0;
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    //Map[i, j].Score = 0;
                    if (Map[i, j].Character != null)
                    {
                        if (Map[i, j].Character.Ally == ally & Map[i, j].Character.Alive == true)
                        {
                            count_team += 1;
                        }
                    }
                }
            }
            return count_team > 0;
        }

        private double CheckReward(bool ally, int x, int y)
        {
            // placeholder for a more complex reward function
            double reward = 0;

            return reward;
        }

        private void SelectToken(PictureBox picture)
        {
            // Show movement range when player clicks on token
            if (ReverseTokenTileCorrespondence[picture].Character != null)
            {
                if (ReverseTokenTileCorrespondence[picture].active == true & ReverseTokenTileCorrespondence[picture].Character.Ally == true)
                {
                    picture.BackColor = Color.DarkGreen;
                    SelectedToken = ReverseTokenTileCorrespondence[picture];
                    DisplayMovementRange(SelectedToken);
                }
            }
        }

        private void DisplayMovementRange(Token token)
        {
            // update colors to display movement range of selected character
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (Map[i, j].Character == null)
                    {
                        if (DistanceBetweenTokens(token, Map[i, j]) <= token.Character.MovementRange * GridSpacing)
                        {
                            Map[i, j].color = Color.GreenYellow;
                            TokenTileCorrespondence[Map[i, j]].BackColor = Map[i, j].color;
                        }
                    }
                    else
                    {
                        if (Map[i, j].Character.Alive == false & DistanceBetweenTokens(token, Map[i, j]) <= token.Character.MovementRange * GridSpacing)
                        {
                            Map[i, j].color = Color.GreenYellow;
                            TokenTileCorrespondence[Map[i, j]].BackColor = Map[i, j].color;
                        }
                    }
                }
            }
        }

        private void MoveOrAttack(PictureBox picture)
        {
            // Move or attack appropriately given a selected token and a target token
            var targettoken = (Token)ReverseTokenTileCorrespondence[picture];
            var selectedpicture = (PictureBox)TokenTileCorrespondence[SelectedToken];
            if (targettoken.Character == null)
            {
                if (DistanceBetweenTokens(SelectedToken,targettoken) <= SelectedToken.Character.MovementRange * GridSpacing)
                {
                    SelectedToken.Character.MovementRange -= DistanceBetweenTokens(SelectedToken, targettoken) / GridSpacing;
                    MoveCharacter(SelectedToken, targettoken);
                }
            }
            else
            {
                if (targettoken.Character.Alive == true)
                {
                    if (DistanceBetweenTokens(SelectedToken, targettoken) == GridSpacing & SelectedToken.Character.Ally != targettoken.Character.Ally)
                    {
                        SelectedToken.Attack(targettoken);
                    }
                    else
                    {
                        // penalty for invalid move
                        SelectedToken.Reward -= 0.1;
                    }
                }
                else
                {
                    if (DistanceBetweenTokens(SelectedToken, targettoken) <= SelectedToken.Character.MovementRange * GridSpacing)
                    {
                        SelectedToken.Character.MovementRange -= DistanceBetweenTokens(SelectedToken, targettoken) / GridSpacing;
                        MoveCharacter(SelectedToken, targettoken);
                    }
                }
            }
        }

        private async Task AITurnAsync(bool ally)
        {
            // This has the AI move all the characters available for a given team. ally == true for player characters, ally == false for enemy
            Random random = new Random();

            // Find tokens eligible for taking actions
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (Map[i, j].Character != null)
                    {
                        if (Map[i, j].Character.Ally == ally)
                        {
                            if (Map[i, j].active == true)
                            {
                                if (Map[i, j].Character.Alive == true)
                                {
                                    Character character = Map[i, j].Character;
                                    SelectedToken = Map[i, j];

                                    // Move token one step at a time so loop over movement range
                                    for (int k = 0; k < character.MovementRange; k++)
                                    {
                                        if (SelectedToken.active == true & character.Alive == true)
                                        {
                                            GenerateFeaturesForModel();
                                            SelectedToken.Reward = 0;
                                            await GetPredictionAsync();
                                            int[] targetcoordinates = DecodePrediction(Target, i, j);
                                            // PathfindingImproved(i, j, targetcoordinates[0], targetcoordinates[1], character.MovementRange);
                                            MoveOrAttack(TokenTileCorrespondence[Map[targetcoordinates[0], targetcoordinates[1]]]);
                                            SelectedToken.Reward += CheckReward(character.Ally, TokenTileCorrespondence[SelectedToken].Location.X / GridSpacing, TokenTileCorrespondence[SelectedToken].Location.Y / GridSpacing);
                                            if (ally)
                                            {
                                                AlliedReward += SelectedToken.Reward;
                                            }
                                            else
                                            {
                                                EnemyReward += SelectedToken.Reward;
                                            }
                                            UpdateColors();
                                            await SendRewardAsync(Target, SelectedToken.Reward);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Character character = Map[i, j].Character;
                                    SelectedToken = Map[i, j];
                                    GenerateFeaturesForModel();
                                    await SendRewardAsync(random.Next(0, 5), 0.0);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int[] DecodePrediction(int prediction, int x, int y)
        {
            // convert classification label to coordinates
            int[] temp_target = new int[2];
            if (prediction == 0)
            {
                temp_target[0] = x;
                temp_target[1] = y;
            }

            if (prediction == 1)
            {
                temp_target[0] = x + 1;
                temp_target[1] = y;
            }

            if (prediction == 2)
            {
                temp_target[0] = x - 1;
                temp_target[1] = y;
            }

            if (prediction == 3)
            {
                temp_target[0] = x;
                temp_target[1] = y + 1;
            }

            if (prediction == 4)
            {
                temp_target[0] = x;
                temp_target[1] = y - 1;
            }

            temp_target[0] = BindIntWithinMapWidth(temp_target[0]);
            temp_target[1] = BindIntWithinMapHeight(temp_target[1]);

            return temp_target;
        }

        private int BindIntWithinMapHeight(int y)
        {
            if(y< 0)
            {
                y = 0;
            }
            if(y>=MapHeight)
            {
                y = MapHeight-1;
            }
            return y;
        }

        private int BindIntWithinMapWidth(int x)
        {
            if(x < 0)
            {
                x = 0;
            }

            if(x >= MapWidth)
            {
                x = MapWidth-1;
            }
            return x;
        }

        private string CharacterInformation(Token token)
        {
            return token.Character.Name + System.Environment.NewLine + "HP: " + token.Character.Health + " / " + token.Character.MaxHealth
                + System.Environment.NewLine + "Strength: " + token.Character.Strength + System.Environment.NewLine + "Speed: " 
                + token.Character.Speed + System.Environment.NewLine + "Skill: " + token.Character.Skill + System.Environment.NewLine 
                + "Armor: " + token.Character.Armor + System.Environment.NewLine + "Magic: " + token.Character.Magic 
                + System.Environment.NewLine + "Resistance: " + token.Character.Resistance;
        }

        private int DistanceBetweenTokens(Token TokenA, Token TokenB)
        {
            PictureBox pictureA = TokenTileCorrespondence[TokenA];
            PictureBox pictureB = TokenTileCorrespondence[TokenB];
            return Math.Abs(pictureA.Location.X - pictureB.Location.X) + Math.Abs(pictureA.Location.Y - pictureB.Location.Y);
        }

        public void GenerateFeaturesForModel()
        {
            // creates a set of features from the perspective of the selected token
            // these features are passed to the ai to make a decision on the next action
            GameState = new int[StateLength * (2 * VisionLength - 1) * (2 * VisionLength - 1)];
            int count = 0;
            int Xposition = TokenTileCorrespondence[SelectedToken].Location.X / GridSpacing;
            int Yposition = TokenTileCorrespondence[SelectedToken].Location.Y / GridSpacing;
            Character SelectedCharacter = SelectedToken.Character;
            for (int i = -VisionLength + 1; i < VisionLength; i++)
            {
                for (int j = -VisionLength + 1; j < VisionLength; j++)
                {
                    if ((Xposition - i) >= 0 & (Xposition - i) < MapWidth)
                    {
                        if ((Yposition - j) >= 0 & (Yposition - j) < MapHeight)
                        {
                            int deltaX = Xposition - i;
                            int deltaY = Yposition - j;

                            if (Map[deltaX, deltaY].Character != null)
                            {
                                Character character = Map[deltaX, deltaY].Character;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = character.Health;
                                count += 1;
                                GameState[count] = character.Strength - SelectedCharacter.Armor;
                                count += 1;
                                GameState[count] = character.Skill - SelectedCharacter.Skill;
                                count += 1;
                                GameState[count] = character.Speed - SelectedCharacter.Speed;
                                count += 1;
                                GameState[count] = character.Magic - SelectedCharacter.Resistance;
                                count += 1;
                                GameState[count] = character.Armor - SelectedCharacter.Strength;
                                count += 1;
                                GameState[count] = character.Resistance - SelectedCharacter.Magic;
                                count += 1;
                                GameState[count] = character.MaxMovementRange;
                                count += 1;
                                if (character.Ally == SelectedCharacter.Ally)
                                {
                                    GameState[count] = -1;
                                    count += 1;
                                }
                                else
                                {
                                    GameState[count] = 1;
                                    count += 1;
                                }

                            }
                            else
                            {
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                                GameState[count] = 0;
                                count += 1;
                            }
                        }
                        else
                        {
                            GameState[count] = 1;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                            GameState[count] = 0;
                            count += 1;
                        }
                    }
                    else
                    {
                        GameState[count] = 1;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                        GameState[count] = 0;
                        count += 1;
                    }
                }
            }
        }

        private async Task GetPredictionAsync()
        {
            // send game state and wait for prediction from ai
            var values = new Dictionary<string, string>{};
            int row_number = 0;
            foreach(int i in GameState)
            {
                values.Add(row_number.ToString(), i.ToString());
                row_number++;
            }
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://127.0.0.1:5000/predict", content);
            var responseString = await response.Content.ReadAsStringAsync();
            Target = int.Parse(responseString);
        }

        private async Task SendRewardAsync(int action, double reward)
        {
            // send state, action, next state, and reward for deep q-learning
            var values = new Dictionary<string, string> { };
            values.Add("action", action.ToString());
            values.Add("reward", reward.ToString());
            int row_number = 0;
            foreach (int i in GameState)
            {
                values.Add(row_number.ToString(), i.ToString());
                row_number++;
            }

            GenerateFeaturesForModel();
            foreach (int i in GameState)
            {
                values.Add(row_number.ToString(), i.ToString());
                row_number++;
            }

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://127.0.0.1:5000/save", content);
        }

        private async Task SendTrainCommand(int batches)
        {
            // tell AI to update based on new data
            var values = new Dictionary<string, string> { };
            values.Add("batches", batches.ToString());
     
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("http://127.0.0.1:5000/train", content);
        }
    }
}
