using UnityEngine;

public class HexWrapperController : MonoBehaviour
{
    public GameObject gameMeneger;
    public GameObject hex;
    public int HexId;
    public bool isOnGameboard = false;
    public GameObject piece;
    public bool isWhite;
    public (int, int) positionOnBoard;
}
