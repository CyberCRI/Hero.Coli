// #define DEV
using UnityEngine;

public class SimpleTeleportation : MonoBehaviour
{
    [SerializeField]
    private Vector3 _tpPoint1;
    [SerializeField]
    private Vector3 _tpPoint2;
    [SerializeField]
    private Vector3 _tpPoint3;

    // Use this for initialization
    void Start()
    {

        var tpObj1 = GameObject.FindGameObjectWithTag("TP1");
        var tpObj2 = GameObject.FindGameObjectWithTag("TP2");
        var tpObj3 = GameObject.FindGameObjectWithTag("TP3");
        _tpPoint1 = tpObj1.transform.position;
        _tpPoint2 = tpObj2.transform.position;
        _tpPoint3 = tpObj3.transform.position;
    }

#if DEV
	// Update is called once per frame
	void Update () {	    
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            this.transform.position = _tpPoint1;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            this.transform.position = _tpPoint2;
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            this.transform.position = _tpPoint3;
        }
	}
#endif
}
