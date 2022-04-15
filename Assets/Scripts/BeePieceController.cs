using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class BeePieceController : MonoBehaviour, IPieceController
    {
        public PieceType GetPieceType() => PieceType.BEE;


        public List<(int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] gameBoard3D)
        {
            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = PieceMovesTools.GetGameBoard2Dfrom3D(gameBoard3D);

            List<(int, int)> positions = PieceMovesTools.GetPositionsNextToNeighboursAroundPosition(hexPosition, gameBoard);

            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);
            List<(int, int)> notAllowedPositions = PieceMovesTools.GetNotAllowedPositionsAroundPosition(neighbours, hexPosition);

            notAllowedPositions.ForEach(notAllowedPosition =>
            {
                if (positions.Contains(notAllowedPosition))
                {
                    positions.Remove(notAllowedPosition);
                }
            });

            return positions;
        }
    }
}
