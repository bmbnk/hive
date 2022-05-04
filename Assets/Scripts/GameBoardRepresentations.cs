namespace Hive
{
    public static class GameBoardRepresentations
    {
        public static int[,] GetFlatGameboard(int[,,] gameBoard, bool trimmed=false)
        {
            int height = gameBoard.GetLength(0);
            int width = gameBoard.GetLength(1);

            int[,] targetGameBoard = new int[height, width];

            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                {
                    string hexesStack = "";
                    for (int depth = gameBoard.GetLength(2)-1; depth >= 0; depth--)
                    {
                        int hexId = gameBoard[row, col, depth];
                        if (hexId != 0)
                        {
                            hexesStack += HexIdToPiecePropertyMapper.GetPieceId(hexId).ToString();
                        }
                    }
                    targetGameBoard[row, col] = hexesStack.Equals("") ? 0 : int.Parse(hexesStack);
                }

            if (trimmed)
                return TrimFlatGameBoardToDim(targetGameBoard, 24);

            return targetGameBoard;
        }

        private static int[,] TrimFlatGameBoardToDim(int[,] gameBoard, int dim=24)
        {
            int minRow = 0;
            int maxRow = gameBoard.GetLength(0) - 1;

            int minCol = 0;
            int maxCol = gameBoard.GetLength(1) - 1;

            bool minColFound = false;
            bool maxColFound = false;
            bool minRowFound = false;
            bool maxRowFound = false;

            while (!minRowFound || !maxRowFound || !minColFound || !maxColFound)
            {
                for (int i = 0; i < gameBoard.GetLength(0); i++)
                {
                    if (!minRowFound && gameBoard[minRow, i] != 0)
                        minRowFound = true;
                    if (!maxRowFound && gameBoard[maxRow, i] != 0)
                        maxRowFound = true;
                    if (!minColFound && gameBoard[i, minCol] != 0)
                        minColFound = true;
                    if (!maxColFound && gameBoard[i, maxCol] != 0)
                        maxColFound = true;
                }
                if (!minRowFound)
                    minRow++;
                if (!maxRowFound)
                    maxRow--;
                if (!minColFound)
                    minCol++;
                if (!maxColFound)
                    maxCol--;
            }

            int[,] targetGameboard = new int[dim, dim];

            if (minRow <= maxRow)
            {
                int rowPadding = (dim - (maxRow - minRow + 1)) / 2;
                int colPadding = (dim - (maxCol - minCol + 1)) / 2;


                for (int i = rowPadding; i < targetGameboard.GetLength(0) - rowPadding; i++)
                    for (int j = colPadding; j < targetGameboard.GetLength(1) - colPadding; j++)
                        targetGameboard[i, j] = gameBoard[
                            minRow + i - rowPadding,
                            minCol + j - colPadding
                            ];
            }

            return targetGameboard;
        }
    }
}
