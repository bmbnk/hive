using UnityEngine;

namespace Hive
{
    public class GameManagerController : MonoBehaviour
    {
        private HexesManagerController _hexesManager;
        private UIController _ui;
        private HexesInfoProvider _hexesInfoProvider;
        private CameraController _camera;
        private RulesValidator _rulesValidator;

        private bool _gameOver = false;
        private bool _gamePaused = false;
        private bool _isWhiteTurn = true;
        private bool _movingHexOnBoard = false;
        private bool _addingHexToBoard = false;
        private bool _isPlayer1White;
        private PieceType _lastSelectedTileType;

        private IPlayer _player1;
        private IPlayer _player2;

        void Start()
        {
            GameObject moveValidatorGameObject = GameObject.FindWithTag("RulesValidator");
            _rulesValidator = moveValidatorGameObject.GetComponent<RulesValidator>();

            GameObject hexesManagerGameobject = GameObject.FindWithTag("HexesManager");
            _hexesManager = hexesManagerGameobject.GetComponent<HexesManagerController>();

            GameObject uiGameobject = GameObject.FindWithTag("UI");
            _ui = uiGameobject.GetComponent<UIController>();

            GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
            _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();

            GameObject cameraGameobject = GameObject.FindWithTag("MainCamera");
            _camera = cameraGameobject.GetComponent<CameraController>();
        }

        void Update()
        {
            if (Input.GetKey("escape") && !_gameOver)
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            _ui.LaunchPauseMenu();
            _gamePaused = true;
        }

        public void ConfirmMove(PieceType pieceType, bool addingPiece)
        {
            if (_hexesInfoProvider.IsGameOver())
                GameOver();
            if (addingPiece)
                UpdateSideMenu(pieceType, _isWhiteTurn);
            if (!_hexesInfoProvider.IsAnyHexLeftInHand())
            {
                _ui.HideSideMenus();
            }
            _camera.UpdateCamera();
            ChangeTurn();
        }

        public void OnTileSelected(PieceType type, bool white)
        {
            IPlayer currentPlayer = GetCurrentPlayer();
            if (_isWhiteTurn == white && currentPlayer.GetType() == typeof(HumanPlayer))
                ((HumanPlayer)currentPlayer).OnTileSelected(type, white);
        }

        public void ResumeGame()
        {
            _ui.HidePauseMenu();
            _gamePaused = false;
        }

        private void UpdateSideMenu(PieceType type, bool white)
        {
            int count = _hexesInfoProvider.GetRemainingHexCount(type, white);
            _ui.UpdateCounterLabel(type, white, count);
        }

        public void GameBoardSelected()
        {
            IPlayer currentPlayer = GetCurrentPlayer();
            if (currentPlayer.GetType() == typeof(HumanPlayer))
                ((HumanPlayer)currentPlayer).GameBoardSelected();
        }

        private void GameOver()
        {
            if (_hexesInfoProvider.GameIsDrawn())
                _ui.LaunchGameDrawnEndingPanel();
            else
                _ui.LaunchWinEndingPanel(_hexesInfoProvider.WhiteHexesWon());
        }

        public void OnHexSelected(GameObject selectedHex)
        {
            IPlayer currentPlayer = GetCurrentPlayer();
            if (currentPlayer.GetType() == typeof(HumanPlayer)
                && !_gameOver
                && !_gamePaused)
                ((HumanPlayer)currentPlayer).OnHexSelected(selectedHex);
        }



        public void StartGame(bool againstComputer)
        {
            _isPlayer1White = Random.Range(0f, 1f) > 0.5;
            if (againstComputer)
            {
                _player1 = Random.Range(0f, 1f) > 0.5 ? new HumanPlayer(_isPlayer1White) : new AIPlayer(_isPlayer1White);
                _player2 = _player1.GetType() == typeof(HumanPlayer) ? new AIPlayer(!_isPlayer1White) : new HumanPlayer(!_isPlayer1White);
            } else
            {
                _player1 = new HumanPlayer(_isPlayer1White);
                _player2 = new HumanPlayer(!_isPlayer1White);
            }

            _gameOver = false;
            SetTurn(_player1.IsWhite());
            _ui.HideColorChoiceMenu();
            _ui.HidePlayModeMenu();
            _ui.LaunchSideMenus();
            _ui.ChangeSideMenu(_isWhiteTurn);

            _player1.RequestMove();
        }

        public void ResetGame()
        {
            _gamePaused = false;
            _gameOver = false;
            _movingHexOnBoard = false;
            _addingHexToBoard = false;
            _hexesManager.ResetHexesState();
            _ui.ResetUI();
            _ui.LaunchStartMenu();
            _camera.ResetCamera();
        }

        public void SetTurn(bool white)
        {
            _isWhiteTurn = white;
        }

        private IPlayer GetCurrentPlayer()
        {
            return _isWhiteTurn == _isPlayer1White ? _player1 : _player2;
        }

        private void ChangeTurn()
        {
            if (_rulesValidator.CanMakeMove(!_isWhiteTurn))
            {
                _isWhiteTurn = !_isWhiteTurn;
                _ui.ChangeSideMenu(_isWhiteTurn);
                GetCurrentPlayer().RequestMove();
            }
        }
    }
}