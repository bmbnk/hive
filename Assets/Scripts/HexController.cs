using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexController : MonoBehaviour
{
    public GameObject gameMeneger;
    public GameObject hexWrapper;
    public TextMeshProUGUI text;

    void Start()
    {
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    text.text = hexWrapper.GetComponent<HexWrapperController>().piece.GetComponent<IPieceController>().GetPieceType().ToString();
        //    gameMeneger.GetComponent<GameMenegerController>().HexSelected(hexWrapper, false);
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    gameMeneger.GetComponent<GameMenegerController>().HexSelected(hexWrapper, true);
        //}
    }

    private void OnMouseDown()
    {
        text.text = hexWrapper.GetComponent<HexWrapperController>().piece.GetComponent<IPieceController>().GetPieceType().ToString();
        gameMeneger.GetComponent<GameMenegerController>().HexSelected(hexWrapper);
    }
}
