using UnityEngine;

public class ContributeLinkButton : MonoBehaviour {
    [SerializeField]
	private CustomDataValue _dataValue;
#if ARCADE
    void Start()
    {
        gameObject.SetActive(false);
    }
#endif
	private void OnPress(bool isPressed)
    {
        if(isPressed) {
            RedMetricsManager.get ().sendEvent (TrackingEvent.SELECTMENU, new CustomData (CustomDataTag.OPTION, _dataValue.ToString()));
		    StudyFormLinker.openFormGame (true);
        }   
    }
}
