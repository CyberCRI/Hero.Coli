using UnityEngine;

public class StudyFormLinker : MonoBehaviour
{
    private const string _studyURLKey = "STUDY.LEARN.LINK";

    // to be called from game
	public static void openFormGame(bool newTab = false)
    {
        // Debug.Log("StudyFormLinker openForm");
        internalOpenForm(true, newTab);
    }

    // to be called from webpage
    // newTab == 1 means there will be a new tab 
	public void openFormWeb(int newTab)
    {
        // Debug.Log(this.GetType() + " OpenForm");
        internalOpenForm(false, (newTab == 1));
    }

    private static void internalOpenForm(bool fromGame, bool newTab)
    {
        // Debug.Log("StudyFormLinker internalOpenForm(fromGame=" + fromGame + ")");
        string playerGUID = MemoryManager.get().configuration.playerGUID;
        string source = fromGame ? CustomDataValue.GAME.ToString() : CustomDataValue.WEBPAGE.ToString();
        CustomData data = new CustomData(CustomDataTag.SOURCE, source);
        data.Add(CustomDataTag.LOCALPLAYERGUID, playerGUID);
        RedMetricsManager.get().sendEvent(TrackingEvent.GOTOSTUDY, data);
        URLOpener.open(_studyURLKey, newTab, playerGUID);
    }
}