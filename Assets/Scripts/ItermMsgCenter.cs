using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItermMsgCenter : MonoBehaviour
{

    public void SendToTag(string msg)
    {
        Transform firstChild = GetComponentsInChildren<Transform>()[1];
        print(firstChild.name);
        if (firstChild.tag == "power")
        {
            firstChild.SendMessage(msg);
            // Debug.Log(firstChild.name + "get msg: " + msg + " from " + this.name);
        }
    }
}
