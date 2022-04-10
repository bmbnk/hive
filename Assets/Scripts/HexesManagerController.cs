using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class HexesManagerController : MonoBehaviour
    {
        public GameObject HexPrefeab;
        public GameObject HexPropositionPrefeab;
        public GameObject BeetlePiece;
        private GameBoardScript _gameBoard;
        private HexesStoreScript _hexesStore;
        private HexesInfoProvider _hexesInfoProvider;

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
        }

        private HexWrapperController GetHexToMoveScript()
        {
            return _hexesStore.hexToMove.GetComponent<HexWrapperController>();
        }

        public bool ConfirmAddedHexOnGameboard(GameObject selectedHex)
        {
            HexPropositionWrapperController propositionHexScript = selectedHex.GetComponent<HexPropositionWrapperController>();
            if (propositionHexScript != null)
            {
                HexWrapperController hexToAddScript = GetHexThatIsAddedScript();

                //hexToAddScript.positionOnBoard = propositionHexScript.positionOnBoard;
                if (_gameBoard.AddElement(hexToAddScript.HexId, propositionHexScript.positionOnBoard))
                {
                    List<int> hexesOnBoardIds = hexToAddScript.isWhite ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
                    hexesOnBoardIds.Add(hexToAddScript.HexId);
                    hexToAddScript.transform.position = propositionHexScript.transform.position;
                    hexToAddScript.isOnGameboard = true;
                    _hexesStore.hexToAdd.SetActive(true);
                    //(int, int) addedHexPosition = hexToAddScript.positionOnBoard;
                    //_gameBoard.gameBoard[addedHexPosition.Item1, addedHexPosition.Item2] = hexToAddScript.HexId;

                    ResetHexToAdd();
                    return true;
                }


            }
            return false;
        }

        public void ResetHexToAdd()
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
            HexPropositionWrapperController propositionHexScript = selectedHex.GetComponent<HexPropositionWrapperController>();
            if (propositionHexScript != null)
            {
                HexWrapperController hexToMoveScript = GetHexToMoveScript();

                ResetHexToMove();

                //(int, int) currentHexToMovePosition = hexToMoveScript.positionOnBoard;
                (int, int) currentHexToMovePosition = _gameBoard.GetPositionByHexId(hexToMoveScript.HexId);

                if (hexToMoveScript.piece.GetComponent<IPieceController>().GetPieceType() == PieceType.BEETLE)
                {
                    var beetlePieceScript = BeetlePiece.GetComponent<BeetlePieceController>();

                    (int, int) startingPosition = _gameBoard.GetPositionByHexId(hexToMoveScript.HexId);
                    (int, int) targetPosition = propositionHexScript.positionOnBoard;
                    int targetPositionHexId = _gameBoard.GetHexIdByPosition(targetPosition);
                    _gameBoard.RemoveElement(targetPositionHexId);

                    if (_gameBoard.MoveElement(hexToMoveScript.HexId, propositionHexScript.positionOnBoard))
                    {
                        int hexUnderneathId = beetlePieceScript.GetIdOfFirstHexUnderneathBeetle(hexToMoveScript.HexId);
                        //_gameBoard.gameBoard[currentHexToMovePosition.Item1, currentHexToMovePosition.Item2] = hexUnderneathId != -1 ? hexUnderneathId : 0;
                        _gameBoard.AddElement(hexUnderneathId != -1 ? hexUnderneathId : 0, startingPosition);

                        beetlePieceScript.RemoveHexUnderneathBeetle(hexToMoveScript.HexId);
                        if (targetPositionHexId != 0)
                            beetlePieceScript.SetHexUnderneathBeetle(hexToMoveScript.HexId, targetPositionHexId);
                    }
                    else
                    {
                        _gameBoard.AddElement(targetPositionHexId, targetPosition);
                        return false;
                    }
                }
                else
                {
                    //_gameBoard.gameBoard[currentHexToMovePosition.Item1, currentHexToMovePosition. Item2] = 0;
                    //hexToMoveScript.positionOnBoard = propositionHexScript.positionOnBoard;
                    //(int, int) movedHexPosition = hexToMoveScript.positionOnBoard;
                    //_gameBoard.gameBoard[movedHexPosition.Item1, movedHexPosition.Item2] = hexToMoveScript.HexId;
                    _gameBoard.MoveElement(hexToMoveScript.HexId, propositionHexScript.positionOnBoard);
                }

                hexToMoveScript.transform.position = propositionHexScript.transform.position;
                return true;
            }
            return false;
        }

        public void ResetHexToMove()
        {
            if (_hexesStore.hexPropositionsToMove != null)
            {
                for (int i = _hexesStore.hexPropositionsToMove.Count - 1; i >= 0; i--)
                {
                    Destroy(_hexesStore.hexPropositionsToMove[i]);
                }
            }
            DehighlightHexToMove();
            SetHexToMove(null, null);
        }

        private void DehighlightHexToMove()
        {
            if (_hexesStore.hexToMove != null)
                _hexesStore.hexToMove.GetComponent<HexWrapperController>().transform.position -= PieceMovesTools.GetHexSelectionVector();
        }

        private void SetHexToMove(GameObject selectedHex, List<GameObject> hexPropositionsToMove)
        {
            _hexesStore.hexToMove = selectedHex;
            _hexesStore.hexPropositionsToMove = hexPropositionsToMove;
            HighlightHexToMove();
        }

        private void HighlightHexToMove()
        {
            if (_hexesStore.hexToMove != null)
                _hexesStore.hexToMove.GetComponent<HexWrapperController>().transform.position += PieceMovesTools.GetHexSelectionVector();
        }

        private HexWrapperController GetHexThatIsAddedScript()
        {
            return _hexesStore.hexToAdd.GetComponent<HexWrapperController>();
        }

        public bool PrepareHexToAddToBoard(PieceType type, bool white)
        {
            ResetHexToAdd();
            ResetHexToMove();

            GameObject hexToAdd = ProposeHexToAdd(type, white);

            if (hexToAdd != null)
            {
                List<(int, int)> availablePositions = GetAvailablePositionsToAddHex(white);
                if (availablePositions.Count > 0)
                {
                    List<GameObject> hexAddPropositions = CreateHexAddPositionsPropositions(availablePositions);
                    SetHexToAdd(hexToAdd, hexAddPropositions);

                    if (_hexesInfoProvider.IsItFirstMove())
                        ConfirmAddedHexOnGameboard(hexAddPropositions[0]);
                    return true;
                }
            }
            return false;
        }

        private GameObject ProposeHexToAdd(PieceType type, bool white)
        {
            GameObject hexToAddProposition = null;

            var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
            List<int> hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

            foreach (var hex in hexes)
            {
                HexWrapperController hexScript = hex.GetComponent<HexWrapperController>();
                if (hexScript.piece.GetComponent<IPieceController>().GetPieceType() == type
                    && !hexesOnBoardIds.Contains(hexScript.HexId))
                {
                    hexToAddProposition = hex;
                    break;
                }
            }
            return hexToAddProposition;
        }

        private List<(int, int)> GetAvailablePositionsToAddHex(bool white)
        {
            List<GameObject> hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
            List<int> hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
            List<int> opponentHexesOnBoardIds = white ? _hexesStore.blackHexesOnBoardIds : _hexesStore.whiteHexesOnBoardIds;

            List<(int, int)> availablePositions;
            if (_hexesInfoProvider.FirstMovesWereMade())
            {
                availablePositions = PieceMovesTools.GetPositionsToAddHex(hexesOnBoardIds, _gameBoard.GetGameBoard());
            }
            else if (opponentHexesOnBoardIds.Count == 1)
            {
                List<GameObject> opponentHexes = white ? _hexesStore.blackHexes : _hexesStore.whiteHexes;
                var opponentHex = opponentHexes.FindLast(hex => hex.GetComponent<HexWrapperController>().HexId == opponentHexesOnBoardIds[0]);
                (int, int) opponentHexPosition = _gameBoard.GetPositionByHexId(opponentHex.GetComponent<HexWrapperController>().HexId);
                availablePositions = PieceMovesTools.GetFreePositionsAroundPosition(opponentHexPosition, _gameBoard.GetGameBoard());
            }
            else
            {
                availablePositions = new List<(int, int)>();
                availablePositions.Add((GameBoardScript.CenterPositionY, GameBoardScript.CenterPositionX));
            }

            return availablePositions;
        }

        public bool PrepareSelectedHexToMove(GameObject selectedHex)
        {
            var selectedHexScript = selectedHex.GetComponent<HexWrapperController>();

            if (selectedHexScript.HexId != 0)
            {
                ResetHexToMove();
                ResetHexToAdd();

                List<(int, int)> availableMovePositions = selectedHexScript
                    .piece
                    .GetComponent<IPieceController>()
                    .GetPieceSpecificPositions(_gameBoard.GetPositionByHexId(selectedHexScript.HexId), _gameBoard.GetGameBoard());

                if (availableMovePositions.Count > 0)
                {
                    List<GameObject> hexMovePropositions = CreateHexMovePositionsPropositions(availableMovePositions);
                    SetHexToMove(selectedHex, hexMovePropositions);
                    return true;
                }
            }
            return false;
        }

        private List<GameObject> CreateHexMovePositionsPropositions(List<(int, int)> positions)
        {
            List<GameObject> propositions = new List<GameObject>();
            //(int, int) centerPosition = (GameBoardScript.CenterPositionX, GameBoardScript.CenterPositionY);
            (int, int) referencePosition = _gameBoard.GetFirstHexOnBoardPosition();

            positions.ForEach(position =>
            {
                Vector3 positionVector = PieceMovesTools.GetVectorFromStartToEnd(referencePosition, position);
                //int hexOnPositionId = _gameBoard.gameBoard[position.Item1, position.Item2];
                int hexOnPositionId = _gameBoard.GetHexIdByPosition(position);
                if (hexOnPositionId != 0)
                {
                    var beetleScript = BeetlePiece.GetComponent<BeetlePieceController>();
                    int hexesOnPositionNumber = 1 + beetleScript.HexesUnderBeetleNumber(hexOnPositionId);

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
            GameObject proposition = Instantiate(HexPropositionPrefeab, positionVector, new Quaternion(0, 0, 0, 0));
            var propositionScript = proposition.GetComponent<HexPropositionWrapperController>();
            GameObject gameManager = GameObject.FindWithTag("GameManager");
            var hexPropositionColor = propositionScript.hex.GetComponent<Renderer>().material.color;
            Color color = new Color(hexPropositionColor.r, hexPropositionColor.g, hexPropositionColor.b, 0.2f);
            propositionScript.hex.GetComponent<Renderer>().material.color = color;
            propositionScript.gameManager = gameManager;
            propositionScript.positionOnBoard = position;
            propositionScript.hex.GetComponent<HexController>().gameManager = gameManager;

            return proposition;
        }

        private List<GameObject> CreateHexAddPositionsPropositions(List<(int, int)> positions)
        {
            List<GameObject> propositions = new List<GameObject>();
            //(int, int) centerPosition = (GameBoardScript.CenterPositionX, GameBoardScript.CenterPositionY);
            (int, int) referencePosition = _gameBoard.GetFirstHexOnBoardPosition();

            positions.ForEach(position =>
            {
                Vector3 positionVector = PieceMovesTools.GetVectorFromStartToEnd(referencePosition, position);
                GameObject proposition = CreateProposition(position, positionVector);
                propositions.Add(proposition);
            });

            return propositions;
        }

        public void ResetHexesState()
        {
            ResetHexToAdd();
            ResetHexToMove();
            ResetHexes();
            _hexesStore.blackHexesOnBoardIds.Clear();
            _hexesStore.whiteHexesOnBoardIds.Clear();
            _gameBoard.ClearGameBoard();
        }


        private void ResetHexes()
        {
            _hexesStore.whiteHexes.ForEach(hex =>
            {
                var hexScript = hex.GetComponent<HexWrapperController>();
                hexScript.isOnGameboard = false;
                //hexScript.positionOnBoard = (-1, -1);
                hex.SetActive(false);
            });

            _hexesStore.blackHexes.ForEach(hex =>
            {
                var hexScript = hex.GetComponent<HexWrapperController>();
                hexScript.isOnGameboard = false;
                //hexScript.positionOnBoard = (-1, -1);
                hex.SetActive(false);
            });
        }
    }
}
