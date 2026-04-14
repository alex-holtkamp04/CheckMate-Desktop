using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public abstract class Piece
    {
        public enum TeamColor { White, Black }

        public TeamColor Team { get; set; }

        public bool isFirstMove;

        public abstract List<Position> GetValidMoves(Board currentBoard, Position currentPosition);
    }
}
