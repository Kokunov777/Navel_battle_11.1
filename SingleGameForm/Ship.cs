using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleGameForm
{
    public class Ship
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; }
        public bool IsHorizontal { get; set; }
        public int Hits { get; set; }

        public Ship(int x, int y, int size, bool isHorizontal)
        {
            X = x;
            Y = y;
            Size = size;
            IsHorizontal = isHorizontal;
            Hits = 0;
        }

        public bool IsSunk => Hits >= Size;
    }
}
