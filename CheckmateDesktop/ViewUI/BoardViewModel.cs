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
    public class BoardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Board gameBoard;
        private SquareViewModel? selectedSquare = null;
        private SolidColorBrush? selectedSquareBaseColor = null;

        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }

        ICommand squareClickCommand;

        public ICommand ResetBoardCommand { get; }

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
                if (gameBoard.MovePiece(selectedSquare.Position, clickedSquare.Position))
                {
                    // Update Pieces
                    clickedSquare.CurrentPiece = gameBoard.GetPiece(clickedSquare.Position);
                    clickedSquare.PieceColorBrush = (clickedSquare.CurrentPiece?.Team == Piece.TeamColor.White) ? Brushes.White : Brushes.Black;
                    selectedSquare.CurrentPiece = null;

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

        private void ResetBoard()
        {
            gameBoard = new Board();

            BoardSquares.Clear();
            InitializeBoard();

            ClearSelection();

            IsGameOver = false;
            GameOverMessage = string.Empty;

            CurrentTurnText = "White to Move";
            CapturedWhitePieces = "";
            CapturedBlackPieces = "";
            WhiteScore = 0;
            BlackScore = 0;
        }
    }
}
