using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ships
{
    internal class FourDeckship : AbstractShip
    {
        public FourDeckship()
        {
            this.hp = 4;
            this.name = "fourDeckShip";
            this.location = new List<GameCoordinate>();
            this.coordinatesForShipCount = 4;
        }
    }
}
