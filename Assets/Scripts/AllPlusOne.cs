using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPlusOne : PowerUp
{

    void Start() {
        powerUpName = "+1 Nearby";
        description = "+1";
    }

    public override void takeEffect(GameObject board)
    {
        Debug.Log("This is a +1 power-up. Location: " + board.name);
    }
}
