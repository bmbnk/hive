using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public class AntPieceController : MonoBehaviour, IPieceController
    {
        public PieceType GetPieceType() => PieceType.ANT;


        public List<(int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] originalGameBoard3D)
        {
            int[,] originalGameBoard2D = PieceMovesTools.GetGameBoard2Dfrom3D(originalGameBoard3D);

            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = GetGameBoardWithoutHex(originalGameBoard2D, hexPosition);

            List<(int, int)> positions = new List<(int, int)>();
            positions.Add(hexPosition);

            bool nextPositionFound = true;
            (int, int) lastPivotNeighbour;
            List<(int, int)> notAllowedPositions;

            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(positions.Last(), gameBoard, false);
            lastPivotNeighbour = neighbours[0];

            while (nextPositionFound)
            {
                nextPositionFound = false;

                neighbours = PieceMovesTools.GetNeighbours(positions.Last(), gameBoard, false);
                notAllowedPositions = PieceMovesTools.GetNotAllowedPositionsAroundPosition(neighbours, positions.Last());
                (int, int) nextPosition = PieceMovesTools.GetNextFreePositionAroundHex(positions.Last(), lastPivotNeighbour, gameBoard);
                if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                {
                    positions.Add(nextPosition);
                    nextPositionFound = true;
                }
                else
                {
                    int lastPivotNeighbourIdx = neighbours.FindIndex(neighbour => neighbour == lastPivotNeighbour);

                    for (int i = 1; i < neighbours.Count; i++)
                    {
                        (int, int) nextNeighbour = neighbours[(lastPivotNeighbourIdx + i) % neighbours.Count];
                        nextPosition = PieceMovesTools.GetNextFreePositionAroundHex(positions.Last(), nextNeighbour, gameBoard);
                        if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                        {
                            positions.Add(nextPosition);
                            lastPivotNeighbour = nextNeighbour;
                            nextPositionFound = true;
                            break;
                        }
                    }
                }

                if (positions.Count > 3 && PieceEnteredLoop(positions))
                {
                    for (int i = 0; i < 2; i++)
                        positions.RemoveAt(positions.Count - 1);
                    break;
                }
            }

            positions.Remove(hexPosition);

            return positions;
        }

        private int[,] GetGameBoardWithoutHex(int[,] originalGameBoard, (int, int) hexPosition)
        {
            int[,] gameBoard = (int[,])originalGameBoard.Clone();
            gameBoard[hexPosition.Item1, hexPosition.Item2] = 0;
            return gameBoard;
        }

        private bool PieceEnteredLoop(List<(int, int)> positions)
        {
            if (positions.Count > 3)
            {
                for (int i = 0; i < positions.Count - 3; i++)
                {
                    if (positions[i] == positions[positions.Count - 2] && positions[i + 1] == positions[positions.Count - 1])
                        return true;
                }
            }
            return false;
        }
    }
}
