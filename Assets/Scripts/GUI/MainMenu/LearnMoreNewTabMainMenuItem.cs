using UnityEngine;
using System.Collections;

public class LearnMoreNewTabMainMenuItem : MainMenuItem {
    
    public LearnMoreOptionsMainMenuItemArray learnMoreOptionsArray;
    
    public override void click () {
        // Debug.Log(this.GetType() + " clicked "+itemName);
		learnMoreOptionsArray.goToMOOCNewTab();
    }
}
