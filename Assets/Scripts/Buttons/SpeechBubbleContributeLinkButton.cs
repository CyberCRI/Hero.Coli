using UnityEngine;

public class SpeechBubbleContributeLinkButton : MonoBehaviour {
    [SerializeField]
	private CustomDataValue _dataValue;
#if ARCADE
    void Start()
    {
        gameObject.SetActive(false);
    }
#else
    void Start()
    {
        Physics.queriesHitTriggers = true;
    }
#endif
	void OnMouseDown()
    {
        RedMetricsManager.get ().sendEvent (TrackingEvent.SELECTMENU, new CustomData (CustomDataTag.OPTION, _dataValue.ToString()));
        StudyFormLinker.openFormGame (true);
    }
}