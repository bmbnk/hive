using System.Collections.Generic;
using UnityEngine;

public class RulesValidator : MonoBehaviour
{
    private HexesStoreScript _hexesStore;
    private HexesInfoProvider _hexesInfoProvider;
    private GameBoardScript _gameBoard;

    void Start()
    {
        GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
        _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
        _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();
    }

    public bool CanMove(GameObject hex)
    {
        var hexScript = hex.GetComponent<HexWrapperController>();
        return _hexesInfoProvider.IsBeeOnBoard(hexScript.isWhite) && !IsOneHiveRuleBroken(hex);
    }

    public bool IsBeeOnGameboardRuleBroken(bool white) //If it is fourth move of the player and the bee piece is not on the table than the rule is broken
    {
        var hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        if (hexesOnBoardIds.Count > 2 && !_hexesInfoProvider.IsBeeOnBoard(white))
            return true;
        return false;
    }

    public bool IsOneHiveRuleBroken(GameObject hexToMove)
    {
        var hexToMoveScript = hexToMove.GetComponent<HexWrapperController>();

        if (hexToMoveScript.transform.position.y > 0)
            return false;

        int[,] gameBoardWithoutHex = (int[,])_gameBoard.gameBoard.Clone();
        gameBoardWithoutHex[hexToMoveScript.positionOnBoard.Item1, hexToMoveScript.positionOnBoard.Item2] = 0;

        // TODO: You can optimize it by choosing one neighbour from groups of neighbours that are connected, because if you can reached one, than you can reach all of them
        List<(int, int)> neighboursPositions = PieceMovesTools.GetNeighbours(hexToMoveScript.positionOnBoard, gameBoardWithoutHex);

        (int, int) firstNeighbour = neighboursPositions[0];
        neighboursPositions.Remove(firstNeighbour);


        foreach (var neighbourPosition in neighboursPositions)
        {
            if (!PositionsAreConnected(firstNeighbour, neighbourPosition, gameBoardWithoutHex))
                return true;
        }
        return false;
    }

    private bool PositionsAreConnected((int, int) startPosition, (int, int) endPosition, int[,] gameBoard)
    {
        List<(int, int)> visitedPositions = new List<(int, int)>();
        visitedPositions.Add(startPosition);

        return DFS(startPosition, endPosition, gameBoard);
    }

    private bool DFS((int, int) startPosition, (int, int) endPosition, int[,] gameBoard)
    {
        List<(int, int)> visitedPositions = new List<(int, int)>();
        Stack<(int, int)> notFullyExploredPositions = new Stack<(int, int)>();
        notFullyExploredPositions.Push(startPosition);

        return DFSStep(endPosition, gameBoard, notFullyExploredPositions, visitedPositions);
    }

    private bool DFSStep((int, int) endPosition, int[,] gameBoard, Stack<(int, int)> notFullyExploredPositions, List<(int, int)> visitedPositions)
    {
        (int, int) currentPosition = notFullyExploredPositions.Pop();

        if (currentPosition == endPosition)
            return true;

        visitedPositions.Add(currentPosition);

        List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(currentPosition, gameBoard);
        foreach (var neighbour in neighbours)
        {
            if (!visitedPositions.Contains(neighbour))
            {
                notFullyExploredPositions.Push(currentPosition);
                notFullyExploredPositions.Push(neighbour);
                return DFSStep(endPosition, gameBoard, notFullyExploredPositions, visitedPositions);
            }
        }

        if (notFullyExploredPositions.Count == 0)
            return false;

        return DFSStep(endPosition, gameBoard, notFullyExploredPositions, visitedPositions);
    }
}
