using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    private HexesManagerController _hexesMeneger;
    private UIController _ui;
    private HexesInfoProvider _hexesInfoProvider;
    private CameraController _camera;

    private bool _addingHexToBoard = false;
    private bool _gameOver = false;
    private bool _gamePaused = false;
    private bool _isWhiteTurn = true;
    private bool _movingHexOnBoard = false;
    private PieceType _lastSelectedTileType;

    void Start()
    {
        GameObject hexesMenegerGameobject = GameObject.FindWithTag("HexesManager");
        _hexesMeneger = hexesMenegerGameobject.GetComponent<HexesManagerController>();

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

    public void ResumeGame()
    {
        _ui.HidePauseMenu();
        _gamePaused = false;
    }

    public void TileSelected(PieceType type, bool white)
    {
        if (white == _isWhiteTurn)
        {
            _lastSelectedTileType = type;
            StartAddingHex(type, white);
        }
    }

    private void StartAddingHex(PieceType type, bool white)
    {
        bool isItFirstMove = _hexesInfoProvider.IsItFirstMove();

        if (_hexesMeneger.PrepareHexToAddToBoard(type, white))
        {
            _movingHexOnBoard = false;
            _addingHexToBoard = true;
            if (isItFirstMove)
            {
                ChangeTurn();
                _addingHexToBoard = false;
                UpdateTileCounterLabel(type, white);
                _camera.UpdateCamera();
            }
        }
    }

    private void UpdateTileCounterLabel(PieceType type, bool white)
    {
        int count = _hexesInfoProvider.GetRemainingHexCount(type, white);
        _ui.UpdateTileCounterLabel(type, white, count);
    }

    public void HexSelected(GameObject selectedHex)
    {
        if (!_gameOver && !_gamePaused)
        {
            if (_hexesInfoProvider.IsItPropositionHex(selectedHex))
            {
                if (_addingHexToBoard)
                {
                    ConfirmAddedHexOnGameboard(selectedHex);
                } else if (_movingHexOnBoard)
                {
                    ConfirmMovingHexOnGameboard(selectedHex);
                }
            } else if (_hexesInfoProvider.IsItCurrentPlayerHex(selectedHex, _isWhiteTurn))
            {
                StartMovingHex(selectedHex);
            }
        }
    }

    private void ConfirmAddedHexOnGameboard(GameObject selectedHex)
    {
        if (_hexesMeneger.ConfirmAddedHexOnGameboard(selectedHex))
        {
            if (_hexesInfoProvider.IsGameOver())
                GameOver();
            UpdateTileCounterLabel(_lastSelectedTileType, _isWhiteTurn);
            if (!_hexesInfoProvider.IsAnyHexLeftInHand())
            {
                _ui.HideSideMenus();
            }
            _addingHexToBoard = false;
            ChangeTurn();
            _camera.UpdateCamera();
        }
    }

    private void GameOver()
    {
        if (_hexesInfoProvider.GameIsDrawn())
            _ui.ShowGameDrawnEndingPanel();
        else
            _ui.ShowWinEndingPanel(_hexesInfoProvider.WhiteHexesWon());
    }


    private void ConfirmMovingHexOnGameboard(GameObject selectedHex)
    {
        if (_hexesMeneger.ConfirmMovingHexOnGameboard(selectedHex))
        {
            if (_hexesInfoProvider.IsGameOver())
                GameOver();
            _movingHexOnBoard = false;
            ChangeTurn();
            _camera.UpdateCamera();
        }
    }

    public void StartGame(bool white)
    {
        _gameOver = false;
        SetTurn(white);
        _ui.HideChoiceMenu();
        _ui.ChangeSideMenu(_isWhiteTurn);
    }

    public void PrepareGame()
    {
        _ui.LaunchChoiceMenu();
    }

    public void ResetGame()
    {
        _gamePaused = false;
        _gameOver = false;
        _movingHexOnBoard = false;
        _addingHexToBoard = false;
        _hexesMeneger.ResetHexesState();
        _ui.ResetUI();
        _ui.LaunchStartMenu();
    }

    private void StartMovingHex(GameObject selectedHex)
    {
        if (_hexesMeneger.PrepareSelectedHexToMove(selectedHex))
        {
            _movingHexOnBoard = true;
            _addingHexToBoard = false;
        }
    }

    public void SetTurn(bool white)
    {
        _isWhiteTurn = white;
    }

    private void ChangeTurn()
    {
        _isWhiteTurn = !_isWhiteTurn;
        _ui.ChangeSideMenu(_isWhiteTurn);
    }
}