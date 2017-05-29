using UnityEngine;
using System.Collections;

public class LearnMoreMainMenuItem : MainMenuItem {

    [SerializeField]
    private bool _newTab;
    [SerializeField]
    private string _urlKey;
    [SerializeField]
    private LearnMoreOptionsMainMenuItemArray _learnMoreOptionsArray;
    
    public override void click () {
        // Debug.Log(this.GetType() + " clicked "+itemName);
		base.click();
		_learnMoreOptionsArray.goToMOOC(_urlKey, _newTab);
    }
}
