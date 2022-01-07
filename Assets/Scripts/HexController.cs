using UnityEngine;

public class HexController : MonoBehaviour
{
    public GameObject gameMeneger;
    public GameObject hexWrapper;

    private void OnMouseDown()
    {
        gameMeneger.GetComponent<GameManagerController>().HexSelected(hexWrapper);
    }
}
