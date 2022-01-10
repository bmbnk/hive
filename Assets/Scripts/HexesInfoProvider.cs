using System.Collections.Generic;
using UnityEngine;

public class HexesInfoProvider : MonoBehaviour
{
    private GameBoardScript _gameBoard;
    private HexesStoreScript _hexesStore;

    private void Start()
    {
        GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
        _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();
    }

    public bool CanMakeActionFromSelectedHex(bool isWhiteTurn)
    {
        return _hexesStore.hexToMove.GetComponent<HexWrapperController>().isWhite == isWhiteTurn || !FirstMovesWereMade();
    }

    public bool FirstMovesWereMade()
    {
        return _hexesStore.blackHexesOnBoardIds.Count > 0 && _hexesStore.whiteHexesOnBoardIds.Count > 0;
    }

    public bool CanSelectedHexMove(bool isWhiteTurn)
    {
        return CanMakeActionFromSelectedHex(isWhiteTurn) || _hexesStore.blackHexesOnBoardIds.Count > 0;
    }

    public int GetRemainingHexCount(PieceType pieceType, bool white)
    {
        var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
        var hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
        int counter = 0;

        hexes.ForEach(hex =>
        {
            var hexScript = hex.GetComponent<HexWrapperController>();
            if (hexScript.piece.GetComponent<IPieceController>().GetPieceType() == pieceType && !hexesOnBoardIds.Contains(hexScript.HexId))
                counter++;
        });

        return counter;
    }

    public bool IsItCurrentPlayerHex(GameObject selectedHex, bool isWhiteTurn)
    {
        return !IsItPropositionHex(selectedHex) && selectedHex.GetComponent<HexWrapperController>().isWhite == isWhiteTurn;
    }

    public bool IsItFirstMove()
    {
        return _hexesStore.blackHexesOnBoardIds.Count == 0 && _hexesStore.whiteHexesOnBoardIds.Count == 0;
    }

    public bool IsItPropositionHex(GameObject selectedHex)
    {
        return selectedHex.GetComponent<HexWrapperController>().HexId == 0;
    }

    public bool IsGameOver()
    {
        return BlackHexesWon() || WhiteHexesWon();
    }

    public bool BlackHexesWon()
    {
        return IsBeeFullySurrounded(true);
    }

    public bool WhiteHexesWon()
    {
        return IsBeeFullySurrounded(false);
    }

    private bool IsBeeFullySurrounded(bool whiteBee)
    {
        if (IsBeeOnBoard(whiteBee))
        {
            int beeHexId = GetFirstFoundPieceId(whiteBee, PieceType.BEE);
            (int, int) beePosition = PieceMovesTools.GetIndiciesByHexId(beeHexId, _gameBoard.gameBoard);
            if (PieceMovesTools.GetNeighbours(beePosition, _gameBoard.gameBoard).Count == 6)
                return true;
        }
        return false;
    }

    public bool IsBeeOnBoard(bool white)
    {
        int beeHexId = GetFirstFoundPieceId(white, PieceType.BEE);
        List<int> hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        if (beeHexId != -1 && hexesOnBoardIds.Contains(beeHexId))
            return true;
        return false;
    }

    private int GetFirstFoundPieceId(bool whiteHexes, PieceType pieceType)
    {
        List<GameObject> hexes = whiteHexes ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
        int beeHexIdx = hexes.FindIndex(hex => hex
            .GetComponent<HexWrapperController>()
            .piece
            .GetComponent<IPieceController>()
            .GetPieceType() == pieceType);

        return hexes[beeHexIdx].GetComponent<HexWrapperController>().HexId;
    }

}
