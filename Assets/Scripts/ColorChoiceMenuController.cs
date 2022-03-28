using UnityEngine;
using UnityEngine.UI;

namespace Hive
{
    public class ColorChoiceMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gameManager;

        [SerializeField]
        private Button _blackStartsButton;

        [SerializeField]
        private Button _whiteStartsButton;


        void Start()
        {
            GameManagerController gameManagerScript = _gameManager.GetComponent<GameManagerController>();


            //text shuld be specified based on option "Computer" or "Friend" (ex. "Who starts"/"Which color you want")
            _blackStartsButton.onClick.AddListener(() => gameManagerScript.StartGame(false));
            _whiteStartsButton.onClick.AddListener(() => gameManagerScript.StartGame(true));
        }
    }
}
