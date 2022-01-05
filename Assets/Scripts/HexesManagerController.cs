using System.Collections.Generic;
using UnityEngine;

public class HexesManagerController : MonoBehaviour
{
    private GameBoardScript _gameBoard;
    private HexesStoreScript _hexesStore;


    void Start()
    {
        GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
        _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        _hexesStore.InitializeHexes();

        PrepareHexToAddToBoard(true);
        HexWrapperController firstHex = GetHexThatIsAdded();
        _hexesStore.whiteHexesOnBoardIds.Add(firstHex.HexId);
        _gameBoard.gameBoard[GameBoardScript.GameBoardSize / 2, GameBoardScript.GameBoardSize / 2] = firstHex.HexId;
    }

    void Update()
    {

    }


    public bool CanMakeActionFromSelectedHex(bool isWhiteTurn)
    {
        return GetSelectedHex().isWhite == isWhiteTurn || !FirstMovesWereMade();
    }

    private HexWrapperController GetSelectedHex()
    {
        return _hexesStore.selectedHex.GetComponent<HexWrapperController>();
    }

    private bool FirstMovesWereMade()
    {
        return _hexesStore.blackHexesOnBoardIds.Count > 0 && _hexesStore.whiteHexesOnBoardIds.Count > 0;
    }

    public bool CanSelectedHexMove(bool isWhiteTurn)
    {
        return CanMakeActionFromSelectedHex(isWhiteTurn) || _hexesStore.blackHexesOnBoardIds.Count > 0;
    }

    public void ConfirmAddedHexOnGameboard()
    {
        HexWrapperController hex = GetHexThatIsAdded();
        List<int> hexesOnBoardIds = hex.isWhite ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        hex.isOnGameboard = true;
        hexesOnBoardIds.Add(hex.HexId);

        (int, int) currentPosition = GetCurrentAddingPosition();
        _gameBoard.gameBoard[currentPosition.Item1, currentPosition.Item2] = hex.HexId;

        _hexesStore.hexToAdd = null;
    }

    private HexWrapperController GetHexThatIsAdded()
    {
        return _hexesStore.hexToAdd.GetComponent<HexWrapperController>();
    }

    private (int, int) GetCurrentAddingPosition()
    {
        return _hexesStore.addingHexAvailablePositions[_hexesStore.addingHexCurrentPositionIndex];
    }

    public void ConfirmMovingHexOnGameboard()
    {
        HexWrapperController selectedHex = GetSelectedHex();

        (int, int) selectedHexConfirmedPosition = GetIndiciesByHexId(selectedHex.HexId);
        _gameBoard.gameBoard[selectedHexConfirmedPosition.Item1, selectedHexConfirmedPosition.Item2] = 0;

        (int, int) hexCurrentPosition = GetCurrentSelectedHexMovePosition();
        _gameBoard.gameBoard[hexCurrentPosition.Item1, hexCurrentPosition.Item2] = selectedHex.HexId;
        SetSelectedHex(null, null);
    }

    private (int, int) GetIndiciesByHexId(int HexId)
    {
        for (int i = 0; i < GameBoardScript.GameBoardSize; i++)
            for (int j = 0; j < GameBoardScript.GameBoardSize; j++)
                if (_gameBoard.gameBoard[i, j] == HexId)
                    return (i, j);

        return (-1, -1);
    }

    private (int, int) GetCurrentSelectedHexMovePosition()
    {
        return _hexesStore.selectedHexAvailablePositions[_hexesStore.selectedHexCurrentPositionIndex];
    }

    private void SetSelectedHex(GameObject selectedHex, List<(int, int)> availablePositions)
    {
        _hexesStore.selectedHex = selectedHex;
        _hexesStore.selectedHexAvailablePositions = availablePositions;
        _hexesStore.selectedHexCurrentPositionIndex = 0;
    }

    public void PrepareSelectedHex(GameObject selectedHex)
    {
        var hexWrapperScript = selectedHex.GetComponent<HexWrapperController>();
        List<(int, int)> availableMovePositions = hexWrapperScript
            .piece
            .GetComponent<IPieceController>()
            .GetPieceSpecificPositions(GetIndiciesByHexId(hexWrapperScript.HexId), _gameBoard.gameBoard);

        availableMovePositions.Add(GetIndiciesByHexId(hexWrapperScript.HexId));
        availableMovePositions.Reverse();

        SetSelectedHex(selectedHex, availableMovePositions);
    }


    public void ProposeNextAddingPosition()
    {
        (int, int) currentPosition = GetCurrentAddingPosition();
        ChangeCurrentAddingPositionToNext();
        (int, int) nextPosition = GetCurrentAddingPosition();

        HexWrapperController hexToAdd = GetHexThatIsAdded();

        MoveHexFromTo(hexToAdd, currentPosition, nextPosition);
    }

    private void ChangeCurrentAddingPositionToNext()
    {
        _hexesStore.addingHexCurrentPositionIndex = (_hexesStore.addingHexCurrentPositionIndex + 1) % _hexesStore.addingHexAvailablePositions.Count;
    }

    private void MoveHexFromTo(HexWrapperController selectedHex, (int, int) startPosition, (int, int) endPositon)
    {
        Vector3 movementVector = PieceMovesTools.getVectorFromStartToEnd(startPosition, endPositon);
        selectedHex.transform.position += movementVector;
    }

    public bool ProposeNextMovePosition()
    {
        (int, int) startPosition = GetCurrentSelectedHexMovePosition();
        (int, int) endPosition = ProposeSelectedHexNextMovePosition();
        if (startPosition != endPosition) //sprawdz jeszcze !IsOneHiveRuleBroken(GetSelectedHex())
        {
            HexWrapperController selectedHex = GetSelectedHex();
            MoveHexFromTo(selectedHex, startPosition, endPosition);
            return true;
        }
        return false;
    }

    private (int, int) ProposeSelectedHexNextMovePosition()
    {
        _hexesStore.selectedHexCurrentPositionIndex++;
        _hexesStore.selectedHexCurrentPositionIndex %= _hexesStore.selectedHexAvailablePositions.Count;

        return GetCurrentSelectedHexMovePosition();
    }

    public bool StartAddingHexToGameboard(bool isWhiteTurn)
    {
        var selectedHexPosition = GetIndiciesByHexId(GetSelectedHex().HexId);
        List<(int, int)> emptyPositionsAroundHex = PieceMovesTools.GetEmptyPositionsAroundPosition(selectedHexPosition, _gameBoard.gameBoard);

        List<GameObject> opponents = isWhiteTurn ? _hexesStore.blackHexes : _hexesStore.whiteHexes;
        List<(int, int)> availablePositions;
        if (FirstMovesWereMade())
            availablePositions = PieceMovesTools.FilterPositionsWithOpponentNeighbours(emptyPositionsAroundHex, opponents, _gameBoard.gameBoard);
        else
            availablePositions = emptyPositionsAroundHex;


        var hexes = isWhiteTurn ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
        List<int> hexesOnBoardIds = isWhiteTurn ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        PrepareHexToAddToBoard(isWhiteTurn);
        HexWrapperController hexScript = GetHexThatIsAdded();


        if (hexesOnBoardIds.Count < hexes.Count && availablePositions.Count != 0)
        {
            _hexesStore.addingHexAvailablePositions = availablePositions;
            _hexesStore.addingHexCurrentPositionIndex = 0;

            Vector3 movementVector = PieceMovesTools.getVectorFromStartToEnd(selectedHexPosition, _hexesStore.addingHexAvailablePositions[_hexesStore.addingHexCurrentPositionIndex]);
            hexScript.transform.position = GetSelectedHex().transform.position + movementVector;
            hexScript.gameObject.SetActive(true);
        
            return true;
        }
        return false;
    }

    private bool PrepareHexToAddToBoard(bool isWhiteTurn)
    {
        var hexes = isWhiteTurn ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
        List<int> hexesOnBoardIds = isWhiteTurn ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
        if (hexesOnBoardIds.Count < hexes.Count)
        {
            _hexesStore.hexToAdd = hexes[hexesOnBoardIds.Count];
            return true;
        }
        return false;
    }

    //private bool PrepareHexToAddToBoard(PieceType type)
    //{

    //}

    public bool StartMovingSelectedHex()
    {
        if (ProposeNextMovePosition())
        {
            return true;
        }
        return false;
    }

    private bool IsOneHiveRuleBroken(HexWrapperController movedHex) //If there are more than one hive than the rule is broken
    {
        return false;
    }

    private bool IsGameOver()
    {
        bool white = true;

        if (IsBeeFullySurrounded(white) || IsBeeFullySurrounded(!white))
            return true;
        return false;
    }

    private bool IsBeeOnGameboardRuleBroken(HexWrapperController hexToAdd) //If it is fourth move of the player and the bee piece is not on the table than the rule is broken
    {
        var hexesOnBoardIds = hexToAdd.isWhite ? _hexesStore.blackHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        if (hexesOnBoardIds.Count > 2 && hexToAdd.GetComponent<IPieceController>().GetPieceType() == PieceType.BEE)
            return true;
        return false;
    }

    private bool CanPlayerMove(bool white) //If the bee of that player is not on the board then the player can not move
    {
        if (!IsBeeOnBoard(white))
            return false;
        return true;
    }


    private bool IsBeeFullySurrounded(bool whiteBee)
    {
        if (IsBeeOnBoard(whiteBee))
        {
            int beeHexId = GetFirstFoundPieceId(whiteBee, PieceType.BEE);
            (int, int) beePosition = GetIndiciesByHexId(beeHexId);
            if (PieceMovesTools.getNeighbours(beePosition, _gameBoard.gameBoard).Count == 6)
                return true;
        }
        return false;
    }

    private bool IsBeeOnBoard(bool white)
    {
        int beeHexId = GetFirstFoundPieceId(white, PieceType.BEE);
        List<int> hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        if (beeHexId != -1 && hexesOnBoardIds.Contains(beeHexId))
            return true;
        return false; 
    }

    private int GetFirstFoundPieceId(bool whiteHexes, PieceType pieceType)
    {
        List<GameObject> hexes = whiteHexes ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
        int beeHexId = hexes.FindIndex(hex => hex
            .GetComponent<HexWrapperController>()
            .piece
            .GetComponent<IPieceController>()
            .GetPieceType() == pieceType);

        return beeHexId;
    }

    //private void MoveHexThatIsAdded(Vector3 movementVector)
    //{
    //    HexWrapperController hex = GetHexThatIsAdded();
    //    hex.transform.position += movementVector;
    //}
}
