using System.Collections.Generic;
using UnityEngine;

public class BeePieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;

    void Start()
    {
        _type = PieceType.BEE;
    }

    public PieceType GetPieceType() { return _type; }

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
