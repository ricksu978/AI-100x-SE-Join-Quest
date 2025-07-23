using Xunit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using ChineseChess;

namespace ChineseChess.Tests.Steps
{
    [Binding]
    public class ChineseChessSteps
    {
        private ChessService _chessService;
        private Board _board;
        private bool _moveResult;
        private bool _gameWon;

        public ChineseChessSteps()
        {
            _chessService = new ChessService();
            _board = new Board();
        }

        [Given(@"the board is empty except for a (.*) (.*) at \((\d+), (\d+)\)")]
        public void GivenTheBoardIsEmptyExceptForAPieceAt(string color, string pieceType, int row, int col)
        {
            // Create a new board for each scenario
            _board = new Board();

            var pieceColor = Enum.Parse<Color>(color);
            var type = Enum.Parse<PieceType>(pieceType);
            var position = new Position(row, col);
            var piece = new Piece(type, pieceColor, position);

            _board.PlacePiece(piece);
        }

        [Given(@"the board has:")]
        public void GivenTheBoardHas(Table table)
        {
            // Create a new board for each scenario
            _board = new Board();

            foreach (var row in table.Rows)
            {
                var pieceDescription = row["Piece"];
                var positionStr = row["Position"];

                // Parse "Red General" -> Color.Red, PieceType.General
                var parts = pieceDescription.Split(' ');
                var colorStr = parts[0];
                var typeStr = parts[1];

                var pieceColor = Enum.Parse<Color>(colorStr);
                var type = Enum.Parse<PieceType>(typeStr);

                // Parse "(2, 4)" -> Position(2, 4)
                var coords = positionStr.Trim('(', ')').Split(", ");
                var position = new Position(int.Parse(coords[0]), int.Parse(coords[1]));

                var piece = new Piece(type, pieceColor, position);
                _board.PlacePiece(piece);
            }
        }

        [When(@"(.*) moves the (.*) from \((\d+), (\d+)\) to \((\d+), (\d+)\)")]
        public void WhenPlayerMovesThePieceFromTo(string color, string pieceType, int fromRow, int fromCol, int toRow, int toCol)
        {
            var fromPosition = new Position(fromRow, fromCol);
            var toPosition = new Position(toRow, toCol);

            _moveResult = _chessService.IsValidMove(_board, fromPosition, toPosition);

            // Check win condition if the move is valid
            if (_moveResult)
            {
                _gameWon = _chessService.CheckWinCondition(_board, fromPosition, toPosition);
            }
        }

        [Then(@"the move is legal")]
        public void ThenTheMoveIsLegal()
        {
            Assert.True(_moveResult, "Expected the move to be legal, but it was illegal.");
        }

        [Then(@"the move is illegal")]
        public void ThenTheMoveIsIllegal()
        {
            Assert.False(_moveResult, "Expected the move to be illegal, but it was legal.");
        }

        [Then(@"(.*) wins immediately")]
        public void ThenPlayerWinsImmediately(string color)
        {
            Assert.True(_gameWon, $"Expected {color} to win immediately, but the game didn't end.");
        }

        [Then(@"the game is not over just from that capture")]
        public void ThenTheGameIsNotOverJustFromThatCapture()
        {
            Assert.False(_gameWon, "Expected the game to continue, but it ended.");
        }
    }
}
