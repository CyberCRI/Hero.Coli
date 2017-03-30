using UnityEngine;
using System.Collections;

public class Scorekeeper
{
    private const int _chapterCount = 10;
    private ChapterCompletion[] _chapters = new ChapterCompletion[_chapterCount];

	public Scorekeeper()
	{
		Debug.Log(this.GetType() + " ctor");
		initialize();
	}

	private void initialize()
	{
		Debug.Log(this.GetType() + " initialize");
		for (int index = 0; index < _chapterCount; index++)
        {
			_chapters[index] = new ChapterCompletion();
		}
	}

    public string getCompletionAbstract()
    {
		Debug.Log(this.GetType() + " getCompletionAbstract");
        string result = "";
        for (int index = 0; index < _chapterCount; index++)
        {
			result = 0 == index ? result : result + "\n";
            result += "CHAPTER " + index + ": " + formatTime(_chapters[index].ownLastCompletionTime)
            + ", your record: " + formatTime(_chapters[index].ownBestCompletionTime)
            + ", world record: " + formatTime(_chapters[index].worldBestCompletionTime)
            ;
        }
        return result;
    }

    private string formatTime(float completionTime)
    {
		Debug.Log(this.GetType() + " formatTime(" + completionTime + ")");
        return completionTime < 0 ? "not completed" : string.Format("HH:mm:ss.fffZ", completionTime);
    }
}

public class ChapterCompletion
{
    public float ownLastCompletionTime;
    public float ownBestCompletionTime;
    public float worldBestCompletionTime;

    public ChapterCompletion()
    {
		Debug.Log(this.GetType() + " ctor");
        ownLastCompletionTime = -1;
        ownBestCompletionTime = -1;
        worldBestCompletionTime = -1;
    }
}


