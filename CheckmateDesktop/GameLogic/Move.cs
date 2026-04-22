using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public class Move
    {
        public Piece? Piece { get; set; }
        public Position? From { get; set; }
        public Position? To { get; set; }
        public Move (Piece piece, Position from, Position to)
        {
            Piece = piece;
            From = from;
            To = to;
        }

    }
}
