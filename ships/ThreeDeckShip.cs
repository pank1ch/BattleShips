using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ships
{
    internal class ThreeDeckShip : AbstractShip
    {
        public ThreeDeckShip()
        {
            this.hp = 3;
            this.name = "threeDeckShip";
            this.location = new List<GameCoordinate>();
            this.coordinatesForShipCount = 3;
        }
    }
}
