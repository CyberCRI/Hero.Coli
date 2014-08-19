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
    }
}
