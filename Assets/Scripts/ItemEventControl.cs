using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemEventControl : MonoBehaviour
{
    public UnityEvent hoverStartEvent;
    public UnityEvent hoverEndEvent;


    void StartHover()
    {
        print("invoke start hover");
        hoverStartEvent.Invoke();
    }

    void EndHover() {
        print("invoke end hover");
        hoverEndEvent.Invoke();
    }
}
