using UnityEngine;
using System.Collections;

public class SoundOptionsMainMenuItemArray : MainMenuItemArray {
    
    public GameObject soundOptionsPanel;
    private bool _isSoundOn = false;
    public bool isSoundOn
    {
        get
        {
            return _isSoundOn;
        }
    }
    public float baseVolume = -1;

    public void setSoundTo(bool setOn)
    {
        _isSoundOn = setOn;
        string soundValue = _isSoundOn?CustomDataValue.ON.ToString():CustomDataValue.OFF.ToString();
        RedMetricsManager.get ().sendEvent(TrackingEvent.CONFIGURE, new CustomData(CustomDataTag.SOUND, soundValue));

        if(baseVolume < 0) {
            if(0 == AudioListener.volume) {
                baseVolume = 1f;
            } else {
                baseVolume = AudioListener.volume;
            }
        }
        AudioListener.volume = _isSoundOn?baseVolume:0f;

        SoundOptionMainMenuItem lmmi;
        foreach(MainMenuItem item in _items) {
            lmmi = item as SoundOptionMainMenuItem;
            if(null != lmmi) {
                lmmi.updateSelection ();
            }
        }
    }
    
    public void toggleSound()
    {
        setSoundTo(!_isSoundOn);
    }
    
    void OnEnable ()
    {
        soundOptionsPanel.SetActive(true);
    }
    
    void OnDisable ()
    {
        soundOptionsPanel.SetActive(false);
    }
}
