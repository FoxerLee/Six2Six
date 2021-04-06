using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;

    public int redScore = 0;
    public int grayScore = 0;
    // x --> red number in design pic, y --> blue number in design pic
    public Dictionary<Tuple<int, int>, Tuple<string, int, int>> chessboard = new Dictionary<Tuple<int, int>, Tuple<string, int, int>>();
    public Dictionary<Tuple<int, int>, bool> isScored = new Dictionary<Tuple<int, int>, bool>();


    private Hashtable allCalPos = new Hashtable();
    

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

    public void resetScoreSystem()
    {
        int id = 1;
        for (int x=-Game.instance.half_horizen; x<=Game.instance.half_horizen; x++)
        {
            for (int y=-Game.instance.half_vertical; y<=Game.instance.half_vertical; y++)
            {

                chessboard[Tuple.Create(x, y)] = Tuple.Create("None", 0, id);
                isScored[Tuple.Create(x, y)] = false;
                id += 1;
            }
        }

        allCalPos = new Hashtable();
        redScore = 0;
        grayScore = 0;

    }
    

    public void CheckAll(int x, int y)
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
                calculatedPos[temp] = chessboard[Tuple.Create(idx.Item1, idx.Item2)].Item3;
                temp += 1;
            }

            Array.Sort(calculatedPos);
            string longid = "";
            foreach (int val in calculatedPos)
            {
                longid += val;
            }
            
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
        return tempScore;
    }


}
