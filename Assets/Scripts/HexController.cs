using UnityEngine;

public class HexController : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject hexWrapper;


    void OnMouseDown()
    {
        gameManager.GetComponent<GameManagerController>().HexSelected(hexWrapper);
    }
}
