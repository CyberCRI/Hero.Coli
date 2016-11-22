using UnityEngine;
using System.Collections;

public class BackendManager : MonoBehaviour
{
    [SerializeField]
    private bool isTestGUID = false;
    [SerializeField]
    private bool forceAdmin = false;
    [SerializeField]
    private int _layer;
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

        if (GameConfiguration.isAdmin)
        {
            // to instantly change the language
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // language swap
                // I18n.Language newLanguage = (I18n.Language.English == I18n.getCurrentLanguage()) ? I18n.Language.French : I18n.Language.English;
                // I18n.changeLanguageTo(newLanguage);

                // layer detection
                GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
                foreach (GameObject go in gos)
                {
                    if (go.layer == _layer)
                    {
                        Debug.Log(go.name);
                    }
                }
            }
            // to be able to go over or under obstacles
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                Vector3 position = Hero.get().transform.position;
                Hero.get().transform.position = new Vector3(position.x, position.y + 1, position.z);
            }
            if (Input.GetKeyDown(KeyCode.KeypadDivide))
            {
                Vector3 position = Hero.get().transform.position;
                Hero.get().transform.position = new Vector3(position.x, position.y - 1, position.z);
            }
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
