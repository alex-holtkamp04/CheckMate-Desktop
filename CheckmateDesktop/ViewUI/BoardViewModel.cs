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

namespace CheckmateDesktop.ViewUI
{
    public class BoardViewModel
    {
        private static Board gameBoard;

        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }

        private static SquareViewModel? selectedSquare = null;
        private static SolidColorBrush? selectedSquareBaseColor = null;

        ICommand squareClickCommand = new RelayCommand<SquareViewModel>(OnSquareClicked);

        public BoardViewModel()
        {
            BoardSquares = new ObservableCollection<SquareViewModel>();
            gameBoard = new Board();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 64; i++)
            {
                int Letter = i / 8;
                int Number = i % 8;

                bool isDarkSquare = (Letter + Number) % 2 != 0;

                var piece = gameBoard.GetPiece(new Position(Letter, Number));

                BoardSquares.Add(new SquareViewModel(squareClickCommand)
                {
                    SquareColorBrush = isDarkSquare ? Brushes.SaddleBrown : Brushes.Wheat,
                    PieceColorBrush = (piece?.Team == Piece.TeamColor.White) ? Brushes.White : Brushes.Black,
                    CurrentPiece = piece,
                    Position = new Position(Letter, Number)
                });
            }
        }

        private Piece? GetStartingPiece(int index)
        {
            int Letter = index / 8;
            int Number = index % 8;
            switch (Letter)
            {
                case 0:
                    if (Number == 0 || Number == 7)
                    {
                        return new Rook
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                    }
                    if (Number == 1 || Number == 6)
                    {
                        return new Knight
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                    }
                    if (Number == 2 || Number == 5)
                    {
                        return new Bishop
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                    }
                    if (Number == 3)
                    {
                        return new Queen
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                    }
                    if (Number == 4)
                    {
                        return new King
                        {
                            Team = Piece.TeamColor.Black,
                            isFirstMove = true
                        };
                    }
                    break;

                case 1:
                    return new Pawn
                    {
                        Team = Piece.TeamColor.Black,
                        isFirstMove = true
                    };

                case 6:
                    return new Pawn
                    {
                        Team = Piece.TeamColor.White,
                        isFirstMove = true
                    };

                case 7:
                    if (Number == 0 || Number == 7)
                    {
                        return new Rook
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                    }
                    if (Number == 1 || Number == 6)
                    {
                        return new Knight
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                    }
                    if (Number == 2 || Number == 5)
                    {
                        return new Bishop
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                    }
                    if (Number == 3)
                    {
                        return new Queen
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                    }
                    if (Number == 4)
                    {
                        return new King
                        {
                            Team = Piece.TeamColor.White,
                            isFirstMove = true
                        };
                    }
                    break;

                default:
                    return null;
            }
            return null;
        }

        private static void OnSquareClicked(SquareViewModel clickedSquare)
        {

            if (clickedSquare.CurrentPiece != null)
            {
                Debug.WriteLine($"Piece on square: {clickedSquare.CurrentPiece.GetType().Name} ({clickedSquare.CurrentPiece.Team})");
            }
            else if (selectedSquare == null)
            {
                Debug.WriteLine("No piece on clicked square");
                return;
            }

            if (selectedSquare == null)
            {
                selectedSquare = clickedSquare;

                Debug.WriteLine($"SELECTED FIRST SQUARE: {selectedSquare.Position.Letter},{selectedSquare.Position.Number}");
                Debug.WriteLine($"Selected piece: {selectedSquare.CurrentPiece?.GetType().Name}");

                selectedSquareBaseColor = clickedSquare.SquareColorBrush;
                clickedSquare.SquareColorBrush = Brushes.SteelBlue;
                return;
            }

            Debug.WriteLine($"TRY MOVE: {selectedSquare.Position.Letter},{selectedSquare.Position.Number} -> {clickedSquare.Position.Letter},{clickedSquare.Position.Number}");
            Piece piece = selectedSquare.CurrentPiece;

            if (piece == null)
            {
                Debug.WriteLine("ERROR: No piece selected");
                ClearSelection();
                return;
            }

            bool isValid = gameBoard.isValidMove(
                piece,
                selectedSquare.Position,
                clickedSquare.Position
            );

            Debug.WriteLine($"isValidMove returned: {isValid}");

            if (isValid)
            {
                gameBoard.MovePiece(selectedSquare.Position, clickedSquare.Position);

                clickedSquare.CurrentPiece = gameBoard.GetPiece(clickedSquare.Position);
                clickedSquare.PieceColorBrush = (clickedSquare.CurrentPiece?.Team == Piece.TeamColor.White) ? Brushes.White : Brushes.Black;
                selectedSquare.CurrentPiece = null;
            }

            ClearSelection();

            gameBoard.CalculateScores();
            Debug.WriteLine($"The Current Score is: White: {gameBoard.whiteValue} - Black: {gameBoard.blackValue}");

        }

        private static void ClearSelection()
        {
            if (selectedSquare != null)
            {
                selectedSquare.SquareColorBrush = selectedSquareBaseColor;
            }

            selectedSquare = null;
            selectedSquareBaseColor = null;

            Debug.WriteLine("SELECTION RESET");
        }
    }
}
