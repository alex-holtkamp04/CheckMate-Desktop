using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Media;

namespace CheckmateDesktop.ViewUI
{
    public class BoardViewModel
    {
        public ObservableCollection<ChessSquare> BoardSquares { get; set; }

        public BoardViewModel() 
        {
            BoardSquares = new ObservableCollection<ChessSquare>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < 64; i++)
            {
                bool isDarkSquare = (i / 8 + i % 8) % 2 != 0;

                BoardSquares.Add(new ChessSquare
                {
                    SquareColorBrush = isDarkSquare ? Brushes.SaddleBrown : Brushes.Wheat,
                    PieceUnicode = "", // Add your piece logic here
                    PieceColorBrush = Brushes.Black
                });
            }
        }
    }
}
