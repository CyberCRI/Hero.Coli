using UnityEngine;
using System.Collections;

public class LearnMoreSameTabMainMenuItem : MainMenuItem {
    
    public LearnMoreOptionsMainMenuItemArray learnMoreOptionsArray;
    
    public override void click () {
        // Debug.Log(this.GetType() + " clicked "+itemName);
		learnMoreOptionsArray.goToMOOCSameTab();
    }
}
