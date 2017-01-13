using CheckersUI.Facade;
using CheckersUI.VMs;
using Xunit;

namespace TestLibrary.Tests
{
    public class GamePageTests
    {
        [Fact]
        public void UndoMove_UndoesMoveHistory()
        {
            var vm = new GamePageViewModel();
            var controller = new GameController(Variant.AmericanCheckers)
                .Move(new Coord(2, 1), new Coord(3, 0))
                .Move(new Coord(5, 6), new Coord(4, 7));

            vm.Controller = controller;

            vm.UndoMoveCommand.Execute(null);

            Assert.Equal(null, vm.Controller.MoveHistory[0].WhiteMove);
        }

        [Fact]
        public void UndoMove_UndoesMoveOnBoard()
        {
            var vm = new GamePageViewModel();
            var controller = new GameController(Variant.AmericanCheckers)
                .Move(new Coord(2, 1), new Coord(3, 0))
                .Move(new Coord(5, 6), new Coord(4, 7));

            vm.Controller = controller;

            vm.UndoMoveCommand.Execute(null);

            Assert.Equal(Piece.WhiteChecker, vm.Controller.Board[5, 6]);
            Assert.Equal(null, vm.Controller.Board[4, 7]);
        }
    }
}
