using System;
using System.Collections.Generic;
using UnityEngine;

public class BeetlePieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;

    void Start()
    {
        _type = PieceType.BEETLE;
    }

    public PieceType GetPieceType() { return _type; }

    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard)
    {
        throw new NotImplementedException();

        //List<(int, int)> positions = new List<(int, int)>();
        //List<(int, int)> neighbours = PieceMovesTools.getNeighbours(hexPosition, gameBoard, _oneStepPositionOffsets);
        //List<(int, int)> notAllowedPositions = PieceMovesTools.getNotAllowedNextPositions(neighbours, hexPosition, _oneStepPositionOffsets);

        //int offsetsListLen = _oneStepPositionOffsets.Count;

        //for (int i = 0; i < _oneStepPositionOffsets.Count; i++)
        //{
        //    (int, int) positionOffset = hexPosition.Item1 % 2 == 1 ?
        //        _oneStepPositionOffsets[i].EvenRowPositionOffset : _oneStepPositionOffsets[i].OddRowPositionOffset;

        //    (int, int) offsetPosition = (hexPosition.Item1 + positionOffset.Item1, hexPosition.Item2 + positionOffset.Item2);

        //    if (neighbours.Contains(offsetPosition))
        //    {
        //        positions.Add(offsetPosition);

        //        (int, int) previousPositionOffset = hexPosition.Item1 % 2 == 1 ? _oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].EvenRowPositionOffset
        //            : _oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].OddRowPositionOffset;
        //        (int, int) previousOffsetPosition = (hexPosition.Item1 + previousPositionOffset.Item1, hexPosition.Item2 + previousPositionOffset.Item2);

        //        if (!neighbours.Contains(previousOffsetPosition) && !notAllowedPositions.Contains(previousOffsetPosition))
        //            positions.Add(previousOffsetPosition);

        //        (int, int) nextPositionOffset = hexPosition.Item1 % 2 == 1 ? _oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].EvenRowPositionOffset
        //            : _oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].OddRowPositionOffset;
        //        (int, int) nextOffsetPosition = (hexPosition.Item1 + nextPositionOffset.Item1, hexPosition.Item2 + nextPositionOffset.Item2);

        //        if (!neighbours.Contains(nextOffsetPosition) && !notAllowedPositions.Contains(nextOffsetPosition))
        //            positions.Add(nextOffsetPosition);
        //    }
        //}

        //return positions;
    }
}
