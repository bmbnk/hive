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

    public Button BlackAntButton;
    public Button BlackGrasshopperButton;
    public Button BlackSpiderButton;
    public Button BlackBeetleButton;
    public Button BlackBeeButton;

    public Button WhiteAntButton;
    public Button WhiteGrasshopperButton;
    public Button WhiteSpiderButton;
    public Button WhiteBeetleButton;
    public Button WhiteBeeButton;

    public TextMeshProUGUI BlackAntsLeftCounter;
    public TextMeshProUGUI BlackGrasshoppersLeftCounter;
    public TextMeshProUGUI BlackSpidersLeftCounter;
    public TextMeshProUGUI BlackBeetlesLeftCounter;
    public TextMeshProUGUI BlackBeesLeftCounter;

    public TextMeshProUGUI WhiteAntsLeftCounter;
    public TextMeshProUGUI WhiteGrasshoppersLeftCounter;
    public TextMeshProUGUI WhiteSpidersLeftCounter;
    public TextMeshProUGUI WhiteBeetlesLeftCounter;
    public TextMeshProUGUI WhiteBeesLeftCounter;

    private List<TextMeshProUGUI> _blackCounters;
    private List<TextMeshProUGUI> _whiteCounters;

    private List<Button> _blackButtons;
    private List<Button> _whiteButtons;


    void Start()
    {
        GamePanel.SetActive(true);
        EndGamePanel.SetActive(false);

        GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();

        BlackAntButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.ANT, false));
        BlackGrasshopperButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.GRASSHOPPER, false));
        BlackSpiderButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.SPIDER, false));
        BlackBeetleButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.BEETLE, false));
        BlackBeeButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.BEE, false));

        WhiteAntButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.ANT, true));
        WhiteGrasshopperButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.GRASSHOPPER, true));
        WhiteSpiderButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.SPIDER, true));
        WhiteBeetleButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.BEETLE, true));
        WhiteBeeButton.onClick.AddListener(() => gameManagerScript.TileSelected(PieceType.BEE, true));

        _blackButtons = new List<Button>();
        _whiteButtons = new List<Button>();
        
        _blackButtons.Add(BlackAntButton);
        _blackButtons.Add(BlackGrasshopperButton);
        _blackButtons.Add(BlackSpiderButton);
        _blackButtons.Add(BlackBeetleButton);
        _blackButtons.Add(BlackBeeButton);
        
        _whiteButtons.Add(WhiteAntButton);
        _whiteButtons.Add(WhiteGrasshopperButton);
        _whiteButtons.Add(WhiteSpiderButton);
        _whiteButtons.Add(WhiteBeetleButton);
        _whiteButtons.Add(WhiteBeeButton);

        _blackCounters = new List<TextMeshProUGUI>();
        _whiteCounters = new List<TextMeshProUGUI>();

        _blackCounters.Add(BlackAntsLeftCounter);
        _blackCounters.Add(BlackGrasshoppersLeftCounter);
        _blackCounters.Add(BlackSpidersLeftCounter);
        _blackCounters.Add(BlackBeetlesLeftCounter);
        _blackCounters.Add(BlackBeesLeftCounter);

        _whiteCounters.Add(WhiteAntsLeftCounter);
        _whiteCounters.Add(WhiteGrasshoppersLeftCounter);
        _whiteCounters.Add(WhiteSpidersLeftCounter);
        _whiteCounters.Add(WhiteBeetlesLeftCounter);
        _whiteCounters.Add(WhiteBeesLeftCounter);
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

    public void UpdateLabel(PieceType type, bool white, int count)
    {
        var counters = white ? _whiteCounters : _blackCounters;
        var counter = counters.FindLast(counter => IsCounterOfType(counter, type));
        counter.text = count > 0 ? count.ToString() : "";
    }

    public void DisableButtons(bool white)
    {
        SetButtonsInteractable(white, false);
    }

    public void EnableButtons(bool white)
    {
        SetButtonsInteractable(white, true);
    }

    private void SetButtonsInteractable(bool white, bool interactable)
    {
        var buttons = white ? _whiteButtons : _blackButtons;
        buttons.ForEach(button => button.interactable = interactable);
    }

    public void GreyOutPlayerMenu(bool white)
    {
        SetAlphaForSideMenu(white, AlphaValue);
    }

    public void UnGreyPlayerMenu(bool white)
    {
        SetAlphaForSideMenu(white, 1.0f);
    }

    private void SetAlphaForSideMenu(bool white, float alpha)
    {
        var buttons = white ? _whiteButtons : _blackButtons;
        var counters = white ? _whiteCounters : _blackCounters;

        buttons.ForEach(button =>
        {
            button.GetComponent<CanvasGroup>().alpha = alpha;
        });

        counters.ForEach(counter =>
        {
            counter.GetComponent<CanvasGroup>().alpha = alpha;
        });
    }

    private bool IsCounterOfType(TextMeshProUGUI counter, PieceType type)
    {
        string counterTag = counter.tag;
        string counterTypeString = counterTag.Replace("Counter", "").ToUpper();
        return counterTypeString.Equals(type.ToString());
    }
}
