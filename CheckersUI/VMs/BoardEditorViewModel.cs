using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CheckersUI.Facade;

namespace CheckersUI.VMs
{
    public enum BoardPosition { Initial, Empty, FEN }

    public class BoardEditorViewModel : INotifyPropertyChanged
    {
        public BoardEditorViewModel()
        {
            Player = Player.Black;
            Position = BoardPosition.Empty;

            Board = Board.EmptyBoard();
        }

        public void AddPiece(Piece piece, int row, int column)
        {
            Board.GameBoard[row, column] = piece;
            Board = new Board(Board);
        }

        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                if (_board != value)
                {
                    _board = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Player> Players =>
            Enum.GetValues(typeof(Player)).Cast<Player>().ToList();

        private Player _player;
        public Player Player
        {
            get { return _player; }
            set {
                if (_player != value)
                {
                    _player = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<BoardPosition> Positions =>
            Enum.GetValues(typeof(BoardPosition)).Cast<BoardPosition>().ToList();

        private BoardPosition _position;
        public BoardPosition Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}