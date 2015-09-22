using UnityEngine;
using System.Collections;

public class SoundOptionsMainMenuItemArray : MainMenuItemArray {
    
    public GameObject soundOptionsPanel;
    public bool isSoundOn = true;
    public float baseVolume = -1;
    
    public void toggleSound()
    {
        isSoundOn = !isSoundOn;
        if(baseVolume < 0) {
            if(0 == AudioListener.volume) {
                baseVolume = 1f;
            } else {
                baseVolume = AudioListener.volume;
            }
        }
        AudioListener.volume = isSoundOn?baseVolume:0f;

        SoundOptionMainMenuItem lmmi;
        foreach(MainMenuItem item in _items) {
            lmmi = item as SoundOptionMainMenuItem;
            if(null != lmmi) {
                lmmi.updateSelection ();
            }
        }
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
