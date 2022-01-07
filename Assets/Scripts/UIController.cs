using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject gameManager;

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

    void Start()
    {
        GameManagerController gameManagerScript = gameManager.GetComponent<GameManagerController>();

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

    public void UpdateLabel(PieceType type, bool white, int count)
    {
        var counters = white ? _whiteCounters : _blackCounters;
        var counter = counters.FindLast(counter => IsCounterOfType(counter, type));
        counter.text = count.ToString();
    }

    private bool IsCounterOfType(TextMeshProUGUI counter, PieceType type)
    {
        string counterTag = counter.tag;
        string counterTypeString = counterTag.Replace("Counter", "").ToUpper();
        return counterTypeString.Equals(type.ToString());
    }
}
