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
using System.Windows.Forms.VisualStyles;

namespace ships
{
    internal class ShipsBattle
    {
        GameCoordinate firstDestroyedCoordinate;
        GameCoordinate lastDestroyedCoordinate;
        List<GameCoordinate> possibleShipCoordinates;
        List<GameCoordinate> coordinatesForAttack;
        string attackDirection;

        bool startFindPlayerShip;

        Button targetButton;
        string targetLocation;

        List<GameCoordinate> playerMissedShots;
        List<GameCoordinate> playerHitShots;
        List<GameCoordinate> computerMissedShots;
        List<GameCoordinate> computerHitShots;

        Button finishGameButton;

        private bool IsPlayerGameField = false;
        private bool IsPlayerTurn = true;
        private bool IsGameActive = true;

        List<GameCoordinate> playingFieldCoordinates;
        private Player player;
        private Player computer;

        private ShipStatusManager shipStatusManager;
        public ShipsBattle(Player player, Player computer, List<GameCoordinate> playingFieldCoordinates, ShipStatusManager shipStatusManager, Button finishGameButton)
        {
            this.player = player;
            this.computer = computer;
            this.playingFieldCoordinates = playingFieldCoordinates;
            this.shipStatusManager = shipStatusManager;
            this.finishGameButton = finishGameButton;

            playerMissedShots = new List<GameCoordinate>();
            playerHitShots = new List<GameCoordinate>();

            computerMissedShots = new List<GameCoordinate>();
            computerHitShots= new List<GameCoordinate>();

            possibleShipCoordinates = new List<GameCoordinate>();
            coordinatesForAttack = new List<GameCoordinate>();
        }


        public void GameFieldButton_Click(object sender, EventArgs e)
        {
            if (sender is Button clickedButton && clickedButton.Name.Contains("Game"))
            {
                if (IsGameActive)
                {
                    if (clickedButton.BackColor == Color.White && IsPlayerTurn && IsPlayerGameField == false)
                    {
                        targetButton = clickedButton;
                        PlayerShot(targetButton);
                        //targetLocation = targetButton.Tag.ToString();
                        //shipFirstLocation = firstClickedButton.Tag.ToString();



                    }
                }
                
            }
        }


        public void PlayerShot(Button targetButton)
        {

            GameCoordinate targetCoordinate = new GameCoordinate(targetButton);
            bool IsHit = false;
            switch (IsPlayerTurn)
            {
                case true:

                    foreach (var ship in computer.fleet)
                    {
                        foreach (var shipCoordinate in ship.location)
                        {
                            if (targetCoordinate.CoordinateLocation == shipCoordinate.CoordinateLocation)
                            {
                                IsHit = true;
                                playerHitShots.Add(shipCoordinate);

                                MessageBox.Show($"{targetCoordinate.CoordinateLocation} — вы попали!");
                                ship.hp--;

                                targetCoordinate.CoordinateButton.BackColor = Color.Red;
                                if (ship.hp == 0)
                                {
                                    ship.isAlive = false;
                                    MessageBox.Show($"{ship.name} — корабль противника уничтожен!");
                                    shipStatusManager.ShowShipStatus(ship);

                                    if (CheckWinStatus(computer) == true)
                                    {
                                        MessageBox.Show("Поздравляю, вы победили!");
                                        IsGameActive = false;
                                        finishGameButton.Enabled = true;

                                    }

                                    
                                }
                                break;
                            }
                        }
                    }
                    if (IsHit == false)
                    {
                        
                        targetCoordinate.CoordinateButton.BackColor = Color.Black;
                        playerMissedShots.Add(targetCoordinate);
                        IsPlayerTurn = false;

                        

                        EnemyTurn();
                    }

                    break;

                case false:

                    
                    foreach (var ship in player.fleet)
                    {
                        foreach (var shipCoordinate in ship.location)
                        {
                            if (targetCoordinate.CoordinateLocation == shipCoordinate.CoordinateLocation)
                            {
                                IsHit = true;
                                computerHitShots.Add(shipCoordinate);

                                MessageBox.Show($"{targetCoordinate.CoordinateLocation} — противник попал!");
                                ship.hp--;


                                

                                if (startFindPlayerShip == true && lastDestroyedCoordinate == null)
                                {
                                    lastDestroyedCoordinate = targetCoordinate;
                                    FindCoordinatesToAttack(lastDestroyedCoordinate);

                                }

                                if (ship.hp > 0 && startFindPlayerShip == false)
                                {
                                    firstDestroyedCoordinate = targetCoordinate;
                                    FindShip(targetCoordinate);                                   
                                    startFindPlayerShip = true;
                                }

                                if (ship.hp == 0)
                                {
                                    ship.isAlive = false;
                                    MessageBox.Show($"{ship.name} — ваш корабль уничтожен!");
                                    startFindPlayerShip = false;
                                    possibleShipCoordinates.Clear();
                                    coordinatesForAttack.Clear();
                                    lastDestroyedCoordinate = null;

                                    if (CheckWinStatus(player) == true)
                                    {
                                        MessageBox.Show("GG! Победил компьютер.");
                                        IsGameActive = false;
                                        finishGameButton.Enabled = true;
                                        return;

                                    }

                                }

                                if (lastDestroyedCoordinate != null)
                                {
                                    lastDestroyedCoordinate = targetCoordinate;
                                }

                                EnemyTurn();
                            }

                        }
                    }
                    if (IsHit == false)
                    {
                        
                        computerMissedShots.Add(targetCoordinate);

                        if (startFindPlayerShip == true  && lastDestroyedCoordinate != null)
                        {
                            ChangeDirection();
                        }
                        IsPlayerTurn = true;



                    }

                    break;

            }

            //GameCoordinate targetCoordinate = new GameCoordinate(targetButton);
            //bool IsHit = false;
            //switch (IsPlayerTurn)
            //{
            //    case true:

            //        foreach (var ship in computer.fleet)
            //        {
            //            foreach (var shipCoordinate in ship.location)
            //            {
            //                if (targetCoordinate.CoordinateLocation == shipCoordinate.CoordinateLocation)
            //                {
            //                    IsHit = true;
            //                    playerHitShots.Add(shipCoordinate);

            //                    MessageBox.Show($"{targetCoordinate.CoordinateLocation} — вы попали!");
            //                    ship.hp--;

            //                    targetCoordinate.CoordinateButton.BackColor = Color.Red;
            //                    if (ship.hp == 0)
            //                    {
            //                        ship.isAlive = false;
            //                        MessageBox.Show($"{ship.name} — корабль противника уничтожен!");
            //                        shipStatusManager.ShowShipStatus(ship);
            //                    }
            //                    break;
            //                }
            //            }
            //        }
            //        if (IsHit == false)
            //        {
            //            targetCoordinate.CoordinateButton.BackColor = Color.Black;
            //            playerMissedShots.Add(targetCoordinate);
            //            IsPlayerTurn = false;
            //            EnemyTurn();
            //        }

            //        break;

            //    case false:

            //        foreach (var ship in player.fleet)
            //        {
            //            foreach (var shipCoordinate in ship.location)
            //            {
            //                if (targetCoordinate.CoordinateLocation == shipCoordinate.CoordinateLocation)
            //                {
            //                    IsHit = true;
            //                    computerHitShots.Add(shipCoordinate);

            //                    MessageBox.Show($"{targetCoordinate.CoordinateLocation} — противник попал!");
            //                    ship.hp--;


            //                    if (ship.hp == 0)
            //                    {
            //                        ship.isAlive = false;
            //                        MessageBox.Show($"{ship.name} — ваш корабль уничтожен!");
            //                    }
            //                    EnemyTurn();
            //                }

            //            }
            //        }
            //        if (IsHit == false)
            //        {

            //            computerMissedShots.Add(targetCoordinate);
            //            IsPlayerTurn = true;

            //        }

            //        break;

            //}
        }

        private void EnemyTurn()
        {
            Random random = new Random();
            if (startFindPlayerShip == true) {

                if (lastDestroyedCoordinate == null) {

                    GameCoordinate shotGameCoordinate = possibleShipCoordinates[random.Next(0, possibleShipCoordinates.Count)];
                    possibleShipCoordinates.Remove(shotGameCoordinate);

                    PlayerShot(shotGameCoordinate.CoordinateButton);
                }
                else
                {
                    GameCoordinate shotGameCoordinate = GetCoordinateForAttack(lastDestroyedCoordinate);
                    if (shotGameCoordinate == null)
                    {
                        ChangeDirection();
                        shotGameCoordinate = GetCoordinateForAttack(firstDestroyedCoordinate);
                        PlayerShot(shotGameCoordinate.CoordinateButton);
                    }
                    else
                    {
                        PlayerShot(shotGameCoordinate.CoordinateButton);
                    }
                    
                    
                }              
            }
            else
            {
                

                GameCoordinate gameCoordinate = null;

                bool isNewCoordinate = false;

                while (isNewCoordinate == false)
                {
                    gameCoordinate = playingFieldCoordinates[random.Next(0, playingFieldCoordinates.Count)];

                    if (computerMissedShots.Find(coordinate => coordinate.CoordinateLocation == gameCoordinate.CoordinateLocation) == null)
                    {
                        if (computerHitShots.Find(coordinate => coordinate.CoordinateLocation == gameCoordinate.CoordinateLocation) == null)
                        {
                            isNewCoordinate = true;
                        }
                    }


                }


                PlayerShot(gameCoordinate.CoordinateButton);
            }
            

        }

        public void SwitchGameField(Button switchButton, Label mainLabel)
        {

            switch (IsPlayerGameField)
            {
                case false:
                    mainLabel.Text = "Ваш флот";

                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        playingFieldCoordinate.CoordinateButton.BackColor = Color.White;
                    }

                    foreach (var missedShot in computerMissedShots)
                    {
                        missedShot.CoordinateButton.BackColor = Color.Black;
                    }

                    foreach (var ship in player.fleet)
                    {
                        shipStatusManager.ShowShipStatus(ship);
                        foreach (var shipCoordinate in ship.location)
                        {
                            shipCoordinate.CoordinateButton.BackColor = Color.Blue;
                        }
                    }

                    foreach (var hitShot in computerHitShots)
                    {
                        hitShot.CoordinateButton.BackColor = Color.Red;
                    }
                    IsPlayerGameField = true;
                    switchButton.Text = "Посмотреть поле противника";

                    break;

                case true:
                    mainLabel.Text = "Флот противника";

                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        playingFieldCoordinate.CoordinateButton.BackColor = Color.White;
                    }

                    foreach (var missedShot in playerMissedShots)
                    {
                        missedShot.CoordinateButton.BackColor = Color.Black;
                    }

                    foreach (var ship in computer.fleet)
                    {
                        shipStatusManager.ShowShipStatus(ship);
                        
                    }

                    foreach (var hitShot in playerHitShots)
                    {
                        hitShot.CoordinateButton.BackColor = Color.Red;
                    }
                    IsPlayerGameField = false;
                    switchButton.Text = "Посмотреть свое поле";
                    break;              
            }

            
        }


        private void FindShip(GameCoordinate targetCoordinate)
        {
            GameCoordinate shipDestroyedCoordinate = targetCoordinate;

            possibleShipCoordinates = new List<GameCoordinate>();



            List<string> nearestCoordinates = new List <string>();

            char leftNearestLetter = Convert.ToChar(Convert.ToInt32(shipDestroyedCoordinate.CoordinateLetter) - 1);
            string leftNearestCoordinate = Convert.ToString(leftNearestLetter) + "-" + shipDestroyedCoordinate.CoordinateNumber;

            char rightNearestLetter = Convert.ToChar(Convert.ToInt32(shipDestroyedCoordinate.CoordinateLetter) + 1);
            string rightNearestCoordinate = Convert.ToString(rightNearestLetter) + "-" + shipDestroyedCoordinate.CoordinateNumber;

            string topNearestCoordinate = Convert.ToString(shipDestroyedCoordinate.CoordinateLetter) + "-" + Convert.ToString(Convert.ToInt32(shipDestroyedCoordinate.CoordinateNumber) - 1);

            string bottomNearestCoordinate = Convert.ToString(shipDestroyedCoordinate.CoordinateLetter) + "-" + Convert.ToString(Convert.ToInt32(shipDestroyedCoordinate.CoordinateNumber) + 1);
         
            nearestCoordinates.Add(leftNearestCoordinate);
            nearestCoordinates.Add(rightNearestCoordinate);
            nearestCoordinates.Add(topNearestCoordinate);
            nearestCoordinates.Add(bottomNearestCoordinate);

            foreach (var nearestCoordinate in nearestCoordinates)
            {
                GameCoordinate gameCoordinate = playingFieldCoordinates.Find(coordinate => coordinate.CoordinateLocation == nearestCoordinate);
                if (gameCoordinate != null)
                {
                    if (computerMissedShots.Find(coordinate => coordinate.CoordinateLocation == gameCoordinate.CoordinateLocation) == null)
                    {
                        if (computerHitShots.Find(coordinate => coordinate.CoordinateLocation == gameCoordinate.CoordinateLocation) == null)
                        {
                            possibleShipCoordinates.Add(gameCoordinate);
                        }
                    }

                    
                }
            }

            
        }

        private void ChangeDirection()
        {
            coordinatesForAttack.Clear();
            if (lastDestroyedCoordinate.CoordinateLetter == firstDestroyedCoordinate.CoordinateLetter)
            {
                if (Convert.ToInt32(lastDestroyedCoordinate.CoordinateNumber)  > Convert.ToInt32(firstDestroyedCoordinate.CoordinateNumber))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateLetter == firstDestroyedCoordinate.CoordinateLetter)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateNumber) < Convert.ToInt32(firstDestroyedCoordinate.CoordinateNumber)){
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "top";
                }


                if (Convert.ToInt32(lastDestroyedCoordinate.CoordinateNumber) < Convert.ToInt32(firstDestroyedCoordinate.CoordinateNumber))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateLetter == firstDestroyedCoordinate.CoordinateLetter)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateNumber) > Convert.ToInt32(firstDestroyedCoordinate.CoordinateNumber))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "bottom";
                }

            }

            if (lastDestroyedCoordinate.CoordinateNumber == firstDestroyedCoordinate.CoordinateNumber)
            {
                if (Convert.ToInt32(lastDestroyedCoordinate.CoordinateLetter) < Convert.ToInt32(firstDestroyedCoordinate.CoordinateLetter))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateNumber == firstDestroyedCoordinate.CoordinateNumber)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateLetter) > Convert.ToInt32(firstDestroyedCoordinate.CoordinateLetter))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "right";
                }


                if (Convert.ToInt32(lastDestroyedCoordinate.CoordinateLetter) > Convert.ToInt32(firstDestroyedCoordinate.CoordinateLetter))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateNumber == firstDestroyedCoordinate.CoordinateNumber)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateLetter) < Convert.ToInt32(firstDestroyedCoordinate.CoordinateLetter))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "left";
                }

            }

        }


        private void FindCoordinatesToAttack(GameCoordinate lastShotedCoordinate)
        {
            coordinatesForAttack = new List<GameCoordinate>();
            if (lastShotedCoordinate.CoordinateLetter == firstDestroyedCoordinate.CoordinateLetter)
            {
                if (Convert.ToInt32(lastShotedCoordinate.CoordinateNumber) < Convert.ToInt32(firstDestroyedCoordinate.CoordinateNumber))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateLetter == firstDestroyedCoordinate.CoordinateLetter)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateNumber) < Convert.ToInt32(lastShotedCoordinate.CoordinateNumber))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "top";
                }


                if (Convert.ToInt32(lastShotedCoordinate.CoordinateNumber) > Convert.ToInt32(firstDestroyedCoordinate.CoordinateNumber))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateLetter == firstDestroyedCoordinate.CoordinateLetter)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateNumber) > Convert.ToInt32(lastShotedCoordinate.CoordinateNumber))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "bottom";
                }

            }

            if (lastShotedCoordinate.CoordinateNumber == firstDestroyedCoordinate.CoordinateNumber)
            {
                if (Convert.ToInt32(lastShotedCoordinate.CoordinateLetter) < Convert.ToInt32(firstDestroyedCoordinate.CoordinateLetter))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateNumber == firstDestroyedCoordinate.CoordinateNumber)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateLetter) < Convert.ToInt32(lastShotedCoordinate.CoordinateLetter))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "left";
                }


                if (Convert.ToInt32(lastShotedCoordinate.CoordinateLetter) > Convert.ToInt32(firstDestroyedCoordinate.CoordinateLetter))
                {
                    foreach (var playingFieldCoordinate in playingFieldCoordinates)
                    {
                        if (playingFieldCoordinate.CoordinateNumber == firstDestroyedCoordinate.CoordinateNumber)
                        {
                            if (Convert.ToInt32(playingFieldCoordinate.CoordinateLetter) > Convert.ToInt32(lastShotedCoordinate.CoordinateLetter))
                            {
                                coordinatesForAttack.Add(playingFieldCoordinate);
                            }

                        }
                    }
                    attackDirection = "right";
                }

            }

        }

        private GameCoordinate GetCoordinateForAttack(GameCoordinate mainCoordinate)
        {
            GameCoordinate gameCoordinate = null;
            string attackCoordinateLocation;
            switch (attackDirection)
            {
                case "top":
                     attackCoordinateLocation = mainCoordinate.CoordinateLetter + "-" + Convert.ToString(Convert.ToInt32(mainCoordinate.CoordinateNumber) - 1);
                     gameCoordinate = coordinatesForAttack.Find(coordinate => coordinate.CoordinateLocation == attackCoordinateLocation);
                     
                    break;

               case "bottom":
                    attackCoordinateLocation = mainCoordinate.CoordinateLetter + "-" + Convert.ToString(Convert.ToInt32(mainCoordinate.CoordinateNumber) + 1);
                    gameCoordinate = coordinatesForAttack.Find(coordinate => coordinate.CoordinateLocation == attackCoordinateLocation);
                   
                    break;

                case "left":
                    attackCoordinateLocation = Convert.ToString(Convert.ToChar(Convert.ToInt32(mainCoordinate.CoordinateLetter) - 1)) + "-" + mainCoordinate.CoordinateNumber;
                    gameCoordinate = coordinatesForAttack.Find(coordinate => coordinate.CoordinateLocation == attackCoordinateLocation);
                              
                    break;

                case "right":
                    attackCoordinateLocation = Convert.ToString(Convert.ToChar(Convert.ToInt32(mainCoordinate.CoordinateLetter) + 1)) + "-" + mainCoordinate.CoordinateNumber;
                    gameCoordinate = coordinatesForAttack.Find(coordinate => coordinate.CoordinateLocation == attackCoordinateLocation);
                    
                    break;
                default:
                    break;
            }

            return gameCoordinate;
        }


        private bool CheckWinStatus(Player player)
        {
            foreach (var ship in player.fleet)
            {
                if (ship.isAlive == true)
                {
                    return false;
                }
            }

            return true;
        }



    }
}
