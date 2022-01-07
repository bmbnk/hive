using System;
using System.Collections.Generic;
using UnityEngine;

public class HexesStoreScript : MonoBehaviour
{
    public GameObject WhiteHex1;
    public GameObject WhiteHex2;
    public GameObject WhiteHex3;
    public GameObject WhiteHex4;
    public GameObject WhiteHex5;
    public GameObject WhiteHex6;
    public GameObject WhiteHex7;
    public GameObject WhiteHex8;
    public GameObject WhiteHex9;
    public GameObject WhiteHex10;
    public GameObject WhiteHex11;

    public GameObject BlackHex1;
    public GameObject BlackHex2;
    public GameObject BlackHex3;
    public GameObject BlackHex4;
    public GameObject BlackHex5;
    public GameObject BlackHex6;
    public GameObject BlackHex7;
    public GameObject BlackHex8;
    public GameObject BlackHex9;
    public GameObject BlackHex10;
    public GameObject BlackHex11;

    public List<int> whiteHexesOnBoardIds = new List<int>();
    public List<int> blackHexesOnBoardIds = new List<int>();

    public GameObject hexToMove;
    public List<GameObject> hexPropositionsToMove;

    public GameObject hexToAdd;
    public List<GameObject> hexPropositionsToAdd;

    public List<GameObject> whiteHexes;
    public List<GameObject> blackHexes;

    void Setup()
    {
        //InitializeHexes();
    }

    public void InitializeHexes()
    {
        WhiteHex1.GetComponent<HexWrapperController>().HexId = 1;
        WhiteHex2.GetComponent<HexWrapperController>().HexId = 2;
        WhiteHex3.GetComponent<HexWrapperController>().HexId = 3;
        WhiteHex4.GetComponent<HexWrapperController>().HexId = 4;
        WhiteHex5.GetComponent<HexWrapperController>().HexId = 5;
        WhiteHex6.GetComponent<HexWrapperController>().HexId = 6;
        WhiteHex7.GetComponent<HexWrapperController>().HexId = 7;
        WhiteHex8.GetComponent<HexWrapperController>().HexId = 8;
        WhiteHex9.GetComponent<HexWrapperController>().HexId = 9;
        WhiteHex10.GetComponent<HexWrapperController>().HexId = 10;
        WhiteHex11.GetComponent<HexWrapperController>().HexId = 11;

        whiteHexes = new List<GameObject>();
        whiteHexes.Add(WhiteHex1);
        whiteHexes.Add(WhiteHex2);
        whiteHexes.Add(WhiteHex3);
        whiteHexes.Add(WhiteHex4);
        whiteHexes.Add(WhiteHex5);
        whiteHexes.Add(WhiteHex6);
        whiteHexes.Add(WhiteHex7);
        whiteHexes.Add(WhiteHex8);
        whiteHexes.Add(WhiteHex9);
        whiteHexes.Add(WhiteHex10);
        whiteHexes.Add(WhiteHex11);

        BlackHex1.GetComponent<HexWrapperController>().HexId = 12;
        BlackHex2.GetComponent<HexWrapperController>().HexId = 13;
        BlackHex3.GetComponent<HexWrapperController>().HexId = 14;
        BlackHex4.GetComponent<HexWrapperController>().HexId = 15;
        BlackHex5.GetComponent<HexWrapperController>().HexId = 16;
        BlackHex6.GetComponent<HexWrapperController>().HexId = 17;
        BlackHex7.GetComponent<HexWrapperController>().HexId = 18;
        BlackHex8.GetComponent<HexWrapperController>().HexId = 19;
        BlackHex9.GetComponent<HexWrapperController>().HexId = 20;
        BlackHex10.GetComponent<HexWrapperController>().HexId = 21;
        BlackHex11.GetComponent<HexWrapperController>().HexId = 22;

        blackHexes = new List<GameObject>();
        blackHexes.Add(BlackHex1);
        blackHexes.Add(BlackHex2);
        blackHexes.Add(BlackHex3);
        blackHexes.Add(BlackHex4);
        blackHexes.Add(BlackHex5);
        blackHexes.Add(BlackHex6);
        blackHexes.Add(BlackHex7);
        blackHexes.Add(BlackHex8);
        blackHexes.Add(BlackHex9);
        blackHexes.Add(BlackHex10);
        blackHexes.Add(BlackHex11);
    }
}
