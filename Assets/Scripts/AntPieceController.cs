using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public class AntPieceController : IPieceController
    {
        public PieceType GetPieceType() => PieceType.ANT;


        public List<(int, int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] originalGameBoard3D)
        {
            int[,] originalGameBoard2D = PieceMovesTools.GetGameBoard2Dfrom3D(originalGameBoard3D);

            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = GetGameBoardWithoutHex(originalGameBoard2D, hexPosition);

            List<(int, int)> positions2D = new List<(int, int)>();
            positions2D.Add(hexPosition);

            bool nextPositionFound = true;
            (int, int) lastPivotNeighbour;
            List<(int, int)> notAllowedPositions;

            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(positions2D.Last(), gameBoard, false);
            lastPivotNeighbour = neighbours[0];

            while (nextPositionFound)
            {
                nextPositionFound = false;

                neighbours = PieceMovesTools.GetNeighbours(positions2D.Last(), gameBoard, false);
                notAllowedPositions = PieceMovesTools.GetNotAllowedPositionsAroundPosition(neighbours, positions2D.Last());
                (int, int) nextPosition = PieceMovesTools.GetNextFreePositionAroundHex(positions2D.Last(), lastPivotNeighbour, gameBoard);
                if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                {
                    positions2D.Add(nextPosition);
                    nextPositionFound = true;
                }
                else
                {
                    int lastPivotNeighbourIdx = neighbours.FindIndex(neighbour => neighbour == lastPivotNeighbour);

                    for (int i = 1; i < neighbours.Count; i++)
                    {
                        (int, int) nextNeighbour = neighbours[(lastPivotNeighbourIdx + i) % neighbours.Count];
                        nextPosition = PieceMovesTools.GetNextFreePositionAroundHex(positions2D.Last(), nextNeighbour, gameBoard);
                        if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                        {
                            positions2D.Add(nextPosition);
                            lastPivotNeighbour = nextNeighbour;
                            nextPositionFound = true;
                            break;
                        }
                    }
                }

                if (positions2D.Count > 3 && PieceEnteredLoop(positions2D))
                {
                    for (int i = 0; i < 2; i++)
                        positions2D.RemoveAt(positions2D.Count - 1);
                    break;
                }
            }

            positions2D.Remove(hexPosition);

            List<(int, int, int)> positions = new List<(int, int, int)>();
            positions2D.ForEach(position => positions.Add((position.Item1, position.Item2, 0)));

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
