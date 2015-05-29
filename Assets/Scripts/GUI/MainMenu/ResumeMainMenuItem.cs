using UnityEngine;
using System.Collections;

public class ResumeMainMenuItem : MainMenuItem {
    public override void click() {
        Debug.LogWarning("the game will resume...");
        GameStateController.get ().resumeGame ();
    }
}