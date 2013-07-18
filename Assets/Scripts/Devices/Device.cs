using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Device : MonoBehaviour {
	
	private static string _activeSuffix = "Active";
	private static string _prefix = "Textures/Devices/";
	private static List<string> textureNames = new List<string>( new string [] {
		"Backdrop"
		,"brick"
		,"brickNM"
		,"burlap"
		,"sand"
	});
	
	
	private UITexture _equipedDeviceIcon;
	private string _uri;
	private bool _isActive;
	private int _deviceID;
	
	
	
	public int getID() {
		return _deviceID;
	}
	
	public static Object prefab = Resources.Load("GUI/screen1/EquipedDevices/EquipedDeviceSlotPrefab");
	public static Device Create(Transform parentTransform, Vector3 localPosition, int deviceID)
	{
	    GameObject newDevice = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
		newDevice.transform.parent = parentTransform;
		newDevice.transform.localPosition = localPosition;
		newDevice.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		
	    Device yourObject = newDevice.GetComponent<Device>();
		yourObject._deviceID = deviceID;
	 
	    //do additional initialization steps here
	 
	    return yourObject;
	}
	
	public void Remove() {
		Destroy(gameObject);
	}
	
	public void Redraw(Vector3 newLocalPosition) {
		gameObject.transform.localPosition = newLocalPosition;
	}
	
	private void setTexture(string textureUri) {
		/*
		// "material" version 
		string materialUri = "Materials/Backdrop";		
		Material myMaterial = Resources.Load(materialUri, typeof(Material)) as Material;		
		_equipedDeviceIcon.material = myMaterial;
		_equipedDeviceIcon.material.mainTexture = myMaterial.mainTexture;
		*/
		
		// "_texture" version
		_equipedDeviceIcon.mainTexture = Resources.Load(textureUri, typeof(Texture)) as Texture;
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
		_isActive = true;
		setTexture(_uri + _activeSuffix);		
	}
	
	public void setInactive() {
		_isActive = false;
		setTexture(_uri);
	}
	
	private string getRandomTexture() {
		int randomIndex = Random.Range(0, textureNames.Count);
		return _prefix + textureNames[randomIndex];
	}
	
	// Use this for initialization
	void Start () {		
		_equipedDeviceIcon = transform.Find ("EquipedDeviceIcon").GetComponent<UITexture>();
		_uri = getRandomTexture();
		setActive();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
