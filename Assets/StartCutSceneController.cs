using UnityEngine;
using System.Collections;

public class StartCutSceneController : MonoBehaviour {

    [SerializeField]
    private iTweenEvent _iTweenEventBacteria;
    [SerializeField]
    private iTweenEvent _itweenEventBacteria2;
    [SerializeField]
    private GameObject _dnaTube;
    [SerializeField]
    private iTweenEvent _iTweenEventDNA;
    [SerializeField]
    private TweenScale _tweenScale;
    private Vector3 _originFromTweenScale;
    private Vector3 _originToTweenScale;
    private CellControl _cellControl;

	// Use this for initialization
	void Start () {
        _originFromTweenScale = _tweenScale.from;
        _originToTweenScale = _tweenScale.to;
        _cellControl = GameObject.FindGameObjectWithTag("Player").GetComponent<CellControl>();
        StartCutScene();
    }

	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            StartCutScene();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _dnaTube.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            _iTweenEventDNA.enabled = true;
            _iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            ReverseAnim();
            _iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            _itweenEventBacteria2.enabled = true;
        }
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
            _cellControl.FreezePLayer(false);
            //_cellControl.GetComponent<PhenoSpeed>().setDefaultFlagellaCount(1);
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
    }

    void StartCutScene()
    {
        _iTweenEventBacteria.enabled = true;
        _cellControl.FreezePLayer(true);
        StartCoroutine(WaitForSecondPart());
    }

    void SecondPart()
    {
        _dnaTube.SetActive(true);
        StartCoroutine(WaitForThirdPart());
    }

    void ThirdPart()
    {
        _iTweenEventDNA.enabled = true;
        _iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(WaitForFourthPart());
    }

    void FourthPart()
    {
        ReverseAnim();
        //_iTweenEventDNA.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        StartCoroutine(WaitForFifthPart());
    }

    void FifthPart()
    {
        _itweenEventBacteria2.enabled = true;
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
}
