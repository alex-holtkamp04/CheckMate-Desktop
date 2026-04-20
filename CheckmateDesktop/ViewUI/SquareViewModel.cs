using CheckmateDesktop.GameLogic;
using CheckmateDesktop.GameLogic.Pieces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;


namespace CheckmateDesktop
{
    public class SquareViewModel(ICommand bossCommand) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // property that sets the color of a tile on the front end
        private SolidColorBrush _squareColorBrush;
        public SolidColorBrush SquareColorBrush
        {
            get { return _squareColorBrush; }
            set
            {
                _squareColorBrush = value;
                OnPropertyChanged(nameof(SquareColorBrush));
            }
        }


        // property that sets the color of a piece on the front end
        private SolidColorBrush _pieceColorBrush;
        public SolidColorBrush PieceColorBrush {
            get
            {
                return _pieceColorBrush;
            }
            set
            {
                _pieceColorBrush = value;
                OnPropertyChanged(nameof(PieceColorBrush));
            }
        }

        // The current Piece on the tile
        private Piece _currentPiece;
        public Piece CurrentPiece
        {
            get
            {
                return _currentPiece;
            }
            set
            {
                // when we change the piece, update this Piece property AND the string representation
                _currentPiece = value;
                OnPropertyChanged(nameof(CurrentPiece));
                OnPropertyChanged(nameof(PieceUnicode));

            }
        }


        public required Position Position { get; set; }

        public string PieceUnicode 
        { 
            get
            {
                if (_currentPiece == null) return "";

                switch (_currentPiece)
                {
                    case Bishop:
                        return "♝";
                    case King:
                        return "♚";
                    case Knight:
                        return "♞";
                    case Pawn:
                        return "♟";
                    case Queen:
                        return "♛";
                    case Rook:
                        return "♜";
                    default:
                        return "";

                }
            }
        }

        public ICommand ClickCommand { get; } = bossCommand;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
