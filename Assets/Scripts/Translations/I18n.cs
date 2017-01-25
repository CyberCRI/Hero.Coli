using UnityEngine;
using System.Collections.Generic;

public static class I18n
{

    public enum Language
    {
        English,
        French,
        Russian
    }

    public static void changeLanguageTo(Language lang)
    {
        // Debug.Log("I18n changeLanguageTo " + lang);
        // RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.LANGUAGE, lang.ToString()));

        Localization.instance.currentLanguage = lang.ToString();

        foreach (UILocalize localize in GameObject.FindObjectsOfType<UILocalize>())
        {
            localize.Localize();
        }

        foreach (TextMeshLocalizer localizer in GameObject.FindObjectsOfType<TextMeshLocalizer>())
        {
            localizer.localize();
        }

        foreach (ILocalizable localizable in _localizables)
        {
            // Debug.Log("I18n changeLanguageTo " + lang + " treats localizable " + localizable.GetType());
            localizable.onLanguageChanged();
        }
    }

    public static Language getCurrentLanguage()
    {
        // Debug.Log("I18n getCurrentLanguage");
        string language = Localization.instance.currentLanguage.ToLower();
        switch (language)
        {
            case "english": return Language.English;
            case "french": return Language.French;
            case "russian": return Language.Russian;
            default: return Language.English;
        }
    }

    private static List<ILocalizable> _localizables = new List<ILocalizable>();
    public static void reset()
    {
        // Debug.Log("I18n reset");
        _localizables.Clear();
    }
    public static void register(ILocalizable localizable)
    {
        // Debug.Log("I18n register " + localizable.GetType());
        _localizables.Add(localizable);
    }
}
