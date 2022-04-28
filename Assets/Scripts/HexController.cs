using UnityEngine;

namespace Hive
{
    public class HexController : MonoBehaviour
    {
        public GameObject hexWrapper;
        private UIController _ui;
        private GameManagerController _gameManager;


        void Start()
        {
            GameObject uiGameobject = GameObject.FindWithTag("UI");
            _ui = uiGameobject.GetComponent<UIController>();

            _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManagerController>();

        }

        void OnMouseDown()
        {
            _ui.OnHexSelected();
            _gameManager.OnHexSelected(hexWrapper);
        }
    }
}
