using CheckmateDesktop.GameLogic;
using CheckmateDesktop.GameLogic.Pieces;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Media;
using static CheckmateDesktop.GameLogic.Piece;
using System.Diagnostics;

namespace CheckmateDesktop
{
    public class Board
    {
        //public enum BoardMarkers {A, B, C, D, E, F, G, H};

        Piece[,] BoardSquares;

        TeamColor ActivePlayer;

        public int whiteValue, blackValue;

        // Loop through the board and calculate the total value of pieces for each player
        public void CalculateScores()
        {
            whiteValue = 0;
            blackValue = 0;

            for (int letter = 0; letter < 8; letter++)
            {
                for (int number = 0; number < 8; number++)
                {
                    Piece piece = BoardSquares[letter, number];
                    if (piece != null)
                    {
                        if (piece.Team == TeamColor.White)
                        {
                            whiteValue += piece.Value;
                        }
                        else if (piece.Team == TeamColor.Black)
                        {
                            blackValue += piece.Value;
                        }
                    }
                }
            }
        }

        // Constructor to initialize the board with pieces in their starting positions
        public Board()
        {
            ActivePlayer = TeamColor.White;
            BoardSquares = new Piece[8, 8];
            setupBoard();
        }

        // Functions to get and set pieces on the board
        public Piece GetPiece(Position pos)
        {
            return BoardSquares[pos.Letter, pos.Number];
        }

        public void SetPiece(Position pos, Piece piece, TeamColor team)
        {
            if (piece != null)
            {
                piece.Team = team;
            }
            BoardSquares[pos.Letter, pos.Number] = piece;
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

            // Checks if the piece being moved belongs to the active player
            if (GetPiece(from).Team != ActivePlayer)
            {
                Debug.WriteLine("Invalid move: piece does not belong to active player.");
                return false;
            }

            // Get the valid moves for the piece and check if the to position is in the list of valid moves
            List<Position> validMoves = piece.GetValidMoves(this, from);

            bool result = validMoves.Any(move => move.Letter == to.Letter && move.Number == to.Number);

            // Return if the selected move is valid or not
            return result;
        }

        // Function to move the piece on the board
        public void MovePiece(Position from, Position to)
        {
            // Get the piece to move
            Piece pieceToMove = GetPiece(from);

            // Set the piece to the new position
            SetPiece(to, pieceToMove, pieceToMove.Team);
            // Clear the old position
            SetPiece(from, null, TeamColor.None);

            if (pieceToMove != null)
            {
                pieceToMove.isFirstMove = false;
            }

            // Switch the active player
            ActivePlayer = ActivePlayer == TeamColor.White ? TeamColor.Black : TeamColor.White;
        }
    }
}
