using UnityEngine;

namespace Hive
{
    public class HexController : MonoBehaviour
    {
        public GameObject gameManager;
        public GameObject hexWrapper;
        private UIController _ui;

        void Start()
        {
            GameObject uiGameobject = GameObject.FindWithTag("UI");
            _ui = uiGameobject.GetComponent<UIController>();
        }

        void OnMouseDown()
        {
            _ui.OnHexSelected();
            gameManager.GetComponent<GameManagerController>().OnHexSelected(hexWrapper);
        }
    }
}
