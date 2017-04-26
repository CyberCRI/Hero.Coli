using UnityEngine;
using System.Collections;

public class BackendManager : MonoBehaviour
{
    [SerializeField]
    private bool _isTestGUID = false;
    [SerializeField]
    private bool _forceAdmin = false;
    [SerializeField]
    private int _layer;
    private int _depth = 0;
    private const float _duration = 2.0f;
    private static bool _isHurtLightEffectsOn = false;
    public static bool isHurtLightEffectsOn
    {
        get
        {
            return _isHurtLightEffectsOn;
        }
    }

    void Start()
    {
        _isTestGUID = GameConfiguration.isAdmin;
    }

    // Update is called once per frame
    void Update()
    {
        //logging mode: to test or to production RedMetrics version
        if (_forceAdmin || (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.LeftAlt)))
        {
            setAdmin();
        }
        else if (GameConfiguration.isAdmin)
        {
            // not managed here: search for 'GameConfiguration.isAdmin'
            // - teleportation
            // - chemicals' concentrations
            // - life and energy
            // - BlackLight (deprecated)

            // to instantly change the language
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                swapLanguage();
            }

            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                // to be able to go over or under obstacles
                moveCharacterDown();
            }
            else if (
              (Input.GetKeyDown(KeyCode.Period) && I18n.getCurrentLanguage() == I18n.Language.English)
              ||
              (Input.GetKeyDown(KeyCode.Semicolon) && I18n.getCurrentLanguage() == I18n.Language.French)
            )
            {
                moveCharacterUp();
            }
            else if (
              (Input.GetKeyDown(KeyCode.Slash) && I18n.getCurrentLanguage() == I18n.Language.English)
              ||
              (Input.GetKeyDown(KeyCode.Exclaim) && I18n.getCurrentLanguage() == I18n.Language.French)
            )
            {
                resetCharacterVerticalPosition();
            }

            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
            {
                // unlock everything
                unlockAll();
            }

            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Backspace))
            {
                // to erase all info in PlayerPrefs
                resetPlayerPrefs();
            }

            else if (Input.GetKeyDown(KeyCode.End))
            {
                // toggles special effects - light dimming when Cellia is hurt
                // to activate in AmbientLighting
                toggleHurtLightEffects();
            }

            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                // to end the game instantly, displaying stats and validating current chapter
                endGame();
            }
        }
    }

    private void setAdmin()
    {
        Debug.Log(this.GetType() + " setAdmin");
        _forceAdmin = false;
        _isTestGUID = MemoryManager.get().configuration.switchMetricsGameVersion();

        //display feedback for logging mode
        string msg;
        if (_isTestGUID)
        {
            msg = "REDMETRICS TEST MODE";
        }
        else
        {
            msg = "REDMETRICS DEFAULT MODE";
        }

        StartCoroutine(waitAndDestroy(createMessage(msg)));
    }

    IEnumerator waitAndDestroy(GameObject go)
    {
        yield return new WaitForSeconds(_duration);
        GameObject.Destroy(go);
    }

    public GameObject createMessage(string msg)
    {
        GameObject go = new GameObject();
        GUIText guiText = go.AddComponent<GUIText>();
        go.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        guiText.text = msg;
        return go;
    }

    private void toggleHurtLightEffects()
    {
        _isHurtLightEffectsOn = !_isHurtLightEffectsOn;
        Debug.Log(this.GetType() + " toggleHurtLightEffects set to " + _isHurtLightEffectsOn);
    }

    private void unlockAll()
    {
        Debug.Log(this.GetType() + " unlockAll");
        GameStateController.unlockAll();
    }

    private void resetPlayerPrefs()
    {
        Debug.Log(this.GetType() + " resetPlayerPrefs");
        MemoryManager.get().configuration.furthestChapter = 0;
        GameStateController.get().resetPlayerPrefsAndRestart();
    }

    private void endGame()
    {
        Debug.Log(this.GetType() + " endGame");
        GameStateController.get().triggerEnd();
    }

    private void swapLanguage()
    {
        Debug.Log(this.GetType() + " swapLanguage");

        // language swap
        I18n.Language newLanguage = (I18n.Language.English == I18n.getCurrentLanguage()) ? I18n.Language.French : I18n.Language.English;
        if (I18n.changeLanguageTo(newLanguage))
        {
            RedMetricsManager.get().sendEvent(TrackingEvent.ADMINCONFIGURE, new CustomData(CustomDataTag.LANGUAGE, I18n.getCurrentLanguage().ToString()));
        }
    }

    private void logObjectsOfLayer(int layer = int.MinValue)
    {
        Debug.Log(this.GetType() + " logObjectsOfLayer");

        int usedLayer = int.MinValue == layer ? _layer : layer;

        // layer detection
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject go in gos)
        {
            if (go.layer == usedLayer)
            {
                Debug.Log(go.name);
            }
        }
    }

    private void resetCharacterVerticalPosition()
    {
        Debug.Log(this.GetType() + " resetCharacterVerticalPosition");
        Vector3 position = Character.get().transform.position;
        Character.get().transform.position = new Vector3(position.x, position.y - _depth, position.z);
        _depth = 0;
    }

    private void moveCharacterUp()
    {
        Debug.Log(this.GetType() + " moveCharacterUp");
        Vector3 position = Character.get().transform.position;
        Character.get().transform.position = new Vector3(position.x, position.y - 1, position.z);
        _depth--;
    }

    private void moveCharacterDown()
    {
        Debug.Log(this.GetType() + " moveCharacterDown");
        Vector3 position = Character.get().transform.position;
        Character.get().transform.position = new Vector3(position.x, position.y + 1, position.z);
        _depth++;
    }
}
