using System.Collections.Generic;
using UnityEngine;

public class GrasshopperPieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;
    private List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> _oneStepPositionOffsets;

    void Start()
    {
        _type = PieceType.GRASSHOPPER;
    }

    public PieceType GetPieceType() { return _type; }

    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard)
    {
        List<(int, int)> positions = new List<(int, int)>();

        _oneStepPositionOffsets.ForEach(offset =>
        {
            var delta = hexPosition.Item1 % 2 == 1 ? offset.EvenRowPositionOffset : offset.OddRowPositionOffset;
            if (gameBoard[hexPosition.Item1 + delta.Item1, hexPosition.Item2 + delta.Item2] != 0)
                positions.Add(getPositionInDirection(hexPosition, offset, gameBoard));
        });

        return positions;
    }

    private (int, int) getPositionInDirection((int, int) currentPositon, ((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset) directionOffsets, int[,] gameBoard)
    {
        var offset = currentPositon.Item1 % 2 == 1 ? directionOffsets.EvenRowPositionOffset : directionOffsets.OddRowPositionOffset;
        var nextPosition = (currentPositon.Item1 + offset.Item1, currentPositon.Item2 + offset.Item2);
        if (gameBoard[nextPosition.Item1, nextPosition.Item2] == 0)
            return nextPosition;
        return getPositionInDirection(nextPosition, directionOffsets, gameBoard);
    }

    public void SetPositionOffsets(List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets)
    {
        _oneStepPositionOffsets = oneStepPositionOffsets;
    }
}
