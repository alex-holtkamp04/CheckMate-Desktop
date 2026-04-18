using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class King : Piece
    {
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            foreach (var move in new (int, int)[] { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) }) // All adjacent squares
            {
                Position nextPosition = new Position(currentPosition.Letter + move.Item1, currentPosition.Number + move.Item2);

                if (IsInBounds(nextPosition))
                {
                    Piece pieceAtNextPosition = currentBoard.GetPiece(nextPosition);

                    if (pieceAtNextPosition == null || pieceAtNextPosition.Team != Team)
                    {
                        ValidMoves.Add(nextPosition); // Can move to empty square or capture opponent's piece
                    }
                }
            }

            // TODO: Add castling logic here (need to check if king and rook have moved
            // , and if squares between them are empty and not under attack)

            return ValidMoves;
        }
    }
}
