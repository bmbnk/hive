using System.Collections.Generic;
using UnityEngine;

public class BeetlePieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;
    private Dictionary<int, int> _beetleIdToHexUnderneathId;

    void Start()
    {
        _beetleIdToHexUnderneathId = new Dictionary<int, int>();
        _type = PieceType.BEETLE;
    }

    public PieceType GetPieceType() { return _type; }

    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] gameBoard)
    {
        List<(int, int)> positions;

        if (HexesOnPositionNumber(hexPosition, gameBoard) > 1)
        {
            positions = PieceMovesTools.GetPositionsAroundPosition(hexPosition);
        } else
        {
            positions = PieceMovesTools.GetPositionsNextToNeighboursAroundPosition(hexPosition, gameBoard);
            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);
            neighbours.ForEach(neighbour => positions.Add(neighbour));
        }


        for (int i = positions.Count - 1; i >= 0; i--)
        {
            int heightOfLowerStackNextToPosition = HexesOnPositionNumber(PieceMovesTools.GetNextPositionAroundHex(positions[i], hexPosition, true), gameBoard);
            int heightOfSecondStackNextToPosition = HexesOnPositionNumber(PieceMovesTools.GetNextPositionAroundHex(positions[i], hexPosition, false), gameBoard);

            if (heightOfSecondStackNextToPosition < heightOfLowerStackNextToPosition)
                    heightOfLowerStackNextToPosition = heightOfSecondStackNextToPosition;

            int heightOfPositionStack = HexesOnPositionNumber(positions[i], gameBoard);
            int heightOfCurrentPositionStack = HexesUnderBeetleNumber(gameBoard[hexPosition.Item1, hexPosition.Item2]);

            if (heightOfPositionStack < heightOfLowerStackNextToPosition && heightOfCurrentPositionStack < heightOfLowerStackNextToPosition)
                positions.Remove(positions[i]);
        }

        return positions;
    }

    public int GetIdOfFirstHexUnderneathBeetle(int beetleId)
    {
        if (_beetleIdToHexUnderneathId.ContainsKey(beetleId))
            return _beetleIdToHexUnderneathId[beetleId];
        return -1;
    }

    public void RemoveHexUnderneathBeetle(int beetleId)
    {
        if (_beetleIdToHexUnderneathId.ContainsKey(beetleId))
            _beetleIdToHexUnderneathId.Remove(beetleId);
    }

    public void SetHexUnderneathBeetle(int beetleId, int hexUnderneathId)
    {
        _beetleIdToHexUnderneathId[beetleId] = hexUnderneathId;
    }

    public bool IsHexUnderneathBeetle(int hexId)
    {
        return _beetleIdToHexUnderneathId.ContainsValue(hexId);
    }

    public int HexesUnderBeetleNumber(int beetleId)
    {
        int hexesUnderBeetleCounter = 0;

        int hexUnderneathId = GetIdOfFirstHexUnderneathBeetle(beetleId);
        while (hexUnderneathId != -1)
        {
            hexesUnderBeetleCounter++;
            hexUnderneathId = GetIdOfFirstHexUnderneathBeetle(hexUnderneathId);
        }
        return hexesUnderBeetleCounter;
    }

    private int HexesOnPositionNumber((int, int) position, int[,] gameBoard)
    {
        int hexId = gameBoard[position.Item1, position.Item2];
        if (hexId > 0)
        {
            int hexesUnderHexNumber = HexesUnderBeetleNumber(hexId);
            return hexesUnderHexNumber + 1;
        }
        return 0;
    }

}
