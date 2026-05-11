using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    // Helper class for cleanly passing row and column values to and from methods
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
