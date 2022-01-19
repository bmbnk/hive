using System;
using TMPro;
using UnityEngine;

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

    public void ChangeSideMenu(bool white)
    {
        if (!GamePanel.activeSelf)
            GamePanel.SetActive(true);

        _sideMenus.EnablePlayerSideMenu(white);
        _sideMenus.DisablePlayerSideMenu(!white);
    }

    public void UpdateCounterLabel(PieceType type, bool white, int count)
    {
        _sideMenus.UpdateCounterLabel(type, white, count);
    }

    public void HideSideMenus()
    {
        GamePanel.SetActive(false);
    }

    public void HideChoiceMenu()
    {
        ChoiceMenuPanel.SetActive(false);
    }

    public void ShowWinEndingPanel(bool whiteWon)
    {
        ShowEndingPanel((whiteWon ? "White " : "Black ") + WinGameEndTextTemplate);
    }

    public void ShowGameDrawnEndingPanel()
    {
        ShowEndingPanel(DrawnGameEndTextTemplate);
    }

    private void ShowEndingPanel(string endText)
    {
        EndGamePanel.GetComponentInChildren<TextMeshProUGUI>().SetText(endText);
        GamePanel.SetActive(false);
        EndGamePanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        PauseMenuPanel.SetActive(false);
    }

    public void LaunchPauseMenu()
    {
        PauseMenuPanel.SetActive(true);
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

    public void LaunchChoiceMenu()
    {
        StartMenuPanel.SetActive(false);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        ChoiceMenuPanel.SetActive(true);
    }

    internal void LaunchStartMenu()
    {
        StartMenuPanel.SetActive(true);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
        ChoiceMenuPanel.SetActive(false);
    }
}
