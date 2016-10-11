using UnityEngine;
using System.Collections;

public class BackendManager : MonoBehaviour
{

    bool isTestGUID = false;
    private float duration = 2.0f;

    // Update is called once per frame
    void Update()
    {
        //logging mode: to test or to production RedMetrics version
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.RightControl))
        {
            MemoryManager.get().configuration.switchMetricsGameVersion();
            isTestGUID = MemoryManager.get().configuration.isTestGUID();

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
        // //toggle tutorial mode
        // else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        // {
        //     string msg;
        //     if(MemoryManager.get().configuration.tutorialMode == GameConfiguration.TutorialMode.START0FLAGELLUMHORIZONTALTRANSFER)
        //     {
        //         MemoryManager.get().configuration.tutorialMode = GameConfiguration.TutorialMode.START1FLAGELLUM4BRICKS;
        //         msg = "MODE = 1FLAGELLUM 4BRICKS";
        //     }
        //     else
        //     {
        //          MemoryManager.get().configuration.tutorialMode = GameConfiguration.TutorialMode.START0FLAGELLUMHORIZONTALTRANSFER;
        //          msg = "MODE = START0FLAGELLUM HORIZONTALTRANSFER";
        //     }
        //     StartCoroutine(waitAndDestroy(createMessage(msg)));
        // }
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
