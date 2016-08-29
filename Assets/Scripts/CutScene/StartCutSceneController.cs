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

    // Use this for initialization
    public override void initialize()
    {

        _originFromTweenScale = _tweenScale.from;
        _originToTweenScale = _tweenScale.to;
        _bacteriaTransform = _iTweenEventBacteria.GetComponent<Transform>();
        
        start();
    }

    void Update()
    {
        _cellControl.freezePlayer(true);
    }

    void ReverseAnim()
    {
        _tweenScale.from = _originToTweenScale;
        _tweenScale.to = _originFromTweenScale;
        _tweenScale.duration = 1.5f;
        _tweenScale.enabled = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.name == "dummyPlayer")
        {
            Destroy(col.gameObject);
            end ();
        }
    }
    
    public override void endCutScene()
    {
        _cellControl.gameObject.AddComponent<MovementHint>();
        Destroy(this.gameObject.transform.parent.gameObject);
    }

    public override void startCutScene()
    {
        _cellControl.freezePlayer(true);
        _iTweenEventBacteria.enabled = true;
        StartCoroutine(WaitForFirstPart());
    }

    void FirstPart()
    {
        _dialogBubble.SetActive(true);
        _textFieldDummy.text = "!";
    }

    void SecondPart()
    {
        _dnaTube.SetActive(true);
        _dialogBubblePlayer.SetActive(true);
        _textFieldPlayer.text = "?";
        StartCoroutine(WaitForThirdPart());
    }

    void ThirdPart()
    {
        _dialogBubblePlayer.SetActive(true);
        _textFieldPlayer.text = "!?";
        _iTweenEventDNA.enabled = true;
        _iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(WaitForFourthPart());
    }

    void FourthPart()
    {
        _dialogBubblePlayer.SetActive(false);
        ReverseAnim();
        //_iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(WaitForFifthPart());
        _scaling = true;
        StartCoroutine(ScalePulse(_iTweenEventDNA.GetComponent<Transform>(), 1f));
    }

    void FifthPart()
    {
        _itweenEventBacteria2.enabled = true;
        //_iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;

        StartCoroutine(WaitForEnd());
    }

    IEnumerator ScalePulse(Transform objTransform, float speed)
    {
        bool test = false;
        while (_scaling == true)
        {
            if (test == true)
            {
                Debug.Log("2");
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
                Debug.Log("1");
                _iTweenEventDNA.transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
                _scaling = false;
                ///////////////////Edit behind to fix bug "Coroutine couldn't be started because the the game object 'CraftSlot1(Clone)' is inactive!"
                CraftZoneManager.get().setDevice(_iTweenEventDNA.transform.GetChild(0).GetComponent<PickableDeviceRef4Bricks>().getDNABit() as Device);
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator WaitForFirstPart()
    {
        yield return new WaitForSeconds(3.5f);
        _dialogBubble.SetActive(true);
        _textFieldDummy.text = "!";
        yield return new WaitForSeconds(1f);
        _dialogBubblePlayer.SetActive(true);
        _textFieldPlayer.text = "!";
        _iTweenEventBacteria.enabled = false;
        yield return new WaitForSeconds(0.5f);
        float turn = 0f;
        float currentAngle = 0;
        _dialogBubble.SetActive(false);
        _dialogBubblePlayer.SetActive(false);
        while (turn < 2f)
        {
            Debug.Log("1");
            _bacteriaTransform.eulerAngles = new Vector3(_bacteriaTransform.eulerAngles.x, _bacteriaTransform.eulerAngles.y + Time.deltaTime * 1000, _bacteriaTransform.eulerAngles.z);
            currentAngle += Time.deltaTime * 1000;
            if (currentAngle >= 810)
            {
                turn += 2;
            }
            yield return null;
        }
        SecondPart();
        yield return null;
    }

    IEnumerator WaitForSecondPart()
    {
        yield return new WaitForSeconds(5);
        SecondPart();
        yield return null;
    }

    IEnumerator WaitForThirdPart()
    {
        yield return new WaitForSeconds(1.5f);
        _dialogBubblePlayer.SetActive(false);
        ThirdPart();
        yield return null;
    }

    IEnumerator WaitForFourthPart()
    {
        yield return new WaitForSeconds(2);
        FourthPart();
        yield return null;
    }

    IEnumerator WaitForFifthPart()
    {
        yield return new WaitForSeconds(0.5f);
        FifthPart();
        yield return null;
    }

    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(2f);
        end();
        yield return null;
    }
}
