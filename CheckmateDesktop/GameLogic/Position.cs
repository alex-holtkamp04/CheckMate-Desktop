using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Position(int column, int row)
        {
            Column = column;
            Row = row;
        }
    }
}
