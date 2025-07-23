using System;

namespace ChineseChess
{
    public class ChessService
    {
        private Board? _board;

        // Main service class that will contain all chess logic
        public bool IsValidMove(Board board, Position from, Position to)
        {
            _board = board; // Store board reference for helper methods
            var piece = board.GetPiece(from);
            if (piece == null) return false;

            return piece.Type switch
            {
                PieceType.General => IsValidGeneralMove(piece, from, to),
                PieceType.Guard => IsValidGuardMove(piece, from, to),
                PieceType.Rook => IsValidRookMove(piece, from, to),
                PieceType.Horse => IsValidHorseMove(piece, from, to),
                PieceType.Cannon => IsValidCannonMove(piece, from, to),
                PieceType.Elephant => IsValidElephantMove(piece, from, to),
                PieceType.Soldier => IsValidSoldierMove(piece, from, to),
                _ => false // Other pieces not implemented yet
            };
        }

        private bool IsValidGeneralMove(Piece piece, Position from, Position to)
        {
            // General movement rules:
            // 1. Can only move within the palace
            // 2. Can move one step horizontally or vertically (not diagonally)
            // 3. Cannot result in the two generals facing each other (flying general rule)

            // Check if it's a one-step move (horizontal or vertical)
            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            if (!((rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1)))
            {
                return false; // Not a one-step orthogonal move
            }

            // Check if the move is within the palace
            if (!IsWithinPalace(piece.Color, to))
            {
                return false;
            }

            // Check if this move would result in flying general (both generals facing each other)
            if (WouldCreateFlyingGeneral(piece, from, to))
            {
                return false;
            }

            return true;
        }

        private bool WouldCreateFlyingGeneral(Piece movingGeneral, Position from, Position to)
        {
            if (_board == null) return false;

            var opponentGeneral = FindOpponentGeneral(movingGeneral.Color);
            if (opponentGeneral == null) return false;

            // Check if both generals would be on the same column after the move
            if (to.Col != opponentGeneral.Position.Col) return false;

            // Check if there would be no pieces between the generals
            return ArePositionsOnSameColumnWithNoPiecesBetween(to, opponentGeneral.Position, from);
        }

        private Piece? FindOpponentGeneral(Color playerColor)
        {
            if (_board == null) return null;

            for (int row = 1; row <= 10; row++)
            {
                for (int col = 1; col <= 9; col++)
                {
                    var position = new Position(row, col);
                    var piece = _board.GetPiece(position);
                    if (piece != null && piece.Type == PieceType.General && piece.Color != playerColor)
                    {
                        return piece;
                    }
                }
            }
            return null;
        }

        private bool ArePositionsOnSameColumnWithNoPiecesBetween(Position pos1, Position pos2, Position excludePosition)
        {
            if (_board == null) return false;

            int minRow = Math.Min(pos1.Row, pos2.Row);
            int maxRow = Math.Max(pos1.Row, pos2.Row);

            for (int row = minRow + 1; row < maxRow; row++)
            {
                var position = new Position(row, pos1.Col);

                // Skip the original position of the moving piece
                if (position.Equals(excludePosition)) continue;

                var piece = _board.GetPiece(position);
                if (piece != null) return false; // There's a piece between them
            }

            return true; // No pieces between the positions
        }

        private bool IsWithinPalace(Color color, Position position)
        {
            if (color == Color.Red)
            {
                // Red palace: rows 1-3, columns 4-6
                return position.Row >= 1 && position.Row <= 3 &&
                       position.Col >= 4 && position.Col <= 6;
            }
            else
            {
                // Black palace: rows 8-10, columns 4-6
                return position.Row >= 8 && position.Row <= 10 &&
                       position.Col >= 4 && position.Col <= 6;
            }
        }

        private bool IsValidGuardMove(Piece piece, Position from, Position to)
        {
            // Guard movement rules:
            // 1. Can only move within the palace
            // 2. Can only move diagonally one step

            // Check if it's a one-step diagonal move
            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            if (!(rowDiff == 1 && colDiff == 1))
            {
                return false; // Not a one-step diagonal move
            }

            // Check if the move is within the palace
            return IsWithinPalace(piece.Color, to);
        }

        private bool IsValidRookMove(Piece piece, Position from, Position to)
        {
            // Rook movement rules:
            // 1. Can move horizontally or vertically any number of steps
            // 2. Cannot jump over other pieces

            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            // Must be either horizontal or vertical move (not both)
            if (!((rowDiff == 0 && colDiff > 0) || (rowDiff > 0 && colDiff == 0)))
            {
                return false;
            }

            // Check if the path is clear (no pieces blocking the way)
            return IsPathClear(from, to);
        }

        private bool IsPathClear(Position from, Position to)
        {
            if (_board == null) return false;

            int rowStep = Math.Sign(to.Row - from.Row);
            int colStep = Math.Sign(to.Col - from.Col);

            int currentRow = from.Row + rowStep;
            int currentCol = from.Col + colStep;

            // Check each position along the path (excluding the destination)
            while (currentRow != to.Row || currentCol != to.Col)
            {
                var position = new Position(currentRow, currentCol);
                if (_board.GetPiece(position) != null)
                {
                    return false; // Path is blocked
                }

                currentRow += rowStep;
                currentCol += colStep;
            }

            return true; // Path is clear
        }

        private bool IsValidHorseMove(Piece piece, Position from, Position to)
        {
            // Horse movement rules:
            // 1. Moves in an "L" shape: 2 steps in one direction and 1 step perpendicular
            // 2. Cannot be blocked by an adjacent piece (leg-block rule)

            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            // Check if it's a valid L-shape move
            if (!((rowDiff == 2 && colDiff == 1) || (rowDiff == 1 && colDiff == 2)))
            {
                return false;
            }

            // Check if the horse is blocked by an adjacent piece (leg-block)
            Position legPosition;
            if (rowDiff == 2)
            {
                // Moving 2 rows, 1 column - check the position 1 row away
                int legRow = from.Row + Math.Sign(to.Row - from.Row);
                legPosition = new Position(legRow, from.Col);
            }
            else
            {
                // Moving 1 row, 2 columns - check the position 1 column away
                int legCol = from.Col + Math.Sign(to.Col - from.Col);
                legPosition = new Position(from.Row, legCol);
            }

            // Check if the leg position is blocked
            if (_board?.GetPiece(legPosition) != null)
            {
                return false; // Horse is blocked
            }

            return true;
        }

        private bool IsValidCannonMove(Piece piece, Position from, Position to)
        {
            // Cannon movement rules:
            // 1. Moves horizontally or vertically like a Rook
            // 2. If not capturing, the path must be clear (like a Rook)
            // 3. If capturing, there must be exactly one piece between the cannon and the target (the "screen")

            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            // Must be either horizontal or vertical move (not both)
            if (!((rowDiff == 0 && colDiff > 0) || (rowDiff > 0 && colDiff == 0)))
            {
                return false;
            }

            // Check if there's a piece at the destination
            var targetPiece = _board?.GetPiece(to);

            if (targetPiece == null)
            {
                // Not capturing - path must be clear (like a Rook)
                return IsPathClear(from, to);
            }
            else
            {
                // Capturing - must have exactly one screen piece between cannon and target
                return HasExactlyOneScreenPiece(from, to);
            }
        }

        private bool HasExactlyOneScreenPiece(Position from, Position to)
        {
            if (_board == null) return false;

            int rowStep = Math.Sign(to.Row - from.Row);
            int colStep = Math.Sign(to.Col - from.Col);

            int currentRow = from.Row + rowStep;
            int currentCol = from.Col + colStep;
            int pieceCount = 0;

            // Count pieces along the path (excluding the destination)
            while (currentRow != to.Row || currentCol != to.Col)
            {
                var position = new Position(currentRow, currentCol);
                if (_board.GetPiece(position) != null)
                {
                    pieceCount++;
                }

                currentRow += rowStep;
                currentCol += colStep;
            }

            return pieceCount == 1; // Exactly one screen piece
        }

        private bool IsValidElephantMove(Piece piece, Position from, Position to)
        {
            // Elephant movement rules:
            // 1. Moves exactly 2 steps diagonally (forming a 2x2 square)
            // 2. Cannot cross the river (stays on own side)
            // 3. Cannot be blocked by a piece at the midpoint

            int rowDiff = to.Row - from.Row;
            int colDiff = to.Col - from.Col;

            // Must be exactly 2 steps diagonally
            if (Math.Abs(rowDiff) != 2 || Math.Abs(colDiff) != 2)
            {
                return false;
            }

            // Check if crossing the river
            if (!IsOnOwnSideOfRiver(piece.Color, to))
            {
                return false;
            }

            // Check if the midpoint is blocked
            Position midpoint = new Position(from.Row + rowDiff / 2, from.Col + colDiff / 2);
            if (_board?.GetPiece(midpoint) != null)
            {
                return false; // Elephant is blocked
            }

            return true;
        }

        private bool IsOnOwnSideOfRiver(Color color, Position position)
        {
            if (color == Color.Red)
            {
                // Red pieces stay on rows 1-5
                return position.Row >= 1 && position.Row <= 5;
            }
            else
            {
                // Black pieces stay on rows 6-10
                return position.Row >= 6 && position.Row <= 10;
            }
        }

        private bool IsValidSoldierMove(Piece piece, Position from, Position to)
        {
            // Soldier movement rules:
            // 1. Before crossing the river: can only move forward (towards the opponent)
            // 2. After crossing the river: can move forward or sideways, but not backward

            int rowDiff = to.Row - from.Row;
            int colDiff = to.Col - from.Col;

            // Must be exactly one step
            if (Math.Abs(rowDiff) + Math.Abs(colDiff) != 1)
            {
                return false;
            }

            bool hasCrossedRiver = HasCrossedRiver(piece.Color, from);

            if (piece.Color == Color.Red)
            {
                // Red moves from bottom to top (increasing row numbers)
                if (hasCrossedRiver)
                {
                    // After crossing: can move forward or sideways, but not backward
                    return rowDiff >= 0; // Not moving backward
                }
                else
                {
                    // Before crossing: can only move forward
                    return rowDiff == 1 && colDiff == 0;
                }
            }
            else
            {
                // Black moves from top to bottom (decreasing row numbers)
                if (hasCrossedRiver)
                {
                    // After crossing: can move forward or sideways, but not backward
                    return rowDiff <= 0; // Not moving backward
                }
                else
                {
                    // Before crossing: can only move forward
                    return rowDiff == -1 && colDiff == 0;
                }
            }
        }

        private bool HasCrossedRiver(Color color, Position position)
        {
            if (color == Color.Red)
            {
                // Red crosses river at row 6
                return position.Row >= 6;
            }
            else
            {
                // Black crosses river at row 5
                return position.Row <= 5;
            }
        }

        public bool CheckWinCondition(Board board, Position from, Position to)
        {
            // Check if this move captures the opponent's General
            var capturedPiece = board.GetPiece(to);
            return capturedPiece != null && capturedPiece.Type == PieceType.General;
        }
    }

    public enum PieceType
    {
        General,
        Guard,
        Rook,
        Horse,
        Cannon,
        Elephant,
        Soldier
    }

    public enum Color
    {
        Red,
        Black
    }

    public record Position(int Row, int Col);

    public class Piece
    {
        public PieceType Type { get; init; }
        public Color Color { get; init; }
        public Position Position { get; set; }

        public Piece(PieceType type, Color color, Position position)
        {
            Type = type;
            Color = color;
            Position = position;
        }
    }

    public class Board
    {
        private readonly Piece?[,] _board = new Piece[10, 9]; // 10 rows, 9 columns

        public void PlacePiece(Piece piece)
        {
            _board[piece.Position.Row - 1, piece.Position.Col - 1] = piece;
        }

        public Piece? GetPiece(Position position)
        {
            return _board[position.Row - 1, position.Col - 1];
        }

        public void RemovePiece(Position position)
        {
            _board[position.Row - 1, position.Col - 1] = null;
        }
    }
}
