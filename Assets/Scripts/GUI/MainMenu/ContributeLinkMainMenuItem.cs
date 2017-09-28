using UnityEngine;
using System.Collections;

public class ContributeLinkMainMenuItem : MainMenuItem
{
    [SerializeField]
    private bool _newTab;

	[SerializeField]
	private CustomDataValue _dataValue;

    private const string _urlKey = "STUDY.LEARN.LINK";

    public override void click()
	{
		// Debug.Log(this.GetType() + " clicked "+itemName);
		base.click ();
		RedMetricsManager.get ().sendEvent (TrackingEvent.SELECTMENU, new CustomData (CustomDataTag.OPTION, _dataValue.ToString()));
		StudyFormLinker.openFormGame (false);
	}
}
