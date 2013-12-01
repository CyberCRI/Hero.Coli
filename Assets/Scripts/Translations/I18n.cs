using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class I18n : MonoBehaviour {

  private static string txtformat = ".txt";
  private static string english = "_en";
  private static string french = "_fr";
  private static string translationsFolder = "Assets/Data/Translations/";
  private static string main = "translations";
  private static string mainEnglish = translationsFolder+main+english+txtformat;
  private static string mainFrench = translationsFolder+main+french+txtformat;

  private Dictionary<string, Language> _translationFiles;

  public enum Language {
    ENGLISH,
    FRENCH
  }

  private Dictionary<string, string> _engDico;
  private Dictionary<string, string> _freDico;
  private Dictionary<Language, IDictionary<string, string>> _dicos;


  //load translations
  void Start() {
    _translationFiles = new Dictionary<string, Language>();
    _translationFiles.Add(mainEnglish, Language.ENGLISH);
    _translationFiles.Add(mainFrench, Language.FRENCH);

    _engDico = new Dictionary<string, string>();
    _freDico = new Dictionary<string, string>();

    _dicos = new Dictionary<Language, IDictionary<string, string>>();
    _dicos.Add(Language.ENGLISH, _engDico);
    _dicos.Add(Language.FRENCH, _freDico);

    //load translations
    I18nLoader iLoader = new I18nLoader(_dicos);
    foreach (KeyValuePair<string, Language> file in _translationFiles) {
      Logger.Log("I18n::loadFromFile loads file "+file.Key+" for language "+file.Value, Logger.Level.DEBUG);
      iLoader.loadFromFile(file.Key, file.Value);
      Logger.Log("I18n::loadFromFile file "+file.Key+" for language "+file.Value+" contains "
        +Logger.ToString<string, string>(_dicos[file.Value]), Logger.Level.DEBUG);
    }
  }

  public string translate(string code, Language lang = Language.ENGLISH)
  {
    return _dicos[lang][code];
  }

}
