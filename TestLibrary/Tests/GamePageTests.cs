using CheckersUI.Facade;
using CheckersUI.VMs;
using Xunit;

namespace TestLibrary.Tests
{
    public class GamePageTests
    {
        [Fact]
        public void Undo_UndoesMove()
        {
            var vm = new GamePageViewModel();
            var controller = new GameController(Variant.AmericanCheckers)
                .Move(new Coord(2, 1), new Coord(3, 0))
                .Move(new Coord(5, 6), new Coord(4, 7));

            vm.Controller = controller;
            vm.BlackOpponent = CheckersUI.Opponent.Human;
            vm.WhiteOpponent = CheckersUI.Opponent.Human;

            Assert.Equal(Piece.WhiteChecker, vm.Controller.Board[4, 7]);

            vm.UndoMoveCommand.Execute(null);

            Assert.Null(vm.Controller.Board[4, 7]);
        }

        [Fact]
        public void Undo_UndoesMoveHistory()
        {
            var vm = new GamePageViewModel();
            var controller = new GameController(Variant.AmericanCheckers)
                .Move(new Coord(2, 1), new Coord(3, 0))
                .Move(new Coord(5, 6), new Coord(4, 7));

            vm.Controller = controller;
            vm.BlackOpponent = CheckersUI.Opponent.Human;
            vm.WhiteOpponent = CheckersUI.Opponent.Human;

            Assert.NotNull(vm.Controller.MoveHistory[0].WhiteMove);

            vm.UndoMoveCommand.Execute(null);

            Assert.Null(vm.Controller.MoveHistory[0].WhiteMove);
        }
    }
}