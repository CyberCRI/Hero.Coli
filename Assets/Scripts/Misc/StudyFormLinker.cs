using UnityEngine;

public class StudyFormLinker : MonoBehaviour
{
    private const string _studyURLKey = "STUDY.LEARN.LINK";

    // to be called from game
    public static void openForm()
    {
        Debug.Log("StudyFormLinker openForm");
        internalOpenForm(true);
    }

    // to be called from webpage
    public void OpenForm()
    {
        Debug.Log(this.GetType() + " OpenForm");
        internalOpenForm(false);
    }

    private static void internalOpenForm(bool fromGame)
    {
        Debug.Log("StudyFormLinker internalOpenForm(fromGame=" + fromGame + ")");
        string playerGUID = MemoryManager.get().configuration.playerGUID;
        string source = fromGame ? CustomDataValue.GAME.ToString() : CustomDataValue.WEBPAGE.ToString();
        CustomData data = new CustomData(CustomDataTag.SOURCE, source);
        data.Add(CustomDataTag.LOCALPLAYERGUID, playerGUID);
        RedMetricsManager.get().sendEvent(TrackingEvent.GOTOSTUDY, data);
        URLOpener.open(_studyURLKey, false, playerGUID);
    }
}