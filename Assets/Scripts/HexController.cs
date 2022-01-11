using UnityEngine;

public class HexController : MonoBehaviour
{
    public GameObject gameMeneger;
    public GameObject hexWrapper;


    void OnMouseDown()
    {
        gameMeneger.GetComponent<GameManagerController>().HexSelected(hexWrapper);
    }
}
