using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    // Parent class for each piece type to inherit
    public abstract class Piece
    {
        public enum TeamColor { White, Black, None };

        public TeamColor Team { get; set; }

        public int Value { get; protected set; }

        public bool isFirstMove;

        // Abstract function each piece type will inherit to calculate valid moves for that piece type
        public abstract List<Position> GetValidMoves(Board currentBoard, Position currentPosition);
        public bool IsInBounds(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Col >= 0 && pos.Col < 8;
        }
        public virtual Piece Clone()
        {
            return (Piece)this.MemberwiseClone();
        }
    }
}
