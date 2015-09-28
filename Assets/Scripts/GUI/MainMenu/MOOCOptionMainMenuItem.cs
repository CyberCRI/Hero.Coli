using UnityEngine;
using System.Collections;

public class MOOCOptionMainMenuItem : MainMenuItem {
    
    public GameObject moocExplanation;
    public MOOCOptionsMainMenuItemArray moocOptionsArray;
    
    public override void click () {
        Logger.Log("clicked "+itemName, Logger.Level.INFO);
		moocOptionsArray.goToMOOC();
    }
}
