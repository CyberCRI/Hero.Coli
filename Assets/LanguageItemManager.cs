using UnityEngine;
using System.Collections;

public class LanguageItemManager : MonoBehaviour {
	[SerializeField] public LanguageMainMenuItem[] _languageMainMenuArray;

	public void selectLanguage(I18n.Language language)
	{
		I18n.changeLanguageTo(language);
		LanguageMainMenuItem lmmi;
		foreach (LanguageMainMenuItem item in _languageMainMenuArray)
		{
			lmmi = item as LanguageMainMenuItem;
			if (null != lmmi)
			{
				lmmi.updateSelection();
			}
		}
	}
}
