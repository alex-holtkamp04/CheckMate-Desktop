using CheckmateDesktop.GameLogic;
using CheckmateDesktop.GameLogic.Pieces;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Media;
using static CheckmateDesktop.GameLogic.Piece;

namespace CheckmateDesktop
{
    public class Board
    {
        //public enum BoardMarkers {A, B, C, D, E, F, G, H};

        Piece[,] BoardSquares;

        TeamColor ActivePlayer;
        public Board()
        {
            ActivePlayer = TeamColor.White;
            BoardSquares = new Piece[8, 8];
            setupBoard();
        }

        private void setupBoard()
        {
            for (int index = 0; index < 64; index++)
            {
                int row = index / 8;
                int col = index % 8;
                switch (row)
                {
                    case 0:
                        if (col == 0 || col == 7)
                        {
                            BoardSquares[row, col] = new Rook
                            {
                                Team = Piece.TeamColor.Black,
                                isFirstMove = true
                            };
                        }
                        if (col == 1 || col == 6)
                        {
                            BoardSquares[row, col] = new Knight
                            {
                                Team = Piece.TeamColor.Black,
                                isFirstMove = true
                            };
                        }
                        if (col == 2 || col == 5)
                        {
                            BoardSquares[row, col] = new Bishop
                            {
                                Team = Piece.TeamColor.Black,
                                isFirstMove = true
                            };
                        }
                        if (col == 3)
                        {
                            BoardSquares[row, col] = new Queen
                            {
                                Team = Piece.TeamColor.Black,
                                isFirstMove = true
                            };
                        }
                        if (col == 4)
                        {
                            BoardSquares[row, col] = new King
                            {
                                Team = Piece.TeamColor.Black,
                                isFirstMove = true
                            };
                        }
                        break;

                    case 1:
                        BoardSquares[row, col] = new Pawn
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                        break;

                    case 6:
                        BoardSquares[row, col] = new Pawn
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                        break;

                    case 7:
                        if (col == 0 || col == 7)
                        {
                            BoardSquares[row, col] = new Rook
                            {
                                Team = Piece.TeamColor.White,
                                isFirstMove = true
                            };
                        }
                        if (col == 1 || col == 6)
                        {
                            BoardSquares[row, col] = new Knight
                            {
                                Team = Piece.TeamColor.White,
                                isFirstMove = true
                            };
                        }
                        if (col == 2 || col == 5)
                        {
                            BoardSquares[row, col] = new Bishop
                            {
                                Team = Piece.TeamColor.White,
                                isFirstMove = true
                            };
                        }
                        if (col == 3)
                        {
                            BoardSquares[row, col] = new Queen
                            {
                                Team = Piece.TeamColor.White,
                                isFirstMove = true
                            };
                        }
                        if (col == 4)
                        {
                            BoardSquares[row, col] = new King
                            {
                                Team = Piece.TeamColor.White,
                                isFirstMove = true
                            };
                        }
                        break;

                    default:
                        BoardSquares[row, col] = null;
                        break;
                }
            }
        }
        
        public bool isValidMove(Piece piece, Position from, Position to)
        {
            return true;
        }
    }
}
