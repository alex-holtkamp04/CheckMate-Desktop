using CheckmateDesktop.GameLogic;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Media;
using static CheckmateDesktop.GameLogic.Piece;

namespace CheckmateDesktop
{
    public class Board
    {
        //public enum BoardMarkers {A, B, C, D, E, F, G, H};

        Piece[,] BoardSquares;

        TeamColor ActivePlayer;
        public Board()
        {
            ActivePlayer = TeamColor.White;
            BoardSquares = new Piece[8, 8];   
        }
    }
}
