using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class I18n {

    public enum Language {
        English,
        French
    }

    public static void changeLanguageTo(Language lang)
    {
        Localization.instance.currentLanguage = lang.ToString();

        CraftZoneManager.get ().OnLanguageChanged();
        //TooltipManager.OnLanguageChanged();
        
        foreach(UILocalize localize in GameObject.FindObjectsOfType<UILocalize>()) {
            localize.Localize();
        }
    }

    public static Language getCurrentLanguage()
    {
        string language = Localization.instance.currentLanguage.ToLower();
        switch(language)
        {
            case "english": return Language.English;
            case "french": return Language.French;
            default: return Language.English;
        }
    }
}
