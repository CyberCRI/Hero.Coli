using UnityEngine;

public class MovementHint : MonoBehaviour {

    private float _elapsedTime = 0f;
    private float _pressedTime = 0f;
    private int _clicksCount = 0;
    
    private const float _waitThreshold = 5.0f;
    private const float _clickWaitThreshold = 1f;
    private const int _clicksThreshold = 5;
	
    private const string _infoWindowCode = "MOVEMENT";
        
	// Update is called once per frame
	void Update () {
        
        
	   if(_elapsedTime > _waitThreshold)
       {
           InfoWindowManager.displayInfoWindow(_infoWindowCode);
           Destroy(this);
       }
       else if ((_pressedTime > _clickWaitThreshold)
           || (_clicksCount >= _clicksThreshold))
       {
           Destroy(this);
       }
       else
       {
           _elapsedTime += Time.deltaTime;
           if(Input.GetMouseButton(0))
           {
               _pressedTime += Time.deltaTime;
           }
           if(Input.GetMouseButtonUp(0))
           {
               _clicksCount++;
           }
       }
	}
}
