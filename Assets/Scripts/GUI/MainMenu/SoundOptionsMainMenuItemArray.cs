using UnityEngine;

public class SoundOptionsMainMenuItemArray : MainMenuItemArray
{
    public GameObject soundOptionsPanel;

    void OnEnable()
    {
        soundOptionsPanel.SetActive(true);

    }

    void OnDisable()
    {
        soundOptionsPanel.SetActive(false);
    }
}
