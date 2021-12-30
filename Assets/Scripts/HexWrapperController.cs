using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexWrapperController : MonoBehaviour
{
    public GameObject gameMeneger;
    public GameObject hex;
    public int HexId;
    public List<Vector3> availableLocationOffsetVectors;
    public int currentOffsetVectorIndex;
    public bool isOnGameboard = false;
    public GameObject piece;
    public bool isWhite;
}
