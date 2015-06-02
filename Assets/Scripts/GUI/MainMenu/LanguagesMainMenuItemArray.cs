using UnityEngine;
using System.Collections;

public class LanguagesMainMenuItemArray : MainMenuItemArray {

    public GameObject languagesPanel;

    public void selectLanguage(I18n.Language language)
    {
        I18n.changeLanguageTo(language);
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
