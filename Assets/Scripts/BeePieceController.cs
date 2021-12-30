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
        List<(int, int)> positions = PieceMovesTools.GetPositionsWithNeighboursAroundPosition(hexPosition, gameBoard);
        List<(int, int)> neighbours = PieceMovesTools.getNeighbours(hexPosition, gameBoard);
        List<(int, int)> notAllowedPositions = PieceMovesTools.getNotAllowedNextPositions(neighbours, hexPosition);

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
