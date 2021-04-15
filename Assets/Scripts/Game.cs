using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    //EFFECTS
    //Key is the tile number attached; strings is the name of the effect attached
    public Dictionary<int, GameObject> redAttachedEffects = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> grayAttachedEffects = new Dictionary<int, GameObject>();

    //Key is the effect name, and value is the effect description
    private Dictionary<string, string> EffectDescription = new Dictionary<string, string>();


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
        oldCardColor = gamePiecesObj[0].GetComponent<Image>().color;
        ChangeAllUI();
        ResetBoard();
        //Initializes effect dictionary's description
        initializeEffectsDictionary(); 
    }

    // Update is called once per frame
    void Update()
    {

        // check if it is finished
        isFinished = false;
        for (int x=half_horizen; x<=half_horizen; x++)
        {
            for (int y=half_vertical; y<=half_vertical; y++)
            {
                isFinished &= ScoreManager.instance.isScored[Tuple.Create(x, y)];
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
        SwitchSide();
    }

    // change use menu bg after each turn
    public void SwitchSide() {
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
        for (int i = 0; i < gamePiecesObj.Length; i++) {
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

    public void ClickBoard(GameObject board)
    {
        if (currentScore == -1) return;
        if (lastClicked == null || lastClicked != board || isSwitch)
        {
            CancelLastClick();
            Animator boardAni = board.GetComponent<Animator>();
            Text boardText = board.GetComponentInChildren<Text>();
            if (isRedTurn)
            {
                boardAni.SetBool("isRed", true);
            }
            else
            {
                boardAni.SetBool("isGray", true);
            }
            boardText.text = "" + currentScore;
            lastClicked = board;
            isSwitch = false;
        }
        else if (lastClicked == board) {
            board.GetComponent<Button>().interactable = false;
            board.GetComponent<Animator>().SetBool("select", true);
            var idx = board.name.Substring(3).Split(',');

            var xs = idx[0].Substring(1);
            var ys = idx[1].Substring(0, idx[1].Length-1);

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
            ScoreManager.instance.chessboard[Tuple.Create(x, y)] = Tuple.Create(curPlayer, currentScore, id);
            for (x=-half_horizen; x<=half_horizen; x++)
            {
                for (y=-half_vertical; y<=half_vertical; y++)
                {
                    ScoreManager.instance.CheckAll(x, y);
                }
            }
            ScoreManager.instance.isScored[Tuple.Create(x, y)] = true;
            Confirmed();
        }
        
    }

    private int StringToInt(string s)
    {
        return Int32.Parse(s);
    }

    void CancelLastClick()
    {
        if (lastClicked != null) {
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
        }
        else
        {
            name.text = "Blue Turn";
            BG.color = grayColor;
            menuBG.SetTrigger("blue");
        }
        for (int i = 0; i < gamePiecesObj.Length; i++)
        {
            // switch to corresponding user's pieces info
            ChangePiece(gamePiecesObj[i], i);
        }

        // Clear all effects
        for (int i = 0; i < gamePiecesObj.Length; i++) 
        {
            if (gamePiecesObj[i].transform.Find("power-up") != null) 
            {
                Destroy(gamePiecesObj[i].transform.Find("power-up").gameObject);
                Debug.Log(gamePiecesObj[i].name + "'s power-up is destroyed!");
            }
        }
        // Show effects attached
        Dictionary<int, GameObject> attachedEffects = isRedTurn ? redAttachedEffects : grayAttachedEffects;
        foreach (KeyValuePair<int, GameObject> entry in attachedEffects) 
        {
            GameObject piece = gamePiecesObj[6 - entry.Key];
            if (piece.transform.Find("power-up") == null) 
            {
                // Create a copy of the power-up card       
                GameObject powerUp = Instantiate(entry.Value);
                powerUp.transform.SetParent(piece.transform);
                powerUp.name = "power-up";
                powerUp.transform.localScale = transform.localScale * 0.8f;
                powerUp.transform.SetAsFirstSibling();
                powerUp.transform.position = transform.position + new Vector3(0, 64, 0);
            }
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
        foreach (Transform child in piece.transform) {
            if (child.tag == "hex_BG") {
                hex_BG = child.gameObject.GetComponent<Animator>();
            }
            if (child.tag == "left") {
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


    private void ResetBoard()
    {

        lastClicked = null;
        isSwitch = false;

        // redPieces = new int[] { 2, 3, 3, 5, 5, 6 };
        // grayPieces = new int[] { 2, 3, 3, 5, 5, 6 };

        // Debug
        redPieces = new int[] { 1, 1, 1, 1, 1, 1 };
        grayPieces = new int[] { 1, 1, 1, 1, 1, 1 };


        ScoreManager.instance.resetScoreSystem();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Helpers
    private void initializeEffectsDictionary(){
        EffectDescription.Add("Effect 1", "DESCRIPTION OF EFFECT 1");
        EffectDescription.Add("Effect 2", "DESCRIPTION OF EFFECT 2");
        EffectDescription.Add("Effect 3", "DESCRIPTION OF EFFECT 3");
        EffectDescription.Add("Effect 4", "DESCRIPTION OF EFFECT 4");
        EffectDescription.Add("Effect 5", "DESCRIPTION OF EFFECT 5");
        EffectDescription.Add("Effect 6", "DESCRIPTION OF EFFECT 6");
    }
}
