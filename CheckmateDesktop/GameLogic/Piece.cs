using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public abstract class Piece
    {
        public enum TeamColor { White, Black, None };

        public TeamColor Team { get; set; }

        public int Value { get; protected set; }

        public bool isFirstMove;

        public abstract List<Position> GetValidMoves(Board currentBoard, Position currentPosition);
        public bool IsInBounds(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Col >= 0 && pos.Col < 8;
        }
    }
}
