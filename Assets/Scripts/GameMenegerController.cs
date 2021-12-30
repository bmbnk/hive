using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenegerController : MonoBehaviour
{
    const int GameBoardSize = 100;

    public GameObject WhiteHex1;
    public GameObject WhiteHex2;
    public GameObject WhiteHex3;
    public GameObject WhiteHex4;
    public GameObject WhiteHex5;
    public GameObject WhiteHex6;
    public GameObject WhiteHex7;
    public GameObject WhiteHex8;
    public GameObject WhiteHex9;
    public GameObject WhiteHex10;
    public GameObject WhiteHex11;

    public GameObject BlackHex1;
    public GameObject BlackHex2;
    public GameObject BlackHex3;
    public GameObject BlackHex4;
    public GameObject BlackHex5;
    public GameObject BlackHex6;
    public GameObject BlackHex7;
    public GameObject BlackHex8;
    public GameObject BlackHex9;
    public GameObject BlackHex10;
    public GameObject BlackHex11;

    public GameObject BeePiece;
    public GameObject BeetlePiece;
    public GameObject GrasshopperPiece;
    public GameObject SpiderPiece;
    public GameObject AntPiece;


    private List<int> _whiteHexesOnBoardIds = new List<int>();
    private List<int> _blackHexesOnBoardIds = new List<int>();
    private bool _addingHexToBoard = false;
    private bool _movingHexOnBoard = false;
    private bool _isWhiteTurn = false;
    private bool _isHexSelected = false;

    private GameObject _selectedHex;
    private List<(int, int)> _selectedHexAvailablePositions;
    private int _selectedHexCurrentPositionIndex = -1;

    private List<(int, int)> _addingHexAvailablePositions;
    private int _addingHexCurrentPositionIndex = -1;


    // it initializes values to zeros
    private int[,] _gameBoardGrid = new int[GameBoardSize, GameBoardSize];

    private List<(GameObject Value, int HexId)> _whiteHexes;
    private List<(GameObject Value, int HexId)> _blackHexes;


    void Start()
    {
        InitializeHexes();
        _gameBoardGrid[GameBoardSize / 2, GameBoardSize / 2] = 1;
        _whiteHexesOnBoardIds.Add(_whiteHexes[0].HexId);
    }

    void Update()
    {   
        if (_addingHexToBoard)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ProposeNextAddingPosition();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ConfirmHexOnGameboard();
            }
        } else if (_movingHexOnBoard)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ProposeNextMovePosition();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                var hexCurrentPosition = _selectedHexAvailablePositions[_selectedHexCurrentPositionIndex];
                _gameBoardGrid[hexCurrentPosition.Item1, hexCurrentPosition.Item2] = _selectedHex.GetComponent<HexWrapperController>().HexId;
                _movingHexOnBoard = false;
                _isWhiteTurn = !_isWhiteTurn;
                _selectedHexCurrentPositionIndex = -1;
                _isHexSelected = false;
            }
        } else if (_isHexSelected)
        {
            bool isWhite = _selectedHex.GetComponent<HexWrapperController>().isWhite;
            if (isWhite == _isWhiteTurn || (_blackHexesOnBoardIds.Count < 1 || _whiteHexesOnBoardIds.Count < 1))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if(_blackHexesOnBoardIds.Count > 0)
                        StartMovingSelectedHex(_selectedHex);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    StartAddingHexToGameboard(_selectedHex);
            }
        }
    }

    private void ProposeNextAddingPosition()
    {
        (int, int) currentPosition = _addingHexAvailablePositions[_addingHexCurrentPositionIndex];
        _addingHexCurrentPositionIndex = (_addingHexCurrentPositionIndex+1) % _addingHexAvailablePositions.Count;
        (int, int) nextPosition = _addingHexAvailablePositions[_addingHexCurrentPositionIndex];

        HexWrapperController hexScript = getHexThatIsAddedScript();
        Vector3 movementVector = PieceMovesTools.getVectorFromStartToEnd(currentPosition, nextPosition);
        hexScript.transform.position += movementVector;
    }

    private HexWrapperController getHexThatIsAddedScript()
    {
        var hexes = _isWhiteTurn ? _whiteHexes : _blackHexes;
        List<int> hexOnBoardIds = _isWhiteTurn ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;
        return hexes[hexOnBoardIds.Count].Value.GetComponent<HexWrapperController>();
    }

    private void ConfirmHexOnGameboard()
    {
        List<int> hexOnBoardIds = _isWhiteTurn ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;

        HexWrapperController hexScript = getHexThatIsAddedScript();
        hexScript.isOnGameboard = true;

        (int, int) currentPosition = _addingHexAvailablePositions[_addingHexCurrentPositionIndex];
        _gameBoardGrid[currentPosition.Item1, currentPosition.Item2] = hexScript.HexId;

        hexOnBoardIds.Add(hexScript.HexId);
        _addingHexToBoard = false;
        _isWhiteTurn = !_isWhiteTurn;
        _isHexSelected = false;
    }

    private void ProposeNextMovePosition()
    {
        (int, int) startPosition;

        if(_selectedHexCurrentPositionIndex != -1)
        {
            startPosition = _selectedHexAvailablePositions[_selectedHexCurrentPositionIndex];
            _selectedHexCurrentPositionIndex++;
            _selectedHexCurrentPositionIndex %= _selectedHexAvailablePositions.Count;
        } else
        {
            startPosition = GetIndiciesByHexId(_selectedHex.GetComponent<HexWrapperController>().HexId);
            _gameBoardGrid[startPosition.Item1, startPosition.Item2] = 0;
            _selectedHexCurrentPositionIndex = 0;
        }

        (int, int) endPosition = _selectedHexAvailablePositions[_selectedHexCurrentPositionIndex];
        MoveHexFromTo(_selectedHex, startPosition, endPosition);
    }

    private void MoveHexFromTo(GameObject selectedHex, (int, int) startPosition, (int, int) endPositon)
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

    private void StartMovingSelectedHex(GameObject selectedHex)
    { 
        if (!IsOneHiveRuleBroken(selectedHex))
        {
            var hexWrapperScript = selectedHex.GetComponent<HexWrapperController>();
            var availableMovePositions = hexWrapperScript
                .piece
                .GetComponent<IPieceController>()
                .GetPieceSpecificPositions(GetIndiciesByHexId(hexWrapperScript.HexId), _gameBoardGrid);

            if (availableMovePositions.Count > 0)
            {
                _selectedHexAvailablePositions = availableMovePositions;
                _selectedHexAvailablePositions.Add(GetIndiciesByHexId(hexWrapperScript.HexId));
                ProposeNextMovePosition();
                _movingHexOnBoard = true;
            }
        }
    }

    private bool IsOneHiveRuleBroken(GameObject selectedHex)
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

    public void StartAddingHexToGameboard(GameObject selectedHex)
    {
        _selectedHex = selectedHex;
        var selectedHexPosition = GetIndiciesByHexId(selectedHex.GetComponent<HexWrapperController>().HexId);
        List<(int, int)> emptyPositionsAroundHex = PieceMovesTools.GetEmptyPositionsAroundPosition(selectedHexPosition, _gameBoardGrid);

        List<(GameObject Value, int HexId)> opponents = _isWhiteTurn ? _blackHexes : _whiteHexes;
        List<(int, int)> availablePositions;
        if (_blackHexesOnBoardIds.Count > 0 && _whiteHexesOnBoardIds.Count > 0)
            availablePositions = PieceMovesTools.FilterPositionsWithOpponentNeighbours(emptyPositionsAroundHex, opponents, _gameBoardGrid);
        else
            availablePositions = emptyPositionsAroundHex;


        var hexes = _isWhiteTurn ? _whiteHexes : _blackHexes;
        List<int> hexOnBoardIds = _isWhiteTurn ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;
        HexWrapperController hexScript = getHexThatIsAddedScript();


        if (hexOnBoardIds.Count < hexes.Count)
        {
            if (availablePositions.Count != 0)
            {
                _addingHexToBoard = true;

                _addingHexAvailablePositions = availablePositions;
                _addingHexCurrentPositionIndex = 0;

                Vector3 movementVector = PieceMovesTools.getVectorFromStartToEnd(selectedHexPosition, _addingHexAvailablePositions[_addingHexCurrentPositionIndex]);
                hexScript.transform.position = selectedHex.GetComponent<HexWrapperController>().transform.position + movementVector;
                hexScript.gameObject.SetActive(true);
            }

        }
    }

    public void HexSelected(GameObject selectedHex)
    {
        if (!_movingHexOnBoard && !_addingHexToBoard)
        {
            _selectedHex = selectedHex;
            _isHexSelected = true;
        }
    }

    private void InitializeHexes()
    {
        _whiteHexes = new List<(GameObject Value, int HexId)>();
        _whiteHexes.Add(
        (
            Value: WhiteHex1,
            HexId: 1
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex2,
            HexId: 2
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex8,
            HexId: 8
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex10,
            HexId: 10
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex7,
            HexId: 7
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex9,
            HexId: 9
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex5,
            HexId: 5
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex6,
            HexId: 6
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex3,
            HexId: 3
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex4,
            HexId: 4
        ));
        _whiteHexes.Add(
        (
            Value: WhiteHex11,
            HexId: 11
        ));
        _whiteHexes.ForEach(hex => hex.Value.GetComponent<HexWrapperController>().HexId = hex.HexId);


        _blackHexes = new List<(GameObject Value, int HexId)>();
        _blackHexes.Add(
        (
            Value: BlackHex1,
            HexId: 12
        ));
        _blackHexes.Add(
        (
            Value: BlackHex2,
            HexId: 13
        ));
        _blackHexes.Add(
        (
            Value: BlackHex9,
            HexId: 20
        ));
        _blackHexes.Add(
        (
            Value: BlackHex10,
            HexId: 21
        ));
        _blackHexes.Add(
        (
            Value: BlackHex7,
            HexId: 18
        ));
        _blackHexes.Add(
        (
            Value: BlackHex8,
            HexId: 19
        ));
        _blackHexes.Add(
        (
            Value: BlackHex5,
            HexId: 16
        ));
        _blackHexes.Add(
        (
            Value: BlackHex6,   
            HexId: 17
        ));
        _blackHexes.Add(
        (
            Value: BlackHex3,
            HexId: 14
        ));
        _blackHexes.Add(
        (
            Value: BlackHex4,
            HexId: 15
        ));
        _blackHexes.Add(
        (
            Value: BlackHex11,
            HexId: 22
        ));
        
        _blackHexes.ForEach(hex => hex.Value.GetComponent<HexWrapperController>().HexId = hex.HexId);
    }
}