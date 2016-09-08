using UnityEngine;
using System.Collections;

public class NPCIndicator : MonoBehaviour {

    [SerializeField]
    private GameObject _npcBody;
    [SerializeField]
    private GameObject _npcIndicator;
    [SerializeField]
    private GameObject[] _slots;
    [SerializeField]
    private GameObject _renderer;

	// Use this for initialization
	void Start () {
        Physics.queriesHitTriggers = true;
	}
	
	// Update is called once per frame
	void Update () {
        _npcIndicator.transform.position = _npcBody.transform.position;
	}

    void OnMouseOver()
    {
        _renderer.SetActive(true);
    }

    void OnMouseExit()
    {
        _renderer.SetActive(false);
    }
}
