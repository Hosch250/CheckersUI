using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CheckersUI.Facade;

namespace CheckersUI.VMs
{
    public enum BoardPosition { Initial, Empty }

    public class BoardEditorViewModel : INotifyPropertyChanged
    {
        public BoardEditorViewModel()
        {
            Orientation = Player.White;
            Board = new Board();

            Player = Player.Black;
            Position = BoardPosition.Initial;
        }

        public void AddPiece(Piece piece, int row, int column)
        {
            Board.GameBoard[row, column] = piece;
            OnPropertyChanged(nameof(Board));
            OnPropertyChanged(nameof(FenString));
        }

        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                _board = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FenString));
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

        private Player _orientation;
        public Player Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FenString
        {
            get
            {
                var controller = new GameController(Board, Player);
                return controller.Fen;
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
                if (_position == value) { return; }

                _position = value;
                switch (value)
                {
                    case BoardPosition.Initial:
                        Board = new Board();
                        break;
                    case BoardPosition.Empty:
                        Board = Board.EmptyBoard();
                        break;
                    default:
                        throw new ArgumentException(nameof(value));
                }
                    
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}