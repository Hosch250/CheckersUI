using Checkers;

namespace CheckersUI
{
    public class MainPageViewModel
    {
        private MainPage _page;

        public MainPageViewModel(MainPage page)
        {
            _page = page;
            Board = new Board();
        }

        private Board _board;
        public Board Board
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
    }
}