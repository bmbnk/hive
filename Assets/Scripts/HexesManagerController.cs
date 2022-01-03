using System.Collections.Generic;
using UnityEngine;

public class HexesManagerController : MonoBehaviour
{
    const int GameBoardSize = 100;

    private int[,] _gameBoardGrid = new int[GameBoardSize, GameBoardSize];

    private HexesStoreScript _hexesStore;


    void Start()
    {
        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        _hexesStore.InitializeHexes();

        PrepareHexToAddToBoard(true);
        HexWrapperController firstHex = GetHexThatIsAdded();
        _hexesStore.whiteHexesOnBoardIds.Add(firstHex.HexId);
        _gameBoardGrid[GameBoardSize / 2, GameBoardSize / 2] = firstHex.HexId;
    }

    void Update()
    {

    }

    public void ProposeNextAddingPosition()
    {
        (int, int) currentPosition = GetCurrentAddingPosition();
        ChangeCurrentAddingPositionToNext();
        (int, int) nextPosition = GetCurrentAddingPosition();

        HexWrapperController hexToAdd = GetHexThatIsAdded();

        MoveHexFromTo(hexToAdd, currentPosition, nextPosition);
    }

    public void ConfirmAddedHexOnGameboard()
    {
        HexWrapperController hex = GetHexThatIsAdded();
        List<int> hexesOnBoardIds = hex.isWhite ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        hex.isOnGameboard = true;
        hexesOnBoardIds.Add(hex.HexId);

        (int, int) currentPosition = GetCurrentAddingPosition();
        _gameBoardGrid[currentPosition.Item1, currentPosition.Item2] = hex.HexId;

        _hexesStore.hexToAdd = null;
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

    public void ConfirmMovingHexOnGameboard()
    {
        HexWrapperController selectedHex = GetSelectedHex();

        (int, int) selectedHexConfirmedPosition = GetIndiciesByHexId(selectedHex.HexId);
        _gameBoardGrid[selectedHexConfirmedPosition.Item1, selectedHexConfirmedPosition.Item2] = 0;

        (int, int) hexCurrentPosition = GetCurrentSelectedHexMovePosition();
        _gameBoardGrid[hexCurrentPosition.Item1, hexCurrentPosition.Item2] = selectedHex.HexId;
        SetSelectedHex(null, null);
    }

    public bool StartMovingSelectedHex()
    {
        if (ProposeNextMovePosition())
        {
            return true;
        }
        return false;
    }

    public bool StartAddingHexToGameboard(bool isWhiteTurn)
    {
        var selectedHexPosition = GetIndiciesByHexId(GetSelectedHex().HexId);
        List<(int, int)> emptyPositionsAroundHex = PieceMovesTools.GetEmptyPositionsAroundPosition(selectedHexPosition, _gameBoardGrid);

        List<GameObject> opponents = isWhiteTurn ? _hexesStore.blackHexes : _hexesStore.whiteHexes;
        List<(int, int)> availablePositions;
        if (FirstMovesWereMade())
            availablePositions = PieceMovesTools.FilterPositionsWithOpponentNeighbours(emptyPositionsAroundHex, opponents, _gameBoardGrid);
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

    public void PrepareSelectedHex(GameObject selectedHex)
    {
        var hexWrapperScript = selectedHex.GetComponent<HexWrapperController>();
        List<(int, int)> availableMovePositions = hexWrapperScript
            .piece
            .GetComponent<IPieceController>()
            .GetPieceSpecificPositions(GetIndiciesByHexId(hexWrapperScript.HexId), _gameBoardGrid);

        availableMovePositions.Add(GetIndiciesByHexId(hexWrapperScript.HexId));
        availableMovePositions.Reverse();

        SetSelectedHex(selectedHex, availableMovePositions);
    }

    public bool CanSelectedHexMove(bool isWhiteTurn)
    {
        return CanMakeActionFromSelectedHex(isWhiteTurn) || _hexesStore.blackHexesOnBoardIds.Count > 0;
    }

    public bool CanMakeActionFromSelectedHex(bool isWhiteTurn)
    {
        return GetSelectedHex().isWhite == isWhiteTurn || !FirstMovesWereMade();
    }

    private void MoveHexFromTo(HexWrapperController selectedHex, (int, int) startPosition, (int, int) endPositon)
    {
        Vector3 movementVector = PieceMovesTools.getVectorFromStartToEnd(startPosition, endPositon);
        selectedHex.transform.position += movementVector;
    }

    private (int, int) GetIndiciesByHexId(int HexId)
    {
        for (int i = 0; i < GameBoardSize; i++)
            for (int j = 0; j < GameBoardSize; j++)
                if (_gameBoardGrid[i, j] == HexId)
                    return (i, j);

        return (-1, -1);
    }

    private bool IsOneHiveRuleBroken(HexWrapperController selectedHex)
    {
        //TODO: If there are more than one hive than the rule is broken
        return false;
    }

    private bool IsBeeOnGameboardRuleBroken()
    {
        //TODO: If it is fourth move of the player and the bee piece is not on the table than the rule is broken
        return false;
    }

    private bool CanPlayerMove()
    {
        //TODO: If the bee of that player is not on the board then the player can not move
        return true;
    }

    private bool IsGameOver()
    {
        //TODO: If one of the bees is surrounded with 6 hexes than the game is over
        return false;
    }



    private void SetSelectedHex(GameObject selectedHex, List<(int, int)> availablePositions)
    {
        _hexesStore.selectedHex = selectedHex;
        _hexesStore.selectedHexAvailablePositions = availablePositions;
        _hexesStore.selectedHexCurrentPositionIndex = 0;
    }

    private HexWrapperController GetSelectedHex()
    {
        return _hexesStore.selectedHex.GetComponent<HexWrapperController>();
    }

    private (int, int) GetCurrentAddingPosition()
    {
        return _hexesStore.addingHexAvailablePositions[_hexesStore.addingHexCurrentPositionIndex];
    }



    private bool FirstMovesWereMade()
    {
        return _hexesStore.blackHexesOnBoardIds.Count > 0 && _hexesStore.whiteHexesOnBoardIds.Count > 0;
    }

    private void ChangeCurrentAddingPositionToNext()
    {
        _hexesStore.addingHexCurrentPositionIndex = (_hexesStore.addingHexCurrentPositionIndex + 1) % _hexesStore.addingHexAvailablePositions.Count;
    }

    private void MoveHexThatIsAdded(Vector3 movementVector)
    {
        HexWrapperController hex = GetHexThatIsAdded();
        hex.transform.position += movementVector;
    }

    private HexWrapperController GetHexThatIsAdded()
    {
        return _hexesStore.hexToAdd.GetComponent<HexWrapperController>();
    }

    private (int, int) GetCurrentSelectedHexMovePosition()
    {
        return _hexesStore.selectedHexAvailablePositions[_hexesStore.selectedHexCurrentPositionIndex];
    }

    private (int, int) ProposeSelectedHexNextMovePosition()
    {
        _hexesStore.selectedHexCurrentPositionIndex++;
        _hexesStore.selectedHexCurrentPositionIndex %= _hexesStore.selectedHexAvailablePositions.Count;

        return GetCurrentSelectedHexMovePosition();
    }

    //privatebool PrepareHexToAddToBoard(PieceType type)
    //{

    //}

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
}
