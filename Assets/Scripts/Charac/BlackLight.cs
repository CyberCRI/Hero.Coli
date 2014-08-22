using UnityEngine;
using System.Collections;

public class BlackLight : MonoBehaviour {


	public bool isActivate = false;
	private Light _directionalLight;
	private float _initialIntensity;

	private Camera _camera;
	private Color _initialBackground;

	//Spotlight values
	private float _range = 115f;
	private float _angle = 125f;
	private float _intensity = 0.25f;

	//BackgroundSpotlight values
	private float _bkRange = 95f;
	private float _bkAngle = 125f;
	private float _bkIntensity = 2.5f;
	private Color32 _bkColor = new Color32(55,65,172,255);

	//RenderSetting values
	private float _density = 0.025f;
	private float _haloStrength = 0.2f;
	private float _initialHaloStrength;

	private bool _shaderChanged = false;

	// Use this for initialization
	void Start () {
	
		_directionalLight = GameObject.Find("Directional light").GetComponent<Light>();
		_initialIntensity = _directionalLight.light.intensity;

		_camera = transform.parent.FindChild("Main Camera").GetComponent<Camera>();
		_initialBackground = _camera.backgroundColor;

		_initialHaloStrength = RenderSettings.haloStrength;

		setDiffuseTransparentPlane();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("x") && !isActivate)
		{
			createBlackLight();
			/*if(!_shaderChanged)
				setDiffuseTransparentPlane();*/
			isActivate = true;
		}
		if(Input.GetKeyDown("c") && isActivate)
		{
			leaveBlackLight();
			isActivate = false;
		}

	
	}

	public void createBlackLight () {

		_directionalLight.intensity = 0f;
		_camera.backgroundColor = Color.black;

		//Add spotlight 
		GameObject spotLightBL = new GameObject();

		spotLightBL.name = "spotLightBL";
		spotLightBL.transform.parent = transform;
		spotLightBL.transform.localPosition = new Vector3(0f,0.05f,0f);
		spotLightBL.transform.localEulerAngles = new Vector3(90f,0f,0f);

		spotLightBL.AddComponent<Light>();

		spotLightBL.light.type = LightType.Spot;
		spotLightBL.light.range = _range;
		spotLightBL.light.spotAngle = _angle;
		spotLightBL.light.intensity = _intensity;
		spotLightBL.light.color = Color.white;

		//Add backGroundSpotlight
		GameObject backGroundSpotlight = new GameObject();
		
		backGroundSpotlight.name = "backGroundSpotlight";
		backGroundSpotlight.transform.parent = transform;
		backGroundSpotlight.transform.localPosition = new Vector3(0f,-0.3f,0f);
		backGroundSpotlight.transform.localEulerAngles = new Vector3(90f,0f,0f);
		
		backGroundSpotlight.AddComponent<Light>();
		
		backGroundSpotlight.light.type = LightType.Spot;
		backGroundSpotlight.light.range = _bkRange;
		backGroundSpotlight.light.spotAngle = _bkAngle;
		backGroundSpotlight.light.intensity = _bkIntensity;
		backGroundSpotlight.light.color = new Color32(_bkColor.r,_bkColor.g,_bkColor.b,_bkColor.a);

		//Change the render setting
	/*	RenderSettings.fog = true;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogMode = FogMode.ExponentialSquared;
		RenderSettings.fogDensity = 0.025f;

		RenderSettings.haloStrength = 0.2f;*/

	}

	public void leaveBlackLight () {

		_directionalLight.intensity = _initialIntensity;
		_camera.backgroundColor = _initialBackground;

		//Destroy spotlight
		Destroy(gameObject.transform.FindChild("spotLightBL").gameObject);

		//Destroy BackgroundSpotlight
		Destroy (gameObject.transform.FindChild("backGroundSpotlight").gameObject);


		//disable the fog
	/*	RenderSettings.fog = false;
		RenderSettings.haloStrength = _initialHaloStrength;*/
	}


	// change the shader of the background planes in the first asset
	public void setDiffuseTransparentPlane ()
	{

		// change shaders of the fog in the level 1
		GameObject go = GameObject.Find ("Assets Level1").transform.FindChild("Environmental Graphics").FindChild("BackgroundFog").gameObject;


		Transform[] allChildren = go.GetComponentsInChildren<Transform>();

		foreach(Transform child in allChildren)
		{
			if(child.name.Contains("Fog") && child.FindChild("plane"))
			{
				child.FindChild("plane").renderer.material.shader = Shader.Find("Transparent/Diffuse");
			}
		}

		_shaderChanged = true;

				
	}
}
