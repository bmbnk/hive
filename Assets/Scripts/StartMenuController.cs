using UnityEngine;
using UnityEngine.UI;

namespace Hive
{
    public class StartMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _ui;

        [SerializeField]
        private Button _startGameButton;

        [SerializeField]
        private Button _exitButton;


        void Start()
        {
            UIController uIController = _ui.GetComponent<UIController>();

            _startGameButton.onClick.AddListener(() => uIController.LaunchPlayModeMenu());
            _exitButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
