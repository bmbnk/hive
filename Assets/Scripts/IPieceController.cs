using System.Collections.Generic;

namespace Hive
{
    public interface IPieceController
    {
        PieceType GetPieceType();
        List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard);
    }
}
