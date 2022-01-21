using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideMenusController : MonoBehaviour
{
    public const float AlphaValue = 0.5f;
    public const float HighlightedButtonScale = 1.5f;

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

    private RulesValidator _rulesValidator;

    private List<TextMeshProUGUI> _blackCounters;
    private List<TextMeshProUGUI> _whiteCounters;

    private List<Button> _activeBlackButtons;
    private List<Button> _activeWhiteButtons;

    private struct Tile
    {
        public Button button;
        public TextMeshProUGUI counter;
    }

    private Tile _highlightedTile = new Tile();

    void Start()
    {
        GameObject moveValidatorGameObject = GameObject.FindWithTag("RulesValidator");
        _rulesValidator = moveValidatorGameObject.GetComponent<RulesValidator>();

        BlackAntButton.onClick.AddListener(() => OnTileSelected(PieceType.ANT, false));
        BlackGrasshopperButton.onClick.AddListener(() => OnTileSelected(PieceType.GRASSHOPPER, false));
        BlackSpiderButton.onClick.AddListener(() => OnTileSelected(PieceType.SPIDER, false));
        BlackBeetleButton.onClick.AddListener(() => OnTileSelected(PieceType.BEETLE, false));
        BlackBeeButton.onClick.AddListener(() => OnTileSelected(PieceType.BEE, false));

        WhiteAntButton.onClick.AddListener(() => OnTileSelected(PieceType.ANT, true));
        WhiteGrasshopperButton.onClick.AddListener(() => OnTileSelected(PieceType.GRASSHOPPER, true));
        WhiteSpiderButton.onClick.AddListener(() => OnTileSelected(PieceType.SPIDER, true));
        WhiteBeetleButton.onClick.AddListener(() => OnTileSelected(PieceType.BEETLE, true));
        WhiteBeeButton.onClick.AddListener(() => OnTileSelected(PieceType.BEE, true));

        ResetButtons();
        ResetCounters();
    }

    private void OnTileSelected(PieceType type, bool white) {
        GameManagerController gameManagerScript = GameManager.GetComponent<GameManagerController>();
        DehighlightTile();
        HighlightTile(type, white);
        gameManagerScript.OnTileSelected(type, white);
    }

    public void DehighlightTile()
    {
        if (!_highlightedTile.Equals(default(Tile)))
        {
            Vector2 targetScale = _highlightedTile
                .button
                .transform
                .localScale / HighlightedButtonScale;

            _highlightedTile
                .button
                .transform.localScale = targetScale;

            _highlightedTile.counter.fontSize = _highlightedTile.counter.fontSize / HighlightedButtonScale;

            //_highlightedTile.counter.fontStyle = FontStyles.Normal;
            _highlightedTile = new Tile();
        }
    }

    private void HighlightTile(PieceType type, bool white)
    {
        var button = GetButton(type, white);
        var counter = GetCounter(type, white);

        Vector2 targetScale = button.transform.localScale * HighlightedButtonScale;
        button.transform.localScale = targetScale;

        //counter.fontStyle = FontStyles.Bold;
        counter.fontSize = counter.fontSize * HighlightedButtonScale;

        _highlightedTile.button = button;
        _highlightedTile.counter = counter;
    }

    private Button GetButton(PieceType type, bool white)
    {
        var buttons = white ? _activeWhiteButtons : _activeBlackButtons;
        var button = buttons.FindLast(button => IsButtonOfType(button, type));
        return button;
    }

    private bool IsButtonOfType(Button button, PieceType type)
    {
        string buttonTag = button.tag;
        string buttonTypeString = buttonTag.Replace("Button", "").ToUpper();
        return buttonTypeString.Equals(type.ToString());
    }

    private TextMeshProUGUI GetCounter(PieceType type, bool white)
    {
        var counters = white ? _whiteCounters : _blackCounters;
        var counter = counters.FindLast(counter => IsCounterOfType(counter, type));
        return counter;
    }

    private bool IsCounterOfType(TextMeshProUGUI counter, PieceType type)
    {
        string counterTag = counter.tag;
        string counterTypeString = counterTag.Replace("Counter", "").ToUpper();
        return counterTypeString.Equals(type.ToString());
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
        DehighlightTile();
        SetButtonsInteractable(white, false);
        SetAlphaForSideMenu(white, AlphaValue);
    }

    public void EnablePlayerSideMenu(bool white)
    {
        if (_rulesValidator.IsBeeOnGameboardRuleBroken(white))
        {
            var button = GetButton(PieceType.BEE, white);
            button.GetComponent<CanvasGroup>().alpha = 1.0f;
            button.interactable = true;
        } else
        {
            SetButtonsInteractable(white, true);
            SetAlphaForSideMenu(white, 1.0f);
        }
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

    public void UpdateCounterLabel(PieceType type, bool white, int count)
    {
        var counter = GetCounter(type, white);
        counter.text = count > 0 ? count.ToString() : "";
        if (count > 0)
        {
            counter.text = count.ToString();
        } else
        {
            counter.text = "";
            DeactivateButton(type, white);
        }
    }

    private void DeactivateButton(PieceType type, bool white)
    {
        var button = GetButton(type, white);
        button.GetComponent<CanvasGroup>().alpha = AlphaValue;
        button.interactable = false;

        var buttons = white ? _activeWhiteButtons : _activeBlackButtons;
        buttons.Remove(button);
    }
}
