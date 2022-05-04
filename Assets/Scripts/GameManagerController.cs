using UnityEngine;

namespace Hive
{
    public class GameManagerController : MonoBehaviour
    {
        private CameraController _camera;
        private GameEngineScript _gameEngine;
        private HexesInfoProvider _hexesInfoProvider;
        private HexesManagerController _hexesManager;
        private RulesValidator _rulesValidator;
        private UIController _ui;

        private bool _gamePaused = false;
        private bool _isPlayer1White;

        private IPlayer _player1;
        private IPlayer _player2;

        //private bool _startedApp = false;

        void Start()
        {
            GameObject gameEngineGameObject = GameObject.FindWithTag("GameEngine");
            _gameEngine = gameEngineGameObject.GetComponent<GameEngineScript>();

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
            //if (!_startedApp)
            //{
            //    StartGame(false);
            //    _startedApp = true;
            //}
            if (Input.GetKey("escape")
                && (_gameEngine.GameState == GameState.InProgress
                || _gameEngine.GameState == GameState.NotStarted))
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            _ui.LaunchPauseMenu();
            _gamePaused = true;
        }

        public void ConfirmMove(PieceType pieceType, bool white, bool addingPiece)
        {
            if (_rulesValidator.IsGameOver())
                GameOver();
            if (addingPiece)
                UpdateSideMenu(pieceType, white);
            if (!_hexesInfoProvider.IsAnyHexLeftInHand())
            {
                _ui.HideSideMenus();
            }
            _camera.UpdateCamera();
            _ui.ChangeSideMenu(_gameEngine.IsWhiteTurn);
            GetCurrentPlayer().RequestMove();
        }

        public void OnTileSelected(PieceType type, bool white)
        {
            IPlayer currentPlayer = GetCurrentPlayer();
            if (_gameEngine.IsWhiteTurn == white && currentPlayer.GetType() == typeof(HumanPlayer))
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
            if (_rulesValidator.GameIsDrawn())
                _ui.LaunchGameDrawnEndingPanel();
            else
                _ui.LaunchWinEndingPanel(_rulesValidator.WhiteHexesWon());
        }

        public void OnHexSelected(GameObject selectedHex)
        {
            IPlayer currentPlayer = GetCurrentPlayer();
            var gameState = _gameEngine.GameState;
            if (currentPlayer.GetType() == typeof(HumanPlayer)
                && (gameState == GameState.InProgress || gameState == GameState.NotStarted)
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

            _gameEngine.Reset();
            _gameEngine.SetWhiteStarts(_player1.IsWhite());

            _ui.HideColorChoiceMenu();
            _ui.HidePlayModeMenu();
            _ui.LaunchSideMenus();
            _ui.ChangeSideMenu(_gameEngine.IsWhiteTurn);

            _player1.RequestMove();
        }

        public void ResetGame()
        {
            _gamePaused = false;
            _hexesManager.ResetHexesState();
            _ui.ResetUI();
            _ui.LaunchStartMenu();
            _camera.ResetCamera();
            _gameEngine.Reset();
        }

        private IPlayer GetCurrentPlayer()
        {
            return _gameEngine.IsWhiteTurn == _isPlayer1White ? _player1 : _player2;
        }
    }
}