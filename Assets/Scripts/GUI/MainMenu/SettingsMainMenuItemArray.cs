using System.Collections.Generic;

public class SettingsMainMenuItemArray : MainMenuItemArray
{
    // graphics: all but webgl & android
    // controls: all but android
    void Start()
    {
#if UNITY_EDITOR

#elif UNITY_WEBGL
		hideIndexes(new List<int>{0});
#elif UNITY_ANDROID
		hideIndexes(new List<int>{0, 2});
#endif
    }
}
