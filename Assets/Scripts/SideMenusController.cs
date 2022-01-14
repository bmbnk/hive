using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideMenusController : MonoBehaviour
{
    public const float AlphaValue = 0.5f;

    public GameObject GameManager;
    public GameObject GamePanel;

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

    private List<Button> _activeBlackButtons;
    private List<Button> _activeWhiteButtons;


    void Start()
    {
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

        ResetButtons();
        ResetCounters();
    }

    private void ResetButtons()
    {
        _activeBlackButtons = new List<Button>();
        _activeWhiteButtons = new List<Button>();

        _activeBlackButtons.Add(BlackAntButton);
        _activeBlackButtons.Add(BlackGrasshopperButton);
        _activeBlackButtons.Add(BlackSpiderButton);
        _activeBlackButtons.Add(BlackBeetleButton);
        _activeBlackButtons.Add(BlackBeeButton);

        _activeWhiteButtons.Add(WhiteAntButton);
        _activeWhiteButtons.Add(WhiteGrasshopperButton);
        _activeWhiteButtons.Add(WhiteSpiderButton);
        _activeWhiteButtons.Add(WhiteBeetleButton);
        _activeWhiteButtons.Add(WhiteBeeButton);

        SetButtonsInteractable(false, true);
        SetButtonsInteractable(true, true);
    }

    private void ResetCounters()
    {
        BlackAntsLeftCounter.text = "3";
        BlackGrasshoppersLeftCounter.text = "3";
        BlackSpidersLeftCounter.text = "2";
        BlackBeetlesLeftCounter.text = "2";
        BlackBeesLeftCounter.text = "1";

        WhiteAntsLeftCounter.text = "3";
        WhiteGrasshoppersLeftCounter.text = "3";
        WhiteSpidersLeftCounter.text = "2";
        WhiteBeetlesLeftCounter.text = "2";
        WhiteBeesLeftCounter.text = "1";

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

    public void ResetSideMenus()
    {
        EnablePlayerSideMenu(false);
        EnablePlayerSideMenu(true);
        ResetButtons();
        ResetCounters();
    }

    public void DisablePlayerSideMenu(bool white)
    {
        SetButtonsInteractable(white, false);
        SetAlphaForSideMenu(white, AlphaValue);
    }

    public void EnablePlayerSideMenu(bool white)
    {
        SetButtonsInteractable(white, true);
        SetAlphaForSideMenu(white, 1.0f);
    }

    private void SetButtonsInteractable(bool white, bool interactable)
    {
        var buttons = white ? _activeWhiteButtons : _activeBlackButtons;
        buttons.ForEach(button => button.interactable = interactable);
    }

    private void SetAlphaForSideMenu(bool white, float alpha)
    {
        var buttons = white ? _activeWhiteButtons : _activeBlackButtons;
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

    public void UpdateTileLayoutElement(PieceType type, bool white, int count)
    {
        var counters = white ? _whiteCounters : _blackCounters;
        var counter = counters.FindLast(counter => IsCounterOfType(counter, type));
        UpdateLabel(counter, count);
        if (counter.text == "")
        {
            DeactivateButton(type, white);
        }
    }

    private bool IsCounterOfType(TextMeshProUGUI counter, PieceType type)
    {
        string counterTag = counter.tag;
        string counterTypeString = counterTag.Replace("Counter", "").ToUpper();
        return counterTypeString.Equals(type.ToString());
    }

    private void UpdateLabel(TextMeshProUGUI counter, int count)
    {
        counter.text = count > 0 ? count.ToString() : "";
    }

    private void DeactivateButton(PieceType type, bool white)
    {
        var buttons = white ? _activeWhiteButtons : _activeBlackButtons;
        var button = buttons.FindLast(button => IsButtonOfType(button, type));
        button.GetComponent<CanvasGroup>().alpha = AlphaValue;
        button.interactable = false;
        buttons.Remove(button);
    }

    private bool IsButtonOfType(Button button, PieceType type)
    {
        string buttonTag = button.tag;
        string buttonTypeString = buttonTag.Replace("Button", "").ToUpper();
        return buttonTypeString.Equals(type.ToString());
    }
}
