using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public const string WinGameEndTextTemplate = "insects won";
    public const string DrawnGameEndTextTemplate = "DRAW";
    public const float AlphaValue = 0.5f;

    public GameObject GameManager;
    public GameObject GamePanel;
    public GameObject EndGamePanel;
    public GameObject StartMenuPanel;
    private SideMenusController _sideMenus;

    void Start()
    {
        GameObject sideMenusGameobject = GameObject.FindWithTag("SideMenus");
        _sideMenus = sideMenusGameobject.GetComponent<SideMenusController>();

        GamePanel.SetActive(true);
        EndGamePanel.SetActive(false);
    }

    public void ChangeSideMenu(bool white)
    {
        if (!GamePanel.activeSelf)
            GamePanel.SetActive(true);

        _sideMenus.EnablePlayerSideMenu(white);
        _sideMenus.DisablePlayerSideMenu(!white);
    }

    public void UpdateTileCounterLabel(PieceType type, bool white, int count)
    {
        _sideMenus.UpdateTileLayoutElement(type, white, count);
    }

    public void HideSideMenus()
    {
        GamePanel.SetActive(false);
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

    public void ResetUI()
    {
        EndGamePanel.SetActive(false);
        StartMenuPanel.SetActive(false);
        GamePanel.SetActive(true);
        _sideMenus.ResetSideMenus();
    }

    public void LaunchStartMenu()
    {
        StartMenuPanel.SetActive(true);
        EndGamePanel.SetActive(false);
        GamePanel.SetActive(false);
    }
}
