using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    public const string WinGameEndTextTemplate = "insects won";
    public const string DrawnGameEndTextTemplate = "DRAW";
    public const float AlphaValue = 0.5f;

    public GameObject GameManager;
    public GameObject StartMenuPanel;
    public GameObject ChoiceMenuPanel;
    public GameObject GamePanel;


    public GameObject PauseMenuPanel;
    public GameObject EndGamePanel;
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

    public void ChangeSideMenu(bool white)
    {
        if (GamePanel.activeSelf)
        {
            _sideMenus.EnablePlayerSideMenu(white);
            _sideMenus.DisablePlayerSideMenu(!white);
        }

    }

    public void HideChoiceMenu()
    {
        ChoiceMenuPanel.SetActive(false);
    }

    public void HidePauseMenu()
    {
        PauseMenuPanel.SetActive(false);
    }

    public void HideSideMenus()
    {
        GamePanel.SetActive(false);
    }

    public void LaunchChoiceMenu()
    {
        StartMenuPanel.SetActive(false);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        ChoiceMenuPanel.SetActive(true);
    }

    private void LaunchEndingPanel(string endText)
    {
        EndGamePanel.GetComponentInChildren<TextMeshProUGUI>().SetText(endText);
        GamePanel.SetActive(false);
        EndGamePanel.SetActive(true);
    }

    public void LaunchGameDrawnEndingPanel()
    {
        LaunchEndingPanel(DrawnGameEndTextTemplate);
    }

    public void LaunchPauseMenu()
    {
        PauseMenuPanel.SetActive(true);
    }

    public void LaunchSideMenus()
    {
        GamePanel.SetActive(true);
    }

    public void LaunchStartMenu()
    {
        StartMenuPanel.SetActive(true);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        ChoiceMenuPanel.SetActive(false);
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
        ChoiceMenuPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
        StartMenuPanel.SetActive(false);
        _sideMenus.ResetSideMenus();
    }

    public void UpdateCounterLabel(PieceType type, bool white, int count)
    {
        _sideMenus.UpdateCounterLabel(type, white, count);
    }
}
