using UnityEngine;
using System;

public class TranslationManager : MonoBehaviour
{
    // called from js
    public static void SetLanguage(string language)
    {
        // foreach(I18n.Language _language in Enum.GetValues(typeof(I18n.Language)).Cast<I18n.Language>)
        foreach (I18n.Language _language in Enum.GetValues(typeof(I18n.Language)))
        {
            if (_language.ToString().ToLowerInvariant() == language.ToLowerInvariant())
            {
                if (I18n.changeLanguageTo(_language, false))
                {
                    RedMetricsManager.get().sendEvent(TrackingEvent.WEBCONFIGURE, new CustomData(CustomDataTag.LANGUAGE, I18n.getCurrentLanguage().ToString()));
                }
                return;
            }
        }
        // Debug.Log("failed to set language to '" + language + "'");
    }

    // called from js
    // calls the js callback 'getLanguageCallback'
    public void GetLanguage()
    {
		// Debug.Log("TranslationManager GetLanguage");
        callJSFunctionWithLanguageJson("getLanguageCallback");
    }

    // called from Unity
    public static void onLanguageChanged()
    {
		// Debug.Log("TranslationManager onLanguageChanged");
        callJSFunctionWithLanguageJson("onLanguageChanged");
    }

    private static void callJSFunctionWithLanguageJson(string functionName)
    {
		// Debug.Log("TranslationManager callJSFunctionWithLanguageJson(" + functionName + ")");
#if UNITY_WEBGL
            string currentLanguage = I18n.getCurrentLanguage().ToString().ToLowerInvariant();
            LanguageData data = new LanguageData(currentLanguage);
            string json = RedMetricsManager.get().getJsonString(data);
            Application.ExternalCall(functionName, json);
#endif
    }

    public class LanguageData
    {
        public string language;

        public LanguageData(string _language)
        {
            language = _language;
        }
    }
}
