using UnityEngine;

public class NanobotsPickUpHandler : MonoBehaviour {

    private static NanobotsCounter _nanoCount;
    private static int _pickedUpNumber = 0;

	// Use this for initialization
	void Start () {
        _nanoCount = (null==_nanoCount)?GameObject.Find("NanobotsIndicator").GetComponent<NanobotsCounter>():_nanoCount;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            _pickedUpNumber += 1;
            if(_pickedUpNumber == 1)
            {
                ModalManager.setModal("T1_NANOBOT");
            }
            _nanoCount.updateLabel(_pickedUpNumber);
            Destroy(this.gameObject);
        }
    }
}
