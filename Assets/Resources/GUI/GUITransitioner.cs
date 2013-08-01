using UnityEngine;
using System.Collections;

public class GUITransitioner : MonoBehaviour {
	
	private float _timeDelta = 0.2f;
	
    private float _timeAtLastFrame = 0f;
    private float _timeAtCurrentFrame = 0f;
    private float _deltaTime = 0f;
	
	public GameScreen _currentScreen = GameScreen.screen1;
	public enum GameScreen {
		screen1,
		screen2,
		screen3
	};
	
	
	public GameObject _mainCameraObject;
	private cameraFollow _mainCameraFollow;
	
	public GameObject _worldScreen;
	public GameObject _craftScreen;
	public DevicesDisplayer _devicesDisplayer;
	// Use this for initialization
	void Start () {
		SetScreen2(false);
		SetScreen3(false);
		SetScreen1(true);
		
		Pause(false);
		_devicesDisplayer.UpdateScreen();
		_currentScreen = GameScreen.screen1;
		
		_timeAtLastFrame = Time.realtimeSinceStartup;
	}
	
	
	private void SetScreen1(bool active) {
		if(active) ZoomOut();
		_worldScreen.SetActive(active);
	}
	
	private void SetScreen2(bool active) {
		if(active) ZoomIn();
		_worldScreen.SetActive(active);
	}
	
	private void SetScreen3(bool active) {
		_craftScreen.SetActive(active);
	}
	
	private void checkCamera() {
		if(_mainCameraFollow == null) {
			_mainCameraFollow = _mainCameraObject.GetComponent<cameraFollow>() as cameraFollow;
		}
	}
	
	private void ZoomIn() {
		checkCamera();
		_mainCameraFollow.SetZoom(true);
	}
	private void ZoomOut() {
		checkCamera();
		_mainCameraFollow.SetZoom(false);
	}
		
	
	private void Pause(bool pause) {
		Time.timeScale = pause?0:1;
	}
	
	// Update is called once per frame
	void Update () {
		
		_timeAtCurrentFrame = Time.realtimeSinceStartup;
        _deltaTime = _timeAtCurrentFrame - _timeAtLastFrame;
		
		if(_deltaTime > _timeDelta) {
			if (Input.GetKey(KeyCode.Alpha1)) {//GOTO screen1
				if(_currentScreen == GameScreen.screen2) {
					Debug.Log("2->1");
					//2 -> 1
					//set zoom1
					//remove inventory device, deviceID
					//add graphs
					//move devices and potions?
					
				} else if(_currentScreen == GameScreen.screen3) {
					Debug.Log("3->1");					
					//3 -> 1
					//set zoom1
					//remove craft screen
					//add graphs
					//add potions
					//add devices
					//add life/energy
					//add medium info
					SetScreen3(false);
					SetScreen1(true);
				}
				Pause(false);
				ZoomOut();
				_currentScreen = GameScreen.screen1;
				_devicesDisplayer.UpdateScreen();
				
			} else if (Input.GetKey(KeyCode.Alpha2)) {//GOTO screen2
				if(_currentScreen == GameScreen.screen1) {
					Debug.Log("1->2");
					//1 -> 2
					//set zoom2
					//add inventory device, deviceID
					//remove graphs
					//move devices and potions?
					
				} else if(_currentScreen == GameScreen.screen3) {
					Debug.Log("3->2");					
					//3 -> 1
					//set zoom2
					//remove craft screen
					//add inventory device, deviceID
					//add potions
					//add devices
					//add life/energy
					//add medium info
					SetScreen3(false);
					SetScreen2(true);
				}
				
				Pause(true);
				ZoomIn();
				_currentScreen = GameScreen.screen2;			
				_devicesDisplayer.UpdateScreen();
				
			} else if (Input.GetKey(KeyCode.Alpha3)) {//GOTO screen3
				if(_currentScreen == GameScreen.screen1) {
					Debug.Log("1->3");
					//1 -> 3
					//remove everything
					//add device inventory, parameters
					//remove graphs
					//move devices and potions?
					SetScreen1(false);
					SetScreen3(true);
					
				} else if(_currentScreen == GameScreen.screen2) {
					Debug.Log("2->3");					
					//3 -> 1
					//remove everything
					//add craft screen
					SetScreen2(false);
					SetScreen3(true);
					
				}	
				Pause(true);
				ZoomIn();					
				_currentScreen = GameScreen.screen3;
				_devicesDisplayer.UpdateScreen();			
				
			}
			
        	_timeAtLastFrame = _timeAtCurrentFrame;
		}
	}
}
