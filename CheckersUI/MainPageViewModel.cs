using Checkers;
using Checkers.Extensions;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System.Collections.Generic;
using static Checkers.Types;

namespace CheckersUI
{
    public class MainPageViewModel
    {
        private MainPage _page;

        public MainPageViewModel(MainPage page)
        {
            _page = page;
            Board = Checkers.Board.defaultBoard as FSharpList<FSharpList<FSharpOption<Piece.Piece>>>;
        }

        private FSharpList<FSharpList<FSharpOption<Piece.Piece>>> _board;
        public FSharpList<FSharpList<FSharpOption<Piece.Piece>>> Board
        {
            get
            {
                return _board;
            }
            set
            {
                _board = value;
                _page.UpdateBoard(value);
            }
        }

        private Coord _selection;
        public Coord Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                if (_selection == null)
                {
                    _selection = value;
                }
                else if (_selection != null && Board.IsValidMove(_selection, value))
                {
                    Board = Board.Move(_selection, value).Value;
                    _selection = null;
                }
                else if (Board[value.Row][value.Column] == FSharpOption<Piece.Piece>.None)
                {
                    _selection = null;
                }
                else
                {
                    _selection = value;
                }
            }
        }
    }
}