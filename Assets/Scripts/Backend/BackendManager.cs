using UnityEngine;

public class BackendManager : MonoBehaviour {
    
    bool isTestGUID = false;
    
    void Start () {
        isTestGUID = MemoryManager.get().configuration.isTestGUID();
    }
	
	// Update is called once per frame
	void Update () {
	   if(Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl)) {
           MemoryManager.get().configuration.switchGUID();
           isTestGUID = MemoryManager.get().configuration.isTestGUID();
           Debug.LogError("switched to testGUID == "+isTestGUID);
       }
       if(isTestGUID) {
           Logger.Log("test GUID", Logger.Level.ONSCREEN);
       }
	}
}
