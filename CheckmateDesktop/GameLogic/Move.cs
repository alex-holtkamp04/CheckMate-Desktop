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
        public bool IsCapture { get; set; }
        public Move(Piece piece, Position from, Position to, bool iscapture)
        {
            Piece = piece;
            From = from;
            To = to;
            IsCapture = iscapture;
        }

    }
}
