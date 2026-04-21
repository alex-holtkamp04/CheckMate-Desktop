using CheckmateDesktop.GameLogic;
using CheckmateDesktop.GameLogic.Pieces;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Windows.Media;
using static CheckmateDesktop.GameLogic.Piece;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace CheckmateDesktop
{
    public class Board
    {
        //public enum BoardMarkers {A, B, C, D, E, F, G, H};

        Piece[,] BoardSquares;

        TeamColor ActivePlayer;

        public int whiteValue, blackValue;

        Position WhiteKing;
        Position BlackKing;

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

            BlackKing = new Position(0, 4);
            WhiteKing = new Position(7, 4);
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

        // Function that sets the correct starting pieces on the board 
        private void setupBoard()
        {
            // do a linear loop
            for (int index = 0; index < 64; index++)
            {
                // adjust linear loop to 2D array values
                int row = index / 8;
                int col = index % 8;
                switch (row)
                {
                    // Firest row is Black's back row
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

                    // Second row are Black's pawnns
                    case 1:
                        BoardSquares[row, col] = new Pawn
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                        break;

                    // Seventh row are White's pawns
                    case 6:
                        BoardSquares[row, col] = new Pawn
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                        break;

                    // Eighth row is White's back row
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

                    // default case for null for all the other squares without starting pieces
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

                // get teams for this move
                TeamColor actingTeam = pieceToMove.Team;
                TeamColor enemyTeam;
                if (actingTeam == TeamColor.White)
                    enemyTeam = TeamColor.Black;
                else
                    enemyTeam = TeamColor.White;

                // update our king positions if we moved the king
                if (pieceToMove is King)
                {
                    if (actingTeam == TeamColor.White)
                        WhiteKing = to;
                    else
                        BlackKing = to;
                }

                // Check if making this move puts the acting player in check
                if (IsInCheck(actingTeam))
                {
                    Debug.WriteLine($"This move put the acting player, {actingTeam.ToString()}, in check -- ILLEGAL MOVE");
                }

                // Check if this move puts the enemy player in check
                if (IsInCheck(enemyTeam))
                {
                    Debug.WriteLine($"This move put the enemy player, {enemyTeam.ToString()}, in check.");
                }
            }

            // Switch the active player
            ActivePlayer = ActivePlayer == TeamColor.White ? TeamColor.Black : TeamColor.White;
        }

        // Function that returns true if a team is in check
        public bool IsInCheck(TeamColor team)
        {
            // get king position based on the team passed in
            Position KingPosition;
            if (team == TeamColor.White)
                KingPosition = WhiteKing;
            else
                KingPosition = BlackKing;

            // get king using king position
            King king = (King) GetPiece(KingPosition);

            // pawn squares search
            Piece? leftSquare = null;
            Piece? rightSquare = null;
            if (team == TeamColor.White)    // get pawn attacking positions based on team
            {
                // check two squares above for black pawns
                leftSquare = GetPiece(new Position(KingPosition.Letter - 1, KingPosition.Number - 1));
                rightSquare = GetPiece(new Position(KingPosition.Letter - 1, KingPosition.Number + 1));
            }
            else
            {
                // check two squares below for white pawns
                leftSquare = GetPiece(new Position(KingPosition.Letter + 1, KingPosition.Number - 1));
                rightSquare = GetPiece(new Position(KingPosition.Letter + 1, KingPosition.Number + 1));
            }

            // check the two pawn attacking positions for enemy pawns
            if (leftSquare != null && leftSquare.Team != team && leftSquare is Pawn)
            {
                return true;
            }
            else if (rightSquare != null && rightSquare.Team != team && rightSquare is Pawn)
            {
                return true;
            }

            // diaganol search
            foreach (var direction in new (int, int)[] { (-1, -1), (-1, 1), (1, -1), (1, 1) }) // Diagonal directions
            {
                int nextLetter = KingPosition.Letter + direction.Item1;
                int nextNumber = KingPosition.Number + direction.Item2;

                // Loop until we go out of bounds or hit a piece
                while (king.IsInBounds(new Position(nextLetter, nextNumber)))
                {
                    Position nextPosition = new Position(nextLetter, nextNumber);
                    Piece pieceAtNextPosition = GetPiece(nextPosition);

                    if (pieceAtNextPosition != null)
                    {
                        // we found a piece -- check if it can capture the King
                        if (pieceAtNextPosition.Team != team && (pieceAtNextPosition is Bishop || pieceAtNextPosition is Queen))
                        {
                            return true;
                        }
                        // if it can't (our team OR invalid moveset)
                        else
                        {
                            break;
                        }
                    }

                    nextLetter += direction.Item1;
                    nextNumber += direction.Item2;
                }
            }

            // horizontal & vertical search
            foreach (var direction in new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }) // Horizontal and vertical directions
            {
                int nextLetter = KingPosition.Letter + direction.Item1;
                int nextNumber = KingPosition.Number + direction.Item2;

                // Loop until we go out of bounds or hit a piece
                while (king.IsInBounds(new Position(nextLetter, nextNumber)))
                {
                    Position nextPosition = new Position(nextLetter, nextNumber);
                    Piece pieceAtNextPosition = GetPiece(nextPosition);

                    if (pieceAtNextPosition != null)
                    {
                        // we found a piece -- check if it can capture the King
                        if (pieceAtNextPosition.Team != team && (pieceAtNextPosition is Rook || pieceAtNextPosition is Queen))
                        {
                            return true;
                        }
                        // if it can't (our team OR invalid moveset)
                        else
                        {
                            break;
                        }
                    }

                    nextLetter += direction.Item1;
                    nextNumber += direction.Item2;
                }
            }

            // knight squares search
            foreach (var move in new (int, int)[] { (-2, -1), (-2, 1), (-1, -2), (-1, 2), (1, -2), (1, 2), (2, -1), (2, 1) }) // All 8 possible knight moves
            {
                Position nextPosition = new Position(KingPosition.Letter + move.Item1, KingPosition.Number + move.Item2);

                if (king.IsInBounds(nextPosition))
                {
                    Piece pieceAtNextPosition = GetPiece(nextPosition);

                    if (pieceAtNextPosition != null && pieceAtNextPosition.Team != team && pieceAtNextPosition is Knight)
                    {
                        return true;
                    }
                }
            }

            // nothing found, return false
            return false;
        }
    }
}
