using UnityEngine;
using System.Collections;

public class BackendManager : MonoBehaviour
{
    [SerializeField]
    private bool isTestGUID = false;
    [SerializeField]
    private bool forceAdmin = false;
    private float duration = 2.0f;

    void Start()
    {
        isTestGUID = GameConfiguration.isAdmin;
    }

    // Update is called once per frame
    void Update()
    {
        //logging mode: to test or to production RedMetrics version
        if (forceAdmin || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl)))
        {
            forceAdmin = false;
            isTestGUID = MemoryManager.get().configuration.switchMetricsGameVersion();

            //display feedback for logging mode
            string msg;
            if (isTestGUID)
            {
                msg = "REDMETRICS TEST MODE";
            }
            else
            {
                msg = "REDMETRICS DEFAULT MODE";
            }

            StartCoroutine(waitAndDestroy(createMessage(msg)));
        }
/*
#if UNITY_EDITOR
        // for optimization process
        float _minFPSThreshold = 30; // frames per second
        float _maxDeltaTime = 1/_minFPSThreshold; // seconds

        // TODO check difference
        // Time.fixedDeltaTime
        // Time.unscaledDeltaTime

        if (Time.deltaTime > _maxDeltaTime)
        {
            // pause the game in editor
            // EditorApplication.
            
        }
#endif
*/
    }

    public GameObject createMessage(string msg)
    {
        GameObject go = new GameObject();
        GUIText guiText = go.AddComponent<GUIText>();
        go.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        guiText.text = msg;
        return go;
    }

    IEnumerator waitAndDestroy(GameObject go)
    {
        yield return new WaitForSeconds(duration);
        GameObject.Destroy(go);
    }
}
