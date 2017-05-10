﻿using UnityEngine;

public class BackMainMenuItem : MainMenuItem
{
    [SerializeField]
    private MainMenuManager.MainMenuScreen _screen;

    public override void click()
    {
        // Debug.Log(this.GetType());
        MainMenuManager.get().switchTo(_screen);
    }

    public override void initialize()
    {
        base.initialize();
    }
}
