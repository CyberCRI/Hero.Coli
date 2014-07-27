using UnityEngine;
using System.Collections;

public class EquipAndCraftButtonPanelPositioner : MonoBehaviour {

  public GraphMoleculeList list;
    private Vector3 _initialLocalPosition;
    
    // Use this for initialization
    void Awake () {
      _initialLocalPosition = transform.localPosition;
    }
    // Use this for initialization
    void Start () {
      _initialLocalPosition = transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
    transform.localPosition = _initialLocalPosition - list.currentDownShift;
	}
}
