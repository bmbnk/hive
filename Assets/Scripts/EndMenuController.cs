using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMenuController : MonoBehaviour
{
    public GameObject GameManager;
    public Button NextGameButton;
    public Button ExitButton;


    void Start()
    {
        GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();

        NextGameButton.onClick.AddListener(() => gameManagerScript.NextGame());
        ExitButton.onClick.AddListener(() => Application.Quit());
    }
}
