using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class BeePieceController : IPieceController
    {
        public PieceType GetPieceType() => PieceType.BEE;


        public List<(int, int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] gameBoard3D)
        {
            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = PieceMovesTools.GetGameBoard2Dfrom3D(gameBoard3D);

            List<(int, int)> positions2D = PieceMovesTools.GetPositionsNextToNeighboursAroundPosition(hexPosition, gameBoard);

            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);
            List<(int, int)> notAllowedPositions = PieceMovesTools.GetNotAllowedPositionsAroundPosition(neighbours, hexPosition);

            notAllowedPositions.ForEach(notAllowedPosition =>
            {
                if (positions2D.Contains(notAllowedPosition))
                {
                    positions2D.Remove(notAllowedPosition);
                }
            });

            List<(int, int, int)> positions = new List<(int, int, int)>();
            positions2D.ForEach(position => positions.Add((position.Item1, position.Item2, 0)));

            return positions;
        }
    }
}
