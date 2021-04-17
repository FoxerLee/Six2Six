using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{

    protected string powerUpName;
    protected string description;

    public string getName() {
        return powerUpName;
    }

    public string getDescription() {
        return description;
    }

    public abstract void takeEffect(GameObject board);

}
