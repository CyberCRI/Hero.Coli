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
            //    new Vector3(522.9f, 172.5f, 0);
            //    y = 204 for optimal placement
            unfoldingMoleculeList.transform.parent.transform.localPosition
                + unfoldingMoleculeList.transform.localPosition                 
                + Vector3.up*unfoldingMoleculeList.transform.localScale.y
                //- new Vector3(0f, 20f, 0f)
                //- Vector3.up*backgroundSprite.transform.localScale.y            
                + offset
                ;
        Logger.Log("EACBPP::setPosition uML.t.p.t.lP="+unfoldingMoleculeList.transform.parent.transform.localPosition, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::setPosition uML.t.lP="+unfoldingMoleculeList.transform.localPosition, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::setPosition V3.u*uML.t.lS.y="+Vector3.up*unfoldingMoleculeList.transform.localScale.y, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::setPosition offset="+offset, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::setPosition _initialLocalPosition="+_initialLocalPosition, Logger.Level.ONSCREEN);
    }
	
	// Update is called once per frame
    void Update () {
        //Logger.Log("EACBPP::Update="+_initialLocalPosition, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::Update t.lP="+transform.localPosition, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::Update _iLP="+_initialLocalPosition, Logger.Level.ONSCREEN);
        Logger.Log("EACBPP::Update l.cH="+list.currentHeight, Logger.Level.ONSCREEN);
        setPosition();
        transform.localPosition = _initialLocalPosition - 2*list.currentHeight;
        //Logger.Log("EACBPP::Update AFTER transform.localPosition="+transform.localPosition, Logger.Level.ONSCREEN);
        //Logger.Log("EACBPP::Update list.currentHeight="+list.currentHeight, Logger.Level.ONSCREEN);
    }
}
