using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    private HexesManagerController _hexesMeneger;
    private HexesInfoProvider _hexesInfoProvider;
    private UIController _ui;

    private bool _addingHexToBoard = false;
    private bool _gameOver = false;
    private bool _isWhiteTurn = true;
    private bool _movingHexOnBoard = false;
    private PieceType _lastSelectedTileType;


    void Start()
    {
        GameObject hexesMenegerGameobject = GameObject.FindWithTag("HexesManager");
        _hexesMeneger = hexesMenegerGameobject.GetComponent<HexesManagerController>();

        GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
        _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();

        GameObject uiGameobject = GameObject.FindWithTag("UI");
        _ui = uiGameobject.GetComponent<UIController>();

        _ui.GreyOutPlayerMenu(!_isWhiteTurn);
        _ui.DisableButtons(!_isWhiteTurn);

        _ui.UnGreyPlayerMenu(_isWhiteTurn);
        _ui.EnableButtons(_isWhiteTurn);
    }

    void Update()
    {
        if (_gameOver)
        {
            if (_hexesInfoProvider.GameIsDrawn())
                _ui.ShowGameDrawnEndingPanel();
            else
                _ui.ShowWinEndingPanel(_hexesInfoProvider.WhiteHexesWon());
        }     
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
            }
        }
    }

    private void UpdateTileCounterLabel(PieceType type, bool white)
    {
        int count = _hexesInfoProvider.GetRemainingHexCount(type, white);
        _ui.UpdateLabel(type, white, count);
    }

    public void HexSelected(GameObject selectedHex)
    {
        if (!_gameOver)
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
            _gameOver = _hexesInfoProvider.IsGameOver();
            UpdateTileCounterLabel(_lastSelectedTileType, _isWhiteTurn);
            _addingHexToBoard = false;
            ChangeTurn();
        }
    }

    private void ConfirmMovingHexOnGameboard(GameObject selectedHex)
    {
        if (_hexesMeneger.ConfirmMovingHexOnGameboard(selectedHex))
        {
            _gameOver = _hexesInfoProvider.IsGameOver();
            _movingHexOnBoard = false;
            ChangeTurn();
        }
    }

    private void StartMovingHex(GameObject selectedHex)
    {
        if (_hexesMeneger.PrepareSelectedHexToMove(selectedHex))
        {
            _movingHexOnBoard = true;
            _addingHexToBoard = false;
        }
    }

    private void ChangeTurn()
    {
        _isWhiteTurn = !_isWhiteTurn;

        _ui.UnGreyPlayerMenu(_isWhiteTurn);
        _ui.EnableButtons(_isWhiteTurn);

        _ui.GreyOutPlayerMenu(!_isWhiteTurn);
        _ui.DisableButtons(!_isWhiteTurn);
    }
}