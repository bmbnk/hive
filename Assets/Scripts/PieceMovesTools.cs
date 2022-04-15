using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public static class PieceMovesTools
    {
        private const float Padding = 0.0f;
        private const float HexHeight = 0.7f;
        private const float SelectedHexPositionHeight = 0.3f;
        private const float HexHalfDistanceBetweenSides = 1.3f;

        private static List<(Vector3 Vector,
            (int, int) EvenRowNeighbourIdxsDelta,
            (int, int) OddRowNeighbourIdxsDelta)> _neighboursLocationParameters;

        static PieceMovesTools()
        {
            _neighboursLocationParameters = new List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)>();
            _neighboursLocationParameters.Add(
            (
                Vector: new Vector3(1, 0, Mathf.Sqrt(3)) * HexHalfDistanceBetweenSides,
                EvenRowNeighbourIdxsDelta: (-1, 0),
                OddRowNeighbourIdxsDelta: (-1, 1)
            )); ;
            _neighboursLocationParameters.Add(
            (
                Vector: new Vector3(2, 0, 0) * HexHalfDistanceBetweenSides,
                EvenRowNeighbourIdxsDelta: (0, 1),
                OddRowNeighbourIdxsDelta: (0, 1)
            ));
            _neighboursLocationParameters.Add(
            (
                Vector: new Vector3(1, 0, -Mathf.Sqrt(3)) * HexHalfDistanceBetweenSides,
                EvenRowNeighbourIdxsDelta: (1, 0),
                OddRowNeighbourIdxsDelta: (1, 1)

            ));
            _neighboursLocationParameters.Add(
            (
                Vector: new Vector3(-1, 0, -Mathf.Sqrt(3)) * HexHalfDistanceBetweenSides,
                EvenRowNeighbourIdxsDelta: (1, -1),
                OddRowNeighbourIdxsDelta: (1, 0)

            ));
            _neighboursLocationParameters.Add(
            (
                Vector: new Vector3(-2, 0, 0) * HexHalfDistanceBetweenSides,
                EvenRowNeighbourIdxsDelta: (0, -1),
                OddRowNeighbourIdxsDelta: (0, -1)
            ));
            _neighboursLocationParameters.Add(
            (
                Vector: new Vector3(-1, 0, Mathf.Sqrt(3)) * HexHalfDistanceBetweenSides,
                EvenRowNeighbourIdxsDelta: (-1, -1),
                OddRowNeighbourIdxsDelta: (-1, 0)
            ));
        }

        public static List<(int, int)> GetNotAllowedPositionsAroundPosition(List<(int, int)> neighbours, (int, int) currentPosition)
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

        public static List<(int, int)> GetPositionsNextToNeighboursAroundPosition((int, int) hexPosition, int[,] gameBoard)
        {
            List<(int, int)> positions = new List<(int, int)>();
            List<(int, int)> neighbours = GetNeighbours(hexPosition, gameBoard);

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

                    if (!neighbours.Contains(previousOffsetPosition) && !positions.Contains(previousOffsetPosition))
                    {
                        positions.Add(previousOffsetPosition);
                    }

                    (int, int) nextPositionOffset = hexPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[(offsetsListLen + i + 1) % offsetsListLen].EvenRowNeighbourIdxsDelta
                        : _neighboursLocationParameters[(offsetsListLen + i + 1) % offsetsListLen].OddRowNeighbourIdxsDelta;
                    (int, int) nextOffsetPosition = (hexPosition.Item1 + nextPositionOffset.Item1, hexPosition.Item2 + nextPositionOffset.Item2);

                    if (!neighbours.Contains(nextOffsetPosition) && !positions.Contains(nextOffsetPosition))
                    {
                        positions.Add(nextOffsetPosition);
                    }
                }
            }

            return positions;
        }

        public static List<(int, int)> GetNeighbours((int, int) currentPosition, int[,] gameBoard, bool clockwise = true)
        {
            List<(int, int)> neighbours = new List<(int, int)>();

            _neighboursLocationParameters.ForEach(locationParams =>
            {
                (int, int) idxsDelta = currentPosition.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
                (int, int) idxs = (currentPosition.Item1 + idxsDelta.Item1, currentPosition.Item2 + idxsDelta.Item2);
                if (gameBoard[idxs.Item1, idxs.Item2] != 0)
                    neighbours.Add(idxs);
            });

            if (!clockwise)
                neighbours.Reverse();

            return neighbours;
        }

        public static List<(int, int)> GetPositionsToAddHex(List<int> playerHexesOnBoardIds, int[,] gameBoard)
        {
            HashSet<(int, int)> positions = new HashSet<(int, int)>();

            List<int> activePlayerHexesOnBoardIds = FilterNotActiveHexIds(playerHexesOnBoardIds, gameBoard);

            activePlayerHexesOnBoardIds.ForEach(hexId =>
            {
                (int, int) hexPosition = GetIndiciesByHexId(hexId, gameBoard);
                List<(int, int)> freePositionsAroundPosition = GetFreePositionsAroundPosition(hexPosition, gameBoard);
                List<(int, int)> availablePositionsAroundPosition = FilterPositionsWithOpponentNeighbours(freePositionsAroundPosition, playerHexesOnBoardIds, gameBoard);
                availablePositionsAroundPosition.ForEach(position => positions.Add(position));
            });

            return positions.ToList();
        }

        public static (int, int) GetNextFreePositionAroundHex((int, int) hexToMove, (int, int) hex, int[,] gameBoard, bool clockwise = true)
        {
            (int, int) nextPositionAroundHex = GetNextPositionAroundHex(hexToMove, hex, clockwise);
            if (nextPositionAroundHex != (-1, -1)
                && gameBoard[nextPositionAroundHex.Item1, nextPositionAroundHex.Item2] == 0)
                return nextPositionAroundHex;
            return (-1, -1);
        }

        public static (int, int) GetNextPositionAroundHex((int, int) startPosition, (int, int) relativePosition, bool clockwise = true)
        {
            (int, int) offset = (startPosition.Item1 - relativePosition.Item1, startPosition.Item2 - relativePosition.Item2);

            for (int i = 0; i < _neighboursLocationParameters.Count; i++)
            {
                (int, int) positionOffset = relativePosition.Item1 % 2 == 1 ?
                    _neighboursLocationParameters[i].EvenRowNeighbourIdxsDelta : _neighboursLocationParameters[i].OddRowNeighbourIdxsDelta;
                if (positionOffset == offset)
                {
                    (int, int) nextPositionOffset;

                    int nextPositionOffsetIdx = clockwise ? (i + 1) % _neighboursLocationParameters.Count
                        : (i + _neighboursLocationParameters.Count - 1) % _neighboursLocationParameters.Count;

                    nextPositionOffset = relativePosition.Item1 % 2 == 1 ?
                    _neighboursLocationParameters[nextPositionOffsetIdx].EvenRowNeighbourIdxsDelta
                        : _neighboursLocationParameters[nextPositionOffsetIdx].OddRowNeighbourIdxsDelta;

                    (int, int) nextPosition = (relativePosition.Item1 + nextPositionOffset.Item1, relativePosition.Item2 + nextPositionOffset.Item2);
                    return nextPosition;
                }
            }
            return (-1, -1);
        }

        public static Vector3 GetVectorFromStartToEnd((int, int) startPosition, (int, int) endPositon)
        {
            Vector3 vector = new Vector3(0, 0, 0);

            //make both positions rows even or odd
            if ((startPosition.Item1 + endPositon.Item1) % 2 == 1)
            {
                var positionOffset = startPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[0].EvenRowNeighbourIdxsDelta : _neighboursLocationParameters[0].OddRowNeighbourIdxsDelta;
                var offsetVector = _neighboursLocationParameters[0].Vector;
                startPosition = (startPosition.Item1 + positionOffset.Item1, startPosition.Item2 + positionOffset.Item2);
                vector += offsetVector + 2 * offsetVector.normalized * Padding;
            }

            Vector3 moveHexLeftVector = _neighboursLocationParameters[1].Vector;
            int positionToLeftNumber = (endPositon.Item2 - startPosition.Item2);
            vector += positionToLeftNumber * (moveHexLeftVector + moveHexLeftVector.normalized * 2 * Padding);

            Vector3 moveHexDownVector = new Vector3(0, 0, 3 * HexHalfDistanceBetweenSides / Mathf.Sqrt(3) * 2);
            int positionsDownNumber = (startPosition.Item1 - endPositon.Item1);
            vector += positionsDownNumber / 2 * (moveHexDownVector + 2 * moveHexDownVector.normalized * Padding * Mathf.Sqrt(3));

            return vector;
        }

        public static Vector3 GetVerticalVector(int hexesNumber)
        {
            Vector3 hexesHeightVector = hexesNumber * new Vector3(0, HexHeight, 0);
            return hexesHeightVector;
        }

        public static Vector3 GetHexSelectionVector()
        {
            return new Vector3(0, SelectedHexPositionHeight, 0);
        }

        public static List<(int, int)> GetFreePositionsAroundPosition((int, int) position, int[,] gameBoard)
        {
            List<(int, int)> freePositionsAroundPosition = GetPositionsAroundPosition(position);

            for (int i = freePositionsAroundPosition.Count - 1; i >= 0; i--)
            {
                var positionAround = freePositionsAroundPosition[i];
                if (gameBoard[positionAround.Item1, positionAround.Item2] != 0)
                    freePositionsAroundPosition.Remove(positionAround);
            }

            return freePositionsAroundPosition;
        }

        public static List<(int, int)> GetPositionsAroundPosition((int, int) position)
        {
            List<(int, int)> positionsAroundPosition = new List<(int, int)>();

            _neighboursLocationParameters.ForEach(locationParams =>
            {
                (int, int) idxsDelta = position.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
                (int, int) nextPositionAround = (position.Item1 + idxsDelta.Item1, position.Item2 + idxsDelta.Item2);
                positionsAroundPosition.Add(nextPositionAround);
            });

            return positionsAroundPosition;
        }

        public static List<(int, int)> FilterPositionsWithOpponentNeighbours(List<(int, int)> postions, List<int> playerHexesOnBoardIds, int[,] gameBoard)
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
                        if (!playerHexesOnBoardIds.Contains(currentNeighbourHexId))
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

        public static (int, int) GetFirstFreePositionInDirectionOfNeighbour((int, int) startPosition, (int, int) neighbour, int[,] gameBoard)
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
                return GetFirstFreePositionInDirection(startPosition, offsetParams, gameBoard);
            }

            return (-1, -1);
        }

        private static (int, int) GetFirstFreePositionInDirection(
            (int, int) startPosition,
            ((int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta) offsetParams,
            int[,] gameBoard)
        {
            var offset = startPosition.Item1 % 2 == 1 ? offsetParams.EvenRowNeighbourIdxsDelta : offsetParams.OddRowNeighbourIdxsDelta;
            var nextPosition = (startPosition.Item1 + offset.Item1, startPosition.Item2 + offset.Item2);
            if (gameBoard[nextPosition.Item1, nextPosition.Item2] == 0)
                return nextPosition;
            return GetFirstFreePositionInDirection(nextPosition, offsetParams, gameBoard);
        }

        private static (int, int) GetIndiciesByHexId(int HexId, int[,] gameBoard)
        {
            for (int i = 0; i < GameBoardScript.GameBoardSize; i++)
                for (int j = 0; j < GameBoardScript.GameBoardSize; j++)
                    if (gameBoard[i, j] == HexId)
                        return (i, j);
            return (-1, -1);
        }

        private static List<int> FilterNotActiveHexIds(List<int> hexIds, int[,] gameBoard)
        {
            List<int> activeHexIds = new List<int>();

            hexIds.ForEach(hexId =>
            {
                var hexPosition = GetIndiciesByHexId(hexId, gameBoard);
                if (hexPosition != (-1, -1))
                    activeHexIds.Add(hexId);
            });

            return activeHexIds;
        }

        public static int[,] GetGameBoard2Dfrom3D(int[,,] gameBoard3D)
        {
            int height = gameBoard3D.GetLength(0);
            int width = gameBoard3D.GetLength(1);

            int[,] gameBoard2D = new int[height, width];

            for (int row = 0; row < height; row++)
                for (int col = 0; col < width; col++)
                    gameBoard2D[row, col] = gameBoard3D[row, col, 0];

            return gameBoard2D;
        }
    }
}