using CheckersUI.Facade;
using CheckersUI.VMs;
using Xunit;

namespace TestLibrary
{
    public class BoardEditorTests
    {
        [Fact]
        public void RemovePiece()
        {
            var vm = new BoardEditorViewModel(new Board());
            vm.RemovePiece(0, 1);

            Assert.Equal(null, vm.Board[0, 1]);
        }

        [Fact]
        public void AddPiece()
        {
            var vm = new BoardEditorViewModel(Board.EmptyBoard());
            vm.AddPiece(Piece.BlackChecker, 0, 1);

            Assert.Equal(Piece.BlackChecker, vm.Board[0, 1]);
        }

        [Fact]
        public void FenStringReturnsBoardFen()
        {
            var vm = new BoardEditorViewModel(Board.EmptyBoard());
            vm.AddPiece(Piece.BlackChecker, 0, 1);
            vm.AddPiece(Piece.WhiteKing, 0, 3);

            Assert.Equal("[FEN \"B:WK2:B1\"]", vm.FenString);
        }
    }
}
