using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Device : MonoBehaviour {
	
	private static string _activeSuffix = "Active";
	private static List<string> spriteNames = new List<string>( new string [] {
		"Backdrop"
		,"brick"
		,"brickNM"
		,"burlap"
		,"sand"
	});
	
	
	public string _currentSpriteName;
	public UIAtlas _atlas;
	public bool _isActive;
	public int _deviceID;
	public UISprite _sprite;
	
	
	public int getID() {
		return _deviceID;
	}
	
	public static Object prefab = Resources.Load("GUI/screen1/EquipedDevices/DeviceSpritePrefab");
	public static Device Create(Transform parentTransform, Vector3 localPosition, int deviceID)
	{
	    GameObject newDevice = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		newDevice.transform.parent = parentTransform;
		newDevice.transform.localPosition = localPosition;
		newDevice.transform.localScale = new Vector3(50f, 50f, 0);
		
	    Device deviceScript = newDevice.GetComponent<Device>();
		deviceScript._deviceID = deviceID;
		deviceScript._sprite = newDevice.GetComponent<UISprite>();
	 
	    //do additional initialization steps here
	 
	    return deviceScript;
	}
	
	public void Remove() {
		Destroy(gameObject);
	}
	
	public void Redraw(Vector3 newLocalPosition) {
		gameObject.transform.localPosition = newLocalPosition;
	}
	
	private void setSprite(string spriteName) {
		Debug.Log("setSprite("+spriteName+")");
		_sprite.spriteName = spriteName;
	}
	
	public void setActivity(bool activity) {
		_isActive = activity;
		if(activity) {
			setActive();
		} else {
			setInactive();
		}
	}
	
	public void setActive() {
		Debug.Log("setActive");
		_isActive = true;
		setSprite(_currentSpriteName + _activeSuffix);		
	}
	
	public void setInactive() {
		Debug.Log("setInactive");
		_isActive = false;
		setSprite(_currentSpriteName);
	}
	
	private string getRandomSprite() {
		int randomIndex = Random.Range(0, spriteNames.Count);
		return spriteNames[randomIndex];
	}
	
	// Use this for initialization
	void Start () {
		//_sprite.atlas = Resources.Load("Atlases/TestAtlas") as UIAtlas;
		_sprite.atlas = _atlas;
		_currentSpriteName = getRandomSprite();
		setActive();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
