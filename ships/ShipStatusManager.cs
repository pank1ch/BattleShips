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

namespace ships
{
    internal class ShipStatusManager
    {
        private List<Label> shipLabels;
        private List<TextBox> shipStatusTextBoxes;
        public ShipStatusManager(List<Label> shipLabels, List<TextBox> shipStatusTextBoxes)
        {
            this.shipLabels = shipLabels;
            this.shipStatusTextBoxes = shipStatusTextBoxes;
        }


        public void InstallShipStatus(AbstractShip currentShip)
        {
            foreach (var label in shipLabels)
            {
                if (label.Name.Contains(currentShip.name))
                {
                    if (label.Tag.ToString() == "Empty")
                    {
                        currentShip.shipLabel = label;

                        label.Tag = "Occupied";

                        currentShip.shipStatusBox = shipStatusTextBoxes.Find(textBox => (textBox.Tag.ToString() == label.Name.ToString()));
                        break;

                    }
                }

            }
        }
        public void ShowShipStatus(AbstractShip currentShip)
        {

            switch (currentShip.isAlive)
            {
                case true:

                    currentShip.shipLabel.ForeColor = Color.Green;
                    currentShip.shipStatusBox.Text = "В строю";
                    currentShip.shipStatusBox.BackColor = Color.Green;
                    break;
                case false:
                    currentShip.shipLabel.ForeColor = Color.Red;
                    currentShip.shipStatusBox.Text = "Уничтожен";
                    currentShip.shipStatusBox.BackColor = Color.Red;
                    break;

            }

        }
    }


}
