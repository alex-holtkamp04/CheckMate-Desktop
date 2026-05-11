using CheckmateDesktop.GameLogic;
using CheckmateDesktop.GameLogic.Pieces;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Diagnostics;
using System.ComponentModel;

namespace CheckmateDesktop.ViewUI
{
    /* 
    This class controls the front-end model for the board.

    It displays the board itself and all the information around the board that is displayed to the user.
    It controls the visual movement of pieces and sends user movement to the backend to handle.
     */
    public class BoardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // back-end state of board
        private Board gameBoard;

        // hold user's currently selected square
        private SquareViewModel? selectedSquare = null;
        private SolidColorBrush? selectedSquareBaseColor = null;

        // Collection of squares to control front-end appearance of board squares
        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }

        ICommand squareClickCommand;

        public ICommand ResetBoardCommand { get; }

        // boolean for displaying game-over messages
        private bool _isGameOver;
        public bool IsGameOver
        {
            get => _isGameOver;
            set
            {
                _isGameOver = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsGameOver)));
            }
        }

        // front-end message that displays when game is over (winner or draw)
        private string _gameOverMessage = string.Empty;
        public string GameOverMessage
        {
            get => _gameOverMessage;
            set
            {
                _gameOverMessage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameOverMessage)));
            }
        }

        // holds value of white's score for score display
        private int _whiteScore;
        public int WhiteScore
        {
            get => _whiteScore;
            set
            {
                _whiteScore = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WhiteScore)));
            }
        }

        // holds value of black's score for score display
        private int _blackScore;
        public int BlackScore
        {
            get => _blackScore;
            set
            {
                _blackScore = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlackScore)));
            }
        }

        // display the current player turn on the front-end
        private string _currentTurnText;
        public string CurrentTurnText
        {
            get => _currentTurnText;
            set
            {
                _currentTurnText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTurnText)));
            }
        }

        // display the captured pieces for each team as a string of their Unicode characters
        private string _capturedWhitePieces;
        public string CapturedWhitePieces
        {
            get => _capturedWhitePieces;
            set
            {
                _capturedWhitePieces = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapturedWhitePieces)));
            }
        }

        private string _capturedBlackPieces;
        public string CapturedBlackPieces
        {
            get => _capturedBlackPieces;
            set
            {
                _capturedBlackPieces = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapturedBlackPieces)));
            }
        }

        // Hold move history as a collection that is bound to front-end list
        private ObservableCollection<string> _MoveHistoryDisplay = new();
        public ObservableCollection<string> MoveHistoryDisplay
        {
            get => _MoveHistoryDisplay;
            set
            {
                _MoveHistoryDisplay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoveHistoryDisplay)));
            }
        }

        public BoardViewModel()
        {
            squareClickCommand = new RelayCommand<SquareViewModel>(OnSquareClicked);
            ResetBoardCommand = new RelayCommand(ResetBoard);

            BoardSquares = new ObservableCollection<SquareViewModel>();
            gameBoard = new Board();
            InitializeBoard();

            CurrentTurnText = "White to Move";
            CapturedWhitePieces = "";
            CapturedBlackPieces = "";
        }

        // Function that initializes the board on the front end
        private void InitializeBoard()
        {
            // loop through 64 tiles linearly
            for (int i = 0; i < 64; i++)
            {
                // convert to 2D array values
                int Letter = i / 8;
                int Number = i % 8;

                // make classic dark-light chess board pattern
                bool isDarkSquare = (Letter + Number) % 2 != 0;

                // we can use the backend board to get the piece at the tile
                var piece = gameBoard.GetPiece(new Position(Letter, Number));

                // create the frontend square and add it to array of squares
                BoardSquares.Add(new SquareViewModel(squareClickCommand)
                {
                    SquareColorBrush = isDarkSquare ? Brushes.SaddleBrown : Brushes.Wheat,
                    PieceColorBrush = (piece?.Team == Piece.TeamColor.White) ? Brushes.White : Brushes.Black,
                    CurrentPiece = piece,
                    Position = new Position(Letter, Number)
                });
            }
        }

        // Function that runs when the player clicks on a square
        private void OnSquareClicked(SquareViewModel clickedSquare)
        {
            // DEBUG MESSAGES:
            //
            // If the plaeyr clicks on a square with a piece, display it
            if (clickedSquare.CurrentPiece != null)
            {
                Debug.WriteLine($"Piece on square: {clickedSquare.CurrentPiece.GetType().Name} ({clickedSquare.CurrentPiece.Team})");
            }
            else if (selectedSquare == null)
            {
                // if player clicks square without piece, print this debug message
                Debug.WriteLine("No piece on clicked square");
                return;
            }

            // If the player has not already selected a square (I.E. they're clicking the piece they want to move)
            if (selectedSquare == null)
            {
                selectedSquare = clickedSquare;

                Debug.WriteLine($"SELECTED FIRST SQUARE: {selectedSquare.Position.Row},{selectedSquare.Position.Col}");
                Debug.WriteLine($"Selected piece: {selectedSquare.CurrentPiece?.GetType().Name}");

                selectedSquareBaseColor = clickedSquare.SquareColorBrush;
                clickedSquare.SquareColorBrush = Brushes.SteelBlue;

                HighlightMoves(clickedSquare);

                return;
            }

            // Debug Message
            Debug.WriteLine($"TRY MOVE: {selectedSquare.Position.Row},{selectedSquare.Position.Col} -> {clickedSquare.Position.Row},{clickedSquare.Position.Col}");
            Piece piece = selectedSquare.CurrentPiece;

            // If the square the player selected on their first click had nothing, there's no piece selected --> can't move
            // print debug message
            if (piece == null)
            {
                Debug.WriteLine("ERROR: No piece selected");
                ClearSelection();
                return;
            }

            // check if move is valid
            bool isValid = gameBoard.isValidMove(
                piece,
                selectedSquare.Position,
                clickedSquare.Position
            );

            Debug.WriteLine($"isValidMove returned: {isValid}");

            // if move is valid, move the piece
            if (isValid)
            {

                Move? move = gameBoard.MovePiece(selectedSquare.Position, clickedSquare.Position);

                if (move == null)
                {
                    Debug.WriteLine("ERROR: MovePiece returned null");
                    ClearSelection();
                    return;
                }

                if (gameBoard.ActivePlayer == Piece.TeamColor.Black)
                {
                    int moveNumber = (MoveHistoryDisplay.Count / 2) + 1;
                    MoveHistoryDisplay.Add($"{moveNumber}. {FormatMove(move)}");
                }
                else
                {
                    // append to last line
                    if (MoveHistoryDisplay.Count > 0)
                    {
                        MoveHistoryDisplay[MoveHistoryDisplay.Count - 1] += $" {FormatMove(move)}";
                    }
                }

                if (move != null)
                {
                    // Update Pieces
                    clickedSquare.CurrentPiece = gameBoard.GetPiece(clickedSquare.Position);
                    clickedSquare.PieceColorBrush = (clickedSquare.CurrentPiece?.Team == Piece.TeamColor.White) ? Brushes.White : Brushes.Black;
                    selectedSquare.CurrentPiece = null;

                    // Check for Castling Move and update rook if necessary
                    if (piece is King)
                    {
                        int colDifference = clickedSquare.Position.Col - selectedSquare.Position.Col;

                        // Kingside castle
                        if (colDifference == 2)
                        {
                            Position rookOldPos = new Position(selectedSquare.Position.Row, 7);
                            Position rookNewPos = new Position(selectedSquare.Position.Row, 5);

                            SquareViewModel rookOldSquare =
                                BoardSquares.First(s =>
                                    s.Position.Row == rookOldPos.Row &&
                                    s.Position.Col == rookOldPos.Col);

                            SquareViewModel rookNewSquare =
                                BoardSquares.First(s =>
                                    s.Position.Row == rookNewPos.Row &&
                                    s.Position.Col == rookNewPos.Col);

                            rookNewSquare.CurrentPiece = gameBoard.GetPiece(rookNewPos);
                            rookNewSquare.PieceColorBrush =
                                (rookNewSquare.CurrentPiece?.Team == Piece.TeamColor.White)
                                ? Brushes.White
                                : Brushes.Black;

                            rookOldSquare.CurrentPiece = null;
                        }

                        // Queenside castle
                        else if (colDifference == -2)
                        {
                            Position rookOldPos = new Position(selectedSquare.Position.Row, 0);
                            Position rookNewPos = new Position(selectedSquare.Position.Row, 3);

                            SquareViewModel rookOldSquare =
                                BoardSquares.First(s =>
                                    s.Position.Row == rookOldPos.Row &&
                                    s.Position.Col == rookOldPos.Col);

                            SquareViewModel rookNewSquare =
                                BoardSquares.First(s =>
                                    s.Position.Row == rookNewPos.Row &&
                                    s.Position.Col == rookNewPos.Col);

                            rookNewSquare.CurrentPiece = gameBoard.GetPiece(rookNewPos);
                            rookNewSquare.PieceColorBrush =
                                (rookNewSquare.CurrentPiece?.Team == Piece.TeamColor.White)
                                ? Brushes.White
                                : Brushes.Black;

                            rookOldSquare.CurrentPiece = null;
                        }
                    }

                    // Update Captured Pieces
                    UpdateCapturedPieces();

                    // Update Score
                    gameBoard.CalculateScores();
                    WhiteScore = gameBoard.WhiteScore;
                    BlackScore = gameBoard.BlackScore;

                    // Update game state
                    if (gameBoard.CurrentState == Board.GameState.Checkmate)
                    {
                        GameOverMessage = $"Checkmate!\n{gameBoard.Winner} Wins!";
                        IsGameOver = true;
                    }
                    else if (gameBoard.CurrentState == Board.GameState.Stalemate)
                    {
                        GameOverMessage = "Stalemate!\nMatch is a Draw.";
                        IsGameOver = true;
                    }

                    // Update the turn text
                    else
                    {
                        CurrentTurnText = $"{gameBoard.ActivePlayer.ToString()} to Move";
                    }
                }
            }

            ClearSelection();

            Debug.WriteLine($"The Current Score is: White: {gameBoard.WhiteScore} - Black: {gameBoard.BlackScore}");
        }

        // Function that clears the selected square in memory
        // ensures that squares maintain their correct color
        private void ClearSelection()
        {
            if (selectedSquare != null)
            {
                selectedSquare.SquareColorBrush = selectedSquareBaseColor;
            }

            selectedSquare = null;
            selectedSquareBaseColor = null;

            foreach (SquareViewModel square in BoardSquares)
            {
                square.IsMoveHighlighted = false;
            }

            Debug.WriteLine("SELECTION RESET");
        }

        // Method that updates the strings that display the captured pieces
        private void UpdateCapturedPieces()
        {
            CapturedWhitePieces = "";
            foreach (Piece p in gameBoard.CapturedWhitePieces)
            {
                CapturedWhitePieces += $"{GetPieceUnicode(p)}";
            }

            CapturedBlackPieces = "";
            foreach (Piece p in gameBoard.CapturedBlackPieces)
            {
                CapturedBlackPieces += $"{GetPieceUnicode(p)}";
            }
        }

        // Helper method for displaying captured pieces
        private string GetPieceUnicode(Piece piece)
        {
            if (piece == null) return "";

            if (piece.Team == Piece.TeamColor.White)
            {
                switch (piece)
                {
                    case Bishop: return "♗";
                    case King: return "♔";
                    case Knight: return "♘";
                    case Pawn: return "♙";
                    case Queen: return "♕";
                    case Rook: return "♖";
                    default: return "";
                }
            }
            else // Piece is Black
            {
                switch (piece)
                {
                    case Bishop: return "♝";
                    case King: return "♚";
                    case Knight: return "♞";
                    case Pawn: return "♟";
                    case Queen: return "♛";
                    case Rook: return "♜";
                    default: return "";
                }
            }
        }

        // Method that highlights valid moves based which square the user has selected
        private void HighlightMoves(SquareViewModel clickedSquare)
        {
            Piece piece = clickedSquare.CurrentPiece;
            if (piece == null) return;

            List<Position> moves = piece.GetValidMoves(gameBoard, clickedSquare.Position);

            foreach (SquareViewModel square in BoardSquares)
            {
                // If the square's position matches any valid move, highlight it
                bool isValid = moves.Any(m => m.Row == square.Position.Row && m.Col == square.Position.Col);

                if (isValid)
                {
                    square.IsMoveHighlighted = true;
                }
            }
        }

        // Method to reset the board and all the surrounding displays
        private void ResetBoard()
        {
            gameBoard = new Board();

            BoardSquares.Clear();
            InitializeBoard();

            ClearSelection();

            MoveHistoryDisplay.Clear();

            IsGameOver = false;
            GameOverMessage = string.Empty;

            CurrentTurnText = "White to Move";
            CapturedWhitePieces = "";
            CapturedBlackPieces = "";
            WhiteScore = 0;
            BlackScore = 0;
        }

        // Method for converting row-column logic to standard chess move display
        private string FormatMove(Move move)
        {
            string pieceLetter = move.Piece switch
            {
                Pawn => "",
                Knight => "N",
                Bishop => "B",
                Rook => "R",
                Queen => "Q",
                King => "K",
                _ => ""
            };

            char letter = (char)('a' + move.To.Col);
            int number = 8 - move.To.Row;

            if (move.Piece is Pawn && move.IsCapture)
            {
                char fromFile = (char)('a' + move.From.Col);
                return $"{fromFile}x{letter}{number}";
            }

            return $"{pieceLetter}{(move.IsCapture ? "x" : "")}{letter}{number}";
        }
    }
}
