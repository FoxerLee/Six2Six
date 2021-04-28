using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllMinusOne : PowerUp
{
    void Start() {
        powerUpName = "-1 Nearby";
        description = "-1";
    }

    public override void takeEffect(GameObject board)
    {
        var idx = board.name.Substring(3).Split(',');

        var xs = idx[0].Substring(1);
        var ys = idx[1].Substring(0, idx[1].Length - 1);

        int x = StringToInt(xs);
        int y = StringToInt(ys);

        int id = StringToInt(idx[2]);

        Tuple<int, int>[] neis = {Tuple.Create(x, y+1), Tuple.Create(x, y-1),
                                Tuple.Create(x+1, y), Tuple.Create(x-1, y),
                                Tuple.Create(x-1, y+1), Tuple.Create(x+1, y-1)};

        Text neiText;

        foreach (Tuple<int, int> neIdx in neis)
        {
            try{
                int neiX = neIdx.Item1;
                int neiY = neIdx.Item2;
                // effect only works for neighbors which already been placed.
                
                if (ScoreManager.instance.isPlaced[Tuple.Create(neiX, neiY)] == true)
                {
                    string curPlayer = ScoreManager.instance.chessboard[Tuple.Create(neiX, neiY)].Item1;
                    int curScore = ScoreManager.instance.chessboard[Tuple.Create(neiX, neiY)].Item2 - 1;
                    int neId = ScoreManager.instance.chessboard[Tuple.Create(neiX, neiY)].Item3;
                    ScoreManager.instance.chessboard[Tuple.Create(neiX, neiY)] = Tuple.Create(curPlayer, curScore, neId);

                    // update UI score
                    neiText = GameObject.Find("hex("+neiX.ToString()+","+neiY.ToString()+"),"+neId.ToString()).GetComponentInChildren<Text>();
                    neiText.text = "" + curScore;
                }
            }
            catch(Exception e){
                Debug.Log("Out of bounds");
            }
        }

    }

    private int StringToInt(string s)
    {
        return Int32.Parse(s);
    }
}
