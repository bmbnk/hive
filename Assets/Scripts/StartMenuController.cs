using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    public GameObject GameManager;
    public Button StartGameButton;
    public Button ExitButton;


    void Start()
    {
        GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();

        StartGameButton.onClick.AddListener(() => gameManagerScript.PrepareGame());
        ExitButton.onClick.AddListener(() => Application.Quit());
    }
}
