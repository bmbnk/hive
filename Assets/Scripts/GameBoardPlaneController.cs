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
        if (!_ui.AreUIElementsPointed())
        {
            _ui.OnGameBoardClicked();
            gameMeneger.GetComponent<GameManagerController>().GameBoardSelected();
        }
    }
}
