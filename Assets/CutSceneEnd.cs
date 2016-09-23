using UnityEngine;
using System.Collections;
using System;

public class CutSceneEnd : CutScene {

    [SerializeField]
    private GameObject[] NPCs;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void startCutScene()
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i].GetComponent<RotationUpdate>().ObjectDirectedRotationUpdate(_cellControl.gameObject);
        }
        end();
    }

    public override void endCutScene()
    {
        _cellControl.freezePlayer(false);
    }

    public override void initialize()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.tag);
        if (col.tag == "CutSceneElement")
        {
            start();
        }
    }
}
