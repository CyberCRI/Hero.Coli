using UnityEngine;
using System.Collections;

public class LanguagesMainMenuItemArray : MainMenuItemArray {

    public GameObject languagesPanel;

    public void selectLanguage(I18n.Language language)
    {
        I18n.changeLanguageTo(language);
        LanguageMainMenuItem lmmi;
        foreach(MainMenuItem item in _items) {
            lmmi = item as LanguageMainMenuItem;
            if(null != lmmi) {
                lmmi.updateSelection ();
            }
        }
    }

    void OnEnable ()
    {
        Debug.LogError("Controls are visible");
        languagesPanel.SetActive(true);
    }
    
    void OnDisable ()
    {
        languagesPanel.SetActive(false);
    }
}
