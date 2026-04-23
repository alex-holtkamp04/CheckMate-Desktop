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
using System.Windows.Navigation;

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

        public enum GameState { Normal, Check, Checkmate, Stalemate };

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

        // Copy Constructor so we can quickly copy Boards when testing Game State
        public Board (Board oldBoard)
        {
            BoardSquares = new Piece[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    BoardSquares[row, col] = oldBoard.BoardSquares[row, col];
                }
            }

            ActivePlayer = oldBoard.ActivePlayer;
            whiteValue = oldBoard.whiteValue;
            blackValue = oldBoard.blackValue;
            WhiteKing = oldBoard.WhiteKing;
            BlackKing = oldBoard.BlackKing;
        }

        // Functions to get and set pieces on the board
        public Piece GetPiece(Position pos)
        {
            return BoardSquares[pos.Row, pos.Col];
        }

        public void SetPiece(Position pos, Piece piece, TeamColor team)
        {
            if (piece != null)
            {
                piece.Team = team;
            }
            BoardSquares[pos.Row, pos.Col] = piece;
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

            bool result = validMoves.Any(move => move.Row == to.Row && move.Col == to.Col);

            // Return if the selected move is valid or not
            return result;
        }

        public void ExecuteMove(Position from, Position to)
        {
            // Get the piece to move
            Piece pieceToMove = GetPiece(from);

            // Set the piece to the new position
            SetPiece(to, pieceToMove, pieceToMove.Team);

            // Clear the old position
            SetPiece(from, null, TeamColor.None);

            // update our king positions if we moved the king
            if (pieceToMove is King)
            {
                if (pieceToMove.Team == TeamColor.White)
                    WhiteKing = to;
                else
                    BlackKing = to;
            }
        }

        // Function to move the piece on the board
        public void MovePiece(Position from, Position to)
        {
            // BEFORE MOVE -- Call GetLegalMoves() to ensure player can make this move (whether they're in check or not)

            // Get the piece to move
            Piece pieceToMove = GetPiece(from);

            ExecuteMove(from, to);

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

                // Get the moves the enemy player can make after the acting player's move
                List<Move> LegalMoves = GetLegalMoves(enemyTeam);

                // What state does this put the enemy player in?
                GameState enemyGameState = GetGameState(enemyTeam, LegalMoves);
                switch (enemyGameState)
                {
                    case GameState.Check:
                        Debug.WriteLine($"This move put the enemy player, {enemyTeam.ToString()}, in check.");
                        break;
                    case GameState.Checkmate:
                        Debug.WriteLine($"This move put the enemy player, {enemyTeam.ToString()}, in checkmate. VICTORY.");
                        break;
                    case GameState.Stalemate:
                        Debug.WriteLine($"This move put the enemy player, {enemyTeam.ToString()}, in stalemate. DRAW.");
                        break;
                    case GameState.Normal:
                        break;
                }
            }

            // Switch the active player
            ActivePlayer = ActivePlayer == TeamColor.White ? TeamColor.Black : TeamColor.White;
        }

        // Function that returns true if a team is in check
        protected bool IsInCheck(TeamColor team)
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
            Position leftSquarePos;
            Piece? leftSquare = null;
            Position rightSquarePos;
            Piece? rightSquare = null;
            if (team == TeamColor.White)    // get pawn attacking positions based on team
            {
                // check two squares above for black pawns
                leftSquarePos = new Position(KingPosition.Row - 1, KingPosition.Col - 1);
                if (king.IsInBounds(leftSquarePos))
                    leftSquare = GetPiece(leftSquarePos);

                rightSquarePos = new Position(KingPosition.Row - 1, KingPosition.Col + 1);
                if (king.IsInBounds(rightSquarePos))
                    rightSquare = GetPiece(rightSquarePos);
            }
            else
            {
                // check two squares below for white pawns
                leftSquarePos = new Position(KingPosition.Row + 1, KingPosition.Col - 1);
                if (king.IsInBounds(leftSquarePos))
                    leftSquare = GetPiece(leftSquarePos);

                rightSquarePos = new Position(KingPosition.Row + 1, KingPosition.Col + 1);
                if (king.IsInBounds(rightSquarePos))
                    rightSquare = GetPiece(rightSquarePos);
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
                int nextLetter = KingPosition.Row + direction.Item1;
                int nextNumber = KingPosition.Col + direction.Item2;

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
                int nextLetter = KingPosition.Row + direction.Item1;
                int nextNumber = KingPosition.Col + direction.Item2;

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
                Position nextPosition = new Position(KingPosition.Row + move.Item1, KingPosition.Col + move.Item2);

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
        
        // Function that gets all possible moves for every single piece for one team
        protected List<Move> GetAllMoves(TeamColor team)
        {
            List<Move> AllMoves = new List<Move>();

            // Loop through all pieces on the board
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    // Check the square for a piece
                    Piece thisPiece = BoardSquares[row, col];

                    // if a piece is on the square AND it's on the correct team
                    if (thisPiece != null && thisPiece.Team == team)
                    {
                        // Get the pieces possible moves
                        Position thisPosition = new Position(row, col);
                        List<Position> Possiblemoves = thisPiece.GetValidMoves(this, thisPosition);

                        // add all moves to the return array
                        foreach (Position move in Possiblemoves)
                        {
                            Move newMove = new Move(thisPiece, thisPosition, move);
                            AllMoves.Add(newMove);
                        }
                    }
                }
            }

            return AllMoves;
        }

        // Function that gets all the legal moves a player can make (i.e. doesn't put the King in Check)
        protected List<Move> GetLegalMoves(TeamColor team)
        {
            // Get all possible moves every piece the player has can make
            List<Move> PossibleMoves = GetAllMoves(team);

            // Track legal moves the player can make
            List<Move> LegalMoves = new List<Move>();

            // Find which of the moves DON'T put the King in check
            foreach (Move move in PossibleMoves)
            {
                if (move.From != null && move.To != null)
                {
                    Board tempBoard = new Board(this);
                    tempBoard.ExecuteMove(move.From, move.To);

                    if (!tempBoard.IsInCheck(team))
                    {
                        LegalMoves.Add(move);
                    }
                }

            }

            return LegalMoves;
        }

        // Function to get the gamestate for a player after a turn
        // takes the list of legal moves that player can make as a parameter
        protected GameState GetGameState(TeamColor team, List<Move> LegalMoves)
        {
            bool isInCheck = IsInCheck(team);

            // if in Check, it's either Check or Checkmate
            if (isInCheck)
            {
                if (LegalMoves.Count > 0)
                {
                    return GameState.Check;
                }
                else
                {
                    return GameState.Checkmate;
                }
            }
            else    // if NOT in check, either Normal or Stalemate
            {
                if (LegalMoves.Count > 0)
                {
                    return GameState.Normal;
                }
                else
                {
                    return GameState.Stalemate;
                }
            }
        }
    }
}
