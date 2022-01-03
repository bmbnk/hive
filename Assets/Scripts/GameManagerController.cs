using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    private bool _addingHexToBoard = false;
    private bool _movingHexOnBoard = false;
    private bool _isWhiteTurn = false;
    private bool _isHexSelected = false;

    private HexesManagerController _hexesMeneger;

    void Start()
    {
        GameObject hexesMenegerGameobject = GameObject.FindWithTag("HexesManager");
        _hexesMeneger = hexesMenegerGameobject.GetComponent<HexesManagerController>();
    }

    void Update()
    {   
        if (_addingHexToBoard)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _hexesMeneger.ProposeNextAddingPosition();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ConfirmAddedHexOnGameboard();
            }
        } else if (_movingHexOnBoard)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _hexesMeneger.ProposeNextMovePosition(); ;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                ConfirmMovingHexOnGameboard();
            }
        } else if (_isHexSelected)
        {
            if (_hexesMeneger.CanMakeActionFromSelectedHex(_isWhiteTurn))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (_hexesMeneger.CanSelectedHexMove(_isWhiteTurn) && _hexesMeneger.StartMovingSelectedHex())
                        _movingHexOnBoard = true;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    _addingHexToBoard = _hexesMeneger.StartAddingHexToGameboard(_isWhiteTurn);
            }
        }
    }

    private void ConfirmMovingHexOnGameboard()
    {
        _hexesMeneger.ConfirmMovingHexOnGameboard();
        _movingHexOnBoard = false;
        _isWhiteTurn = !_isWhiteTurn;
        _isHexSelected = false;
    }

    private void ConfirmAddedHexOnGameboard()
    {
        _hexesMeneger.ConfirmAddedHexOnGameboard();
        _addingHexToBoard = false;
        _isWhiteTurn = !_isWhiteTurn;
        _isHexSelected = false;
    }

    public void HexSelected(GameObject selectedHex)
    {
        if (!_movingHexOnBoard && !_addingHexToBoard)
        {
            _hexesMeneger.PrepareSelectedHex(selectedHex);
            _isHexSelected = true;
        }
    }
}