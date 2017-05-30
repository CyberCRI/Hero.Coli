// #define QUICKTEST

using UnityEngine;
using System.Collections;

public class StartCutScene : CutScene
{
    [SerializeField]
    private iTweenEvent _npcStartITweenEvent;
    [SerializeField]
    private iTweenEvent _npcEndITweenEvent;
    [SerializeField]
    private Transform _npcTransform;
    [SerializeField]
    private GameObject _dnaTube;
    [SerializeField]
    private iTweenEvent _iTweenEventDNA;
    [SerializeField]
    private PickableDeviceRef4Bricks[] _devices;
    [SerializeField]
    private PickableDeviceRef4Bricks[] _developmentDevices;
    [SerializeField]
    private bool _devMode;
    [SerializeField]
    private TweenScale _tweenScale;
    [SerializeField]
    private GameObject _npcDialogBubble;
    [SerializeField]
    private GameObject _playerDialogBubble;
    [SerializeField]
    private TextMesh _npcTextField;
    [SerializeField]
    private TextMesh _playerTextField;
    private Vector3 _originFromTweenScale;
    private Vector3 _originToTweenScale;
    private bool _scaleUp = true;
    private bool _scaling = false;
	private bool _first = true;
	[SerializeField]
	private PlayableUISound dialogSound;

#if QUICKTEST
    private float[] waitTimes = new float[9] { 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f };
    private const float _tweenScaleDuration = 0.1f;
    private const float _pulseSpeed = 50f;
#else
    private float[] waitTimes = new float[9]{5f,3.5f,1f,0.5f,5f,1.5f,2f,0.5f,3f};
    private const float _tweenScaleDuration = 1.5f;
    private const float _pulseSpeed = 1f;
#endif

    void say(string s, GameObject bubble, TextMesh text)
    {
		dialogSound.Play ();
        bubble.SetActive(true);
        text.text = s;
    }

    void makePlayerSay(string s)
    {
        say(s, _playerDialogBubble, _playerTextField);
    }

    void makeNPCSay(string s)
    {
        say(s, _npcDialogBubble, _npcTextField);
    }

	public override void initialize ()
	{
		_originFromTweenScale = _tweenScale.from;
		_originToTweenScale = _tweenScale.to;
	}

    IEnumerator waitForBeginning()
    {
        // Debug.Log(this.GetType() + "freezePlayer(true)");
        _cellControl.freezePlayer(true);
        yield return new WaitForSeconds(waitTimes[0]);
        start();
        yield return null;
    }

    void reverseAnim()
    {
        _tweenScale.from = _originToTweenScale;
        _tweenScale.to = _originFromTweenScale;
        _tweenScale.duration = _tweenScaleDuration;
        _tweenScale.enabled = true;
    }

    public override void endCutScene()
    {
        // FocusMaskManager.get().blockClicks(false);
        Destroy(this.transform.parent.gameObject);
        // Debug.Log(this.GetType() + "AddComponent CraftDiscoveryHint to Character");
        Character.get().gameObject.AddComponent<CraftDiscoveryHint>();
    }

    public override void startCutScene()
    {
        // Debug.Log(this.GetType() + "freezePlayer(true)");
        _cellControl.freezePlayer(true);
        _npcStartITweenEvent.enabled = true;
        StartCoroutine(waitForFirstPart());
    }

    void secondPart()
    {
        _dnaTube.SetActive(true);
        makePlayerSay("?");
        StartCoroutine(waitForThirdPart());
    }

    void thirdPart()
    {
        makePlayerSay("!?");
        _iTweenEventDNA.enabled = true;
        _iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(waitForFourthPart());
    }

    void fourthPart()
    {
        _playerDialogBubble.SetActive(false);
        reverseAnim();
        //_iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(waitForFifthPart());
        _scaling = true;
        StartCoroutine(scalePulse(_iTweenEventDNA.GetComponent<Transform>(), _pulseSpeed));
    }

    void fifthPart()
    {
        _npcEndITweenEvent.enabled = true;
        //_iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

        StartCoroutine(waitForEnd());
    }

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == Character.playerTag)
		{
			if (_first)
			{
				_first = false;
				StartCoroutine (waitForBeginning());
			}
		}
	}

    IEnumerator scalePulse(Transform objTransform, float speed)
    {
        while (_scaling)
        {
            
            if (_scaleUp && objTransform.localScale.x < 2f)
            {
                objTransform.localScale = new Vector3(objTransform.localScale.x + (Time.deltaTime * speed), objTransform.localScale.y + (Time.deltaTime * speed), objTransform.localScale.z + (Time.deltaTime * speed));
                
                _scaleUp = (objTransform.localScale.x <= 2f);
                
            }

            if (!_scaleUp)
            {
                objTransform.eulerAngles = new Vector3(objTransform.eulerAngles.x, objTransform.eulerAngles.y - Time.deltaTime * 200, objTransform.eulerAngles.z);
                if (objTransform.localScale.x > 0.1f)
                {
                    objTransform.localScale = new Vector3(objTransform.localScale.x - (Time.deltaTime * speed), objTransform.localScale.y - (Time.deltaTime * speed), objTransform.localScale.z - (Time.deltaTime * speed));
                }
                else if (_iTweenEventDNA.transform != null && !_iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled)
                {
                    _iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
                    _scaling = false;
                    
                    PickableDeviceRef4Bricks[] addedDevices = _devMode ? _developmentDevices : _devices;
                    CraftZoneManager czm = CraftZoneManager.get();
                    
                    int currentCount = czm.getSlotCount();
                    int targetCount = addedDevices.Length;
                    for (int index = 0; index < targetCount - currentCount; index++)
                    {
                        czm.addSlot();
                    }
                    
                    foreach(PickableDeviceRef4Bricks device in addedDevices)
                    {
                        czm.addAndEquipDevice(device.getDNABit() as Device, false);
                    }
                }
            }
            yield return null;
        }
        //end();
        yield return null;
    }

    IEnumerator waitForFirstPart()
    {
        yield return new WaitForSeconds(waitTimes[1]);
        makeNPCSay("!");
        yield return new WaitForSeconds(waitTimes[2]);
        makePlayerSay("!");
        _npcStartITweenEvent.enabled = false;
        yield return new WaitForSeconds(waitTimes[3]);
        float currentAngle = 0;
        _npcDialogBubble.SetActive(false);
        _playerDialogBubble.SetActive(false);
        while (currentAngle < 810)
        {
            _npcTransform.eulerAngles = new Vector3(_npcTransform.eulerAngles.x, _npcTransform.eulerAngles.y + Time.deltaTime * 1000, _npcTransform.eulerAngles.z);
            currentAngle += Time.deltaTime * 1000;
            yield return null;
        }
        secondPart();
        yield return null;
    }

    IEnumerator waitForSecondPart()
    {
        yield return new WaitForSeconds(waitTimes[4]);
        secondPart();
        yield return null;
    }

    IEnumerator waitForThirdPart()
    {
        yield return new WaitForSeconds(waitTimes[5]);
        _playerDialogBubble.SetActive(false);
        thirdPart();
        yield return null;
    }

    IEnumerator waitForFourthPart()
    {
        yield return new WaitForSeconds(waitTimes[6]);
        fourthPart();
        yield return null;
    }

    IEnumerator waitForFifthPart()
    {
        yield return new WaitForSeconds(waitTimes[7]);
        fifthPart();
        yield return null;
    }

    IEnumerator waitForEnd()
    {
        yield return new WaitForSeconds(waitTimes[8]);        
        end();
        yield return null;
    }
}
