using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Media;

namespace CheckmateDesktop
{
    public class Board
    {
        public ChessSquare[,] boardSquares;
        public enum BoardMarkers {A, B, C, D, E, F, G, H};
        public Board()
        {
            
        }
    }
}
