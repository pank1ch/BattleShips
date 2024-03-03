using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ships
{
    internal class GameAlerts
    {

        const string SHIP_COLLISION_MESSAGE = "Невозможно установить корабль, судна сталкиваются бортами";
        const string SHIP_COORDINATE_OCCUPIED_MESSAGE = "Невозможно установить корабль, установке мешает другое судно";
        const string OUT_OF_BOUNDS_MESSAGE = "Невозможно установить корабль, координаты судна выходят за пределы поля";
        const string SHIP_NOT_ON_FIELD_MESSAGE = "Сначала расположите корабль на игровом поле";
        const string SHIP_NOT_FULLY_LOCATED_MESSAGE = "Вы не полностью разместили ваш корабль";
        const string SHIP_IS_LOCATING_NOW_MESSAGE = "Вы уже размещаете корабль";
        public string ShipsCollisionAlert()
        {
            return SHIP_COLLISION_MESSAGE;
        }

        public string CoordinateForShipOccupiedAlert()
        {
            return SHIP_COORDINATE_OCCUPIED_MESSAGE;
        }

        public string ShipIsOutOfBoundsAlert()
        {
            return OUT_OF_BOUNDS_MESSAGE;
        }

        public string ShipIsNotOnFieldAlert ()
        {
            return SHIP_NOT_ON_FIELD_MESSAGE;
        }

        public string ShipIsNotFullyLocatedAlert()
        {
            return SHIP_NOT_FULLY_LOCATED_MESSAGE;
        }

        public string ShipIsLocatingAlert()
        {
            return SHIP_IS_LOCATING_NOW_MESSAGE;
        }
    }
}
