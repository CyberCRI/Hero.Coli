using UnityEngine;

public class SoundOptionMainMenuItem : MainMenuItem
{
    public Vector3 offset;
    public GameObject soundONIcon;
    public UISprite soundONSprite;
    public GameObject soundOFFIcon;
    public UISprite soundOFFSprite;
    public SoundOptionsMainMenuItemArray soundOptionsArray;
    public UILocalize label;
    private string keySoundON = "MENU.SOUND.ON";
    private string keySoundOFF = "MENU.SOUND.OFF";

    public override void click()
    {
        // Debug.Log(this.GetType() + " clicked " + itemName);
        soundOptionsArray.toggleSound();
        string soundValue = soundOptionsArray.isSoundOn ? CustomDataValue.ON.ToString() : CustomDataValue.OFF.ToString();
        RedMetricsManager.get().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.SOUND, soundValue));
    }

    public void updateSelection()
    {
        label.key = soundOptionsArray.isSoundOn ? keySoundON : keySoundOFF;
        label.Localize();
        soundONIcon.SetActive(soundOptionsArray.isSoundOn);
        soundOFFIcon.SetActive(!soundOptionsArray.isSoundOn);
    }

    void OnEnable()
    {
        if (null != soundONSprite && null != soundOFFSprite)
        {
            soundONSprite.transform.position = this.gameObject.transform.position + offset;
            soundOFFSprite.transform.position = this.gameObject.transform.position + offset;
            updateSelection();
        }
    }

    void OnDisable()
    {
        if (null != soundONSprite && null != soundOFFSprite)
        {
            soundONIcon.SetActive(false);
            soundOFFIcon.SetActive(false);
        }
    }
}
