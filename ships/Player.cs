using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ships
{
    internal class Player
    {
        //AbstractShip[] fleet;
        public List<AbstractShip> fleet;
        public bool isEnemy;
        

        public Player(bool isEnemy)
        {
            
            this.isEnemy = isEnemy;

            fleet = new List<AbstractShip>();

            if (isEnemy )
            {
                FourDeckship fourDeckship = new FourDeckship();
                fleet.Add(fourDeckship);

                for (int i = 1; i <= 2; i++)
                {
                    ThreeDeckShip threeDeckShip = new ThreeDeckShip();
                    fleet.Add(threeDeckShip);
                }

                for (int i = 1; i <= 3; i++)
                {
                    DoubleDeckShip doubleDeckShip = new DoubleDeckShip();
                    fleet.Add(doubleDeckShip);
                }


                for (int i = 1; i <= 4; i++)
                {
                    SingleDeckShip singleDeckShip = new SingleDeckShip();

                    fleet.Add(singleDeckShip);
                }

                

                


            }

        }
    }
}
