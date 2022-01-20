using UnityEngine;

public class GameBoardPlaneController : MonoBehaviour
{
    public GameObject gameMeneger;
    private UIController _ui;

    void Start()
    {
        GameObject uiGameobject = GameObject.FindWithTag("UI");
        _ui = uiGameobject.GetComponent<UIController>();
    }

    void OnMouseDown()
    {
        if (!_ui.AreSideMenusPointed())
        {
            gameMeneger.GetComponent<GameManagerController>().GameBoardSelected();
        }
    }
}
