using UnityEngine;

public class CutSceneBlackBarHandler : MonoBehaviour {

    [SerializeField]
    private Animator _topAnimation;
    [SerializeField]
    private Animator _bottomAnimation;
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            closeBar(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            closeBar(false);
        }

    }

    public void closeBar(bool close)
    {
        _bottomAnimation.SetBool("Close", close);
        _topAnimation.SetBool("Close", close);
    }
}
