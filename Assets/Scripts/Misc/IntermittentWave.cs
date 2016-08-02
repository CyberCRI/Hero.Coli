using UnityEngine;
using System.Collections;

public class IntermittentWave : MonoBehaviour {

    private ParticleSystem _child;
    [SerializeField]
    private float _timer = 3f;
    private float _timerOrigin;
    [SerializeField]
    private bool _on = true;
    [SerializeField]
    private bool _waitIsDifferent;
    [SerializeField]
    private float _timerWait;

	// Use this for initialization
	void Start () {
        _child = transform.GetChild(0).GetComponent<ParticleSystem>();
        _timerOrigin = _timer;
	}
	
	// Update is called once per frame
	void Update () {

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = _timerOrigin;
            ActivateChild();
        }

        
    }

    void ActivateChild()
    {
        if (_on == false)
        {
            _child.Play();
            _on = true;
        }
        else if (_on == true)
        {
            _child.Stop(true);
            _on = false;
            if (_waitIsDifferent == true)
            {
                _timer = _timerWait;
            }
        }
    }
}
