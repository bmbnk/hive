using System;
using System.Collections.Generic;

public static class PieceMovesTools
{

    public static List<(int, int)> getNotAllowedNextPositions(List<(int, int)> neighbours, (int, int) currentPosition,
        List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets)
    {
        List<(int, int)> notAllowedPositions = new List<(int, int)>();

        int offsetsListLen = oneStepPositionOffsets.Count;

        for (int i = 0; i < oneStepPositionOffsets.Count; i++)
        {
            (int, int) offset = currentPosition.Item1 % 2 == 1 ? oneStepPositionOffsets[i].EvenRowPositionOffset
                : oneStepPositionOffsets[i].OddRowPositionOffset;

            (int, int) offsetPosition = (currentPosition.Item1 + offset.Item1, currentPosition.Item2 + offset.Item2);

            if (!neighbours.Contains(offsetPosition))
            {
                int neighoursNextToPositionCunter = 0;

                (int, int) nextOffset = currentPosition.Item1 % 2 == 1 ? oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].EvenRowPositionOffset
                    : oneStepPositionOffsets[(offsetsListLen + i + 1) % offsetsListLen].OddRowPositionOffset;
                (int, int) nextOffsetPosition = (currentPosition.Item1 + nextOffset.Item1, currentPosition.Item2 + nextOffset.Item2);

                if (neighbours.Contains(nextOffsetPosition))
                    neighoursNextToPositionCunter++;

                (int, int) previousOffset = currentPosition.Item1 % 2 == 1 ? oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].EvenRowPositionOffset
                    : oneStepPositionOffsets[(offsetsListLen + i - 1) % offsetsListLen].OddRowPositionOffset;
                (int, int) previousOffsetPosition = (currentPosition.Item1 + previousOffset.Item1, currentPosition.Item2 + previousOffset.Item2);

                if (neighbours.Contains(previousOffsetPosition))
                    neighoursNextToPositionCunter++;

                if (neighoursNextToPositionCunter == 2)
                    notAllowedPositions.Add(offsetPosition);
            }
        }

        return notAllowedPositions;
    }

    public static List<(int, int)> getNeighbours((int, int) currentPosition, int[,] gameBoard,
        List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets,
        bool clockwise=true)
    {
        List<(int, int)> neighours = new List<(int, int)>();

        oneStepPositionOffsets.ForEach(locationParams =>
        {
            (int, int) idxsDelta = currentPosition.Item1 % 2 == 1 ? locationParams.EvenRowPositionOffset : locationParams.OddRowPositionOffset;
            (int, int) idxs = (currentPosition.Item1 + idxsDelta.Item1, currentPosition.Item2 + idxsDelta.Item2);
            if (gameBoard[idxs.Item1, idxs.Item2] != 0)
                neighours.Add(idxs);
        });

        if (!clockwise)
            neighours.Reverse();

        return neighours;
    }

    public static (int, int) nextPositionAroundHex((int, int) hexToMove, (int, int) hex, int[,] gameBoard,
        List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets,
        bool clockwise=true)
    {
        (int, int) relativePosition = (hexToMove.Item1 - hex.Item1, hexToMove.Item2 - hex.Item2);

        for (int i = 0; i < oneStepPositionOffsets.Count; i++)
        {
            (int, int) positionOffset = hex.Item1 % 2 == 1 ?
                oneStepPositionOffsets[i].EvenRowPositionOffset : oneStepPositionOffsets[i].OddRowPositionOffset;
            if (positionOffset == relativePosition)
            {
                (int, int) nextPositionOffset;

                int nextPositionOffsetIdx = clockwise ? (i + 1) % oneStepPositionOffsets.Count
                    : (i + oneStepPositionOffsets.Count - 1) % oneStepPositionOffsets.Count;

                nextPositionOffset = hex.Item1 % 2 == 1 ?
                oneStepPositionOffsets[nextPositionOffsetIdx].EvenRowPositionOffset
                    : oneStepPositionOffsets[nextPositionOffsetIdx].OddRowPositionOffset;

                (int, int) nextPosition = (hex.Item1 + nextPositionOffset.Item1, hex.Item2 + nextPositionOffset.Item2);
                if (gameBoard[nextPosition.Item1, nextPosition.Item2] == 0)
                    return nextPosition;
                break;
            }
        }
        return (-1, -1);
    }
}
