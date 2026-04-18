using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class Pawn : Piece
    {
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            // Set the direction of movement based on the team color
            int direction = (Team == TeamColor.White) ? -1 : 1;

            int nextLetter = currentPosition.Letter + direction;

            Position nextPostion = new Position(nextLetter, currentPosition.Number);

            // Check if the next row is within the board limits and if the square directly in front of the pawn is empty
            if (IsInBounds(nextPostion) && currentBoard.GetPiece(nextPostion) == null)
            {
                ValidMoves.Add(nextPostion);

                // If it's the pawn's first move, it can move two squares forward
                if (isFirstMove)
                {
                    int twoSquaresLetter = currentPosition.Letter + 2 * direction;
                    nextPostion = new Position(twoSquaresLetter, currentPosition.Number);
                    // Check if the two squares forward is within the board limits and if both squares in front of the pawn are empty
                    if (IsInBounds(nextPostion) && currentBoard.GetPiece(nextPostion) == null)
                    {
                        ValidMoves.Add(nextPostion);
                    }
                }
            }

            // TODO: Add Logic for capturing pieces diagonally and en passant

            return ValidMoves;
        }
    }
}
