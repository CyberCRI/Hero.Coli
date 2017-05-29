using UnityEngine;
using System.Collections;

public class ContributeLinkMainMenuItem : MainMenuItem
{
    [SerializeField]
    private bool _newTab;
    private const string _urlKey = "STUDY.LEARN.LINK";

    public override void click()
    {
        // Debug.Log(this.GetType() + " clicked "+itemName);
		base.click();
        RedMetricsManager.get().sendEvent(TrackingEvent.SELECTMENU, new CustomData(CustomDataTag.OPTION, CustomDataValue.CONTRIBUTE.ToString()));
        StudyFormLinker.openForm();
    }
}
