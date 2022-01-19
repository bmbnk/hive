using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuController : MonoBehaviour
{
    public GameObject GameManager;
    public GameObject EndGamePanel;
    public Button NextGameButton;
    public Button ExitButton;


    void Start()
    {
        GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();

        NextGameButton.onClick.AddListener(() => gameManagerScript.ResetGame());
        ExitButton.onClick.AddListener(() => Application.Quit());
    }
}