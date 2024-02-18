using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ships
{
    internal abstract class AbstractShip
    {
        
        public bool isAlive = true;
        public int hp;
        public string name;
        public List<GameCoordinate> location;
        public System.Windows.Forms.Label shipLabel;
        public TextBox shipStatusBox;

        public int coordinatesForShipCount;

    }
}
