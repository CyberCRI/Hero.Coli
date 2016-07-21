using UnityEngine;
using System.Collections;

public class AmpicillinCutScene : MonoBehaviour {

    [SerializeField]
    private iTweenEvent _iTween2Flagellum;
    [SerializeField]
    private iTweenEvent _iTween3Flagellum;
    private CellControl _cellControl;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            _cellControl = col.GetComponent<CellControl>();
            StartCutScene();
        }
    }

    void StartCutScene()
    {
        _cellControl.FreezePLayer(true);
        _iTween2Flagellum.enabled = true;
        _iTween3Flagellum.enabled = true;
    }
}
