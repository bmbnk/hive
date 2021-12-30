using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexController : MonoBehaviour
{
    public GameObject gameMeneger;
    public GameObject hexWrapper;
    public TextMeshProUGUI text;

    private void OnMouseDown()
    {
        text.text = hexWrapper.GetComponent<HexWrapperController>().piece.GetComponent<IPieceController>().GetPieceType().ToString();
        gameMeneger.GetComponent<GameMenegerController>().HexSelected(hexWrapper);
    }
}
