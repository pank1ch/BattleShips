using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ships
{
    internal class SingleDeckShip: AbstractShip
    {
        public SingleDeckShip()
        {
            this.hp = 1;
            this.name = "singleDeckShip";
            this.location = new List<GameCoordinate>();
            this.coordinatesForShipCount = 1;
        }
    }
}
