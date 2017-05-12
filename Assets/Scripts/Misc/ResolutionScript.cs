using UnityEngine;
using System.Collections;

public class ResolutionScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if UNITY_ANDROID && !UNITY_EDITOR
		// Screen.SetResolution(800, 450, true);
		// this.GetComponent<Camera>().aspect = 16f / 9f;
		Screen.SetResolution(1024, 640, true);
		this.GetComponent<Camera>().aspect = 16f / 10f;
		#else
		Screen.SetResolution(1280, 720, false);
		#endif
	}

	// Update is called once per frame
	void Update () {
	
	}
}
