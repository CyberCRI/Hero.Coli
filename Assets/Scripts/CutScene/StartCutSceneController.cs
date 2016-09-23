// #define QUICKTEST
using UnityEngine;
using System.Collections;

public class StartCutSceneController : CutScene {

    [SerializeField]
    private iTweenEvent _iTweenEventBacteria;
    [SerializeField]
    private iTweenEvent _itweenEventBacteria2;
    private Transform _bacteriaTransform;
    [SerializeField]
    private GameObject _dnaTube;
    [SerializeField]
    private iTweenEvent _iTweenEventDNA;
    [SerializeField]
    private PickableDeviceRef4Bricks[] _devices;
    [SerializeField]
    private TweenScale _tweenScale;
    [SerializeField]
    private GameObject _dialogBubble;
    [SerializeField]
    private GameObject _dialogBubblePlayer;
    [SerializeField]
    private TextMesh _textFieldDummy;
    [SerializeField]
    private TextMesh _textFieldPlayer;
    private Vector3 _originFromTweenScale;
    private Vector3 _originToTweenScale;
    private bool _scaleUp = true;
    private bool _scaling = false;

#if QUICKTEST
    private float[] waitTimes = new float[9]{0.1f,0.1f,0.1f,0.1f,0.1f,0.1f,0.1f,0.1f,0.1f};
    private const float _tweenScaleDuration = 0.1f;
    private const float _pulseSpeed = 50f;
#else
    private float[] waitTimes = new float[9]{5f,3.5f,1f,0.5f,5f,1.5f,2f,0.5f,3f};
    private const float _tweenScaleDuration = 1.5f;
    private const float _pulseSpeed = 1f;
#endif

    // Use this for initialization
    public override void initialize()
    {

        _originFromTweenScale = _tweenScale.from;
        _originToTweenScale = _tweenScale.to;
        _bacteriaTransform = _iTweenEventBacteria.GetComponent<Transform>();

        StartCoroutine(waitForBeginning());
        //start();
    }

    IEnumerator waitForBeginning()
    {   
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

    void OnTriggerEnter(Collider col)
    {
        /*if (col.gameObject.name == "dummyPlayer")
        {
            end ();
            Destroy(col.gameObject);
        }*/
    }
    
    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
        //Destroy(this.gameObject.transform.parent.gameObject);
        FocusMaskManager.get().blockClicks(false);
        Destroy(this.transform.parent.gameObject);
    }

    public override void startCutScene()
    {
        _cellControl.freezePlayer(true);
        _iTweenEventBacteria.enabled = true;
        StartCoroutine(waitForFirstPart());
    }

    void firstPart()
    {
        _dialogBubble.SetActive(true);
        _textFieldDummy.text = "!";
    }

    void secondPart()
    {
        _dnaTube.SetActive(true);
        _dialogBubblePlayer.SetActive(true);
        _textFieldPlayer.text = "?";
        StartCoroutine(waitForThirdPart());
    }

    void thirdPart()
    {
        _dialogBubblePlayer.SetActive(true);
        _textFieldPlayer.text = "!?";
        _iTweenEventDNA.enabled = true;
        _iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(waitForFourthPart());
    }

    void fourthPart()
    {
        _dialogBubblePlayer.SetActive(false);
        reverseAnim();
        //_iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(waitForFifthPart());
        _scaling = true;
        StartCoroutine(scalePulse(_iTweenEventDNA.GetComponent<Transform>(), _pulseSpeed));
    }

    void fifthPart()
    {
        _itweenEventBacteria2.enabled = true;
        //_iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

        StartCoroutine(waitForEnd());
    }

    IEnumerator scalePulse(Transform objTransform, float speed)
    {
        bool test = false;
        while (_scaling == true)
        {
            if (test == true)
            {
            }
            if (_scaleUp == true && objTransform.localScale.x < 2f)
            {
                objTransform.localScale = new Vector3(objTransform.localScale.x + (Time.deltaTime * speed), objTransform.localScale.y + (Time.deltaTime * speed), objTransform.localScale.z + (Time.deltaTime * speed));
                if (objTransform.localScale.x > 2f)
                {
                    _scaleUp = false;
                }
            }
            if (_scaleUp == false)
            {
                objTransform.eulerAngles = new Vector3(objTransform.eulerAngles.x, objTransform.eulerAngles.y - Time.deltaTime * 200, objTransform.eulerAngles.z);
            }
            if (_scaleUp == false && objTransform.localScale.x > 0.1f)
            {
                objTransform.localScale = new Vector3(objTransform.localScale.x - (Time.deltaTime * speed), objTransform.localScale.y - (Time.deltaTime * speed), objTransform.localScale.z - (Time.deltaTime * speed));
            }
            else if (_scaleUp == false && _iTweenEventDNA.transform != null && _iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled == false)
            {
                _iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
                _scaling = false;
                
                foreach(PickableDeviceRef4Bricks device in _devices)
                {
                    CraftZoneManager.get().addAndEquipDevice(device.getDNABit() as Device, false);
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
        _dialogBubble.SetActive(true);
        _textFieldDummy.text = "!";
        yield return new WaitForSeconds(waitTimes[2]);
        _dialogBubblePlayer.SetActive(true);
        _textFieldPlayer.text = "!";
        _iTweenEventBacteria.enabled = false;
        yield return new WaitForSeconds(waitTimes[3]);
        float currentAngle = 0;
        _dialogBubble.SetActive(false);
        _dialogBubblePlayer.SetActive(false);
        while (currentAngle < 810)
        {
            _bacteriaTransform.eulerAngles = new Vector3(_bacteriaTransform.eulerAngles.x, _bacteriaTransform.eulerAngles.y + Time.deltaTime * 1000, _bacteriaTransform.eulerAngles.z);
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
        _dialogBubblePlayer.SetActive(false);
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
        Hero.get().gameObject.AddComponent<CraftDiscoveryHint>();
        end();
        yield return null;
    }
}
