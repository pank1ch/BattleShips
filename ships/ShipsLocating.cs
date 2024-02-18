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
    internal class ShipsLocating
    {

        enum shipDirections
        {
            Top = 1,
            Right = 2,
            Bottom = 3,
            Left = 4
        }
        private Player player;
        private Player computer;

        private DataGridView shipTable;
        private DataGridViewRow selectedShipRow;

        

        private Button firstClickedButton;

        private AbstractShip currentShip;
        private string shipFirstLocation;
        private string shipType;
        //private int shipLocationsCount;
        
        private shipDirections shipDirection = shipDirections.Top;
        private int directionNumber = 1;

        private bool isShipLocateAllowed = false;
        private bool isShipLocating = false;

        private GameAlerts gameAlerts;
        private ShipStatusManager shipStatusManager;

        private List<Button> playingFieldButtons;
        private List<Label> shipLabels;
        private List<TextBox> shipStatusTextBoxes;
        private List<GameCoordinate> playingFieldCoordinates;

        private Label directionLabel;

        public ShipsLocating(Player player, Player computer, DataGridView shipTable, List<Button> playingFieldButtons, List<GameCoordinate> playingFieldCoordinates, Label directionLabel, List<Label> shipLabels, List<TextBox> shipStatusTextBoxes, ShipStatusManager shipStatusManager)
        {
            this.player = player;
            this.computer = computer;
            this.shipTable = shipTable; 
            this.playingFieldButtons = playingFieldButtons;
            this.playingFieldCoordinates = playingFieldCoordinates; 
            this.directionLabel = directionLabel;
            this.shipLabels = shipLabels;
            this.shipStatusTextBoxes = shipStatusTextBoxes;
            gameAlerts = new GameAlerts();
            this.shipStatusManager = shipStatusManager;
        }

        public void GameFieldButton_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.Name.Contains("Game"))
            {
                if (clickedButton.BackColor == Color.White && isShipLocateAllowed)
                {
                    firstClickedButton = clickedButton;
                    //shipFirstLocation = firstClickedButton.Tag.ToString();
                    ShipInstallation();

                    if (!(currentShip.location.Count == 0))
                    {
                        isShipLocateAllowed = false;
                    }
                }
            }
        }

        public void StartLocating()
        {
            if (isShipLocating)
            {             
                MessageBox.Show(gameAlerts.ShipIsLocatingAlert());
                return;

            }
            
            if (shipTable.SelectedRows.Count > 0)
            {
                // Получаем ссылку на выбранную строку

                selectedShipRow = shipTable.SelectedRows[0];

                var shipValueFromTable = selectedShipRow.Cells["Корабли"].Value;

                if (shipValueFromTable != null)
                {
                    // Преобразуем значение в строку и используем его
                    shipType = shipValueFromTable.ToString();

                }
            }

            switch (shipType)
            {
                case "Линкор":               
                    currentShip = new FourDeckship();
                    break;

                case "Крейсер":
                    currentShip = new ThreeDeckShip();                    
                    break;

                case "Эсминец":
                    currentShip = new DoubleDeckShip();               
                   break;

                case "Катер":
                    currentShip = new SingleDeckShip();                  
                    break;
            }

            isShipLocating = true;
            isShipLocateAllowed = true;
            MessageBox.Show("Разместите корабль");
        }

        public void ShipInstallation()
        {

            GameCoordinate shipFirstCoordinate = new GameCoordinate(firstClickedButton);     
            
            // ------------------------Код для нахождения соседних клеток ---------------------------------//

            if (ShipCollision(shipFirstCoordinate, currentShip, player))
            {
                MessageBox.Show(gameAlerts.ShipsCollisionAlert());
                return;
            }

            // ----------------------------------------------------------------------------------------------------
        
            currentShip.location.Add(shipFirstCoordinate);

            currentShip.location[0].CoordinateButton.BackColor = Color.Blue;
            

            int lowerBound;
            int upperBound;

            GameCoordinate currentShipCoordinate;

            switch (shipDirection)
            {
                case shipDirections.Top:
                    if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    {
                        MessageBox.Show("Корабль выходит за пределы поля");
                        return;
                    }
                    lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) - (currentShip.coordinatesForShipCount - 1);
                    upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) - 1;



                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                        {
                            break;
                        }

                        currentShipCoordinate = playingFieldCoordinate;

                        if (shipFirstCoordinate.CoordinateLetter == currentShipCoordinate.CoordinateLetter)
                        {
                            // Проверяем, попадает ли случайное число в диапазон
                            if (Convert.ToInt32(currentShipCoordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateNumber) <= upperBound)
                            {

                                if (currentShipCoordinate.CoordinateButton.BackColor == Color.Blue)
                                {
                                    MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                                    return;
                                }

                                if (ShipCollision(currentShipCoordinate, currentShip, player))
                                {
                                    MessageBox.Show("Невозможно установить корабль, судна сталкиваются бортами");

                                    return;
                                }
                                currentShipCoordinate.CoordinateButton.BackColor = Color.Blue;
                                currentShip.location.Add(currentShipCoordinate);
                            }
                        }
                    }
                    break;

                case shipDirections.Bottom:

                    if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    {
                        MessageBox.Show("Корабль выходит за пределы поля");
                        return;
                    }

                    lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) + 1;
                    upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) + (currentShip.coordinatesForShipCount - 1);

                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                        {
                            break;
                        }
                        currentShipCoordinate = playingFieldCoordinate;


                        if (shipFirstCoordinate.CoordinateLetter == currentShipCoordinate.CoordinateLetter)
                        {


                            // Проверяем, попадает ли случайное число в диапазон
                            if (Convert.ToInt32(currentShipCoordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateNumber) <= upperBound)
                            {
                                if (currentShipCoordinate.CoordinateButton.BackColor == Color.Blue)
                                {
                                    MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                                    return;
                                }

                                if (ShipCollision(currentShipCoordinate, currentShip, player))
                                {
                                    MessageBox.Show("Невозможно установить корабль, судна сталкиваются бортами");

                                    return;
                                }

                                currentShipCoordinate.CoordinateButton.BackColor = Color.Blue;
                                currentShip.location.Add(currentShipCoordinate);

                            }
                        }
                    }
                    break;


                case shipDirections.Left:

                    if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    {
                        MessageBox.Show("Корабль выходит за пределы поля");
                        return;
                    }

                    upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) - 1;
                    lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) - (currentShip.coordinatesForShipCount - 1);

                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                        {
                            break;
                        }

                        currentShipCoordinate = playingFieldCoordinate;


                        if (shipFirstCoordinate.CoordinateNumber == currentShipCoordinate.CoordinateNumber)
                        {


                            // Проверяем, попадает ли случайное число в диапазон
                            if (Convert.ToInt32(currentShipCoordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateLetter) <= upperBound)
                            {
                                if (currentShipCoordinate.CoordinateButton.BackColor == Color.Blue)
                                {
                                    MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                                    return;
                                }

                                if (ShipCollision(currentShipCoordinate, currentShip, player))
                                {
                                    MessageBox.Show("Невозможно установить корабль, судна сталкиваются бортами");

                                    return;
                                }

                                currentShipCoordinate.CoordinateButton.BackColor = Color.Blue;
                                currentShip.location.Add(currentShipCoordinate);

                            }

                        }
                    }
                    break;


                case shipDirections.Right:

                    if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    {
                        MessageBox.Show("Корабль выходит за пределы поля");
                        return;
                    }

                    lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) + 1;
                    upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) + (currentShip.coordinatesForShipCount - 1);

                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                        {
                            break;
                        }

                        currentShipCoordinate = playingFieldCoordinate;


                        if (shipFirstCoordinate.CoordinateNumber == currentShipCoordinate.CoordinateNumber)
                        {


                            // Проверяем, попадает ли случайное число в диапазон
                            if (Convert.ToInt32(currentShipCoordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateLetter) <= upperBound)
                            {

                                if (currentShipCoordinate.CoordinateButton.BackColor == Color.Blue)
                                {
                                    MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                                    return;
                                }

                                if (ShipCollision(currentShipCoordinate, currentShip, player))
                                {
                                    MessageBox.Show("Невозможно установить корабль, судна сталкиваются Бортами");

                                    return;
                                }

                                currentShipCoordinate.CoordinateButton.BackColor = Color.Blue;
                                currentShip.location.Add(currentShipCoordinate);
                            }
                        }
                    }
                    break;
            }

                    //switch (shipDirection)
                    //{
                    //    case shipDirections.Top:
                    //        if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    //        {
                    //            MessageBox.Show("Корабль выходит за пределы поля");
                    //            return;
                    //        }
                    //        lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) - (currentShip.coordinatesForShipCount - 1);
                    //        upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) - 1;



                    //        foreach (var playingButton in playingFieldButtons)
                    //        {
                    //            if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                    //            {
                    //                break;
                    //            }

                    //            currentShipCoordinate = new GameCoordinate(playingButton.Tag.ToString());

                    //            if (shipFirstCoordinate.CoordinateLetter == currentShipCoordinate.CoordinateLetter)
                    //            {
                    //                // Проверяем, попадает ли случайное число в диапазон
                    //                if (Convert.ToInt32(currentShipCoordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateNumber) <= upperBound)
                    //                {

                    //                    if (playingButton.BackColor == Color.Blue)
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                    //                        return;
                    //                    }

                    //                    if (ShipCollision(currentShipCoordinate, currentShip, player))
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, судна сталкиваются бортами");

                    //                        return;
                    //                    }
                    //                    playingButton.BackColor = Color.Blue;
                    //                    currentShip.location.Add(currentShipCoordinate);
                    //                }
                    //            }
                    //        }
                    //        break;

                    //    case shipDirections.Bottom:

                    //        if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    //        {
                    //            MessageBox.Show("Корабль выходит за пределы поля");
                    //            return;
                    //        }

                    //        lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) + 1;
                    //        upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) + (currentShip.coordinatesForShipCount - 1);

                    //        foreach (var playingButton in playingFieldButtons)
                    //        {
                    //            if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                    //            {
                    //                break;
                    //            }
                    //            currentShipCoordinate = new GameCoordinate(playingButton.Tag.ToString());




                    //            if (shipFirstCoordinate.CoordinateLetter == currentShipCoordinate.CoordinateLetter)
                    //            {


                    //                // Проверяем, попадает ли случайное число в диапазон
                    //                if (Convert.ToInt32(currentShipCoordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateNumber) <= upperBound)
                    //                {
                    //                    if (playingButton.BackColor == Color.Blue)
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                    //                        return;
                    //                    }

                    //                    if (ShipCollision(currentShipCoordinate, currentShip, player))
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, судна сталкиваются бортами");

                    //                        return;
                    //                    }

                    //                    playingButton.BackColor = Color.Blue;
                    //                    currentShip.location.Add(currentShipCoordinate);

                    //                }
                    //            }
                    //        }
                    //        break;


                    //    case shipDirections.Left:

                    //        if (OutOfBoundsCheck(shipFirstCoordinate, currentShip, shipDirection) == true)
                    //        {
                    //            MessageBox.Show("Корабль выходит за пределы поля");
                    //            return;
                    //        }

                    //        upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) - 1;
                    //        lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) - (currentShip.coordinatesForShipCount - 1);

                    //        foreach (var playingButton in playingFieldButtons)
                    //        {
                    //            if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                    //            {
                    //                break;
                    //            }

                    //            currentShipCoordinate = new GameCoordinate(playingButton.Tag.ToString());


                    //            if (shipFirstCoordinate.CoordinateNumber == currentShipCoordinate.CoordinateNumber)
                    //            {


                    //                // Проверяем, попадает ли случайное число в диапазон
                    //                if (Convert.ToInt32(currentShipCoordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateLetter) <= upperBound)
                    //                {
                    //                    if (playingButton.BackColor == Color.Blue)
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                    //                        return;
                    //                    }

                    //                    if (ShipCollision(currentShipCoordinate, currentShip, player))
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, судна сталкиваются бортами");

                    //                        return;
                    //                    }

                    //                    playingButton.BackColor = Color.Blue;
                    //                    currentShip.location.Add(currentShipCoordinate);

                    //                }

                    //            }
                    //        }
                    //        break;


                    //    case shipDirections.Right:

                    //        if (OutOfBoundsCheck(shipFirstCoordinate,currentShip,shipDirection) == true)
                    //        {
                    //            MessageBox.Show("Корабль выходит за пределы поля");
                    //            return;
                    //        }

                    //        lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) + 1;
                    //        upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) + (currentShip.coordinatesForShipCount - 1);

                    //        foreach (var button in playingFieldButtons)
                    //        {
                    //            if (currentShip.location.Count == currentShip.coordinatesForShipCount)
                    //            {
                    //                break;
                    //            }

                    //            currentShipCoordinate = new GameCoordinate(button.Tag.ToString());


                    //            if (shipFirstCoordinate.CoordinateNumber == currentShipCoordinate.CoordinateNumber)
                    //            {


                    //                // Проверяем, попадает ли случайное число в диапазон
                    //                if (Convert.ToInt32(currentShipCoordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateLetter) <= upperBound)
                    //                {

                    //                    if (button.BackColor == Color.Blue)
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, установке мешает другое судно");
                    //                        return;
                    //                    }

                    //                    if (ShipCollision(currentShipCoordinate, currentShip, player))
                    //                    {
                    //                        MessageBox.Show("Невозможно установить корабль, судна сталкиваются Бортами");

                    //                        return;
                    //                    }

                    //                    button.BackColor = Color.Blue;
                    //                    currentShip.location.Add(currentShipCoordinate);
                    //                }
                    //            }
                    //        }
                    //        break;
                    //}
        }

        public bool ShipCollision(GameCoordinate shipCoordinate, AbstractShip currentShip, Player player)
        {
            List<Button> nearestButtons = new List<Button>();
            List<GameCoordinate> nearestCoordinates = new List<GameCoordinate>();

            char leftNearestLetter = Convert.ToChar(Convert.ToInt32(shipCoordinate.CoordinateLetter) - 1);

            GameCoordinate leftNearestCoordinate =  new GameCoordinate(Convert.ToString(leftNearestLetter) + "-" + shipCoordinate.CoordinateNumber);

            char rightNearestLetter = Convert.ToChar(Convert.ToInt32(shipCoordinate.CoordinateLetter) + 1);
            GameCoordinate rightNearestCoordinate = new GameCoordinate(Convert.ToString(rightNearestLetter) + "-" + shipCoordinate.CoordinateNumber);

            GameCoordinate topNearestCoordinate = new GameCoordinate(Convert.ToString(shipCoordinate.CoordinateLetter) + "-" + Convert.ToString(Convert.ToInt32(shipCoordinate.CoordinateNumber) - 1));

            GameCoordinate bottomNearestCoordinate = new GameCoordinate(Convert.ToString(shipCoordinate.CoordinateLetter) + "-" + Convert.ToString(Convert.ToInt32(shipCoordinate.CoordinateNumber) + 1));

            nearestCoordinates.Add(leftNearestCoordinate);         
            nearestCoordinates.Add(rightNearestCoordinate);     
            nearestCoordinates.Add(topNearestCoordinate); 
            nearestCoordinates.Add(bottomNearestCoordinate);
           
            foreach (var nearestCoordinate in nearestCoordinates)
            {
                Button nearestButton = playingFieldButtons.Find(button => button.Tag.ToString() == nearestCoordinate.CoordinateLocation);
                if (nearestButton != null)
                {
                    nearestButtons.Add(nearestButton);
                }
            }

            bool isInShipCoordinates = false;

            foreach (var nearestButton in nearestButtons)
            {
                isInShipCoordinates = false;
                foreach (var shipLocation in currentShip.location)
                {

                    if (nearestButton.Tag.ToString() == shipLocation.CoordinateLocation)
                    {

                        isInShipCoordinates = true;
                        break;
                    }


                }
                if (!isInShipCoordinates)
                {
                    foreach (var ship in player.fleet)
                    {
                        foreach (var currentShipCoordinate in ship.location)
                        {
                            if (nearestButton.Tag.ToString() == currentShipCoordinate.CoordinateLocation)
                            {
                                //MessageBox.Show(nearestButton.Tag.ToString());
                                return true;
                            }
                        }
                    }

                    //if (nearestButton.BackColor == Color.Blue)
                    //{

                    //    MessageBox.Show(nearestButton.Tag.ToString());
                    //    return true;
                    //}
                }
            }

            return false;
        }

        public void ChangeShipDirection()
        {
            if (isShipLocating)
            {

                if (firstClickedButton == null)
                {
                    MessageBox.Show(gameAlerts.ShipIsNotOnFieldAlert());
                    return;
                }

                foreach (var location in currentShip.location)
                {

                    Button shipButton = playingFieldButtons.Find(button => button.Tag.ToString() == location.CoordinateLocation);

                    shipButton.BackColor = Color.White;
               
                }

                currentShip.location.Clear();

                if (directionNumber == 4)
                {
                    directionNumber = 1;
                }
                else
                {
                    directionNumber++;
                }

                switch (directionNumber)
                {
                    case 1:
                        shipDirection = shipDirections.Top;
                        break;
                    case 2:
                        shipDirection = shipDirections.Right;
                        break;
                    case 3:
                        shipDirection = shipDirections.Bottom;
                        break;
                    case 4:
                        shipDirection = shipDirections.Left;
                        break;
                    default:
                        break;
                }
                directionLabel.Text = shipDirection.ToString();

                ShipInstallation();
            }
            else
            {
                MessageBox.Show(gameAlerts.ShipIsNotOnFieldAlert());
            }
        }

        public void ConfirmShipInstallation()
        {
            if (isShipLocating)
            {
                if (firstClickedButton == null)
                {
                    MessageBox.Show(gameAlerts.ShipIsNotOnFieldAlert());
                    return;
                }

                if (currentShip.location.Count < currentShip.coordinatesForShipCount)
                {
                    MessageBox.Show(gameAlerts.ShipIsNotFullyLocatedAlert());
                    return;
                }

                shipStatusManager.InstallShipStatus(currentShip);
                shipStatusManager.ShowShipStatus(currentShip);
              
                player.fleet.Add(currentShip);

                isShipLocating = false;
                shipTable.Rows.Remove(selectedShipRow);
                firstClickedButton = null;

                
            }
            else
            {
                MessageBox.Show(gameAlerts.ShipIsNotOnFieldAlert());
            }
        }

        public void CancelShipInstallation()
        {
            if (isShipLocating)
            {

                if (firstClickedButton == null)
                {
                    MessageBox.Show(gameAlerts.ShipIsNotOnFieldAlert());
                    return;
                }

                foreach (var location in currentShip.location)
                {
                    foreach (var button in playingFieldButtons)
                    {
                        if (button.Tag.ToString() == location.CoordinateLocation)
                        {
                            button.BackColor = Color.White;
                        }
                    }
                }

                currentShip.location.Clear();
                isShipLocateAllowed = true;
                
                firstClickedButton = null;

            }
            else
            {
                MessageBox.Show(gameAlerts.ShipIsNotOnFieldAlert());
            }
        }


        //private void InstallShipStatus(AbstractShip currentShip)
        //{
        //    foreach (var label in shipLabels)
        //    {
        //        if (label.Name.Contains(currentShip.name))
        //        {
        //            if (label.Tag.ToString() == "Empty")
        //            {
        //                currentShip.shipLabel = label;

        //                label.Tag = "Occupied";

        //                currentShip.shipStatusBox = shipStatusTextBoxes.Find(textBox => (textBox.Tag.ToString() == label.Name.ToString()));                       
        //                break;

        //            }
        //        }

        //    }
        //}
        //public void ShowShipStatus(AbstractShip currentShip)
        //{

        //    switch (currentShip.isAlive)
        //    {
        //        case true:

        //            currentShip.shipLabel.ForeColor= Color.Green;
        //            currentShip.shipStatusBox.Text = "В строю";
        //            currentShip.shipStatusBox.BackColor = Color.Green;
        //            break;
        //        case false:
        //            currentShip.shipLabel.ForeColor = Color.Red;
        //            currentShip.shipStatusBox.Text = "Уничтожен";
        //            currentShip.shipStatusBox.BackColor = Color.Red;
        //            break;
                
        //    }
           
        //}

        // работает
        public void InstallEnemyShips()
        {
            foreach (var label in shipLabels)
            {
                label.Tag = "Empty";
            }


            foreach (var enemyShip in computer.fleet)
            {

                shipStatusManager.InstallShipStatus(enemyShip);

                Random random = new Random();

                
                
                int lowerBound;
                int upperBound;
                bool notFullyInstalled = true;
                GameCoordinate currentShipCoordinate;
                List<GameCoordinate> possibleShipCoordinates;
                List<GameCoordinate> bestShipCoordinates;

                InstallFirstEnemyShipCoordinate(enemyShip);

                shipDirections currentShipDirection = (shipDirections)random.Next(1, 4);
                GameCoordinate shipFirstCoordinate = enemyShip.location[0];


                int dontFitDirectionsCount = 0;

                while (notFullyInstalled)
                {
                    if (dontFitDirectionsCount == 4)
                    {
                        InstallFirstEnemyShipCoordinate(enemyShip);
                        shipFirstCoordinate = enemyShip.location[0];
                        dontFitDirectionsCount = 0;
                    }



                    



                    switch (currentShipDirection)
                    {
                        case shipDirections.Top:

                            if (OutOfBoundsCheck(shipFirstCoordinate, enemyShip, currentShipDirection) == true)
                            {
                                currentShipDirection = shipDirections.Right;
                                break;
                            }
                            
                            lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) - (enemyShip.coordinatesForShipCount - 1);
                            upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) - 1;

                                 
                            possibleShipCoordinates = playingFieldCoordinates.Where(coordinate => coordinate.CoordinateLetter == shipFirstCoordinate.CoordinateLetter).ToList();
                            bestShipCoordinates = possibleShipCoordinates.Where(coordinate => (Convert.ToInt32(coordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(coordinate.CoordinateNumber) <= upperBound)).ToList();
                            foreach (var bestShipCoordinate in bestShipCoordinates)
                            {
                                currentShipCoordinate = bestShipCoordinate;
                                // Проверяем, попадает ли случайное число в диапазон
                                
                                if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                                {
                                    if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                                    {
                                        enemyShip.location.Add(currentShipCoordinate);
                                                
                                    }

                                }
                                            
                            }

                            if (enemyShip.location.Count < enemyShip.coordinatesForShipCount)
                            {
                                enemyShip.location.Clear();
                                currentShipDirection = shipDirections.Right;
                                dontFitDirectionsCount++;
                                break;
                            }
                            else
                            {
                                notFullyInstalled = false;
                            }

                            
                            
                            break;



                            //foreach (var playingButton in playingField)
                            //{
                            //    if (enemyShip.location.Count == enemyShip.coordinatesForShipCount)
                            //    {
                            //        notFullyInstalled = false;
                            //        break;
                            //    }

                            //    currentShipCoordinate = new ShipCoordinate(playingButton.Tag.ToString());

                            //    if (shipFirstCoordinate.CoordinateLetter == currentShipCoordinate.CoordinateLetter)
                            //    {
                            //        // Проверяем, попадает ли случайное число в диапазон
                            //        if (Convert.ToInt32(currentShipCoordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateNumber) <= upperBound)
                            //        {

                            //            if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                            //            {                                           
                            //                if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                            //                {
                            //                    enemyShip.location.Add(currentShipCoordinate);
                            //                }
                                            
                            //            }                 
                            //        }
                            //    }
                            //}

                            //break;

                        case shipDirections.Right:
                            if (OutOfBoundsCheck(shipFirstCoordinate, enemyShip, currentShipDirection) == true)
                            {
                                currentShipDirection = shipDirections.Bottom;
                                break;
                            }
                       
                            lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) + 1;
                            upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) + (enemyShip.coordinatesForShipCount - 1);

                                
                            possibleShipCoordinates = playingFieldCoordinates.Where(coordinate => coordinate.CoordinateNumber == shipFirstCoordinate.CoordinateNumber).ToList();
                            bestShipCoordinates = possibleShipCoordinates.Where(coordinate => (Convert.ToInt32(coordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(coordinate.CoordinateLetter) <= upperBound)).ToList();
                            foreach (var bestShipCoordinate in bestShipCoordinates)
                            {
                                currentShipCoordinate = bestShipCoordinate;
                                // Проверяем, попадает ли случайное число в диапазон

                                if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                                {
                                    if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                                    {
                                        enemyShip.location.Add(currentShipCoordinate);

                                    }

                                }

                            }



                            

                            if (enemyShip.location.Count < enemyShip.coordinatesForShipCount)
                            {
                                enemyShip.location.Clear();
                                currentShipDirection = shipDirections.Bottom;
                                dontFitDirectionsCount++;
                                break;
                            }
                            else
                            {
                                notFullyInstalled = false;
                            }

                           
                            break;


                            //foreach (var playingButton in playingField)
                            //{
                            //    if (enemyShip.location.Count == enemyShip.coordinatesForShipCount)
                            //    {
                            //        notFullyInstalled = false;
                            //        break;
                            //    }

                            //    currentShipCoordinate = new ShipCoordinate(playingButton.Tag.ToString());

                            //    if (shipFirstCoordinate.CoordinateNumber == currentShipCoordinate.CoordinateNumber)
                            //    {
                            //        Проверяем, попадает ли случайное число в диапазон
                            //        if (Convert.ToInt32(currentShipCoordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateLetter) <= upperBound)
                            //        {

                            //            if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                            //            {
                            //                if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                            //                {
                            //                    enemyShip.location.Add(currentShipCoordinate);
                            //                }

                            //            }
                            //        }
                            //    }
                            //}


                            //break;

                        case shipDirections.Bottom:
                            if (OutOfBoundsCheck(shipFirstCoordinate, enemyShip, currentShipDirection) == true)
                            {
                                currentShipDirection = shipDirections.Left;
                                break;
                            }

                            lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) + 1;
                            upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateNumber) + (enemyShip.coordinatesForShipCount - 1);



                            //Button playingButton = playingField[random.Next(0, playingField.Count)];



                            possibleShipCoordinates = playingFieldCoordinates.Where(coordinate => coordinate.CoordinateLetter == shipFirstCoordinate.CoordinateLetter).ToList();
                            bestShipCoordinates = possibleShipCoordinates.Where(coordinate => (Convert.ToInt32(coordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(coordinate.CoordinateNumber) <= upperBound)).ToList();
                            foreach (var bestShipCoordinate in bestShipCoordinates)
                            {
                                currentShipCoordinate = bestShipCoordinate;
                                // Проверяем, попадает ли случайное число в диапазон

                                if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                                {
                                    if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                                    {
                                        enemyShip.location.Add(currentShipCoordinate);

                                    }

                                }

                            }

                            if (enemyShip.location.Count < enemyShip.coordinatesForShipCount)
                            {
                                enemyShip.location.Clear();
                                currentShipDirection = shipDirections.Left;
                                dontFitDirectionsCount++;
                                break;
                            }
                            else
                            {
                                notFullyInstalled = false;
                            }                        
                            break;


                            //foreach (var playingButton in playingField)
                            //{
                            //    if (enemyShip.location.Count == enemyShip.coordinatesForShipCount)
                            //    {
                            //        notFullyInstalled = false;
                            //        break;
                            //    }

                            //    currentShipCoordinate = new ShipCoordinate(playingButton.Tag.ToString());

                            //    if (shipFirstCoordinate.CoordinateLetter == currentShipCoordinate.CoordinateLetter)
                            //    {
                            //        // Проверяем, попадает ли случайное число в диапазон
                            //        if (Convert.ToInt32(currentShipCoordinate.CoordinateNumber) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateNumber) <= upperBound)
                            //        {

                            //            if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                            //            {
                            //                if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                            //                {
                            //                    enemyShip.location.Add(currentShipCoordinate);
                            //                }

                            //            }
                            //        }
                            //    }
                            //}

                            //break;

                        case shipDirections.Left:
                            if (OutOfBoundsCheck(shipFirstCoordinate, enemyShip, currentShipDirection) == true)
                            {
                                currentShipDirection = shipDirections.Top;
                                break;
                            }

                            upperBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) - 1;
                            lowerBound = Convert.ToInt32(shipFirstCoordinate.CoordinateLetter) - (enemyShip.coordinatesForShipCount - 1);


                            possibleShipCoordinates = playingFieldCoordinates.Where(coordinate => coordinate.CoordinateNumber == shipFirstCoordinate.CoordinateNumber).ToList();
                            bestShipCoordinates = possibleShipCoordinates.Where(coordinate => (Convert.ToInt32(coordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(coordinate.CoordinateLetter) <= upperBound)).ToList();
                            foreach (var bestShipCoordinate in bestShipCoordinates)
                            {
                                currentShipCoordinate = bestShipCoordinate;
                                // Проверяем, попадает ли случайное число в диапазон

                                if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                                {
                                    if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                                    {
                                        enemyShip.location.Add(currentShipCoordinate);

                                    }

                                }

                            }

                            if (enemyShip.location.Count < enemyShip.coordinatesForShipCount)
                            {
                                enemyShip.location.Clear();
                                currentShipDirection = shipDirections.Top;
                                dontFitDirectionsCount++;
                                break;
                            }
                            else
                            {
                                notFullyInstalled = false;
                            }
                            
                            break;



                            //foreach (var playingButton in playingField)
                            //{
                            //    if (enemyShip.location.Count == enemyShip.coordinatesForShipCount)
                            //    {
                            //        notFullyInstalled = false;
                            //        break;
                            //    }
                                

                            //    currentShipCoordinate = new ShipCoordinate(playingButton.Tag.ToString());

                            //    if (shipFirstCoordinate.CoordinateNumber == currentShipCoordinate.CoordinateNumber)
                            //    {
                            //        // Проверяем, попадает ли случайное число в диапазон
                            //        if (Convert.ToInt32(currentShipCoordinate.CoordinateLetter) >= lowerBound && Convert.ToInt32(currentShipCoordinate.CoordinateLetter) <= upperBound)
                            //        {

                            //            if (CoordinateOccupiedCheck(currentShipCoordinate) == false)
                            //            {
                            //                if (ShipCollision(currentShipCoordinate, enemyShip, computer) == false)
                            //                {
                            //                    enemyShip.location.Add(currentShipCoordinate);
                            //                }

                            //            }
                            //        }
                            //    }
                            //}

                            //break;
                        
                    }
                }
                


            }

        }


        private bool CoordinateOccupiedCheck(GameCoordinate currentShipCoordinate)
        {
            foreach (var ship in computer.fleet)
            {

                GameCoordinate shipCoordinate = ship.location.Find(coordinate => coordinate.CoordinateLocation == currentShipCoordinate.CoordinateLocation);

                if (shipCoordinate != null)
                {
                    return true;
                }

            }
            return false;
        }

        private bool OutOfBoundsCheck(GameCoordinate currentShipCoordinate, AbstractShip currentShip, shipDirections shipDirection)
        {

            switch (shipDirection)
            {
                case shipDirections.Top:
                    if ((Convert.ToInt32(currentShipCoordinate.CoordinateNumber) - (currentShip.coordinatesForShipCount - 1)) < 1)
                    {             
                        return true;
                    }
                    break;

                case shipDirections.Bottom:
                    if ((Convert.ToInt32(currentShipCoordinate.CoordinateNumber) + (currentShip.coordinatesForShipCount - 1)) > 10)
                    {
                        
                        return true;
                    }
                    break;

                case shipDirections.Left:
                    if ((Convert.ToInt32(currentShipCoordinate.CoordinateLetter) - (currentShip.coordinatesForShipCount - 1)) < 65)
                    {

                        return true;
                    }
                    break;

                case shipDirections.Right:
                    if ((Convert.ToInt32(currentShipCoordinate.CoordinateLetter) + (currentShip.coordinatesForShipCount - 1)) > 74)
                    {
                        
                        return true;
                    }
                    break;
                
            }
            return false;

            
        }

        private void InstallFirstEnemyShipCoordinate(AbstractShip enemyShip)
        {
            Random random = new Random();
            bool coordinateIsNotFit = true;
            while (coordinateIsNotFit)
            {
                Button shipLocationButton = playingFieldButtons[random.Next(0, playingFieldButtons.Count)];

                GameCoordinate shipCoordinate = new GameCoordinate(shipLocationButton);


                if (CoordinateOccupiedCheck(shipCoordinate) == false)
                {
                    if (ShipCollision(shipCoordinate, enemyShip, computer) == false)
                    {
                        enemyShip.location.Add(shipCoordinate);
                        coordinateIsNotFit = false;

                    }

                }
            }
        }
    }


}
