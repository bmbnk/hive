using UnityEngine;
using UnityEngine.UI;

namespace Hive
{
    public class PlayModeMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gameManager;

        [SerializeField]
        private GameObject _ui;

        [SerializeField]
        private Button _computerButton;

        [SerializeField]
        public Button _friendButton;


        void Start()
        {
            GameManagerController gameManagerScript = _gameManager.GetComponent<GameManagerController>();
            UIController uIController = _ui.GetComponent<UIController>();

            _computerButton.onClick.AddListener(() =>
            {
                uIController.LaunchColorChoiceMenu();
            });

            _friendButton.onClick.AddListener(() =>
            {
                uIController.LaunchColorChoiceMenu();
            });
        }
    }
}
