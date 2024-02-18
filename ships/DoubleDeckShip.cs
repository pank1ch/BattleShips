using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ships
{
    internal class DoubleDeckShip : AbstractShip
    {
        public DoubleDeckShip()
        {
            this.hp = 2;
            this.name = "doubleDeckShip";
            this.location = new List<GameCoordinate>();
            this.coordinatesForShipCount = 2;
        }
    }
}
