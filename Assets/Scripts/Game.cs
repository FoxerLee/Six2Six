using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public bool isRedTurn = false;
    public int currentScore = -1;
    public Color redColor;
    public Color grayColor;
    public Color selectedColor;
    public GameObject playerTitle;
    public GameObject[] gamePiecesObj;

    private Color oldCardColor;
    private GameObject lastClicked = null;
    private int[] redPieces = new int[] { 10, 10, 9, 7, 5, 3 };
    private int[] grayPieces = new int[] { 10, 10, 9, 7, 5, 3 };

    // Start is called before the first frame update
    void Start()
    {
        oldCardColor = gamePiecesObj[0].GetComponent<Image>().color;
        ChangeAllUI();
    }

    // Update is called once per frame
    void Update()
    {

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

    public void SwitchSide() {
        isRedTurn = !isRedTurn;
        currentScore = -1;
        lastClicked = null;
        ChangeAllUI();
    }

    public void SelectedHexScore(int score)
    {
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
        if (lastClicked == null || lastClicked != board)
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
        }
        else if (lastClicked == board) {
            board.GetComponent<Button>().interactable = false;
            board.GetComponent<Animator>().SetBool("select", true);
            Confirmed();
        }
        
    }

    void CancelLastClick()
    {
        if (lastClicked != null) {
            lastClicked.GetComponent<Animator>().SetTrigger("return");
            lastClicked.GetComponent<Animator>().SetBool("isRed", false);
            lastClicked.GetComponent<Animator>().SetBool("isGray", false);
        }
        lastClicked = null;
    }


    void ChangeAllUI()
    {
        Text name = playerTitle.GetComponentInChildren<Text>();
        Image BG = playerTitle.GetComponent<Image>();

        if (isRedTurn)
        {
            name.text = "Red Turn";
            BG.color = redColor;
        }
        else
        {
            name.text = "Gray Turn";
            BG.color = grayColor;
        }
        for (int i = 0; i < gamePiecesObj.Length; i++)
        {
            ChangePiece(gamePiecesObj[i], i);
        }
    }

    void ChangePiece(GameObject piece,int pos)
    {
        Text left = null;
        Image background = piece.GetComponent<Image>();
        int score = -1;
        Animator hex_BG = piece.GetComponentInChildren<Animator>();
        foreach (Transform child in piece.transform) {
            if (child.tag == "left") {
                left = child.gameObject.GetComponent<Text>();
            }
        }

        background.color = oldCardColor;
        hex_BG.SetBool("isRed", isRedTurn);

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
    }

    //public void Red6Click()
    //{
    //    // Debug.Log("Click 6");
    //    var button = GameObject.Find("Red6").GetComponent<Button>();
    //    var buttonText = GameObject.Find("Red6").GetComponentInChildren<Text>();
        
    //    redPieces[5] -= 1;
    //    int cur = redPieces[5];
    //    buttonText.text = $"six: {cur.ToString()} left";

    //    if (redPieces[5] == 0)
    //    {
    //        button.interactable = false;
    //    }

    //}

}
