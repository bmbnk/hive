using System.Collections.Generic;
using UnityEngine;

public class BeePieceController : MonoBehaviour, IPieceController
{
    public PieceType GetPieceType() => PieceType.BEE;


    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard)
    {
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
