using UnityEngine;
using System.Collections;

public class BadGuyCutSceneTrigger : MonoBehaviour {

    private bool _first = true;
    [SerializeField]
    private iTweenEvent _iTweenEvent;
    private bool _isSlowingAnimation = false;
    [SerializeField]
    private float slowSpeed = 1;
    [SerializeField]
    private GameObject _deadBigBadGuy;
    [SerializeField]
    private BoundCamera _mainCamBound;
    private Transform _initialTarget;
    private CellControl _cellControl;
    private bool _freezePlayer = false;

    // Use this for initialization
    void Start () {
        _mainCamBound = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BoundCamera>();
        _initialTarget = _mainCamBound.target;
        _cellControl = GameObject.FindGameObjectWithTag("Player").GetComponent<CellControl>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (_freezePlayer == true)
        {
            _cellControl.stopMovement();
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "CutSceneElement")
        {
            Debug.Log("EndCutScene");
            _isSlowingAnimation = true;
            StartCoroutine(SlowAnimation(col.gameObject));
        }

        if (col.tag == "Player")
        {
            if (_first == true)
            {
                Debug.Log("Start Cutscene");
                _first = false;
                _iTweenEvent.enabled = true;
                _mainCamBound.target = _iTweenEvent.gameObject.transform;
                _freezePlayer = true;
            }
        }
    }

    IEnumerator SlowAnimation(GameObject bigbadGuy)
    {
        Debug.Log("WTF");
        var anim = bigbadGuy.GetComponent<Animation>() as Animation;
        while (_isSlowingAnimation == true)
        {
            float animSPeed = anim["medusa_slow"].speed;
            animSPeed -= 1 / (100 / slowSpeed);
            Debug.Log(animSPeed);
            anim["medusa_slow"].speed = animSPeed;
            if (animSPeed <= 0)
            {
                _isSlowingAnimation = false;
                //var instance = Instantiate(_deadBigBadGuy, bigbadGuy.transform.position, bigbadGuy.transform.rotation) as GameObject;
                _deadBigBadGuy.SetActive(true);
                Destroy(bigbadGuy.gameObject);
                _mainCamBound.target = _initialTarget;
                _freezePlayer = false;
            }
            yield return null;
        }
        Debug.Log("Fin Enum");

        yield return null;
    }
}
