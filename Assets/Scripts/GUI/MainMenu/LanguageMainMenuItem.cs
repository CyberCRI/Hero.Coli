using UnityEngine;
using System.Collections;

public class LanguageMainMenuItem : MainMenuItem {
        
    public Vector3 offset;
    public GameObject languageIcon;
    public LanguagesMainMenuItemArray languagesArray;
    public I18n.Language language;
    
    public override void click () {
        Logger.Log("clicked "+itemName, Logger.Level.INFO);
        languagesArray.selectLanguage(language);
    }
    
    /*
    void Update ()
    {
        //TODO remove
        if (null != languageIcon) {
            languageIcon.transform.position = this.gameObject.transform.position + offset;
        }
    }
    */
    
    void OnEnable ()
    {
        if (null != languageIcon) {
            languageIcon.transform.position = this.gameObject.transform.position + offset;
            languageIcon.gameObject.SetActive(true);
        }
    }
    
    void OnDisable ()
    {
        if (null != languageIcon) {
            languageIcon.SetActive(false);
        }
    }
}
