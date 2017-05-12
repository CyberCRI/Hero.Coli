using UnityEngine;

// don't inherit StepByStepTutorial
public class PutMovBackHint : MonoBehaviour
{
    private float _elapsedTime = 0f;
    private const float _waitThreshold = 15.0f;
    private const string _infoWindowCode = "PUTMOVBACK";

    // Update is called once per frame
    void Update()
    {
        if (_elapsedTime > _waitThreshold)
        {
            InfoWindowManager.displayInfoWindow(_infoWindowCode);
            RedMetricsManager.get().sendEvent(TrackingEvent.HINT, new CustomData(CustomDataTag.MESSAGE, _infoWindowCode.ToString()));
            _elapsedTime = 0;
        }
        else if (
               (CraftFinalizer.get().isEquipped(StepByStepTutorial.moveDevice1))
            || (CraftFinalizer.get().isEquipped(StepByStepTutorial.moveDevice2))
            || (CraftFinalizer.get().isEquipped(StepByStepTutorial.moveDevice3))
            )
        {
            Destroy(this);
        }
        else
        {
            _elapsedTime += Time.deltaTime;
        }
    }
}
