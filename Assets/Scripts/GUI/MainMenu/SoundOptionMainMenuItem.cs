using UnityEngine;

public class SoundOptionMainMenuItem : MainMenuItem
{
    public Vector3 offset;
    public GameObject soundONIcon;
    public UISprite soundONSprite;
    public GameObject soundOFFIcon;
    public UISprite soundOFFSprite;
    public UILocalize label;
    private const string _keySoundON = "MENU.SOUND.ON";
    private const string _keySoundOFF = "MENU.SOUND.OFF";

    public override void click()
    {
        // Debug.Log(this.GetType() + " clicked " + itemName);
        base.click();
        SoundManager.instance.toggleSound();
        updateSelection();
    }

    public void updateSelection()
    {
        bool isOn = MemoryManager.get().configuration.isSoundOn;
        label.key = isOn ? _keySoundON : _keySoundOFF;
        label.Localize();
        soundONIcon.SetActive(isOn);
        soundOFFIcon.SetActive(!isOn);
    }

    void OnEnable()
    {
        if (null != soundONSprite && null != soundOFFSprite)
        {
            SoundManager.onSoundToggle += updateSelection;
            soundONSprite.transform.position = this.gameObject.transform.position + offset;
            soundOFFSprite.transform.position = this.gameObject.transform.position + offset;
            updateSelection();
        }
    }

    void OnDisable()
    {
        if (null != soundONSprite && null != soundOFFSprite)
        {
            SoundManager.onSoundToggle -= updateSelection;
            soundONIcon.SetActive(false);
            soundOFFIcon.SetActive(false);
        }
    }
}
