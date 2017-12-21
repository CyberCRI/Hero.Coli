using UnityEngine;

// don't inherit StepByStepTutorial
public class StudyHint : MonoBehaviour
{
    private float _elapsedTime = 0f;
    private float _displayedTime = 0f;
    private const float _waitThreshold = 45.0f;
    private const float _displayThreshold = 10.0f;
    private const string _messageCode = "STUDY";
    [SerializeField]
    private GameObject _speechBubble;

#if ARCADE
    void Start()
    {
        Destroy(this);
    }
#endif

    // Update is called once per frame
    void Update()
    {
        if (_displayedTime > 0)
        {
            if (_displayedTime > _displayThreshold)
            {
                _speechBubble.SetActive(false);
                _displayedTime = 0;
                _elapsedTime = 0;
            }
            else
            {
                _speechBubble.transform.position = this.transform.position;
                _displayedTime += Time.deltaTime;
            }
        }
        else
        {
            if (_elapsedTime > _waitThreshold)
            {
                if (null != _speechBubble)
                {
                    _speechBubble.transform.position = this.transform.position;
                    _speechBubble.SetActive(true);
                    RedMetricsManager.get().sendEvent(TrackingEvent.HINT, new CustomData(CustomDataTag.MESSAGE, _messageCode));
                    _elapsedTime = 0;
                    _displayedTime += Time.deltaTime;
                }
            }
            else
            {

                if (
                    !Input.GetMouseButton(0)
                    && !Input.GetMouseButtonUp(0)
                    && (0 == Input.GetAxis("Horizontal"))
                    && (0 == Input.GetAxis("Vertical"))
                )
                {
                    _elapsedTime += Time.deltaTime;
                }
                else if (0 != _elapsedTime)
                {
                    _elapsedTime = 0;
                }
            }
        }
    }
}

