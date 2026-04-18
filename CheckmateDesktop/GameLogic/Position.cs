using System;
using System.Collections.Generic;
using System.Text;

namespace CheckmateDesktop.GameLogic
{
    public class Position
    {
        public int Letter { get; set; }
        public int Number { get; set; }
        public Position(int letter, int number)
        {
            Letter = letter;
            Number = number;
        }
    }
}
