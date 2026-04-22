using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class Knight : Piece
    {
        // Set the value of the knight to 3 (used for scoring)
        public Knight()
        {
            Value = 3;
        }
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            foreach (var move in new (int, int)[] { (-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1) }) // All 8 possible knight moves
            {
                Position nextPosition = new Position(currentPosition.Row + move.Item1, currentPosition.Col + move.Item2);

                if (IsInBounds(nextPosition))
                {
                    Piece pieceAtNextPosition = currentBoard.GetPiece(nextPosition);

                    if (pieceAtNextPosition == null || pieceAtNextPosition.Team != Team)
                    {
                        ValidMoves.Add(nextPosition); // Can move to empty square or capture opponent's piece
                    }
                }
            }

            return ValidMoves;
        }
    }
}
