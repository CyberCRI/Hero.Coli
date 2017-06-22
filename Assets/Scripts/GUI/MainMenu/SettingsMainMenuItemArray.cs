using System.Collections.Generic;

public class SettingsMainMenuItemArray : MainMenuItemArray
{
    // 0 - graphics: all but arcade, webgl & android
    // 1 - sound: all
    // 2 - controls: all but arcade, android
    void Start()
    {
#if UNITY_EDITOR
#elif ARCADE
        // hide graphics and controls
		hideIndexes(new List<int>{0, 2});
#elif UNITY_WEBGL
        // hide graphics
		hideIndexes(new List<int>{0});
#elif UNITY_ANDROID
        // hide graphics and controls
		hideIndexes(new List<int>{0, 2});
#endif
    }
}
