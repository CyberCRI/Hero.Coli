using UnityEngine;
using System.Collections;

public class BlackLight : MonoBehaviour {


	public bool isActive = false;
	private bool _isCreating = false;			//used for the lerp during the creation of the black light
	private bool _isDestroying = false;			

	private Light _directionalLight;
	private float _initialIntensity;



	private Camera _camera;
	private Color _initialBackground;

	//Spotlight values
	private GameObject _spotLightBL;
	private float _range = 115f;
	private float _angle = 125f;
	private float _intensity = 0.25f;

	//BackgroundSpotlight values
	private GameObject _backGroundSpotlight;
	private float _bkRange = 70f;
	private float _bkAngle = 125f;
	private float _bkIntensity = 2.5f;
	private Color32 _bkColor = new Color32(55,65,172,255);
	

	private bool _shaderChanged = false;
  private static string _blackLight = "BL.";
  private static string _blackLightOn = _blackLight+"ON";
  private static string _blackLightOff = _blackLight+"OFF";

	// Use this for initialization
	void Start () {
	
		_directionalLight = GameObject.Find("Directional light").GetComponent<Light>();
		_initialIntensity = _directionalLight.GetComponent<Light>().intensity;

		_camera = transform.parent.FindChild("Main Camera").GetComponent<Camera>();
		_initialBackground = _camera.backgroundColor;

		setDiffuseTransparentPlane();
	}
	
	// Update is called once per frame
	void Update () {


		lerpLight();
		switchLight();


	
	}

	public void createBlackLight () {

		_isCreating = true;
		_isDestroying = false;


		//Add spotlight 
		_spotLightBL = new GameObject();

		_spotLightBL.name = "spotLightBL";
		_spotLightBL.transform.parent = transform;
		_spotLightBL.transform.localPosition = new Vector3(0f,0.05f,0f);
		_spotLightBL.transform.localEulerAngles = new Vector3(90f,0f,0f);

		_spotLightBL.AddComponent<Light>();

		_spotLightBL.GetComponent<Light>().type = LightType.Spot;
		_spotLightBL.GetComponent<Light>().range = _range;
		_spotLightBL.GetComponent<Light>().spotAngle = _angle;
		_spotLightBL.GetComponent<Light>().intensity = 0f;
		_spotLightBL.GetComponent<Light>().color = Color.white;

		//Add backGroundSpotlight
		 _backGroundSpotlight = new GameObject();
		
		_backGroundSpotlight.name = "backGroundSpotlight";
		_backGroundSpotlight.transform.parent = transform;
		_backGroundSpotlight.transform.localPosition = new Vector3(0f,-0.25f,0f);
		_backGroundSpotlight.transform.localEulerAngles = new Vector3(90f,0f,0f);
		
		_backGroundSpotlight.AddComponent<Light>();
		
		_backGroundSpotlight.GetComponent<Light>().type = LightType.Spot;
		_backGroundSpotlight.GetComponent<Light>().range = _bkRange;
		_backGroundSpotlight.GetComponent<Light>().spotAngle = _bkAngle;
		_backGroundSpotlight.GetComponent<Light>().intensity = 0;
		_backGroundSpotlight.GetComponent<Light>().color = new Color32(_bkColor.r,_bkColor.g,_bkColor.b,_bkColor.a);


	}

	public void leaveBlackLight () {

		_isCreating = false;
		_isDestroying = true;

		//Destroy BackgroundSpotlight
		Destroy (gameObject.transform.FindChild("backGroundSpotlight").gameObject);

	}


	// change the shader of the background planes in the first asset. The shaders from the other assets are already "Transparent/Diffuse"
	public void setDiffuseTransparentPlane ()
	{

		// change shaders of the fog in the level 1
		GameObject go = GameObject.Find ("Assets Level1").transform.FindChild("Environmental Graphics").FindChild("BackgroundFog").gameObject;


		Transform[] allChildren = go.GetComponentsInChildren<Transform>();

		foreach(Transform child in allChildren)
		{
			if(child.name.Contains("Fog") && child.FindChild("plane"))
			{
				child.FindChild("plane").GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
			}
		}

		_shaderChanged = true;

				
	}

	// add lerp effect on light and color for the transition between normal light and black light
	public void lerpLight ()
	{
		if(_isCreating)
		{
			_camera.backgroundColor = Color.Lerp (_camera.backgroundColor, Color.black, 3*Time.deltaTime);
			_directionalLight.intensity = Mathf.Lerp (_directionalLight.intensity, 0f, 3*Time.deltaTime);
			_spotLightBL.GetComponent<Light>().intensity = Mathf.Lerp(_spotLightBL.GetComponent<Light>().intensity, _intensity, 1.5f*Time.deltaTime);
			_backGroundSpotlight.GetComponent<Light>().intensity = Mathf.Lerp(_backGroundSpotlight.GetComponent<Light>().intensity, _bkIntensity, 3*Time.deltaTime);
			//when finished
			if (_camera.backgroundColor == Color.black)
				_isCreating = false;
		}
		
		if(_isDestroying)
		{
			_camera.backgroundColor = Color.Lerp (_camera.backgroundColor, _initialBackground, 1.5f*Time.deltaTime);
			_directionalLight.intensity = Mathf.Lerp (_directionalLight.intensity, _initialIntensity, 1.5f*Time.deltaTime);
			_spotLightBL.GetComponent<Light>().intensity = Mathf.Lerp(_spotLightBL.GetComponent<Light>().intensity, 0f, 3f*Time.deltaTime);
			//when finished
			if (_camera.backgroundColor == _initialBackground)
			{
				Destroy(gameObject.transform.FindChild("spotLightBL").gameObject);
				
				_isDestroying = false;
			}
		}
	}


	// light on/off for the black light
	public void switchLight ()
	{
    if (GameStateController.isShortcutKey(GameStateController.keyPrefix+_blackLightOn)
                && !isActive
       )
		{
			createBlackLight();
			if(!_shaderChanged)
				setDiffuseTransparentPlane();
			isActive = true;
		}
    if (GameStateController.isShortcutKey(GameStateController.keyPrefix+_blackLightOff)
            && isActive
       )
		{
			leaveBlackLight();
			isActive = false;
		}
	}
}
