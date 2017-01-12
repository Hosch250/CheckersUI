using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.DataTransfer;
using CheckersUI.Command;
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
        }

        public void AddPiece(Piece piece, int row, int column)
        {
            var newBoard = Board.Copy();
            newBoard.GameBoard[row, column] = piece;
            Board = newBoard;
        }

        public void RemovePiece(int row, int column)
        {
            var newBoard = Board.Copy();
            newBoard.GameBoard[row, column] = null;
            Board = newBoard;
        }

        public void UpdateFen() => OnPropertyChanged(nameof(FenString));

        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                _board = value;
                OnPropertyChanged();
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
                    OnPropertyChanged(nameof(FenString));
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
                var controller = new GameController(Variant.AmericanCheckers, Board, Player);
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

        private DelegateCommand _copyFenCommand;
        public DelegateCommand CopyFenCommand
        {
            get
            {
                if (_copyFenCommand != null)
                {
                    return _copyFenCommand;
                }

                _copyFenCommand = new DelegateCommand(param => SetClipboardContent(FenString));
                return _copyFenCommand;
            }
        }

        private void SetClipboardContent(string content)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}