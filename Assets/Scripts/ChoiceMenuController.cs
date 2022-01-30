using UnityEngine;
using UnityEngine.UI;

namespace Hive
{
    public class ChoiceMenuController : MonoBehaviour
    {
        public GameObject GameManager;
        public Button BlackStartsButton;
        public Button WhiteStartsButton;


        void Start()
        {
            GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();

            BlackStartsButton.onClick.AddListener(() => gameManagerScript.StartGame(false));
            WhiteStartsButton.onClick.AddListener(() => gameManagerScript.StartGame(true));
        }
    }
}
