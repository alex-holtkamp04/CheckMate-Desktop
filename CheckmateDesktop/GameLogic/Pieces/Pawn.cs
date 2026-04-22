using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class Pawn : Piece
    {
        // Set the value of the pawn to 1
        public Pawn()
        {
            Value = 1;
        }
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            // Set the direction of movement based on the team color
            int direction = (Team == TeamColor.White) ? -1 : 1;

            int nextRow = currentPosition.Row + direction;

            Position nextPostion = new Position(nextRow, currentPosition.Col);

            // Check if the next row is within the board limits and if the square directly in front of the pawn is empty
            if (IsInBounds(nextPostion) && currentBoard.GetPiece(nextPostion) == null)
            {
                ValidMoves.Add(nextPostion);

                // If it's the pawn's first move, it can move two squares forward
                if (isFirstMove)
                {
                    int twoSquaresLetter = currentPosition.Row + 2 * direction;
                    nextPostion = new Position(twoSquaresLetter, currentPosition.Col);
                    // Check if the two squares forward is within the board limits and if both squares in front of the pawn are empty
                    if (IsInBounds(nextPostion) && currentBoard.GetPiece(nextPostion) == null)
                    {
                        ValidMoves.Add(nextPostion);
                    }
                }
            }

            // Logic for capturing pieces diagonally
            int[] diagonalOffsets = { -1, 1 }; // Left and right diagonal
            foreach (int offset in diagonalOffsets)
            {
                Position diagonalPosition = new Position(nextRow, currentPosition.Col + offset);
                if (IsInBounds(diagonalPosition))
                {
                    Piece pieceAtDiagonal = currentBoard.GetPiece(diagonalPosition);
                    if (pieceAtDiagonal != null && pieceAtDiagonal.Team != Team)
                    {
                        ValidMoves.Add(diagonalPosition);
                    }
                }
            }

            // TODO: Add Logic for en passant

            return ValidMoves;
        }
    }
}
