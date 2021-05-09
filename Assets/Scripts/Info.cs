using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info : MonoBehaviour
{
    public GameObject infoBox;
    private bool toggle = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickInfoButton(){
        infoBox.SetActive(toggle);
        toggle = !toggle;
    }
}
