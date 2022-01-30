using UnityEngine;
using UnityEngine.UI;

namespace Hive
{
    public class PauseMenuController : MonoBehaviour
    {
        public GameObject GameManager;
        public GameObject PauseMenuPanel;
        public Button CancelButton;
        public Button QuitButton;


        void Start()
        {
            GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();

            CancelButton.onClick.AddListener(() => gameManagerScript.ResumeGame());
            QuitButton.onClick.AddListener(() => gameManagerScript.ResetGame());
        }
    }
}
