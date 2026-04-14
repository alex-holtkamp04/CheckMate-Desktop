using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic.Pieces
{
    public class Bishop : Piece
    {
        public override List<Position> GetValidMoves(Board currentBoard, Position currentPosition)
        {
            List<Position> ValidMoves = new List<Position>();

            if (Team == TeamColor.White)
            {

            }
            if (Team == TeamColor.Black)
            {

            }

            return ValidMoves;
        }

    }
}
