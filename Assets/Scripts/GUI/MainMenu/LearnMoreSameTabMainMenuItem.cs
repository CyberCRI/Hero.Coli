using UnityEngine;
using System.Collections;

public class LearnMoreSameTabMainMenuItem : MainMenuItem {
    
    public LearnMoreOptionsMainMenuItemArray learnMoreOptionsArray;
    
    public override void click () {
        Logger.Log("clicked "+itemName, Logger.Level.INFO);
		learnMoreOptionsArray.goToMOOCSameTab();
    }
}
