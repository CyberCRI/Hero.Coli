using UnityEngine;
using System.Collections;

public class CutSceneBlackBarHandler : MonoBehaviour {

    [SerializeField]
    private Animator _animTop;
    [SerializeField]
    private Animator _animBottom;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            CloseBar(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            CloseBar(false);
        }

    }

    public void CloseBar(bool value)
    {
        _animBottom.SetBool("Close", value);
        _animTop.SetBool("Close", value);
    }
}
