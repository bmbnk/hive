using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class GrasshopperPieceController : MonoBehaviour, IPieceController
    {
        public PieceType GetPieceType() => PieceType.GRASSHOPPER;


        public List<(int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] gameBoard3D)
        {
            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = PieceMovesTools.GetGameBoard2Dfrom3D(gameBoard3D);

            List<(int, int)> positions = new List<(int, int)>();
            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);

            neighbours.ForEach(neighbour =>
            {
                if (gameBoard[neighbour.Item1, neighbour.Item2] != 0)
                {
                    (int, int) position = PieceMovesTools.GetFirstFreePositionInDirectionOfNeighbour(hexPosition, neighbour, gameBoard);
                    if (position != (-1, -1))
                        positions.Add(position);
                }
            });

            return positions;
        }
    }
}
