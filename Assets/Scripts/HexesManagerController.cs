using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class HexesManagerController : MonoBehaviour
    {
        public GameObject HexPrefeab;
        public GameObject HexPropositionPrefeab;
        public GameObject BeetlePiece;

        private HexesStoreScript _hexesStore;
        private HexesInfoProvider _hexesInfoProvider;
        private GameEngineScript _gameEngine;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
            _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

            GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
            _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();

            GameObject gameEngineGameObject = GameObject.FindWithTag("GameEngine");
            _gameEngine = gameEngineGameObject.GetComponent<GameEngineScript>();
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

                if (_gameEngine.MakeMove(hexToAddScript.HexId, propositionHexScript.positionOnBoard))
                {
                    hexToAddScript.transform.position = propositionHexScript.transform.position;
                    _hexesStore.hexToAdd.SetActive(true);

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

                if (_gameEngine.MakeMove(hexToMoveScript.HexId, propositionHexScript.positionOnBoard))
                {
                    ResetHexToMove();
                    hexToMoveScript.transform.position = propositionHexScript.transform.position;
                    return true;
                }
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
                List<(int, int, int)> availablePositions = _gameEngine.GetAvailableAddingPositions(white);
                if (availablePositions.Count > 0)
                {
                    Vector3 referenceHexLocation = new Vector3();
                    var referenceHexPosition = (GameBoardScript.CenterPositionY, GameBoardScript.CenterPositionX, 0);

                    if (!_hexesInfoProvider.IsItFirstMove())
                    {
                        var opponentHexOnBoardIds = white ? _gameEngine.BlackHexesOnBoardIds
                            : _gameEngine.WhiteHexesOnBoardIds;

                        var opponentHexes = white ? _hexesStore.blackHexes
                            : _hexesStore.whiteHexes;

                        foreach (var hex in opponentHexes)
                        {
                            var hexScript = hex.GetComponent<HexWrapperController>();
                            if (opponentHexOnBoardIds.Contains(hexScript.HexId))
                            {
                                referenceHexLocation = hexScript.transform.position;
                                referenceHexPosition = _gameEngine.GetPositionByHexId(hexScript.HexId);
                                break;
                            }
                        }
                    }

                    List<GameObject> hexAddPropositions = CreateHexPropositions(
                        referenceHexPosition,
                        referenceHexLocation,
                        availablePositions);

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
            var hexesOnBoardIds = white ? _gameEngine.WhiteHexesOnBoardIds : _gameEngine.BlackHexesOnBoardIds;

            foreach (var hex in hexes)
            {
                HexWrapperController hexScript = hex.GetComponent<HexWrapperController>();
                if (_gameEngine.GetPieceType(hexScript.HexId) == type
                    && !hexesOnBoardIds.Contains(hexScript.HexId))
                {
                    hexToAddProposition = hex;
                    break;
                }
            }
            return hexToAddProposition;
        }

        public bool PrepareSelectedHexToMove(GameObject selectedHex)
        {
            var selectedHexScript = selectedHex.GetComponent<HexWrapperController>();

            if (selectedHexScript.HexId != 0)
            {
                ResetHexToMove();
                ResetHexToAdd();

                List<(int, int, int)> availableMovePositions = _gameEngine.GetAvailableMovePositionsForHex(selectedHexScript.HexId);

                if (availableMovePositions.Count > 0)
                {
                    List<GameObject> hexMovePropositions = CreateHexPropositions(
                        _gameEngine.GetPositionByHexId(selectedHexScript.HexId),
                        selectedHexScript.transform.position,
                        availableMovePositions);

                    SetHexToMove(selectedHex, hexMovePropositions);
                    return true;
                }
            }
            return false;
        }

        private List<GameObject> CreateHexPropositions((int, int, int) referenceHexPosition, Vector3 referenceHexLocation, List<(int, int, int)> positions)
        {
            List<GameObject> propositions = new List<GameObject>();

            positions.ForEach(position =>
            {
                Vector3 deltaVector = PieceMovesTools.GetVectorFromStartToEnd(referenceHexPosition, position);
                Vector3 positionVector = referenceHexLocation + deltaVector;
                GameObject proposition = CreateProposition(position, positionVector);
                propositions.Add(proposition);
            });

            return propositions;
        }

        private GameObject CreateProposition((int, int, int) position, Vector3 positionVector)
        {
            GameObject proposition = Instantiate(HexPropositionPrefeab, positionVector, new Quaternion(0, 0, 0, 0));
            var propositionScript = proposition.GetComponent<HexPropositionWrapperController>();
            var hexPropositionColor = propositionScript.Hex.GetComponent<Renderer>().material.color;
            Color color = new Color(hexPropositionColor.r, hexPropositionColor.g, hexPropositionColor.b, 0.2f);
            propositionScript.Hex.GetComponent<Renderer>().material.color = color;
            propositionScript.positionOnBoard = position;

            return proposition;
        }

        public void ResetHexesState()
        {
            ResetHexToAdd();
            ResetHexToMove();
            ResetHexes();
            _gameEngine.Reset();
        }

        private void ResetHexes()
        {
            _hexesStore.whiteHexes.ForEach(hex => hex.SetActive(false));
            _hexesStore.blackHexes.ForEach(hex => hex.SetActive(false));
        }
    }
}
