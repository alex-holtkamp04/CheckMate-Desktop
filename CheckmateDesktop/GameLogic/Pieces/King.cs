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

            // Castling Logic
            if (isFirstMove)
            {
                int row = currentPosition.Row;

                // Kingside Castling
                Position kingsideRookPos = new Position(row, 7);
                Piece kingsideRook = currentBoard.GetPiece(kingsideRookPos);

                if (kingsideRook is Rook &&
                    kingsideRook.Team == Team &&
                    kingsideRook.isFirstMove)
                {
                    // Squares between king and rook must be empty
                    if (currentBoard.GetPiece(new Position(row, 5)) == null &&
                        currentBoard.GetPiece(new Position(row, 6)) == null)
                    {
                        ValidMoves.Add(new Position(row, 6));
                    }
                }

                // Queenside Castling
                Position queensideRookPos = new Position(row, 0);
                Piece queensideRook = currentBoard.GetPiece(queensideRookPos);

                if (queensideRook is Rook &&
                    queensideRook.Team == Team &&
                    queensideRook.isFirstMove)
                {
                    // Squares between king and rook must be empty
                    if (currentBoard.GetPiece(new Position(row, 1)) == null &&
                        currentBoard.GetPiece(new Position(row, 2)) == null &&
                        currentBoard.GetPiece(new Position(row, 3)) == null)
                    {
                        ValidMoves.Add(new Position(row, 2));
                    }
                }
            }

            return ValidMoves;
        }
    }
}
