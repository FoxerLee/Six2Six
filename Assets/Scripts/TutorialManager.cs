using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    public int currentStage = 0;
    public UnityEvent[] stage;

    public void getStage() {
        if (currentStage < stage.Length) {
            stage[currentStage].Invoke();
        }
        currentStage++;
    }
}
