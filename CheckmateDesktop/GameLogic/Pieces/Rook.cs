using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class Rook : Piece
    {
        // Set the value of the rook to 5 (used for scoring)
        public Rook()
        {
            Value = 5;
        }
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            foreach (var direction in new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) // Horizontal and vertical directions
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

            // TODO: Add castling logic here

            return ValidMoves;
        }
    }
}
