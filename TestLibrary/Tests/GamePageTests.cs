using CheckersUI.Facade;
using CheckersUI.VMs;
using Xunit;

#pragma warning disable VSD0025 // Implements the most common configuration of naming conventions.
namespace TestLibrary.Tests
{
    public class GamePageTests
    {
        [Fact]
        public void Undo_UndoesMove()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers)
                    .Move(new Coord(2, 1), new Coord(3, 0))
                    .Move(new Coord(5, 6), new Coord(4, 7)),
                BlackOpponent = CheckersUI.Opponent.Human,
                WhiteOpponent = CheckersUI.Opponent.Human
            };


            Assert.Equal(Piece.WhiteChecker, vm.Controller.Board[4, 7]);

            vm.UndoMoveCommand.Execute(null);

            Assert.Null(vm.Controller.Board[4, 7]);
        }

        [Fact]
        public void Undo_UndoesMoveHistory()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers)
                    .Move(new Coord(2, 1), new Coord(3, 0))
                    .Move(new Coord(5, 6), new Coord(4, 7)),
                BlackOpponent = CheckersUI.Opponent.Human,
                WhiteOpponent = CheckersUI.Opponent.Human
            };


            Assert.NotNull(vm.Controller.MoveHistory[0].WhiteMove);

            vm.UndoMoveCommand.Execute(null);

            Assert.Null(vm.Controller.MoveHistory[0].WhiteMove);
        }

        [Fact]
        public void NewGameDisplay_TurnsOn()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers),
                DisplayCreateGameGrid = false
            };

            vm.DisplayCreateGameCommand.Execute(null);

            Assert.True(vm.DisplayCreateGameGrid);
        }

        [Fact]
        public void CreateGame_HidesNewGameDisplay()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers),
                DisplayCreateGameGrid = true
            };

            vm.CreateGameCommand.Execute(null);

            Assert.False(vm.DisplayCreateGameGrid);
        }

        [Fact]
        public void CreateGame_SetsGameController()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers)
                    .Move(new Coord(2, 1), new Coord(3, 0))
                    .Move(new Coord(5, 6), new Coord(4, 7))
            };

            Assert.NotEmpty(vm.Controller.MoveHistory);

            vm.CreateGameCommand.Execute(null);

            Assert.Empty(vm.Controller.MoveHistory);
        }

        [Fact]
        public void CreateGame_SetsGameController_FromPosition_CreatesControllerCorrectly()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers),
                SetupOption = CheckersUI.Setup.FromPosition
            };

            var expectedFen = @"[FEN ""B:W20,21,22,23,25,26,27,28,29,30,31,32:B1,2,3,4,5,6,7,8,10,11,12,13""]";
            vm.CreateGameCommand.Execute(expectedFen);

            Assert.Equal(expectedFen, vm.Controller.Fen);
        }

        [Fact]
        public void CreateGame_SetsGameController_FromPosition_EmptyString_CreatesControllerCorrectly()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers),
                SetupOption = CheckersUI.Setup.FromPosition
            };

            var expectedFen = @"[FEN ""B:W21,22,23,24,25,26,27,28,29,30,31,32:B1,2,3,4,5,6,7,8,9,10,11,12""]";
            vm.CreateGameCommand.Execute(expectedFen);

            Assert.Equal(expectedFen, vm.Controller.Fen);
        }

        [Fact]
        public void CancelGame()
        {
            var vm = new GamePageViewModel {Controller = new GameController(Variant.AmericanCheckers)};

            Assert.True(vm.IsGameInProgress);

            vm.CancelGameCommand.Execute(null);
            Assert.False(vm.IsGameInProgress);
        }

        [Fact]
        public void SelectingMoveHistory_BlackMove_DisplaysBoardAtSelectedMove()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers)
                    .Move(new Coord(2, 1), new Coord(3, 0))
                    .Move(new Coord(5, 6), new Coord(4, 7))
            };

            Assert.NotEqual(vm.Controller.MoveHistory[0].BlackMove.ResultingFen, vm.Controller.Fen);

            vm.MoveHistoryCommand.Execute(vm.Controller.MoveHistory[0].BlackMove.ResultingFen);

            // we are just checking the board--remove the character that says the next move
            Assert.Equal(vm.Controller.MoveHistory[0].BlackMove.ResultingFen.Remove(6, 1), vm.Controller.Fen.Remove(6, 1));
        }

        [Fact]
        public void SelectingMoveHistory_WhiteMove_DisplaysBoardAtSelectedMove()
        {
            var vm = new GamePageViewModel
            {
                Controller = new GameController(Variant.AmericanCheckers)
                    .Move(new Coord(2, 1), new Coord(3, 0))
                    .Move(new Coord(5, 6), new Coord(4, 7))
                    .Move(new Coord(2, 3), new Coord(3, 2))
            };

            Assert.NotEqual(vm.Controller.MoveHistory[0].WhiteMove.ResultingFen, vm.Controller.Fen);

            vm.MoveHistoryCommand.Execute(vm.Controller.MoveHistory[0].WhiteMove.ResultingFen);

            // we are just checking the board--remove the character that says the next move
            Assert.Equal(vm.Controller.MoveHistory[0].WhiteMove.ResultingFen.Remove(6, 1), vm.Controller.Fen.Remove(6, 1));
        }
    }
}