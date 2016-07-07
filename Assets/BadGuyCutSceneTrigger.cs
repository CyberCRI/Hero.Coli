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

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
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
            }
            yield return null;
        }
        Debug.Log("Fin Enum");

        yield return null;
    }
}
