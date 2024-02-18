using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


// разработал позиционирование кораблей во все 4 стороны,
// теперь нужно добавить смену сторон расположения корабля (кнопку)
// фиксануть корабль из одной клетки (катер)
// начать думать над логикой предотвращения столкновения бортов

//стороны работают, осталось логику предотвращения столкновения бортов.





// дорабоать логику столкновения бортов (с первой коордиантой все работает, осталось с дополнительными решить
// Я ПОНЯЛ СУТЬ ошибки, кнопки появляются в рандомном порядке, поэтому херня выходит,

// коллизия работает осталось доработать ошибки;

// установка кораблей противника наконец-то заработала, благодаря этому каждая игра будет уникальной





namespace ships
{
    public partial class Game : Form
    {
        private Player player;
        private Player computer;
                    
        private List<Button> playingFieldButtons;
        private List<Label> shipLabels;
        private List<TextBox> shipStatusTextBoxes;
       
        private ShipsLocating shipsLocating;
        private ShipsBattle shipsBattle;

        private ShipStatusManager shipStatusManager;

        private List<GameCoordinate> playingFieldCoordinates;

        public Game()
        {
            InitializeComponent();

            
            StartBattleButton.BackColor = Color.FromArgb(100, StartBattleButton.BackColor);
        
            switchFieldButton.BackColor = Color.FromArgb(100, switchFieldButton.BackColor);


            playingFieldButtons = new List<Button>();
            foreach (var button in Controls.OfType<Button>())
            {
                if (button.Name.Contains("Game")){
                    playingFieldButtons.Add(button);
                }
            }

            shipLabels = new List<Label>();
            foreach (var label in Controls.OfType<Label>())
            {
                if (label.Name.Contains("Ship"))
                {
                    shipLabels.Add(label);
                }
            }

            shipStatusTextBoxes = new List<TextBox>();
            foreach (var textBox in Controls.OfType<TextBox>())
            {
                shipStatusTextBoxes.Add(textBox);
            }

            playingFieldCoordinates = new List<GameCoordinate>();
            foreach (var playingFieldButton in playingFieldButtons)
            {
                GameCoordinate gameCoordinate = new GameCoordinate(playingFieldButton);
                playingFieldCoordinates.Add(gameCoordinate);
            }


            FillShipTable();

            player = new Player(false);
            computer = new Player(true);
            shipStatusManager = new ShipStatusManager(shipLabels, shipStatusTextBoxes);

            shipsLocating = new ShipsLocating(
                player,
                computer,
                dataGridView1,
                playingFieldButtons,
                playingFieldCoordinates,
                directionLabel,
                shipLabels,
                shipStatusTextBoxes,
                shipStatusManager);

            shipsBattle = new ShipsBattle(
                player,
                computer,
                playingFieldCoordinates,
                shipStatusManager,
                finishBattleButton
                );

            

            foreach (var button in playingFieldButtons)
            {
                button.Click += shipsLocating.GameFieldButton_Click;
            }
        }


        private void FillShipTable()
        {
            dataGridView1.Rows.Add("Линкор");
            dataGridView1.Rows.Add("Крейсер");
            dataGridView1.Rows.Add("Крейсер");
            dataGridView1.Rows.Add("Эсминец");
            dataGridView1.Rows.Add("Эсминец");
            dataGridView1.Rows.Add("Эсминец");
            dataGridView1.Rows.Add("Катер");
            dataGridView1.Rows.Add("Катер");
            dataGridView1.Rows.Add("Катер");
            dataGridView1.Rows.Add("Катер");
        }

        private void StartInstallingButton_Click(object sender, EventArgs e)
        {
            shipsLocating.StartLocating();
        }


        private void changeDirectionbutton_Click(object sender, EventArgs e)
        {
            shipsLocating.ChangeShipDirection();
        }

        private void confirmInstalationButton_Click(object sender, EventArgs e)
        {
            shipsLocating.ConfirmShipInstallation();

            if (dataGridView1.RowCount == 0)
            {
                StartInstallingButton.Enabled = false;
                StartInstallingButton.BackColor = Color.FromArgb(100, StartInstallingButton.BackColor);

                cancelInstallationButton.Enabled = false;
                cancelInstallationButton.BackColor = Color.FromArgb(100, cancelInstallationButton.BackColor);

                changeDirectionbutton.Enabled = false;
                changeDirectionbutton.BackColor = Color.FromArgb(100, changeDirectionbutton.BackColor);
                directionLabel.Visible = false;


                confirmInstalationButton.Enabled = false;
                confirmInstalationButton.BackColor = Color.FromArgb(100, confirmInstalationButton.BackColor);



                dataGridView1.Visible = false;

                StartBattleButton.Enabled = true;
                StartBattleButton.BackColor = Color.FromArgb(255, StartBattleButton.BackColor);

                
            }
            


        }

        private void cancelInstallationButton_Click(object sender, EventArgs e)
        {
            shipsLocating.CancelShipInstallation();

        } 

        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void StartBattleButton_Click(object sender, EventArgs e)
        {
            foreach (var button in playingFieldButtons)
            {
                button.Click -= shipsLocating.GameFieldButton_Click;
                button.BackColor = Color.White;
            }

            foreach (var button in playingFieldButtons)
            {
                button.Click += shipsBattle.GameFieldButton_Click;
            }


            shipsLocating.InstallEnemyShips();

            foreach (var ship in computer.fleet)
            {
                shipStatusManager.ShowShipStatus(ship);
            }
            mainLabel.Text = "Флот противника";

            switchFieldButton.Enabled = true;
            switchFieldButton.BackColor = Color.FromArgb(255, switchFieldButton.BackColor);

            StartBattleButton.Enabled = false;
            StartBattleButton.BackColor = Color.FromArgb(100, StartBattleButton.BackColor);

            MessageBox.Show("Игра началась, атакуйте противника на игровом поле!");




        }



        
        private void TestButton_Click(object sender, EventArgs e)
        {
            foreach (var enemyship in computer.fleet)
            {
                foreach (var shipcoordinate in enemyship.location)
                {
                    shipcoordinate.CoordinateButton.BackColor = Color.White;
                }
                enemyship.location.Clear();
            }


            shipsLocating.InstallEnemyShips();

            foreach (var enemyship in computer.fleet)
            {
                //MessageBox.Show(enemyship.name);
                foreach (var shipcoordinate in enemyship.location)
                {
                    shipcoordinate.CoordinateButton.BackColor = Color.Red;
                }

            }


        }

        private void Game_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("Разместите все корабли на игровом поле");
        }

        private void switchFieldButton_Click(object sender, EventArgs e)
        {
            shipsBattle.SwitchGameField(switchFieldButton, mainLabel);
            
        }

        private void finishBattleButton_Click(object sender, EventArgs e)
        {
            MainMenu MainMenuForm = new MainMenu();

            this.Hide();

            MainMenuForm.Show();
        }
    }
}
