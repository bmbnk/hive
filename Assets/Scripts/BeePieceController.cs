using System.Collections.Generic;
using UnityEngine;

public class BeePieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;
    private List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> _oneStepPositionOffsets;

    void Start()
    {
        _type = PieceType.BEE;
    }

    public PieceType GetPieceType() { return _type; }

    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard)
    {
        List<(int, int)> positions = new List<(int, int)>();
        List<(int, int)> neighbours = PieceMovesTools.getNeighbours(hexPosition, gameBoard, _oneStepPositionOffsets);
        List<(int, int)> notAllowedPositions = PieceMovesTools.getNotAllowedNextPositions(neighbours, hexPosition, _oneStepPositionOffsets);

        int offsetsListLen = _oneStepPositionOffsets.Count;

        for (int i = 0; i < _oneStepPositionOffsets.Count; i++)
        {
            (int, int) positionOffset = hexPosition.Item1 % 2 == 1 ?
                _oneStepPositionOffsets[i].EvenRowPositionOffset : _oneStepPositionOffsets[i].OddRowPositionOffset;

            (int, int) offsetPosition = (hexPosition.Item1 + positionOffset.Item1, hexPosition.Item2 + positionOffset.Item2);

            if (neighbours.Contains(offsetPosition))
            {
                (int, int) previousPositionOffset = hexPosition.Item1 % 2 == 1 ? _oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].EvenRowPositionOffset
                    : _oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].OddRowPositionOffset;
                (int, int) previousOffsetPosition = (hexPosition.Item1 + previousPositionOffset.Item1, hexPosition.Item2 + previousPositionOffset.Item2);

                if (!neighbours.Contains(previousOffsetPosition) && !notAllowedPositions.Contains(previousOffsetPosition))
                    positions.Add(previousOffsetPosition);

                (int, int) nextPositionOffset = hexPosition.Item1 % 2 == 1 ? _oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].EvenRowPositionOffset
                    : _oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].OddRowPositionOffset;
                (int, int) nextOffsetPosition = (hexPosition.Item1 + nextPositionOffset.Item1, hexPosition.Item2 + nextPositionOffset.Item2);

                if (!neighbours.Contains(nextOffsetPosition) && !notAllowedPositions.Contains(nextOffsetPosition))
                    positions.Add(nextOffsetPosition);
            }
        }

        return positions;
    }

    //public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard)
    //{
    //    List<(int, int)> positions = new List<(int, int)>();

    //    int offsetsListLen = _oneStepPositionOffsets.Count;

    //    for (int i = 0; i < _oneStepPositionOffsets.Count; i++)
    //    {
    //        (int, int) positionOffset = hexPosition.Item1 % 2 == 1 ?
    //            _oneStepPositionOffsets[i].EvenRowPositionOffset : _oneStepPositionOffsets[i].OddRowPositionOffset;

    //        (int, int) offsetPosition = (hexPosition.Item1 + positionOffset.Item1, hexPosition.Item2 + positionOffset.Item2);

    //        if (gameBoard[offsetPosition.Item1, offsetPosition.Item2] != 0)
    //        {
    //            (int, int) previousPositionOffset = hexPosition.Item1 % 2 == 1 ? _oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].EvenRowPositionOffset
    //                : _oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].OddRowPositionOffset;
    //            (int, int) previousOffsetPosition = (hexPosition.Item1 + previousPositionOffset.Item1, hexPosition.Item2 + previousPositionOffset.Item2);

    //            if (gameBoard[previousOffsetPosition.Item1, previousOffsetPosition.Item2] == 0)
    //                if (positions.Contains(previousOffsetPosition))
    //                    positions.Remove(previousOffsetPosition); //if empty position is next to two neighbours of the hex than it shouldn't move there
    //                else
    //                    positions.Add(previousOffsetPosition);

    //            (int, int) nextPositionOffset = hexPosition.Item1 % 2 == 1 ? _oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].EvenRowPositionOffset
    //                : _oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].OddRowPositionOffset;
    //            (int, int) nextOffsetPosition = (hexPosition.Item1 + nextPositionOffset.Item1, hexPosition.Item2 + nextPositionOffset.Item2);

    //            if (gameBoard[nextOffsetPosition.Item1, nextOffsetPosition.Item2] == 0)
    //                if (positions.Contains(nextOffsetPosition))
    //                    positions.Remove(nextOffsetPosition); //if empty position is next to two neighbours of the hex than it shouldn't move there
    //                else
    //                    positions.Add(nextOffsetPosition);
    //        }
    //    }

    //    return positions;
    //}

    public void SetPositionOffsets(List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets)
    {
        _oneStepPositionOffsets = oneStepPositionOffsets;
    }
}
