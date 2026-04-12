using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace CheckmateDesktop
{
    public class ChessSquare
    {
        public SolidColorBrush SquareColorBrush { get; set; }

        // E.g., "♞", "♖", "♚", or empty string "" if the square is empty
        public string PieceUnicode { get; set; }

        // E.g., Brushes.Black or Brushes.White
        public SolidColorBrush PieceColorBrush { get; set; }

        // Add your grid coordinates (X, Y) or (Rank, File) here for game logic
    }
}
