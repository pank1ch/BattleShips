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
    internal class GameCoordinate
    {
    
        private string coordinateLocation;
		public string CoordinateLocation
        {
			get { return coordinateLocation; }
		}

        private char coordinateLetter;
        private string coordinateNumber;

        

        public char CoordinateLetter
        {
            get { return coordinateLetter; }

        }
     
        public string CoordinateNumber
        {
            get { return coordinateNumber; }
            
        }

        private Button coordinateButton;

        public Button CoordinateButton
        {
            get { return coordinateButton; }
            
        }

        public GameCoordinate(Button coordinateButton)
        {
            this.coordinateButton = coordinateButton;

            this.coordinateLocation = coordinateButton.Tag.ToString();

            string[] coordinateLocationParts = coordinateLocation.Split('-');


            coordinateLetter = coordinateLocationParts[0][0];
            coordinateNumber = coordinateLocationParts[1];
        }

        public GameCoordinate(string coordinateLocation)
        {
            this.coordinateLocation = coordinateLocation;
            string[] coordinateLocationParts = coordinateLocation.Split('-');

            coordinateLetter = coordinateLocationParts[0][0];
            coordinateNumber = coordinateLocationParts[1];
        }

    }
}
