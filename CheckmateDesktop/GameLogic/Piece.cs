using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public abstract class Piece
    {
        public enum TeamColor { White, Black, None };

        public TeamColor Team { get; set; }

        public bool isFirstMove;

        public abstract List<Position> GetValidMoves(Board currentBoard, Position currentPosition);
        protected bool IsInBounds(Position pos)
        {
            return pos.Letter >= 0 && pos.Letter < 8 && pos.Number >= 0 && pos.Number < 8;
        }
    }
}
