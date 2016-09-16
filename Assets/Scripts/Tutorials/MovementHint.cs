using UnityEngine;

// don't inherit StepByStepTutorial
public class MovementHint : MonoBehaviour {

    private float _elapsedTime = 0f;
    private float _mousePressedTime = 0f;
    private int _mousePressCount = 0;
    private float _keyboardPressedTime = 0f;
    
    private const float _waitThreshold = 5.0f;
    private const float _pressedWaitThreshold = 1f;
    private const int _clicksThreshold = 5;
	
    private const string _infoWindowCode = "MOVEMENT";
        
	// Update is called once per frame
	void Update () {
        
        
	   if(_elapsedTime > _waitThreshold)
       {
           InfoWindowManager.displayInfoWindow(_infoWindowCode);
           Destroy(this);
       }
       else if (
                (_mousePressedTime > _pressedWaitThreshold)
           ||   (_mousePressCount >= _clicksThreshold)
           ||   (_keyboardPressedTime >= _pressedWaitThreshold)
           )
       {
           Destroy(this);
       }
       else
       {
           _elapsedTime += Time.deltaTime;
           if(Input.GetMouseButton(0))
           {
               _mousePressedTime += Time.deltaTime;
           }
           if(Input.GetMouseButtonUp(0))
           {
               _mousePressCount++;
           }
           if(0 != Input.GetAxis("Horizontal") || 0 != Input.GetAxis("Vertical"))
           {
               _keyboardPressedTime += Time.deltaTime;
           }
       }
	}
}
