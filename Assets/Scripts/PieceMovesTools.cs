using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PieceMovesTools
{
    private const float Padding = 0.4f;
    private static List<(Vector3 Vector,
        (int, int) EvenRowNeighbourIdxsDelta,
        (int, int) OddRowNeighbourIdxsDelta)> _neighboursLocationParameters;

    static PieceMovesTools()
    {
        _neighboursLocationParameters = new List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)>();
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(1, 0, Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (-1, 0),
            OddRowNeighbourIdxsDelta: (-1, 1)
        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(2, 0, 0),
            EvenRowNeighbourIdxsDelta: (0, 1),
            OddRowNeighbourIdxsDelta: (0, 1)
        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(1, 0, -Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (1, 0),
            OddRowNeighbourIdxsDelta: (1, 1)

        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(-1, 0, -Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (1, -1),
            OddRowNeighbourIdxsDelta: (1, 0)

        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(-2, 0, 0),
            EvenRowNeighbourIdxsDelta: (0, -1),
            OddRowNeighbourIdxsDelta: (0, -1)
        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(-1, 0, Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (-1, -1),
            OddRowNeighbourIdxsDelta: (-1, 0)
        ));
    }

    public static List<(int, int)> getNotAllowedNextPositions(List<(int, int)> neighbours, (int, int) currentPosition)
    {
        List<(int, int)> notAllowedPositions = new List<(int, int)>();

        int offsetsListLen = _neighboursLocationParameters.Count;

        for (int i = 0; i < _neighboursLocationParameters.Count; i++)
        {
            (int, int) offset = currentPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[i].EvenRowNeighbourIdxsDelta
                : _neighboursLocationParameters[i].OddRowNeighbourIdxsDelta;

            (int, int) offsetPosition = (currentPosition.Item1 + offset.Item1, currentPosition.Item2 + offset.Item2);

            if (!neighbours.Contains(offsetPosition))
            {
                int neighoursNextToPositionCunter = 0;

                (int, int) nextOffset = currentPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[(offsetsListLen + i + 1) % offsetsListLen].EvenRowNeighbourIdxsDelta
                    : _neighboursLocationParameters[(offsetsListLen + i + 1) % offsetsListLen].OddRowNeighbourIdxsDelta;
                (int, int) nextOffsetPosition = (currentPosition.Item1 + nextOffset.Item1, currentPosition.Item2 + nextOffset.Item2);

                if (neighbours.Contains(nextOffsetPosition))
                    neighoursNextToPositionCunter++;

                (int, int) previousOffset = currentPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[(offsetsListLen + i - 1) % offsetsListLen].EvenRowNeighbourIdxsDelta
                    : _neighboursLocationParameters[(offsetsListLen + i - 1) % offsetsListLen].OddRowNeighbourIdxsDelta;
                (int, int) previousOffsetPosition = (currentPosition.Item1 + previousOffset.Item1, currentPosition.Item2 + previousOffset.Item2);

                if (neighbours.Contains(previousOffsetPosition))
                    neighoursNextToPositionCunter++;

                if (neighoursNextToPositionCunter == 2)
                    notAllowedPositions.Add(offsetPosition);
            }
        }

        return notAllowedPositions;
    }

    internal static List<(int, int)> GetPositionsWithNeighboursAroundPosition((int, int) hexPosition, int[,] gameBoard)
    {
        List<(int, int)> positions = new List<(int, int)>();
        List<(int, int)> neighbours = PieceMovesTools.getNeighbours(hexPosition, gameBoard);
        List<(int, int)> notAllowedPositions = PieceMovesTools.getNotAllowedNextPositions(neighbours, hexPosition);

        int offsetsListLen = _neighboursLocationParameters.Count;

        for (int i = 0; i < _neighboursLocationParameters.Count; i++)
        {
            (int, int) positionOffset = hexPosition.Item1 % 2 == 1 ?
                _neighboursLocationParameters[i].EvenRowNeighbourIdxsDelta : _neighboursLocationParameters[i].OddRowNeighbourIdxsDelta;

            (int, int) offsetPosition = (hexPosition.Item1 + positionOffset.Item1, hexPosition.Item2 + positionOffset.Item2);

            if (neighbours.Contains(offsetPosition))
            {
                (int, int) previousPositionOffset = hexPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[(offsetsListLen + i - 1) % offsetsListLen].EvenRowNeighbourIdxsDelta
                    : _neighboursLocationParameters[(offsetsListLen + i - 1) % offsetsListLen].OddRowNeighbourIdxsDelta;
                (int, int) previousOffsetPosition = (hexPosition.Item1 + previousPositionOffset.Item1, hexPosition.Item2 + previousPositionOffset.Item2);

                if (!neighbours.Contains(previousOffsetPosition) && !notAllowedPositions.Contains(previousOffsetPosition))
                    positions.Add(previousOffsetPosition);

                (int, int) nextPositionOffset = hexPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[(offsetsListLen + i + 1) % offsetsListLen].EvenRowNeighbourIdxsDelta
                    : _neighboursLocationParameters[(offsetsListLen + i + 1) % offsetsListLen].OddRowNeighbourIdxsDelta;
                (int, int) nextOffsetPosition = (hexPosition.Item1 + nextPositionOffset.Item1, hexPosition.Item2 + nextPositionOffset.Item2);

                if (!neighbours.Contains(nextOffsetPosition) && !notAllowedPositions.Contains(nextOffsetPosition))
                    positions.Add(nextOffsetPosition);
            }
        }

        return positions;
    }

    public static List<(int, int)> getNeighbours((int, int) currentPosition, int[,] gameBoard, bool clockwise=true)
    {
        List<(int, int)> neighours = new List<(int, int)>();

        _neighboursLocationParameters.ForEach(locationParams =>
        {
            (int, int) idxsDelta = currentPosition.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
            (int, int) idxs = (currentPosition.Item1 + idxsDelta.Item1, currentPosition.Item2 + idxsDelta.Item2);
            if (gameBoard[idxs.Item1, idxs.Item2] != 0)
                neighours.Add(idxs);
        });

        if (!clockwise)
            neighours.Reverse();

        return neighours;
    }

    public static (int, int) nextPositionAroundHex((int, int) hexToMove, (int, int) hex, int[,] gameBoard, bool clockwise=true)
    {
        (int, int) relativePosition = (hexToMove.Item1 - hex.Item1, hexToMove.Item2 - hex.Item2);

        for (int i = 0; i < _neighboursLocationParameters.Count; i++)
        {
            (int, int) positionOffset = hex.Item1 % 2 == 1 ?
                _neighboursLocationParameters[i].EvenRowNeighbourIdxsDelta : _neighboursLocationParameters[i].OddRowNeighbourIdxsDelta;
            if (positionOffset == relativePosition)
            {
                (int, int) nextPositionOffset;

                int nextPositionOffsetIdx = clockwise ? (i + 1) % _neighboursLocationParameters.Count
                    : (i + _neighboursLocationParameters.Count - 1) % _neighboursLocationParameters.Count;

                nextPositionOffset = hex.Item1 % 2 == 1 ?
                _neighboursLocationParameters[nextPositionOffsetIdx].EvenRowNeighbourIdxsDelta
                    : _neighboursLocationParameters[nextPositionOffsetIdx].OddRowNeighbourIdxsDelta;

                (int, int) nextPosition = (hex.Item1 + nextPositionOffset.Item1, hex.Item2 + nextPositionOffset.Item2);
                if (gameBoard[nextPosition.Item1, nextPosition.Item2] == 0)
                    return nextPosition;
                break;
            }
        }
        return (-1, -1);
    }

    public static Vector3 getVectorFromStartToEnd((int, int) startPosition, (int, int) endPositon)
    {
        Vector3 vector = new Vector3(0, 0, 0);
        if ((startPosition.Item1 + endPositon.Item1) % 2 == 1)
        {
            var positionOffset = startPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[0].EvenRowNeighbourIdxsDelta : _neighboursLocationParameters[0].OddRowNeighbourIdxsDelta;
            var offsetVector = _neighboursLocationParameters[0].Vector;
            startPosition = (startPosition.Item1 + positionOffset.Item1, startPosition.Item2 + positionOffset.Item2);
            vector += offsetVector + 2 * offsetVector.normalized * Padding;
        }

        vector += new Vector3(
            (endPositon.Item2 - startPosition.Item2) * (2 + 2 * Padding),
            0,
            (-(endPositon.Item1 - startPosition.Item1) / 2) * (3 * 2 / Mathf.Sqrt(3) + 2 * Padding * Mathf.Sqrt(3)));

        return vector;
    }

    public static List<(int, int)> GetEmptyPositionsAroundPosition((int, int) position, int[,] gameBoard)
    {
        List<(int, int)> emptyPositionsAroundPosition = new List<(int, int)>();

        _neighboursLocationParameters.ForEach(locationParams =>
        {
            (int, int) idxsDelta = position.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
            (int, int) nextPositionAround = (position.Item1 + idxsDelta.Item1, position.Item2 + idxsDelta.Item2);
            if (gameBoard[nextPositionAround.Item1, nextPositionAround.Item2] == 0)
                emptyPositionsAroundPosition.Add(nextPositionAround);
        });

        return emptyPositionsAroundPosition;
    }

    public static List<(int, int)> FilterPositionsWithOpponentNeighbours(List<(int, int)> postions, List<GameObject> opponents, int[,] gameBoard)
    {
        List<(int, int)> filteredPositions = new List<(int, int)>();
        bool noOpponentNeighbour;

        foreach (var position in postions)
        {
            noOpponentNeighbour = true;

            foreach (var locationParams in _neighboursLocationParameters)
            {
                (int, int) offset = position.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
                (int, int) positionAround = (position.Item1 + offset.Item1, position.Item2 + offset.Item2);

                int currentNeighbourHexId = gameBoard[positionAround.Item1, positionAround.Item2];

                if (currentNeighbourHexId != 0)
                {
                    int opponentNeighbourIdx = opponents.FindIndex(hex => hex.GetComponent<HexWrapperController>().HexId == currentNeighbourHexId);

                    if (opponentNeighbourIdx != -1 && opponents[opponentNeighbourIdx].activeSelf)
                    {
                        noOpponentNeighbour = false;
                        break;
                    }
                }
            }
            if (noOpponentNeighbour)
                filteredPositions.Add(position);
        }
        return filteredPositions;
    }

    public static (int, int) getFirstEmptyPositionInDirectionOfNeighbour((int, int) startPosition, (int, int) neighbour, int[,] gameBoard)
    {
        (int, int) offset = (neighbour.Item1 - startPosition.Item1, neighbour.Item2 - startPosition.Item2);
        int offsetParamsIdx = _neighboursLocationParameters.FindIndex(parameters =>
        {
            (int, int) parameter = startPosition.Item1 % 2 == 1 ? parameters.EvenRowNeighbourIdxsDelta : parameters.OddRowNeighbourIdxsDelta;
            return offset == parameter;
        });

        if (offsetParamsIdx != -1)
        {
            var offsetParams = (_neighboursLocationParameters[offsetParamsIdx].EvenRowNeighbourIdxsDelta,
                    _neighboursLocationParameters[offsetParamsIdx].OddRowNeighbourIdxsDelta);
            return getFirstEmptyPositionInDirection(startPosition, offsetParams, gameBoard);
        }

        return (-1, -1);
    }

    private static (int, int) getFirstEmptyPositionInDirection(
        (int, int) startPosition,
        ((int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta) offsetParams,
        int[,] gameBoard)
    {
        var offset = startPosition.Item1 % 2 == 1 ? offsetParams.EvenRowNeighbourIdxsDelta: offsetParams.OddRowNeighbourIdxsDelta;
        var nextPosition = (startPosition.Item1 + offset.Item1, startPosition.Item2 + offset.Item2);
        if (gameBoard[nextPosition.Item1, nextPosition.Item2] == 0)
            return nextPosition;
        return getFirstEmptyPositionInDirection(nextPosition, offsetParams, gameBoard);
    }
}
