// #define DEV
using UnityEngine;

public class CutSceneBlackBarHandler : MonoBehaviour {

    [SerializeField]
    private Animator _topAnimation;
    [SerializeField]
    private Animator _bottomAnimation;
	
    void Awake()
    {
        /*_bottomAnimation.speed = 0;
        _topAnimation.speed = 0;*/
    }

	// Update is called once per frame
	void Update () {
#if DEV
	    if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            closeBar(true);
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            closeBar(false);
        }
#endif
    }

    public void closeBar(bool close)
    {
        _bottomAnimation.SetBool("Close", close);
        _bottomAnimation.SetBool("Start", false);
        _topAnimation.SetBool("Close", close);
        _topAnimation.SetBool("Start", false);
    }
}
