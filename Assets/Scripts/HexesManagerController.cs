using System.Collections.Generic;
using UnityEngine;

public class HexesManagerController : MonoBehaviour
{
    public GameObject HexPrefeab;
    public GameObject BeetlePiece;
    private GameBoardScript _gameBoard;
    private HexesStoreScript _hexesStore;
    private HexesInfoProvider _hexesInfoProvider;
    private IMoveValidator _moveValidator;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
        _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
        _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();

        GameObject moveValidatorGameObject = GameObject.FindWithTag("MoveValidator");
        _moveValidator = moveValidatorGameObject.GetComponent<IMoveValidator>();

        _hexesStore.InitializeHexes();
    }

    private HexWrapperController GetHexToMoveScript()
    {
        return _hexesStore.hexToMove.GetComponent<HexWrapperController>();
    }

    public bool ConfirmAddedHexOnGameboard(GameObject selectedHex)
    {
        HexWrapperController selectedHexScript = selectedHex.GetComponent<HexWrapperController>();
        if (selectedHexScript.HexId == 0)
        {
            HexWrapperController hexToAddScript = GetHexThatIsAddedScript();
            List<int> hexesOnBoardIds = hexToAddScript.isWhite ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

            hexToAddScript.isOnGameboard = true;
            hexesOnBoardIds.Add(hexToAddScript.HexId);

            hexToAddScript.transform.position = selectedHexScript.transform.position;
            hexToAddScript.positionOnBoard = selectedHexScript.positionOnBoard;
            hexToAddScript.isOnGameboard = true;
            _hexesStore.hexToAdd.SetActive(true);
            (int, int) addedHexPosition = hexToAddScript.positionOnBoard;
            _gameBoard.gameBoard[addedHexPosition.Item1, addedHexPosition.Item2] = hexToAddScript.HexId;

            ResetHexToAdd();
            return true;
        }
        return false;
    }

    private void ResetHexToAdd()
    {
        if (_hexesStore.hexPropositionsToAdd != null)
        {
            for (int i = _hexesStore.hexPropositionsToAdd.Count - 1; i >= 0; i--)
            {
                Destroy(_hexesStore.hexPropositionsToAdd[i]);
            }
        }
        SetHexToAdd(null, null);
    }

    private void SetHexToAdd(GameObject hexToAdd, List<GameObject> hexPropositionsToAdd)
    {
        _hexesStore.hexToAdd = hexToAdd;
        _hexesStore.hexPropositionsToAdd = hexPropositionsToAdd; 
    }

    public bool ConfirmMovingHexOnGameboard(GameObject selectedHex)
    {
        HexWrapperController selectedHexScript = selectedHex.GetComponent<HexWrapperController>();
        if (selectedHexScript.HexId == 0)
        {
            HexWrapperController hexToMoveScript = GetHexToMoveScript();

            (int, int) currentHexToMovePosition = hexToMoveScript.positionOnBoard;

            if (hexToMoveScript.piece.GetComponent<IPieceController>().GetPieceType() == PieceType.BEETLE)
            {
                var beetlePieceScript = BeetlePiece.GetComponent<BeetlePieceController>();

                int hexUnderneathId = beetlePieceScript.GetIdOfFirstHexUnderneathBeetle(hexToMoveScript.HexId);
                _gameBoard.gameBoard[currentHexToMovePosition.Item1, currentHexToMovePosition.Item2] = hexUnderneathId != -1 ? hexUnderneathId : 0;

                beetlePieceScript.RemoveHexUnderneathBeetle(hexToMoveScript.HexId);

                (int, int) targetPosition = selectedHexScript.positionOnBoard;
                int targetPositionHexId = _gameBoard.gameBoard[targetPosition.Item1, targetPosition.Item2];
                if (targetPositionHexId != 0)
                    beetlePieceScript.SetHexUnderneathBeetle(hexToMoveScript.HexId, targetPositionHexId);
            }
            else
            {
                _gameBoard.gameBoard[currentHexToMovePosition.Item1, currentHexToMovePosition.Item2] = 0;
            }

            hexToMoveScript.positionOnBoard = selectedHexScript.positionOnBoard;

            (int, int) movedHexPosition = hexToMoveScript.positionOnBoard;
            _gameBoard.gameBoard[movedHexPosition.Item1, movedHexPosition.Item2] = hexToMoveScript.HexId;

            hexToMoveScript.transform.position = selectedHexScript.transform.position;

            ResetHexToMove();
            return true;
        }
        return false;
    }

    private void ResetHexToMove()
    {
        if (_hexesStore.hexPropositionsToMove != null)
        {
            for (int i = _hexesStore.hexPropositionsToMove.Count - 1; i >= 0; i--)
            {
                Destroy(_hexesStore.hexPropositionsToMove[i]);
            }
        }
        SetHexToMove(null, null);
    }

    private void SetHexToMove(GameObject selectedHex, List<GameObject> hexPropositionsToMove)
    {
        _hexesStore.hexToMove = selectedHex;
        _hexesStore.hexPropositionsToMove = hexPropositionsToMove;
    }

    private HexWrapperController GetHexThatIsAddedScript()
    {
        return _hexesStore.hexToAdd.GetComponent<HexWrapperController>();
    }

    public bool PrepareHexToAddToBoard(PieceType type, bool white)
    {
        if (_moveValidator.CanAdd(type, white))
        {
            if (_hexesStore.hexToAdd != null)
            {
                ResetHexToAdd();
            }

            var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
            List<int> hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

            GameObject hexToAdd = null;
            foreach (var hex in hexes)
            {
                HexWrapperController hexScript = hex.GetComponent<HexWrapperController>();
                if (hexScript.piece.GetComponent<IPieceController>().GetPieceType() == type
                    && !hexesOnBoardIds.Contains(hexScript.HexId))
                {
                    hexToAdd = hex;
                    break;
                }
            }

            if (hexToAdd != null)
            {
                List<(int, int)> availablePositions = GetAvailablePositionsToAddHex(white);
                if (availablePositions.Count > 0)
                {
                    List<GameObject> hexAddPropositions = CreateHexAddPositionsPropositions(availablePositions);
                    SetHexToAdd(hexToAdd, hexAddPropositions);
                    ResetHexToMove();

                    if (_hexesInfoProvider.IsItFirstMove() && hexAddPropositions.Count == 1)
                        ConfirmAddedHexOnGameboard(hexAddPropositions[0]);
                    return true;
                }
            }
        }
        return false;
    }

    private List<(int, int)> GetAvailablePositionsToAddHex(bool white)
    {
        List<GameObject> hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
        List<int> hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
        List<int> opponentHexesOnBoardIds = white ? _hexesStore.blackHexesOnBoardIds : _hexesStore.whiteHexesOnBoardIds;

        List<(int, int)> availablePositions;
        if (_hexesInfoProvider.FirstMovesWereMade())
        {
            availablePositions = PieceMovesTools.GetPositionsToAddHex(hexesOnBoardIds, _gameBoard.gameBoard);
        }
        else if (opponentHexesOnBoardIds.Count == 1)
        {
            List<GameObject> opponentHexes = white ? _hexesStore.blackHexes : _hexesStore.whiteHexes;
            var opponentHex = opponentHexes.FindLast(hex => hex.GetComponent<HexWrapperController>().HexId == opponentHexesOnBoardIds[0]);
            (int, int) opponentHexPosition = opponentHex.GetComponent<HexWrapperController>().positionOnBoard;
            availablePositions = PieceMovesTools.GetFreePositionsAroundPosition(opponentHexPosition, _gameBoard.gameBoard);
        }
        else
        {
            availablePositions = new List<(int, int)>();
            availablePositions.Add((GameBoardScript.CenterPositionX, GameBoardScript.CenterPositionY));
        }

        return availablePositions;
    }

    public bool PrepareSelectedHexToMove(GameObject selectedHex)
    {
        if (_moveValidator.CanMove(selectedHex))
        {
            var beetleScript = BeetlePiece.GetComponent<BeetlePieceController>();
            int hexId = selectedHex.GetComponent<HexWrapperController>().HexId;
            if (!beetleScript.IsHexUnderneathBeetle(hexId))
            {
                if (_hexesStore.hexToMove != null)
                    ResetHexToMove();

                if (_hexesInfoProvider.FirstMovesWereMade())
                {
                    var selectedHexScript = selectedHex.GetComponent<HexWrapperController>();
                    if (selectedHexScript.HexId != 0)
                    {
                        List<(int, int)> availableMovePositions = selectedHexScript
                            .piece
                            .GetComponent<IPieceController>()
                            .GetPieceSpecificPositions(selectedHexScript.positionOnBoard, _gameBoard.gameBoard);

                        if (availableMovePositions.Count > 0)
                        {
                            List<GameObject> hexMovePropositions = CreateHexMovePositionsPropositions(selectedHex, availableMovePositions);
                            SetHexToMove(selectedHex, hexMovePropositions);
                            ResetHexToAdd();
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private List<GameObject> CreateHexMovePositionsPropositions(GameObject hexToMove, List<(int, int)> positions)
    {
        List<GameObject> propositions = new List<GameObject>();
        (int, int) centerPosition = (GameBoardScript.CenterPositionX, GameBoardScript.CenterPositionY);

        positions.ForEach(position =>
        {
            Vector3 positionVector = PieceMovesTools.GetVectorFromStartToEnd(centerPosition, position);
            int hexOnPositionId = _gameBoard.gameBoard[position.Item1, position.Item2];
            if (hexOnPositionId != 0)
            {
                var beetleScript = BeetlePiece.GetComponent<BeetlePieceController>();
                int hexesOnPositionNumber = 1;

                int hexUnderHexOnPositionId = beetleScript.GetIdOfFirstHexUnderneathBeetle(hexOnPositionId);
                if (hexUnderHexOnPositionId != -1)
                    hexesOnPositionNumber += beetleScript.HexesUnderBeetleNumber(hexOnPositionId);

                Vector3 verticalVector = PieceMovesTools.GetVerticalVector(hexesOnPositionNumber);
                positionVector += verticalVector;
            }
            GameObject proposition = CreateProposition(position, positionVector);
            propositions.Add(proposition);
        });

        return propositions;
    }

    private GameObject CreateProposition((int, int) position, Vector3 positionVector)
    {
        GameObject proposition = Instantiate(HexPrefeab, positionVector, new Quaternion(0, 0, 0, 0));
        var propositionScript = proposition.GetComponent<HexWrapperController>();
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        var hexPropositionColor = propositionScript.hex.GetComponent<Renderer>().material.color;
        Color color = new Color(hexPropositionColor.r, hexPropositionColor.g, hexPropositionColor.b, 0.2f);
        propositionScript.hex.GetComponent<Renderer>().material.color = color;
        propositionScript.gameMeneger = gameManager;
        propositionScript.positionOnBoard = position;
        propositionScript.hex.GetComponent<HexController>().gameMeneger = gameManager;

        return proposition;
    }

    private List<GameObject> CreateHexAddPositionsPropositions(List<(int, int)> positions)
    {
        List<GameObject> propositions = new List<GameObject>();
        (int, int) centerPosition = (GameBoardScript.CenterPositionX, GameBoardScript.CenterPositionY);

        positions.ForEach(position =>
        {
            Vector3 positionVector = PieceMovesTools.GetVectorFromStartToEnd(centerPosition, position);
            GameObject proposition = CreateProposition(position, positionVector);
            propositions.Add(proposition);
        });

        return propositions;
    }
}
