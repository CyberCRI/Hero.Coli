using UnityEngine;
using System.Collections;

public class EquipAndCraftButtonPanelPositioner : MonoBehaviour {

  public GraphMoleculeList list;
    private Vector3 _initialLocalPosition;
    public GameObject unfoldingMoleculeList;
    public GameObject backgroundSprite;

    // Use this for initialization
    void Start () {
        _initialLocalPosition =
        //    new Vector3(522.9f, 172.5f, 0);
        //    y = 204 for optimal placement
              unfoldingMoleculeList.transform.parent.transform.localPosition
            + unfoldingMoleculeList.transform.localPosition                 
            + Vector3.up*unfoldingMoleculeList.transform.localScale.y
            - new Vector3(0f, 20f, 0f)
            //- Vector3.up*backgroundSprite.transform.localScale.y            
              ;
    }
	
	// Update is called once per frame
	void Update () {
    transform.localPosition = _initialLocalPosition - list.currentHeight;
	}
}
