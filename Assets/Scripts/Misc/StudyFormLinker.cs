using UnityEngine;

public class StudyFormLinker : MonoBehaviour
{
    private const string _studyURLKey = "STUDY.LEARN.LINK";
    public void OpenForm()
    {
        URLOpener.open(_studyURLKey, false, MemoryManager.get().configuration.playerGUID);
    }
}