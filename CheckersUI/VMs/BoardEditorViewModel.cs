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

    public class BoardEditorViewModel : INotifyPropertyChanged, INavigatable
    {
        public BoardEditorViewModel(Board board)
        {
            Orientation = Player.White;
            Board = board;

            Player = Player.Black;
            IsAppBarVisible = true;
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
                var controller = new GameController(Variant, Board, Player);
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

        public List<Variant> Variants =>
            Enum.GetValues(typeof(Variant)).Cast<Variant>().ToList();

        private Variant _variant;
        public Variant Variant
        {
            get { return _variant; }
            set
            {
                if (_variant != value)
                {
                    _variant = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isAppBarVisible;
        public bool IsAppBarVisible
        {
            get
            {
                return _isAppBarVisible;
            }
            set
            {
                if (value != _isAppBarVisible)
                {
                    _isAppBarVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isVariantOptionsVisible;
        public bool IsVariantOptionsVisible
        {
            get
            {
                return _isVariantOptionsVisible;
            }
            set
            {
                if (value != _isVariantOptionsVisible)
                {
                    _isVariantOptionsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isPlayerOptionsVisible;
        public bool IsPlayerOptionsVisible
        {
            get
            {
                return _isPlayerOptionsVisible;
            }
            set
            {
                if (value != _isPlayerOptionsVisible)
                {
                    _isPlayerOptionsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isBuiltinBoardPositionOptionsVisible;
        public bool IsBuiltinBoardPositionOptionsVisible
        {
            get
            {
                return _isBuiltinBoardPositionOptionsVisible;
            }
            set
            {
                if (value != _isBuiltinBoardPositionOptionsVisible)
                {
                    _isBuiltinBoardPositionOptionsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> Pages { get; } = new List<string> { "Game Page", "Board Editor", "Rules" };
        public string NavigationElement
        {
            get { return "Board Editor"; }
            set
            {
                if (value != "Board Editor")
                {
                    OnNavigationRequest(value);
                }
            }
        }

        private DelegateCommand _hideAppBarCommand;
        public DelegateCommand HideAppBarCommand
        {
            get
            {
                if (_hideAppBarCommand != null)
                {
                    return _hideAppBarCommand;
                }

                _hideAppBarCommand = new DelegateCommand(param =>
                {
                    IsVariantOptionsVisible = false;
                    IsPlayerOptionsVisible = false;
                    IsBuiltinBoardPositionOptionsVisible = false;
                    IsAppBarVisible = false;
                });
                return _hideAppBarCommand;
            }
        }

        private DelegateCommand _displayAppBarCommand;
        public DelegateCommand DisplayAppBarCommand
        {
            get
            {
                if (_displayAppBarCommand != null)
                {
                    return _displayAppBarCommand;
                }

                _displayAppBarCommand = new DelegateCommand(param =>
                {
                    IsVariantOptionsVisible = false;
                    IsPlayerOptionsVisible = false;
                    IsBuiltinBoardPositionOptionsVisible = false;
                    IsAppBarVisible = true;
                });
                return _displayAppBarCommand;
            }
        }

        private DelegateCommand _displayVariantOptionsCommand;
        public DelegateCommand DisplayVariantOptionsCommand
        {
            get
            {
                if (_displayVariantOptionsCommand != null)
                {
                    return _displayVariantOptionsCommand;
                }

                _displayVariantOptionsCommand = new DelegateCommand(param =>
                {
                    IsVariantOptionsVisible = true;
                    IsAppBarVisible = false;
                });
                return _displayVariantOptionsCommand;
            }
        }

        private DelegateCommand _displayPlayerOptionsCommand;
        public DelegateCommand DisplayPlayerOptionsCommand
        {
            get
            {
                if (_displayPlayerOptionsCommand != null)
                {
                    return _displayPlayerOptionsCommand;
                }

                _displayPlayerOptionsCommand = new DelegateCommand(param =>
                {
                    IsPlayerOptionsVisible = true;
                    IsAppBarVisible = false;
                });
                return _displayPlayerOptionsCommand;
            }
        }

        private DelegateCommand _displayBuiltinBoardPositionOptionsCommand;
        public DelegateCommand DisplayBuiltinBoardPositionOptionsCommand
        {
            get
            {
                if (_displayBuiltinBoardPositionOptionsCommand != null)
                {
                    return _displayBuiltinBoardPositionOptionsCommand;
                }

                _displayBuiltinBoardPositionOptionsCommand = new DelegateCommand(param =>
                {
                    IsBuiltinBoardPositionOptionsVisible = true;
                    IsAppBarVisible = false;
                });
                return _displayBuiltinBoardPositionOptionsCommand;
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

        public event EventHandler<string> NavigationRequest;
        protected virtual void OnNavigationRequest(string target) =>
            NavigationRequest?.Invoke(this, target);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}