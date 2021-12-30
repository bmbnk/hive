using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenegerController : MonoBehaviour
{
    const int GameBoardSize = 100;
    const float Padding = 0.4f;

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

    // it initializes values to zeros
    private int[,] _gameBoardGrid = new int[GameBoardSize, GameBoardSize];

    private List<(Vector3 Vector,
        (int, int) EvenRowNeighbourIdxsDelta,
        (int, int) OddRowNeighbourIdxsDelta)> _neighboursLocationParameters;
    private List<(GameObject Value, int HexId)> _whiteHexes;
    private List<(GameObject Value, int HexId)> _blackHexes;


    void Start()
    {
        InitializeNeighboursLocationParameters();
        InitializeHexes();
        InitializePieces();

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
        HexWrapperController hexScript = getHexThatIsAddedScript();
        //MoveHexFromTo(_selectedHex, _selectedHexAvailablePositions[_selectedHexCurrentPositionIndex], _selectedHexAvailablePositions[_selectedHexCurrentPositionIndex]);
        hexScript.transform.position -= hexScript.availableLocationOffsetVectors[hexScript.currentOffsetVectorIndex] + 2 * Padding * hexScript.availableLocationOffsetVectors[hexScript.currentOffsetVectorIndex].normalized;
        hexScript.currentOffsetVectorIndex = (hexScript.currentOffsetVectorIndex + 1) % hexScript.availableLocationOffsetVectors.Count;
        hexScript.transform.position += hexScript.availableLocationOffsetVectors[hexScript.currentOffsetVectorIndex] + 2 * Padding * hexScript.availableLocationOffsetVectors[hexScript.currentOffsetVectorIndex].normalized;
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
        var hexOffsetVector = hexScript.availableLocationOffsetVectors[hexScript.currentOffsetVectorIndex];
        hexScript.isOnGameboard = true;

        var offsetParameters = _neighboursLocationParameters.Find(p => p.Vector == hexOffsetVector);
        var selectedTilePosition = GetIndiciesByHexId(_selectedHex.GetComponent<HexWrapperController>().HexId);
        var delta = selectedTilePosition.Item1 % 2 == 1 ? offsetParameters.EvenRowNeighbourIdxsDelta : offsetParameters.OddRowNeighbourIdxsDelta;
        var idxs = (selectedTilePosition.Item1 + delta.Item1, selectedTilePosition.Item2 + delta.Item2);
        _gameBoardGrid[idxs.Item1, idxs.Item2] = hexScript.HexId;

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
        Vector3 movementVector = new Vector3(0, 0, 0);
        if ((startPosition.Item1 + endPositon.Item1) % 2 == 1)
        {
            var deltaPosition = startPosition.Item1 % 2 == 1 ? _neighboursLocationParameters[0].EvenRowNeighbourIdxsDelta : _neighboursLocationParameters[0].OddRowNeighbourIdxsDelta;
            var deltaVector = _neighboursLocationParameters[0].Vector;
            startPosition = (startPosition.Item1 + deltaPosition.Item1, startPosition.Item2 + deltaPosition.Item2);
            movementVector += deltaVector + 2 * deltaVector.normalized * Padding;
        }

        movementVector += new Vector3(
            (endPositon.Item2 - startPosition.Item2) * (2 + 2 * Padding),
            0,
            (-(endPositon.Item1 - startPosition.Item1) / 2) * (3 * 2 / Mathf.Sqrt(3) + 2 * Padding * Mathf.Sqrt(3)));

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
        var hexPosition = GetIndiciesByHexId(selectedHex.GetComponent<HexWrapperController>().HexId);
        _selectedHex = selectedHex;
        List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)> availablePositionOffsetParams = GetAvailablePositionOffsetParams(hexPosition);

        List<Vector3> availablePositionOffsetVectors;
        if (_blackHexesOnBoardIds.Count > 0 && _whiteHexesOnBoardIds.Count > 0)
        {
            availablePositionOffsetVectors = FilterPositionsWithOpponentNeighbours(availablePositionOffsetParams, hexPosition);
        }
        else
        {
            availablePositionOffsetVectors = new List<Vector3>();
            availablePositionOffsetParams.ForEach(positionParams => availablePositionOffsetVectors.Add(positionParams.Vector));
        }

        var hexes = _isWhiteTurn ? _whiteHexes : _blackHexes;
        List<int> hexOnBoardIds = _isWhiteTurn ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;
        HexWrapperController hexScript = getHexThatIsAddedScript();


        if (hexOnBoardIds.Count < hexes.Count)
        {
            if (availablePositionOffsetVectors.Count != 0)
            {
                _addingHexToBoard = true;

                availablePositionOffsetVectors.ForEach(vector => {
                    hexScript
                        .availableLocationOffsetVectors
                        .Add(vector);
                });

                hexScript.transform.position = selectedHex.GetComponent<HexWrapperController>().transform.position + availablePositionOffsetVectors[0] + 2 * Padding * availablePositionOffsetVectors[0].normalized;
                hexScript.currentOffsetVectorIndex = 0;
                hexScript.gameObject.SetActive(true);
            }

        }
    }

    private List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)> GetAvailablePositionOffsetParams((int, int) startPosition)
    {
        List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)> availablePositionOffsetParams
            = new List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)>();

        _neighboursLocationParameters.ForEach(locationParams =>
        {
            (int, int) idxsDelta = startPosition.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
            (int, int) nextPositionAround = (startPosition.Item1 + idxsDelta.Item1, startPosition.Item2 + idxsDelta.Item2);
            if (_gameBoardGrid[nextPositionAround.Item1, nextPositionAround.Item2] == 0)
                availablePositionOffsetParams.Add(locationParams);
        });

        return availablePositionOffsetParams;
    }

    private List<Vector3> FilterPositionsWithOpponentNeighbours(List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)> postionOffsetParams, (int, int) hexPosition)
    {
        List<Vector3> filteredPositionVectors = new List<Vector3>();
        bool noOpponentNeighbour;

        foreach (var offsetParameters in postionOffsetParams)
        {
            noOpponentNeighbour = true;
            (int, int) offset = hexPosition.Item1 % 2 == 1 ? offsetParameters.EvenRowNeighbourIdxsDelta : offsetParameters.OddRowNeighbourIdxsDelta;
            (int, int) position = (hexPosition.Item1 + offset.Item1, hexPosition.Item2 + offset.Item2);

            foreach (var locationParams in _neighboursLocationParameters)
            {
                (int, int) idxsDelta = position.Item1 % 2 == 1 ? locationParams.EvenRowNeighbourIdxsDelta : locationParams.OddRowNeighbourIdxsDelta;
                (int, int) idxs = (position.Item1 + idxsDelta.Item1, position.Item2 + idxsDelta.Item2);
                int currentNeighbourHexId = _gameBoardGrid[idxs.Item1, idxs.Item2];
                if (currentNeighbourHexId != 0)
                {
                    List<(GameObject Value, int HexId)> neighboursList = new List<(GameObject Value, int HexId)>();

                    if (_isWhiteTurn)
                        neighboursList = _blackHexes.FindAll(hex => hex.HexId == currentNeighbourHexId);
                    else
                        neighboursList = _whiteHexes.FindAll(hex => hex.HexId == currentNeighbourHexId);

                    if (neighboursList.Count == 1)
                    {
                        noOpponentNeighbour = false;
                        break;
                    }
                }
            }
            if (noOpponentNeighbour)
                filteredPositionVectors.Add(offsetParameters.Vector);
        }
        return filteredPositionVectors;
    }

    public void HexSelected(GameObject selectedHex)
    {
        if (!_movingHexOnBoard && !_addingHexToBoard)
        {
            _selectedHex = selectedHex;
            _isHexSelected = true;
        }
    }

    private void InitializeNeighboursLocationParameters()
    {
        _neighboursLocationParameters = new List<(Vector3 Vector, (int, int) EvenRowNeighbourIdxsDelta, (int, int) OddRowNeighbourIdxsDelta)>();
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(1, 0, Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (-1, 0),
            OddRowNeighbourIdxsDelta: (-1, 1)
        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(2, 0, 0),
            EvenRowNeighbourIdxsDelta: (0, 1),
            OddRowNeighbourIdxsDelta: (0, 1)
        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(1, 0, -Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (1, 0),
            OddRowNeighbourIdxsDelta: (1, 1)

        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(-1, 0, -Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (1, -1),
            OddRowNeighbourIdxsDelta: (1, 0)

        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(-2, 0, 0),
            EvenRowNeighbourIdxsDelta: (0, -1),
            OddRowNeighbourIdxsDelta: (0, -1)
        ));
        _neighboursLocationParameters.Add(
        (
            Vector: new Vector3(-1, 0, Mathf.Sqrt(3)),
            EvenRowNeighbourIdxsDelta: (-1, -1),
            OddRowNeighbourIdxsDelta: (-1, 0)
        ));

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

    private void InitializePieces()
    {
        List<((int, int) EvenRowPositionOffset,
            (int, int) OddRowPositionOffset)>
            oneStepPositionOffsets = new List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)>();

        _neighboursLocationParameters.ForEach(locationParams =>
        {
            oneStepPositionOffsets.Add((
                EvenRowPositionOffset: locationParams.EvenRowNeighbourIdxsDelta,
                OddRowPositionOffset: locationParams.OddRowNeighbourIdxsDelta
            ));
        });

        BeePiece.GetComponent<BeePieceController>().SetPositionOffsets(oneStepPositionOffsets);
        BeetlePiece.GetComponent<BeetlePieceController>().SetPositionOffsets(oneStepPositionOffsets);
        GrasshopperPiece.GetComponent<GrasshopperPieceController>().SetPositionOffsets(oneStepPositionOffsets);
        SpiderPiece.GetComponent<SpiderPieceController>().SetPositionOffsets(oneStepPositionOffsets);
        AntPiece.GetComponent<AntPieceController>().SetPositionOffsets(oneStepPositionOffsets);
    }
}