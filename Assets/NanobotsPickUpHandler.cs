using UnityEngine;
using System.Collections;

public class NanobotsPickUpHandler : MonoBehaviour {

    private NanobotsCounter _nanoCount;
    private int _pickedUpNumber = 0;

	// Use this for initialization
	void Start () {
        _nanoCount = GameObject.Find("NanobotsIndicator").GetComponent<NanobotsCounter>();
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
            _nanoCount.UpdateLabel(_pickedUpNumber);
            Destroy(this.gameObject);
        }
    }
}
