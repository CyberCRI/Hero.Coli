using UnityEngine;
using System.Collections;

public class ContributeLinkMainMenuItem : MainMenuItem
{
	public enum ContributeOrigin {
		MAINMENU,
		QUIT,
		HUD,
		END
	};
    [SerializeField]
    private bool _newTab;

	[SerializeField]
	private ContributeOrigin _origin;

    private const string _urlKey = "STUDY.LEARN.LINK";

    public override void click()
	{
		// Debug.Log(this.GetType() + " clicked "+itemName);
		base.click ();
		RedMetricsManager.get ().sendEvent (TrackingEvent.SELECTMENU, new CustomData (CustomDataTag.OPTION, getDataValue()));
		StudyFormLinker.openForm ();
	}


	private string getDataValue()
	{
		string res = "";

		switch (_origin) {
		case ContributeOrigin.MAINMENU:
			res = CustomDataValue.CONTRIBUTEMAINMENU.ToString ();
			break;
		case ContributeOrigin.HUD:
			res = CustomDataValue.CONTRIBUTEHUD.ToString ();
			break;
		case ContributeOrigin.QUIT:
			res = CustomDataValue.CONTRIBUTEEND.ToString ();
			break;
		case ContributeOrigin.END:
			res = CustomDataValue.CONTRIBUTEQUIT.ToString ();
			break;
		}
		return res;
	}
}
