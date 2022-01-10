using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePostValidator : MonoBehaviour, IMovePostValidator
{
    private HexesStoreScript _hexesStore;

    void Start()
    {
        GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
        _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();
    }

    public bool IsAddingCorrect(GameObject hexToAdd, GameObject moveProposition)
    {
        return true;
    }

    public bool IsMovingCorrect(GameObject hexToMOve, GameObject moveProposition)
    {
        var hexScript = moveProposition.GetComponent<HexWrapperController>();
        return !IsOneHiveRuleBroken(hexScript);
    }

    private bool IsOneHiveRuleBroken(HexWrapperController movedHex) //If there are more than one hive than the rule is broken
    {
        return false;
    }
}
