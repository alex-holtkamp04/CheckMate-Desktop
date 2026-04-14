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

namespace CheckmateDesktop.ViewUI
{
    public class BoardViewModel
    {
        private Board gameBoard;
        public ObservableCollection<SquareViewModel> BoardSquares { get; set; }

        private static SquareViewModel? selectedSquare = null;

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
                bool isDarkSquare = (i / 8 + i % 8) % 2 != 0;
                Piece? newPiece = GetStartingPiece(i);

                BoardSquares.Add(new SquareViewModel(squareClickCommand)
                {
                    SquareColorBrush = isDarkSquare ? Brushes.SaddleBrown : Brushes.Wheat,
                    PieceColorBrush = (newPiece?.Team == Piece.TeamColor.White) ? Brushes.White : Brushes.Black,
                    CurrentPiece = newPiece
                });
            }
        }

        private Piece? GetStartingPiece(int index)
        {
            int row = index / 8;
            int col = index % 8;
            switch (row)
            {
                case 0:
                    if (col == 0 || col == 7)
                    {
                        return new Rook
                        {
                            Team = Piece.TeamColor.Black
                        };
                    }
                    if (col == 1 || col == 6)
                    {
                        return new Knight
                        {
                            Team = Piece.TeamColor.Black
                        };
                    }
                    if (col == 2 || col == 5)
                    {
                        return new Bishop
                        {
                            Team = Piece.TeamColor.Black
                        };
                    }
                    if (col == 3)
                    {
                        return new Queen
                        {
                            Team = Piece.TeamColor.Black
                        };
                    }
                    if (col == 4)
                    {
                        return new King
                        {
                            Team = Piece.TeamColor.Black
                        };
                    }
                    break;

                case 1:
                    return new Pawn
                    {
                        Team = Piece.TeamColor.Black
                    };

                case 6:
                    return new Pawn
                    {
                        Team = Piece.TeamColor.White
                    };

                case 7:
                    if (col == 0 || col == 7)
                    {
                        return new Rook
                        {
                            Team = Piece.TeamColor.White
                        };
                    }
                    if (col == 1 || col == 6)
                    {
                        return new Knight
                        {
                            Team = Piece.TeamColor.White
                        };
                    }
                    if (col == 2 || col == 5)
                    {
                        return new Bishop
                        {
                            Team = Piece.TeamColor.White
                        };
                    }
                    if (col == 3)
                    {
                        return new Queen
                        {
                            Team = Piece.TeamColor.White
                        };
                    }
                    if (col == 4)
                    {
                        return new King
                        {
                            Team = Piece.TeamColor.White
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
            if (selectedSquare == null)
            {
                selectedSquare = clickedSquare;
                MessageBox.Show("clicked on " + clickedSquare.PieceUnicode);
            }
            else
            {
                MessageBox.Show("moved " + selectedSquare.PieceUnicode + " to square ");
                selectedSquare = null;
            }
        }
    }
}
