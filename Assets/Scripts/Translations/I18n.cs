using UnityEngine;

public class I18n {

    public enum Language {
        English,
        French,
        Russian
    }

    public static void changeLanguageTo(Language lang)
    {
        RedMetricsManager.get ().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.LANGUAGE, lang.ToString()));

        Localization.instance.currentLanguage = lang.ToString();

        MemoryManager.get ().configuration.language = lang;
        
        foreach(UILocalize localize in GameObject.FindObjectsOfType<UILocalize>()) {
            localize.Localize();
        }
        
        foreach(TextMeshLocalizer localizer in GameObject.FindObjectsOfType<TextMeshLocalizer>()) {
            localizer.localize();
        }
    }

    public static Language getCurrentLanguage()
    {
        string language = Localization.instance.currentLanguage.ToLower();
        switch(language)
        {
            case "english": return Language.English;
            case "french": return Language.French;
            case "russian": return Language.Russian;
            default: return Language.English;
        }
    }
}
