using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{

    public bool isRedTurn = false;
    public int currentScore = -1;
    public Color redColor;
    public Color grayColor;
    public Color selectedColor;
    public GameObject playerTitle;
    public GameObject[] gamePiecesObj;
    public GameObject endingItem;
    public Text blackEndScore;
    public Text redEndScore;

    private int half_horizen = 4;
    private int half_vertical = 4;

    // x --> red number in design pic, y --> blue number in design pic
    private Dictionary<Tuple<int, int>, Tuple<string, int, int>> chessboard = new Dictionary<Tuple<int, int>, Tuple<string, int, int>>();
    private bool isSwitch = false;
    private Color oldCardColor;
    private GameObject lastClicked = null;
    private int[] redPieces = new int[] { 3, 5, 7, 9, 10, 10 };
    private int[] grayPieces = new int[] { 3, 5, 7, 9, 10, 10 };
    private int redScore = 0;
    private int grayScore = 0;
    private Dictionary<Tuple<int, int>, bool> isScored = new Dictionary<Tuple<int, int>, bool>();
    private Hashtable allCalPos = new Hashtable();
    private bool isFinished;

    // Start is called before the first frame update
    void Start()
    {
        oldCardColor = gamePiecesObj[0].GetComponent<Image>().color;
        ChangeAllUI();
        ResetBoard();

        // CheckLine(0, 0);
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
                isFinished &= isScored[Tuple.Create(x, y)];
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
            blackEndScore.text = $"{grayScore}";
            redEndScore.text = $"{redScore}";
            //     for (int x=-half_horizen; x<=half_horizen; x++)
            //     {
            //         for (int y=-half_vertical; y<=half_vertical; y++)
            //         {
            //             CheckAll(x, y);
            //         }
            //     }

            //     Debug.Log($"Red: {redScore}");
            //     Debug.Log($"Gray: {grayScore}");
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
            chessboard[Tuple.Create(x, y)] = Tuple.Create(curPlayer, currentScore, id);
            // isScored[Tuple.Create(x, y)] = true;

            for (x=-half_horizen; x<=half_horizen; x++)
            {
                for (y=-half_vertical; y<=half_vertical; y++)
                {
                    CheckAll(x, y);
                }
            }
            isScored[Tuple.Create(x, y)] = true;

            // if (curPlayer == "Red")
            // {
            //     redScore /= 6;
            // }
            // else
            // {
            //     grayScore /= 6;
            // }


            Debug.Log($"Red: {redScore}");
            Debug.Log($"Gray: {grayScore}");

            // Debug.Log(board.name.Substring(3));
            

            // Debug.Log(x);
            // Debug.Log(y);


            // CheckLine(x, y);

            // Debug.Log($"Red: {redScore}");
            // Debug.Log($"Gray: {grayScore}");

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
        }
        lastClicked = null;
    }

    // change pieces selection UI
    void ChangeAllUI()
    {
        Text name = playerTitle.GetComponentInChildren<Text>();
        Image BG = playerTitle.GetComponent<Image>();
        Text curScore = GameObject.Find("PlayerScore").GetComponent<Text>();

        if (isRedTurn)
        {
            name.text = "Red Turn";
            BG.color = redColor;
            curScore.text = $"Score: {redScore}";
        }
        else
        {
            name.text = "Gray Turn";
            BG.color = grayColor;
            curScore.text = $"Score: {grayScore}";
        }
        for (int i = 0; i < gamePiecesObj.Length; i++)
        {
            // switch to corresponding user's pieces info
            ChangePiece(gamePiecesObj[i], i);
        }
    }

    // switch to corresponding user's pieces info
    void ChangePiece(GameObject piece,int pos)
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

    private void CheckAll(int x, int y)
    {
        

        CheckLine(x, y);
        CheckCircle(x, y);
        CheckTriangle(x, y);
        
    }

    private void CheckLine(int x, int y)
    {
        var curPlayer = chessboard[Tuple.Create(x, y)].Item1;
        int tempScore = 0;
        // right forward
        Tuple<int, int>[] indexs =  {Tuple.Create(x, y), Tuple.Create(x+1, y), 
                                     Tuple.Create(x+2, y), Tuple.Create(x+3, y), 
                                     Tuple.Create(x+4, y), Tuple.Create(x+5, y)};
        tempScore += CalculateScore(indexs, curPlayer);

        // right down
        Tuple<int, int>[] indexs_rd =  {Tuple.Create(x, y), Tuple.Create(x+1, y-1), 
                                     Tuple.Create(x+2, y-2), Tuple.Create(x+3, y-3), 
                                     Tuple.Create(x+4, y-4), Tuple.Create(x+5, y-5)};
        tempScore += CalculateScore(indexs_rd, curPlayer);

        // left down
        Tuple<int, int>[] indexs_ld =  {Tuple.Create(x, y), Tuple.Create(x, y-1), 
                                     Tuple.Create(x, y-2), Tuple.Create(x, y-3), 
                                     Tuple.Create(x, y-4), Tuple.Create(x, y-5)};
        tempScore += CalculateScore(indexs_ld, curPlayer);

        // left
        Tuple<int, int>[] indexs_l =  {Tuple.Create(x, y), Tuple.Create(x-1, y),
                                     Tuple.Create(x-2, y), Tuple.Create(x-3, y),
                                     Tuple.Create(x-4, y), Tuple.Create(x-5, y)};
        tempScore += CalculateScore(indexs_l, curPlayer);

        // left up
        Tuple<int, int>[] indexs_lu =  {Tuple.Create(x, y), Tuple.Create(x-1, y+1),
                                     Tuple.Create(x-2, y+2), Tuple.Create(x-3, y+3),
                                     Tuple.Create(x-4, y+4), Tuple.Create(x-5, y+5)};
        tempScore += CalculateScore(indexs_lu, curPlayer);

        // right up
        Tuple<int, int>[] indexs_ru =  {Tuple.Create(x, y), Tuple.Create(x, y+1), 
                                     Tuple.Create(x, y+2), Tuple.Create(x, y+3), 
                                     Tuple.Create(x, y+4), Tuple.Create(x, y+5)};
        tempScore += CalculateScore(indexs_ru, curPlayer);


        if (curPlayer == "Red")
        {
            redScore += tempScore;
        }
        else
        {
            grayScore += tempScore;
        }
    }

    private void CheckCircle(int x, int y)
    {
        var curPlayer = chessboard[Tuple.Create(x, y)].Item1;
        int tempScore = 0;

        // right
        Tuple<int, int>[] indexs_r =  {Tuple.Create(x, y), Tuple.Create(x+1, y), 
                                     Tuple.Create(x+2, y-1), Tuple.Create(x+2, y-2), 
                                     Tuple.Create(x+1, y-2), Tuple.Create(x, y-1)};
        tempScore += CalculateScore(indexs_r, curPlayer);

        // right down
        Tuple<int, int>[] indexs_rd =  {Tuple.Create(x, y), Tuple.Create(x+1, y-1), 
                                        Tuple.Create(x+1, y-2), Tuple.Create(x, y-2), 
                                        Tuple.Create(x-1, y-1), Tuple.Create(x-1, y)};
        tempScore += CalculateScore(indexs_rd, curPlayer);

        // left down
        Tuple<int, int>[] indexs_ld =  {Tuple.Create(x, y), Tuple.Create(x, y-1), 
                                     Tuple.Create(x-1, y-1), Tuple.Create(x-2, y), 
                                     Tuple.Create(x-2, y+1), Tuple.Create(x-1, y+1)};
        tempScore += CalculateScore(indexs_ld, curPlayer);

        // left
        Tuple<int, int>[] indexs_l =  {Tuple.Create(x, y), Tuple.Create(x-1, y),
                                     Tuple.Create(x-2, y+1), Tuple.Create(x-2, y+2),
                                     Tuple.Create(x-1, y+2), Tuple.Create(x, y+1)};
        tempScore += CalculateScore(indexs_l, curPlayer);

        // left up
        Tuple<int, int>[] indexs_lu =  {Tuple.Create(x, y), Tuple.Create(x-1, y+1),
                                     Tuple.Create(x-1, y+2), Tuple.Create(x, y+2),
                                     Tuple.Create(x+1, y+1), Tuple.Create(x+1, y)};
        tempScore += CalculateScore(indexs_lu, curPlayer);

        // right up
        Tuple<int, int>[] indexs_ru =  {Tuple.Create(x, y), Tuple.Create(x, y+1), 
                                     Tuple.Create(x+1, y+1), Tuple.Create(x+2, y), 
                                     Tuple.Create(x+2, y-1), Tuple.Create(x+1, y-1)};
        tempScore += CalculateScore(indexs_ru, curPlayer);

        if (curPlayer == "Red")
        {
            redScore += tempScore;
        }
        else
        {
            grayScore += tempScore;
        }
    }

    private void CheckTriangle(int x, int y)
    {
        var curPlayer = chessboard[Tuple.Create(x, y)].Item1;
        int tempScore = 0;

        // right
        Tuple<int, int>[] indexs_r =  {Tuple.Create(x, y), Tuple.Create(x+1, y), 
                                     Tuple.Create(x+2, y), Tuple.Create(x+1, y-1), 
                                     Tuple.Create(x+2, y-1), Tuple.Create(x+2, y-2)};
        tempScore += CalculateScore(indexs_r, curPlayer);

        // right down
        Tuple<int, int>[] indexs_rd =  {Tuple.Create(x, y), Tuple.Create(x, y-1), 
                                        Tuple.Create(x+1, y-1), Tuple.Create(x, y-2), 
                                        Tuple.Create(x+1, y-2), Tuple.Create(x+2, y-2)};
        tempScore += CalculateScore(indexs_rd, curPlayer);

        // left down
        Tuple<int, int>[] indexs_ld =  {Tuple.Create(x, y), Tuple.Create(x-2, y), 
                                     Tuple.Create(x-1, y), Tuple.Create(x-1, y-1), 
                                     Tuple.Create(x, y-1), Tuple.Create(x, y-2)};
        tempScore += CalculateScore(indexs_ld, curPlayer);

        // left
        Tuple<int, int>[] indexs_l =  {Tuple.Create(x, y), Tuple.Create(x-2, y),
                                     Tuple.Create(x-1, y), Tuple.Create(x-2, y+1),
                                     Tuple.Create(x-1, y+1), Tuple.Create(x-2, y+2)};
        tempScore += CalculateScore(indexs_l, curPlayer);

        // left up
        Tuple<int, int>[] indexs_lu =  {Tuple.Create(x, y), Tuple.Create(x-1, y+1),
                                     Tuple.Create(x, y+1), Tuple.Create(x-2, y+2),
                                     Tuple.Create(x-1, y+2), Tuple.Create(x, y+2)};
        tempScore += CalculateScore(indexs_lu, curPlayer);

        // right up
        Tuple<int, int>[] indexs_ru =  {Tuple.Create(x, y), Tuple.Create(x+1, y), 
                                     Tuple.Create(x+2, y), Tuple.Create(x, y+1), 
                                     Tuple.Create(x+1, y+1), Tuple.Create(x, y+2)};
        tempScore += CalculateScore(indexs_ru, curPlayer);

        if (curPlayer == "Red")
        {
            redScore += tempScore;
        }
        else
        {
            grayScore += tempScore;
        }
    }

    private int CalculateScore(Array indexs, string curPlayer)
    {
        int tempScore = 0;
        bool allScored = true;
        foreach (Tuple<int, int> idx in indexs)
        {
            
            int x = idx.Item1;
            int y = idx.Item2;
            if (chessboard.ContainsKey(Tuple.Create(x, y)) && chessboard[Tuple.Create(x, y)].Item1 == curPlayer)
            {
                tempScore += chessboard[Tuple.Create(x, y)].Item2;
                allScored &= isScored[Tuple.Create(x, y)];
            }
            else
            {
                tempScore = 0;
                break;
            }
        }
        if (!allScored && tempScore != 0)
        {
            int[] calculatedPos = new int[6];
            int temp = 0;
            foreach (Tuple<int, int> idx in indexs)
            {   
                // calculatedPos.Add(chessboard[Tuple.Create(idx.Item1, idx.Item2)].Item3);
                // int x = idx.Item1;
                // int y = idx.Item2;
                // isScored[Tuple.Create(x, y)] = true;
                calculatedPos[temp] = chessboard[Tuple.Create(idx.Item1, idx.Item2)].Item3;
                temp += 1;
            }
            Array.Sort(calculatedPos);
            string longid = "";
            foreach (int val in calculatedPos)
            {
                longid += val;
            }
            // Debug.Log(calculatedPos);
            if (allCalPos.ContainsKey(longid))
            {
                tempScore = 0;
                return 0;
            }
            else
            {
                allCalPos.Add(longid, "true");
            }
        }
        // else
        // // if (allScored)
        // {
        //     tempScore = 0;
        // }

        return tempScore;
    }

    private void ResetBoard()
    {
        int id = 1;
        for (int x=-half_horizen; x<=half_horizen; x++)
        {
            for (int y=-half_vertical; y<=half_vertical; y++)
            {

                chessboard[Tuple.Create(x, y)] = Tuple.Create("None", 0, id);
                isScored[Tuple.Create(x, y)] = false;
                id += 1;
            }
        }

        lastClicked = null;
        isSwitch = false;

        redScore = 0;
        grayScore = 0;

        redPieces = new int[] { 3, 5, 7, 9, 10, 10 };
        grayPieces = new int[] { 3, 5, 7, 9, 10, 10 };

        allCalPos = new Hashtable();
    }

    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
