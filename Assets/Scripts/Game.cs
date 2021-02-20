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

    private int[] redPieces = new int[] { 10, 10, 9, 7, 5, 3 };
    private int[] grayPieces = new int[] { 10, 10, 9, 7, 5, 3 };

    // Start is called before the first frame update
    void Start()
    {
        changeAllUI();
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
        isRedTurn = !isRedTurn;
    }

    public void SelectedHexScore(int score)
    {
        currentScore = score;
        int i = gamePiecesObj.Length - currentScore;
        gamePiecesObj[i].GetComponent<Image>().color = selectedColor;
    }


    void changeAllUI()
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
            changePiece(gamePiecesObj[i], i);
        }
    }

    void changePiece(GameObject piece,int pos)
    {
        Text left = null;
        int score = -1;
        Animator hex_BG = piece.GetComponentInChildren<Animator>();
        foreach (Transform child in piece.transform) {
            if (child.tag == "left") {
                left = child.gameObject.GetComponent<Text>();
            }
        }

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
