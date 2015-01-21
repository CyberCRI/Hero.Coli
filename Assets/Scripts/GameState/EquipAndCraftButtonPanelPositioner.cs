using UnityEngine;
using System.Collections;

public class EquipAndCraftButtonPanelPositioner : MonoBehaviour {

    public GraphMoleculeList list;
    private Vector3 _initialLocalPosition;
    public GameObject unfoldingMoleculeList;
    public GameObject backgroundSprite;
    public Vector3 offset;

    // Use this for initialization
    void Start () {
        setPosition();
    }

    private void setPosition()
    {
        _initialLocalPosition =
            unfoldingMoleculeList.transform.parent.transform.localPosition
                + unfoldingMoleculeList.transform.localPosition                 
                + Vector3.up*unfoldingMoleculeList.transform.localScale.y
                + offset
                ;
    }
	
	// Update is called once per frame
    void Update () {
        setPosition();
        transform.localPosition = _initialLocalPosition - 2*list.currentHeight;
    }
}
