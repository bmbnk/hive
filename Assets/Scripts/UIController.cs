using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hive
{
    public class UIController : MonoBehaviour
    {
        public const string WinGameEndTextTemplate = "insects won";
        public const string DrawnGameEndTextTemplate = "DRAW";
        public const float AlphaValue = 0.5f;

        [SerializeField]
        private GameObject _gameManager;

        [SerializeField]
        private GameObject _startMenuPanel;

        [SerializeField]
        private GameObject _colorChoiceMenuPanel;

        [SerializeField]
        private GameObject _roomChoicePanel;

        [SerializeField]
        private GameObject _enterNamePanel;

        [SerializeField]
        private GameObject _gamePanel;

        [SerializeField]
        private GameObject _pauseMenuPanel;

        [SerializeField]
        private GameObject _endGamePanel;


        private SideMenusController _sideMenus;

        void Start()
        {
            GameObject sideMenusGameobject = GameObject.FindWithTag("SideMenus");
            _sideMenus = sideMenusGameobject.GetComponent<SideMenusController>();
        }

        public bool AreUIElementsPointed()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (result.gameObject.CompareTag("RightSideMenu")
                        || result.gameObject.CompareTag("LeftSideMenu")
                        || result.gameObject.CompareTag("PauseMenuPanel")
                        || result.gameObject.CompareTag("EndGamePanel")
                        || result.gameObject.CompareTag("StartMenuPanel")
                        || result.gameObject.CompareTag("ChoiceMenuPanel"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ChangeSideMenu()
        {
            if (_gamePanel.activeSelf)
            {
                bool whiteTurn = _gameManager.GetComponent<GameManagerController>().IsWhiteTurn();
                _sideMenus.EnablePlayerSideMenu(whiteTurn);
                _sideMenus.DisablePlayerSideMenu(!whiteTurn);
            }

        }

        public void ExitApplication()
        {
            Application.Quit();
        }

        public void HideChoiceMenu()
        {
            _colorChoiceMenuPanel.SetActive(false);
        }

        public void HidePauseMenu()
        {
            _pauseMenuPanel.SetActive(false);
        }

        public void HideSideMenus()
        {
            _gamePanel.SetActive(false);
        }

        public void LaunchChoiceMenu()
        {
            _startMenuPanel.SetActive(false);
            _endGamePanel.SetActive(false);
            _gamePanel.SetActive(false);
            _pauseMenuPanel.SetActive(false);
            _colorChoiceMenuPanel.SetActive(true);
        }

        private void LaunchEndingPanel(string endText)
        {
            _endGamePanel.GetComponentInChildren<TextMeshProUGUI>().SetText(endText);
            _gamePanel.SetActive(false);
            _endGamePanel.SetActive(true);
        }

        public void LaunchGameDrawnEndingPanel()
        {
            LaunchEndingPanel(DrawnGameEndTextTemplate);
        }

        public void LaunchNameMenu()
        {
            _startMenuPanel.SetActive(false);
            _colorChoiceMenuPanel.SetActive(false);
            _roomChoicePanel.SetActive(false);
            _enterNamePanel.SetActive(true);
        }

        public void LaunchPauseMenu()
        {
            _pauseMenuPanel.SetActive(true);
        }

        public void LaunchSideMenus()
        {
            _gamePanel.SetActive(true);
        }

        public void LaunchStartMenu()
        {
            _startMenuPanel.SetActive(true);
            _endGamePanel.SetActive(false);
            _gamePanel.SetActive(false);
            _pauseMenuPanel.SetActive(false);
            _colorChoiceMenuPanel.SetActive(false);
        }

        public void LaunchWinEndingPanel(bool whiteWon)
        {
            LaunchEndingPanel((whiteWon ? "White " : "Black ") + WinGameEndTextTemplate);
        }

        public void OnGameBoardClicked()
        {
            _sideMenus.DehighlightTile();
        }

        public void OnHexSelected()
        {
            _sideMenus.DehighlightTile();
        }

        public void ResetUI()
        {
            _colorChoiceMenuPanel.SetActive(false);
            _pauseMenuPanel.SetActive(false);
            _endGamePanel.SetActive(false);
            _gamePanel.SetActive(false);
            _startMenuPanel.SetActive(false);
            _sideMenus.ResetSideMenus();
        }

        public void UpdateCounterLabel(PieceType type, bool white, int count)
        {
            _sideMenus.UpdateCounterLabel(type, white, count);
        }
    }
}
