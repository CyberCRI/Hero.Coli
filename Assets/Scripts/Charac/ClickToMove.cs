using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
    
    private int _smooth; // Determines how quickly object moves towards position
    private float _hitdist = 0.0f;
    
    private Vector3 targetPosition;
    
     
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {                
        if(Input.GetKeyDown(KeyCode.Mouse0))            
        {
            _smooth = 1;

            Plane playerPlane = new Plane(Vector3.up, transform.position);            
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);            

            if (playerPlane.Raycast (ray, out _hitdist)) {                
                Vector3 targetPoint = ray.GetPoint(_hitdist);                
                targetPosition = ray.GetPoint(_hitdist);                
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);                
                transform.rotation = targetRotation;
            }
        }
        transform.position = Vector3.Slerp (transform.position, targetPosition, Time.deltaTime * _smooth); 
	}
}
