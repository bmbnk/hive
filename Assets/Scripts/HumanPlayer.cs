using UnityEngine;

namespace Hive
{
    public class HumanPlayer : IPlayer
    {
        private bool _isWhite;
        private bool _myTurn = false;
        public bool IsWhite() => _isWhite;

        private HexesManagerController _hexesManager;
        private HexesInfoProvider _hexesInfoProvider;
        private GameManagerController _gameManager;

        private bool _addingHexToBoard = false;
        private bool _movingHexOnBoard = false;

        private PieceType _lastSelectedTileType;


        public HumanPlayer(bool isWhite)
        {
            _isWhite = isWhite;

            GameObject gameManagerGameObject = GameObject.FindWithTag("GameManager");
            _gameManager = gameManagerGameObject.GetComponent<GameManagerController>();

            GameObject hexesManagerGameobject = GameObject.FindWithTag("HexesManager");
            _hexesManager = hexesManagerGameobject.GetComponent<HexesManagerController>();

            GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
            _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();
        }

        public void RequestMove()
        {
            _myTurn = true;
        }

        public void OnTileSelected(PieceType type, bool white)
        {
            if (_myTurn && white == _isWhite)
            {
                _lastSelectedTileType = type;
                StartAddingHex(type);
            }
        }

        private void StartAddingHex(PieceType type)
        {
            bool isItFirstMove = _hexesInfoProvider.IsItFirstMove();

            if (_hexesManager.PrepareHexToAddToBoard(type, _isWhite))
            {
                _movingHexOnBoard = false;
                _addingHexToBoard = true;
                if (isItFirstMove)
                {
                    _addingHexToBoard = false;
                    _gameManager.ConfirmMove(type, _isWhite, true);
                    _myTurn = false;
                }
            }
            else
            {
                _hexesManager.ResetHexToAdd();
            }
        }

        public void GameBoardSelected()
        {
            if (_myTurn)
            {
                if (_addingHexToBoard)
                    _hexesManager.ResetHexToAdd();
                if (_movingHexOnBoard)
                    _hexesManager.ResetHexToMove();
            }
        }

        public void OnHexSelected(GameObject selectedHex)
        {
            if (_myTurn)
            {
                if (_hexesInfoProvider.IsItPropositionHex(selectedHex))
                {
                    if (_addingHexToBoard)
                    {
                        ConfirmAddedHexOnGameboard(selectedHex);
                    }
                    else if (_movingHexOnBoard)
                    {
                        ConfirmMovingHexOnGameboard(selectedHex);
                    }
                }
                else if (_hexesInfoProvider.IsItCurrentPlayerHex(selectedHex, _isWhite))
                {
                    StartMovingHex(selectedHex);
                }
            }
        }

        private void ConfirmAddedHexOnGameboard(GameObject selectedHex)
        {
            if (_hexesManager.ConfirmAddedHexOnGameboard(selectedHex))
            {
                _addingHexToBoard = false;
                _myTurn = false;
                _gameManager.ConfirmMove(_lastSelectedTileType, _isWhite, true);
            }
        }

        private void StartMovingHex(GameObject selectedHex)
        {
            if (_hexesManager.PrepareSelectedHexToMove(selectedHex))
            {
                _movingHexOnBoard = true;
                _addingHexToBoard = false;
            }
            else
            {
                _hexesManager.ResetHexToMove();
            }
        }

        private void ConfirmMovingHexOnGameboard(GameObject selectedHex)
        {
            if (_hexesManager.ConfirmMovingHexOnGameboard(selectedHex))
            {
                _movingHexOnBoard = false;
                _myTurn = false;
                _gameManager.ConfirmMove(_lastSelectedTileType, _isWhite, false);
            }
        }
    }
}
