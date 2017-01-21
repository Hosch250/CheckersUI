﻿using CheckersUI.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CheckersUI.Facade;
using Windows.Storage;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Media.Core;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace CheckersUI.VMs
{
    public class GamePageViewModel : INotifyPropertyChanged
    {
        private IPropertySet RoamingSettings
        {
            get
            {
                try
                {
                    return ApplicationData.Current.RoamingSettings.Values;
                }
                catch (InvalidOperationException)
                {
                    // we are running in a test and can't load the settings
                    return new PropertySet
                    {
                        {"Theme", "Wood"},
                        {"EnableSoundEffects", bool.FalseString},
                    };
                }
            }
        }

        public GamePageViewModel()
        {
            Controller = new GameController(Variant.AmericanCheckers);

            GameCancelled = false;
            BlackOpponent = Opponent.Human;
            WhiteOpponent = Opponent.Computer;
            Variant = Variant.AmericanCheckers;
            Level = 9;

            SetupOption = Setup.Default;

            PlayerTurn += HandlePlayerTurnAsync;
        }

        private static CancellationTokenSource _cancelComputerMoveTokenSource;
        private async void HandlePlayerTurnAsync(object sender, Player e)
        {
            if ((e == Player.Black && BlackOpponent == Opponent.Computer ||
                e == Player.White && WhiteOpponent == Opponent.Computer) &&
                e == Controller.CurrentPlayer &&
                Controller.GetWinningPlayer() == null)
            {
                _cancelComputerMoveTokenSource?.Dispose();
                _cancelComputerMoveTokenSource = new CancellationTokenSource();

                List<Coord> move;
                await Task.Run(async () =>
                {
                    move = Controller.GetMove(Level).ToList();
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        MovePiece(move));
                }, _cancelComputerMoveTokenSource.Token);

                if (!_cancelComputerMoveTokenSource.IsCancellationRequested)
                {
                    OnPlayerTurn(OtherPlayer(e));
                }
            }
        }

        private static Player OtherPlayer(Player player) =>
            player == Player.Black ? Player.White : Player.Black;

        private GameController _controller;
        public GameController Controller
        {
            get { return _controller; }
            set
            {
                _controller = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(CanTakeback));
                OnPropertyChanged(nameof(IsGameInProgress));
            }
        }

        private async void PlayEffectAsync()
        {
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync($"Assets\\{(string)RoamingSettings["Theme"]}Theme");
            var file = await folder.GetFileAsync("CheckerClick.mp3");
            var stream = await file.OpenAsync(FileAccessMode.Read);

            var player = BackgroundMediaPlayer.Current;
            var mediaSource = MediaSource.CreateFromStream(stream, file.ContentType);
            (player.Source as IDisposable)?.Dispose();
            player.Source = mediaSource;
            
            player.Play();
        }

        private void MovePiece(List<Coord> move)
        {
            if (!IsGameInProgress)
            {
                return;
            }

            if (bool.Parse((string)RoamingSettings["EnableSoundEffects"]))
            {
                PlayEffectAsync();
            }

            _selection = move.Last();
            Controller = Controller.WithBoard(LastMove()).Move(move);
        }

        private bool IsFenLastMove(string fen)
        {
            if (!Controller.MoveHistory.Any())
            {
                return true;
            }

            if (Controller.CurrentPlayer == Player.Black)
            {
                var isContinuedMove = Controller.MoveHistory.Last().WhiteMove == null;
                return isContinuedMove
                    ? Controller.MoveHistory.Last().BlackMove.ResultingFen == fen
                    : Controller.MoveHistory.Last().WhiteMove.ResultingFen == fen;
            }
            else
            {
                var isContinuedMove = Controller.MoveHistory.Last().WhiteMove != null;
                return isContinuedMove
                    ? Controller.MoveHistory.Last().WhiteMove.ResultingFen == fen
                    : Controller.MoveHistory.Last().BlackMove.ResultingFen == fen;
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
                if (Controller.CurrentPlayer == Player.Black && BlackOpponent == Opponent.Computer ||
                    Controller.CurrentPlayer == Player.White && WhiteOpponent == Opponent.Computer)
                {
                    return;
                }

                if (!IsFenLastMove(Controller.Fen)) { return; }

                if (_selection != null && Controller.IsValidMove(_selection, value))
                {
                    MovePiece(new List<Coord> {_selection, value});

                    var piece = Controller.Board[_selection];
                    OnPlayerTurn(OtherPlayer(piece.Player));
                }
                else if (Controller.Board.GameBoard[value.Row, value.Column] == null)
                {
                    _selection = null;
                }
                else
                {
                    _selection = value;
                }
            }
        }

        private Variant _variant;
        public Variant Variant
        {
            get { return _variant; }
            set
            {
                _variant = value;
                OnPropertyChanged();
            }
        }

        private int _level;
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }

        private string LastMove()
        {
            var pdnMove = Controller.MoveHistory.LastOrDefault();
            if (pdnMove == null)
            {
                return Controller.InitialPosition;
            }

            return pdnMove.WhiteMove?.ResultingFen ?? pdnMove.BlackMove.ResultingFen;
        }

        public string Status
        {
            get
            {
                var winningPlayer = Controller.WithBoard(LastMove()).GetWinningPlayer();
                return winningPlayer.HasValue && winningPlayer.Value != Controller.CurrentPlayer
                       ? $"{winningPlayer.Value} Won!"
                       : $"{Controller.CurrentPlayer}'s turn";
            }
        }

        private bool _displayCreateGameGrid;
        public bool DisplayCreateGameGrid
        {
            get { return _displayCreateGameGrid; }
            set
            {
                _displayCreateGameGrid = value;
                OnPropertyChanged();
            }
        }

        private Setup _setupOption;
        public Setup SetupOption
        {
            get { return _setupOption; }
            set
            {
                if (_setupOption != value)
                {
                    _setupOption = value;
                    OnPropertyChanged();
                }
            }
        }

        public Player BoardOrientation =>
            BlackOpponent == Opponent.Human ? Player.Black : Player.White;

        public List<Opponent> Opponents =>
            Enum.GetValues(typeof(Opponent)).Cast<Opponent>().ToList();

        public List<Variant> Variants =>
            Enum.GetValues(typeof(Variant)).Cast<Variant>().ToList();

        public List<Setup> SetupOptions =>
            Enum.GetValues(typeof(Setup)).Cast<Setup>().ToList();

        private string GetOpponentText(Opponent opponent) =>
            opponent == Opponent.Human
            ? opponent.ToString()
            : opponent.ToString() + " Level " + Level;

        private Opponent _whiteOpponent;
        public Opponent WhiteOpponent
        {
            get { return _whiteOpponent; }
            set
            {
                if (_whiteOpponent != value)
                {
                    _whiteOpponent = value;
                    OnPropertyChanged();
                }
            }
        }

        public string WhiteOpponentText =>
            GetOpponentText(WhiteOpponent);

        private Opponent _blackOpponent;
        public Opponent BlackOpponent
        {
            get { return _blackOpponent; }
            set
            {
                if (_blackOpponent != value)
                {
                    _blackOpponent = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BlackOpponentText =>
            GetOpponentText(BlackOpponent);
        
        private bool GameCancelled { get; set; }
        public bool IsGameInProgress
        {
            get
            {
                var winningPlayer = Controller.WithBoard(LastMove()).GetWinningPlayer();
                return !GameCancelled && !(winningPlayer.HasValue && winningPlayer.Value != Controller.CurrentPlayer);
            }
        }
        
        public bool CanTakeback
        {
            get
            {
                return Controller.MoveHistory.Any() &&
                       Controller.GetWinningPlayer() == null &&
                       (BlackOpponent == Opponent.Human || WhiteOpponent == Opponent.Human);
            }
        }

        private DelegateCommand _displayCreateGameCommand;
        public DelegateCommand DisplayCreateGameCommand
        {
            get
            {
                if (_displayCreateGameCommand != null)
                {
                    return _displayCreateGameCommand;
                }

                _displayCreateGameCommand = new DelegateCommand(sender => DisplayCreateGameGrid = true);
                return _displayCreateGameCommand;
            }
        }

        private DelegateCommand _createGameCommand;
        public DelegateCommand CreateGameCommand
        {
            get
            {
                if (_createGameCommand != null)
                {
                    return _createGameCommand;
                }
                
                _createGameCommand = new DelegateCommand(param => CreateGame(SetupOption == Setup.FromPosition ? (string)param : string.Empty));
                return _createGameCommand;
            }
        }

        private DelegateCommand _undoMoveCommand;
        public DelegateCommand UndoMoveCommand
        {
            get
            {
                if (_undoMoveCommand != null)
                {
                    return _undoMoveCommand;
                }

                _undoMoveCommand = new DelegateCommand(sender => TakebackMove());
                return _undoMoveCommand;
            }
        }

        private DelegateCommand _cancelGameCommand;
        public DelegateCommand CancelGameCommand
        {
            get
            {
                if (_cancelGameCommand != null)
                {
                    return _cancelGameCommand;
                }

                _cancelGameCommand = new DelegateCommand(sender =>
                {
                    GameCancelled = true;
                    OnPropertyChanged(nameof(IsGameInProgress));
                });
                return _cancelGameCommand;
            }
        }

        private DelegateCommand _moveHistoryCommand;
        public DelegateCommand MoveHistoryCommand
        {
            get
            {
                if (_moveHistoryCommand != null)
                {
                    return _moveHistoryCommand;
                }

                _moveHistoryCommand = new DelegateCommand(param => Controller = Controller.WithBoard((string)param));
                return _moveHistoryCommand;
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

                _copyFenCommand = new DelegateCommand(param =>
                {
                    var move = (PdnMove)param;
                    SetClipboardContent(move.ResultingFen);
                });
                return _copyFenCommand;
            }
        }

        private void CreateGame(string param)
        {
            DisplayCreateGameGrid = false;
            GameCancelled = false;

            Controller = string.IsNullOrEmpty(param) ? new GameController(Variant) : GameController.FromPosition(Variant, param);

            OnPropertyChanged(nameof(BoardOrientation));
            OnPropertyChanged(nameof(BlackOpponentText));
            OnPropertyChanged(nameof(WhiteOpponentText));
            OnPropertyChanged(nameof(IsGameInProgress));

            OnPlayerTurn(Controller.CurrentPlayer);
        }

        private void TakebackMove()
        {
            Controller = Controller.TakebackMove();

            if (Controller.CurrentPlayer == Player.Black && BlackOpponent == Opponent.Computer ||
                Controller.CurrentPlayer == Player.White && WhiteOpponent == Opponent.Computer)
            {
                Controller = Controller.TakebackMove();
            }

            OnPlayerTurn(Controller.CurrentPlayer);
        }

        private void SetClipboardContent(string content)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);
        }

        public event EventHandler<Player> PlayerTurn;
        protected virtual void OnPlayerTurn(Player e) =>
            PlayerTurn?.Invoke(this, e);

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}