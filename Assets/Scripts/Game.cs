using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{

    public static Game instance = null;

    public bool isRedTurn = false;
    public int currentScore = -1;
    public Color redColor;
    public Color grayColor;
    public Color selectedColor;
    public GameObject playerTitle;
    public GameObject menu;
    public GameObject[] gamePiecesObj;
    public GameObject endingItem;
    public Text blackEndScore;
    public Text redEndScore;

    public int half_horizen = 4;
    public int half_vertical = 4;

    private bool isSwitch = false;
    private Color oldCardColor;
    private GameObject lastClicked = null;
    private int[] redPieces = new int[] { 2, 3, 3, 5, 5, 6 };
    private int[] grayPieces = new int[] { 2, 3, 3, 5, 5, 6 };
    private bool isFinished;

    private Canvas canvas;
    private Transform cardDeck;

    //EFFECTS
    //Key is the tile number attached; Value is the gameobject of the effect attached
    public Dictionary<int, GameObject> redAttachedEffects = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> grayAttachedEffects = new Dictionary<int, GameObject>();

    //Key is the effect name, and value is the effect description
    private Dictionary<string, string> EffectDescription = new Dictionary<string, string>();

    [SerializeField] private GameObject[] powerUps;
    private const int maxNumOfPowerUps = 5;
    [HideInInspector] public int redNumOfPowerUps;
    [HideInInspector] public int grayNumOfPowerUps;
    private Transform p1, p2;
    private GameObject powerUp1, powerUp2;

    private PowerUp powerUp;

    public AudioSource audio;
    public AudioClip placePiecefx;
    public AudioClip powerUpfx;
    public enum SoundOptions
    {
        placeTile,
        powerUp
    }
    [Range(0, 1)]
    public float soundfxlvl = 0.9f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        // DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize power-ups for red and gray players
        redNumOfPowerUps = maxNumOfPowerUps;
        grayNumOfPowerUps = maxNumOfPowerUps;

        // Get canvas and power-up card deck
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        cardDeck = canvas.transform.Find("Menu_BG").Find("Card-deck");
        p1 = cardDeck.Find("Card-holder-1");
        p2 = cardDeck.Find("Card-holder-2");
        p2.SetAsFirstSibling();

        oldCardColor = gamePiecesObj[0].GetComponent<Image>().color;
        ChangeAllUI();
        ResetBoard();
        //Initializes effect dictionary's description
        initializeEffectsDictionary();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        // check if it is finished
        isFinished = false;
        for (int x = half_horizen; x <= half_horizen; x++)
        {
            for (int y = half_vertical; y <= half_vertical; y++)
            {
                isFinished &= ScoreManager.instance.isPlaced[Tuple.Create(x, y)];
            }
        }
        // pieces out of usage
        if (redPieces.Sum() == 0 && grayPieces.Sum() == 0)
        {
            isFinished = true;
        }

        if (isFinished)
        {
            endingItem.SetActive(true);
            blackEndScore.text = $"{ScoreManager.instance.grayScore}";
            redEndScore.text = $"{ScoreManager.instance.redScore}";

        }
    }

    public void Confirmed()
    {
        int pos = redPieces.Length - currentScore;
        if (isRedTurn)
        {
            redPieces[pos] -= 1;
        }
        else
        {
            grayPieces[pos] -= 1;
        }

        //Play Sound
        playSound(SoundOptions.placeTile);
        SwitchSide();
    }

    // change use menu bg after each turn
    public void SwitchSide()
    {
        isRedTurn = !isRedTurn;
        currentScore = -1;
        lastClicked = null;
        isSwitch = false;
        ChangeAllUI();
    }

    public void SelectedHexScore(int score)
    {
        if (currentScore != -1)
        {
            isSwitch = true;
        }
        currentScore = score;
        int j = gamePiecesObj.Length - currentScore;
        for (int i = 0; i < gamePiecesObj.Length; i++)
        {
            if (i == j)
            {
                gamePiecesObj[i].GetComponent<Image>().color = selectedColor;
            }
            else
            {
                gamePiecesObj[i].GetComponent<Image>().color = oldCardColor;
            }
        }
    }

    public void GetPowerUp(GameObject card)
    {
        if (card.transform.Find("power-up") != null)
        {
            if (card.transform.Find("power-up").gameObject.GetComponent<PowerUp>() != null)
            {
                powerUp = card.transform.Find("power-up").gameObject.GetComponent<PowerUp>();
            }
            else
            {
                Debug.Log("ERROR: Tab card does not have a power-up attached!");
            }
        }
        else
        {
            powerUp = null;
        }
    }

    public void ClickBoard(GameObject board)
    {
        Button button = board.GetComponent<Button>();


        if (currentScore == -1) return;
        if (lastClicked == null || lastClicked != board || isSwitch)
        {
            CancelLastClick();
            Animator boardAni = board.GetComponent<Animator>();
            Text boardText = board.GetComponentInChildren<Text>();
            if (isRedTurn)
            {
                // change text color for colorblind
                // board.GetComponentInChildren<Text>().color = Color.black;

                // change background for colorblind
                ChangePieceSprite(button, "");

                boardAni.SetBool("isRed", true);
            }
            else
            {
                // change text color for colorblind
                // board.GetComponentInChildren<Text>().color = Color.white;
                
                // change background for colorblind
                ChangePieceSprite(button, "-p2");
                
                boardAni.SetBool("isGray", true);
            }
            boardText.text = "" + currentScore;
            lastClicked = board;
            isSwitch = false;
        }
        else if (lastClicked == board)
        {
            board.GetComponent<Button>().interactable = false;
            board.GetComponent<Animator>().SetBool("select", true);
            var idx = board.name.Substring(3).Split(',');

            var xs = idx[0].Substring(1);
            var ys = idx[1].Substring(0, idx[1].Length - 1);

            int x = StringToInt(xs);
            int y = StringToInt(ys);

            int id = StringToInt(idx[2]);


            string curPlayer = "None";
            if (isRedTurn)
            {
                curPlayer = "Red";
            }
            else
            {
                curPlayer = "Gray";
            }


            if (powerUp != null)
            {
                // Power-up take effect
                powerUp.takeEffect(board);

                // Destroy power-up after it is used
                Dictionary<int, GameObject> attachedEffects = isRedTurn ? redAttachedEffects : grayAttachedEffects;
                attachedEffects.Remove(currentScore);

                // Show power-up mark on board
                board.transform.Find("mark").gameObject.SetActive(true);
            }

            ScoreManager.instance.chessboard[Tuple.Create(x, y)] = Tuple.Create(curPlayer, currentScore, id);

            ScoreManager.instance.CheckScore();
            ScoreManager.instance.isPlaced[Tuple.Create(x, y)] = true;

            Confirmed();
        }

    }

    private int StringToInt(string s)
    {
        return Int32.Parse(s);
    }

    void CancelLastClick()
    {
        if (lastClicked != null)
        {
            lastClicked.GetComponent<Animator>().SetTrigger("return");
            lastClicked.GetComponent<Animator>().SetBool("isRed", false);
            lastClicked.GetComponent<Animator>().SetBool("isGray", false);
            lastClicked.GetComponentInChildren<Text>().text = "";
        }
        lastClicked = null;
    }

    // change pieces selection UI
    void ChangeAllUI()
    {
        Text name = playerTitle.GetComponentInChildren<Text>();
        Animator menuBG = menu.GetComponent<Animator>();
        Image BG = playerTitle.GetComponent<Image>();
        Text redScore = GameObject.Find("RedScore").GetComponent<Text>();
        Text blueScore = GameObject.Find("BlueScore").GetComponent<Text>();
        redScore.text = $"{ScoreManager.instance.redScore}";
        blueScore.text = $"{ScoreManager.instance.grayScore}";

        if (isRedTurn)
        {
            name.text = "Red Turn";
            BG.color = redColor;
            menuBG.SetTrigger("red");

            for (int i=1; i <7; i++)
            {
                ChangeBGSprite(i.ToString(), "");
            }
        }
        else
        {
            name.text = "Blue Turn";
            BG.color = grayColor;
            menuBG.SetTrigger("blue");

            for (int i=1; i <7; i++)
            {
                ChangeBGSprite(i.ToString(), "-p2");
            }
        }
        for (int i = 0; i < gamePiecesObj.Length; i++)
        {
            // switch to corresponding user's pieces info
            ChangePiece(gamePiecesObj[i], i);
        }

        // Remove all power-up cards from card-deck
        powerUp1 = null;
        powerUp2 = null;
        foreach (Transform child in p1)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in p2)
        {
            Destroy(child.gameObject);
        }
        // Each player is given two power-up cards randomly chosen from all cards
        int numOfPowerUps = isRedTurn ? redNumOfPowerUps : grayNumOfPowerUps;
        if (numOfPowerUps > 0)
        {
            // powerUp1 = Instantiate(powerUps[Random.Range(0, powerUps.Length)]);
            powerUp1 = Instantiate(powerUps[0]);
            powerUp1.transform.SetParent(p1);
            powerUp1.transform.localScale = new Vector3(1, 1, 1);
        }
        if (numOfPowerUps >= 2)
        {
            // powerUp2 = Instantiate(powerUps[Random.Range(0, powerUps.Length)]);
            powerUp2 = Instantiate(powerUps[1]);
            powerUp2.transform.SetParent(p2);
            powerUp2.transform.localScale = new Vector3(1, 1, 1);
            p2.SetAsFirstSibling();
        }

        // Clear all effects
        for (int i = 0; i < gamePiecesObj.Length; i++)
        {
            if (gamePiecesObj[i].transform.Find("power-up") != null)
            {
                Destroy(gamePiecesObj[i].transform.Find("power-up").gameObject);
            }
        }
        // Show effects attached
        Dictionary<int, GameObject> attachedEffects = isRedTurn ? redAttachedEffects : grayAttachedEffects;
        foreach (KeyValuePair<int, GameObject> entry in attachedEffects)
        {
            // Create a copy of the power-up card       
            GameObject powerUpTab = Instantiate(entry.Value);
            powerUpTab.transform.SetParent(gamePiecesObj[6 - entry.Key].transform);
            powerUpTab.name = "power-up";
            powerUpTab.transform.localScale = transform.localScale * 0.8f;
            powerUpTab.transform.SetAsFirstSibling();
            powerUpTab.transform.position = transform.position + Drop.DropLocationAdjustment;
        }
    }

    // switch to corresponding user's pieces info
    void ChangePiece(GameObject piece, int pos)
    {
        Text left = null;
        Image background = piece.GetComponent<Image>();
        int score = -1;
        Animator hex = piece.GetComponent<Animator>();
        Animator hex_BG = null;
        foreach (Transform child in piece.transform)
        {
            if (child.tag == "hex_BG")
            {
                hex_BG = child.gameObject.GetComponent<Animator>();
            }
            if (child.tag == "left")
            {
                left = child.gameObject.GetComponent<Text>();
            }
        }

        background.color = oldCardColor;
        hex_BG.SetBool("isRed", isRedTurn);
        hex.SetTrigger("show");

        if (isRedTurn)
        {
            score = redPieces[pos];
        }
        else
        {
            score = grayPieces[pos];
        }

        left.text = score + " left";

        if (score == 0)
        {
            piece.GetComponent<Button>().interactable = false;
        }
        else
        {
            piece.GetComponent<Button>().interactable = true;
        }
    }

    public void DisablePowerUps()
    {
        if (powerUp1 != null)
        {
            powerUp1.GetComponent<Drag>().enabled = false;
            powerUp1.GetComponent<Animator>().enabled = false;
            powerUp1.GetComponent<Button>().interactable = false;
        }
        if (powerUp2 != null)
        {
            powerUp2.GetComponent<Drag>().enabled = false;
            powerUp2.GetComponent<Animator>().enabled = false;
            powerUp2.GetComponent<Button>().interactable = false;
        }
    }

    private void ResetBoard()
    {

        lastClicked = null;
        isSwitch = false;

        redPieces = new int[] { 2, 3, 3, 5, 5, 6 };
        grayPieces = new int[] { 2, 3, 3, 5, 5, 6 };

        // Debug
        // redPieces = new int[] { 1, 1, 1, 1, 1, 1 };
        // grayPieces = new int[] { 1, 1, 1, 1, 1, 1 };


        ScoreManager.instance.resetScoreSystem();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    //Helpers
    private void initializeEffectsDictionary()
    {
        EffectDescription.Add("Effect 1", "DESCRIPTION OF EFFECT 1");
        EffectDescription.Add("Effect 2", "DESCRIPTION OF EFFECT 2");
        EffectDescription.Add("Effect 3", "DESCRIPTION OF EFFECT 3");
        EffectDescription.Add("Effect 4", "DESCRIPTION OF EFFECT 4");
        EffectDescription.Add("Effect 5", "DESCRIPTION OF EFFECT 5");
        EffectDescription.Add("Effect 6", "DESCRIPTION OF EFFECT 6");
    }

    public void playSound(SoundOptions soundName)
    {
        switch (soundName)
        {
            case SoundOptions.placeTile:
                audio.PlayOneShot(placePiecefx, soundfxlvl);
                break;
            case SoundOptions.powerUp:
                audio.PlayOneShot(powerUpfx, soundfxlvl);
                break;
            default:
                // code block
                break;
        }
    }

    // load image
    private Sprite LoadByIO(string path, int width, int height)
    {
        FileStream fileStream = new FileStream(Application.dataPath + path, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);

        byte[] bytes = new byte[fileStream.Length];

        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        // int width = 300;
        // int height = 372;
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        return sprite;

    }

    private void ChangePieceSprite(Button button, string player)
    {
        int width = 80;
        int height = 80;
        var path = "";
        SpriteState spriteState = new SpriteState();

        path = "/Resources/btn/hex-empty" + player + ".png";
        Sprite selectedSprite = LoadByIO(path, width, height);
        path = "/Resources/btn/hex-aqu" + player + ".png";
        Sprite disabledSprite = LoadByIO(path, width, height);

        spriteState = button.spriteState;
        spriteState.disabledSprite = disabledSprite;
        spriteState.selectedSprite = selectedSprite;
        button.spriteState = spriteState;

    }

    private void ChangeBGSprite(string id, string player)
    {
        int width = 320;
        int height = 400;
        var path = "";
        SpriteState spriteState = new SpriteState();

        Button button = GameObject.Find("Menu_BG/"+id).GetComponent<Button>();

        path = "/Resources/btn/btn-" + id + "-h" + player + "@2x.png";
        Sprite highlightedSprite = LoadByIO(path, width, height);
        path = "/Resources/btn/btn-" + id + "-s" + player + "@2x.png";
        Sprite selectedSprite = LoadByIO(path, width, height);
        path = "/Resources/btn/btn-" + id + "-x" + player + "@2x.png";
        Sprite disabledSprite = LoadByIO(path, width, height);

        spriteState = button.spriteState;
        spriteState.disabledSprite = disabledSprite;
        spriteState.selectedSprite = selectedSprite;
        spriteState.highlightedSprite = highlightedSprite;
        button.spriteState = spriteState;
    }
}
