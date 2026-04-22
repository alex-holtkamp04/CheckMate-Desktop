using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public class Position
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
