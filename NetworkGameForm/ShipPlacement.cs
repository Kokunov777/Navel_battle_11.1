using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkGameForm
{
    [Serializable]
    public class ShipPlacement
    {
        public Point[] Positions { get; set; } // Позиции корабля
        public bool IsHorizontal { get; set; }  // Ориентация
        public ShipType Type { get; set; }      // Тип корабля

        public enum ShipType
        {
            Carrier,     // Авианосец (5 клеток)
            Battleship,  // Линкор (4 клетки)
            Cruiser,     // Крейсер (3 клетки)
            Submarine,   // Подлодка (3 клетки)
            Destroyer    // Эсминец (2 клетки)
        }
    }
}
