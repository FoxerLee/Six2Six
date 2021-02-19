using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    
    private int [] redPieces = new int[] {10, 10, 9, 7, 5, 3};
    private int [] grayPieces = new int[] {10, 10, 9, 7, 5, 3};
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Red6Click()
    {
        // Debug.Log("Click 6");
        var button = GameObject.Find("Red6").GetComponent<Button>();
        var buttonText = GameObject.Find("Red6").GetComponentInChildren<Text>();
        
        redPieces[5] -= 1;
        int cur = redPieces[5];
        buttonText.text = $"six: {cur.ToString()} left";

        if (redPieces[5] == 0)
        {
            button.interactable = false;
        }

    }

}
