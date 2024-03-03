using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ships
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            versionLabel.Text += " "  + Properties.Settings.Default.Version;
            versionLabel.Size = new Size(this.Width - 15, 26);
            versionLabel.AutoSize = false;
            this.MaximizeBox = false;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Game game = new Game();

            this.Hide();
            game.Show();
        }

        
    }
}
