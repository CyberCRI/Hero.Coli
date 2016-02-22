using UnityEngine;
using System.Collections;

public class BackendManager : MonoBehaviour {
    
    bool isTestGUID = false;
    private float duration = 2.0f;
    
	// Update is called once per frame
	void Update () {
	   if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl)) {
           MemoryManager.get().configuration.switchMetricsGameVersion();
           isTestGUID = MemoryManager.get().configuration.isTestGUID();
           
           //display feedback for logging mode
            GameObject go = new GameObject();
            GUIText guiText = go.AddComponent<GUIText>();
            go.transform.position = new Vector3(0.5f,0.5f,0.0f);
            if(isTestGUID) {
                guiText.text = "REDMETRICS TEST MODE";
            } else {
                guiText.text = "REDMETRICS DEFAULT MODE";
            }
            
            StartCoroutine(waitAndDestroy(go));
       }
	}

    IEnumerator waitAndDestroy(GameObject go)
    {
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(go);
    }
}
