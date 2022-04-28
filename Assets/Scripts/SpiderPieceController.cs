using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public class SpiderPieceController : IPieceController
    {
        public PieceType GetPieceType() => PieceType.SPIDER;


        public List<(int, int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] originalGameBoard3D)
        {
            //TODO: Check in the manual for this game, but you should probably check also path
            //from different neighbours that you will meet on this 3 steps road

            int[,] originalGameBoard2D = PieceMovesTools.GetGameBoard2Dfrom3D(originalGameBoard3D);

            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = GetGameBoardWithoutHex(originalGameBoard2D, hexPosition);

            var positions2D = getMovePositions(gameBoard, hexPosition);

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

        private List<(int, int)> getMovePositions(int[,] gameBoard, (int, int) hexPosition)
        {
            List<(int, int)> positions = new List<(int, int)>();
            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);

            neighbours.ForEach(neighbour =>
            {
                positions.Add(getMovePositionFromNeighbour(hexPosition, neighbour, gameBoard, true));
                positions.Add(getMovePositionFromNeighbour(hexPosition, neighbour, gameBoard, false));
            });

            positions = positions.Distinct().ToList();
            positions.Remove(hexPosition);

            return positions;
        }

        private (int, int) getMovePositionFromNeighbour((int, int) hexPosition, (int, int) neighbour, int[,] gameBoard, bool clockwise = true)
        {

            (int, int) movePosition = hexPosition;

            (int, int) lastPivotNeighbour;
            List<(int, int)> notAllowedPositions;
            List<(int, int)> neighbours;

            lastPivotNeighbour = neighbour;


            for (int i = 0; i < 3; i++)
            {
                neighbours = PieceMovesTools.GetNeighbours(movePosition, gameBoard, !clockwise);
                notAllowedPositions = PieceMovesTools.GetNotAllowedPositionsAroundPosition(neighbours, movePosition);
                (int, int) nextPosition = PieceMovesTools.GetNextFreePositionAroundHex(movePosition, lastPivotNeighbour, gameBoard, clockwise);
                if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                {
                    movePosition = nextPosition;
                }
                else
                {
                    int lastPivotNeighbourIdx = neighbours.FindIndex(neighbour => neighbour == lastPivotNeighbour);

                    for (int j = 1; j < neighbours.Count; j++)
                    {
                        (int, int) nextNeighbour = neighbours[(lastPivotNeighbourIdx + j) % neighbours.Count];
                        nextPosition = PieceMovesTools.GetNextFreePositionAroundHex(movePosition, nextNeighbour, gameBoard, clockwise);
                        if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                        {
                            movePosition = nextPosition;
                            lastPivotNeighbour = nextNeighbour;
                            break;
                        }
                    }
                }
            }

            return movePosition;
        }
    }
}
