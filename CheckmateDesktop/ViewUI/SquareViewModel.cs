using CheckmateDesktop.GameLogic;
using CheckmateDesktop.GameLogic.Pieces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace CheckmateDesktop
{
    public class SquareViewModel: INotifyPropertyChanged
    {
        public SolidColorBrush SquareColorBrush { get; set; }

        public SolidColorBrush PieceColorBrush { get; set; }

        private Piece _currentPiece;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Piece CurrentPiece
        {
            get
            {
                return _currentPiece;
            }
            set
            {
                _currentPiece = value;
                OnPropertyChanged(nameof(CurrentPiece));

            }
        }

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

        private void OnPropertyChanged(string propertyName)
        {

        }

    }
}
