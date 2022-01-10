using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePreValidator : MonoBehaviour, IMovePreValidator
{
    private HexesStoreScript _hexesStore;
    private HexesInfoProvider _hexesInfoProvider;   

    void Start()
    {
        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

        GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
        _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();
    }

    public bool CanAdd(PieceType type, bool white)
    {
        return !IsBeeOnGameboardRuleBroken(type, white);
    }

    public bool CanMove(GameObject hex)
    {
        var hexScript = hex.GetComponent<HexWrapperController>();
        return CanPlayerMove(hexScript.isWhite);
    }

    private bool IsBeeOnGameboardRuleBroken(PieceType type, bool white) //If it is fourth move of the player and the bee piece is not on the table than the rule is broken
    {
        var hexesOnBoardIds = white ? _hexesStore.blackHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

        if (hexesOnBoardIds.Count > 2 && !_hexesInfoProvider.IsBeeOnBoard(white) && type != PieceType.BEE)
            return true;
        return false;
    }

    private bool CanPlayerMove(bool white) //If the bee of that player is not on the board then the player can not move
    {
        return _hexesInfoProvider.IsBeeOnBoard(white);
    }
}
