using UnityEngine;
using System.Collections;

public class LanguageMainMenuItem : MainMenuItem, ILocalizable
{

    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    private GameObject languageIcon;
    [SerializeField]
	private LanguagesMainMenuItemArray languagesArray;
    [SerializeField]
    private I18n.Language language;
    [SerializeField]
    private Vector3 selectedScale;
    [SerializeField]
    private Vector3 deselectedScale;
    [SerializeField]
    private float selectedAlpha;
    [SerializeField]
    private float deselectedAlpha;
    [SerializeField]
    private UISprite languageSprite;

    public override void initialize()
    {
        base.initialize();
        I18n.register(this);
    }

    public void onLanguageChanged()
    {
        updateSelection();
    }

    public override void click()
    {
        // Debug.Log(this.GetType() + " clicked "+itemName);
		base.click();
        languagesArray.selectLanguage(language);
		RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.LANGUAGE, I18n.getCurrentLanguage().ToString()));
    }

    public void updateSelection()
    {
        select(I18n.getCurrentLanguage() == language);
    }

    private void select(bool select)
    {
        if (select)
        {
            languageSprite.alpha = selectedAlpha;
            languageIcon.transform.localScale = selectedScale;
        }
        else
        {
            languageSprite.alpha = deselectedAlpha;
            languageIcon.transform.localScale = deselectedScale;
        }
    }

    void OnEnable()
    {
        if (languageIcon != null)
        {
            languageIcon.transform.position = this.gameObject.transform.position + offset;
            languageIcon.gameObject.SetActive(true);
            updateSelection();
        }
    }

    void OnDisable()
    {
		/*
        if (languageIcon != null)
        {
            languageIcon.SetActive(false);
        }
        */
    }
}
