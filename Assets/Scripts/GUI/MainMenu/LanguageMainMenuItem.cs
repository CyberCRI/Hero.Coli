using UnityEngine;
using System.Collections;

public class LanguageMainMenuItem : MainMenuItem {
        
    public Vector3 offset;
    public GameObject languageIcon;
    public LanguagesMainMenuItemArray languagesArray;
    public I18n.Language language;
    public Vector3 selectedScale;
    public Vector3 deselectedScale;
    public float selectedAlpha;
    public float deselectedAlpha;
    public UISprite languageSprite;
    
    public override void click () {
        Debug.Log(this.GetType() + " clicked "+itemName);
        languagesArray.selectLanguage(language);
    }

    public void updateSelection()
    {
        select (I18n.getCurrentLanguage()==language);
    }

    private void select(bool select)
    {
        if(select) {
            languageSprite.alpha = selectedAlpha;
            languageIcon.transform.localScale = selectedScale;
        } else {
            languageSprite.alpha = deselectedAlpha;
            languageIcon.transform.localScale = deselectedScale;
        }
    }
    
    void OnEnable ()
    {
        if (null != languageIcon) {
            languageIcon.transform.position = this.gameObject.transform.position + offset;
            languageIcon.gameObject.SetActive(true);
            updateSelection ();
        }
    }
    
    void OnDisable ()
    {
        if (null != languageIcon) {
            languageIcon.SetActive(false);
        }
    }
}
