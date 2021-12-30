using System.Collections.Generic;

public interface IPieceController
{
    PieceType GetPieceType();
    List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard);
    void SetPositionOffsets(List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets);
}
