using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class Bishop : Piece
    {
        // Set the value of the bishop to 3 (used for scoring)
        public Bishop()
        {
            Value = 3;
        }
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            foreach (var direction in new (int, int)[] { (-1, -1), (-1, 1), (1, -1), (1, 1) }) // Diagonal directions
            {
                int nextLetter = currentPosition.Letter + direction.Item1;
                int nextNumber = currentPosition.Number + direction.Item2;

                // Loop until we go out of bounds or hit a piece
                while (IsInBounds(new Position(nextLetter, nextNumber)))
                {
                    Position nextPosition = new Position(nextLetter, nextNumber);
                    Piece pieceAtNextPosition = currentBoard.GetPiece(nextPosition);

                    if (pieceAtNextPosition == null)
                    {
                        ValidMoves.Add(nextPosition);
                    }
                    else
                    {
                        if (pieceAtNextPosition.Team != Team)
                        {
                            ValidMoves.Add(nextPosition); // Can capture opponent's piece
                        }
                        break; // Stop searching in this direction if we hit any piece
                    }

                    nextLetter += direction.Item1;
                    nextNumber += direction.Item2;
                }
            }

            return ValidMoves;
        }

    }
}
