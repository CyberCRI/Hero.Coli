using UnityEngine;
using System;

public class Scorekeeper
{
    private const int _chapterCount = 10;
    private ChapterCompletion[] _chapters = new ChapterCompletion[_chapterCount];
    private float _startTime;
    private int _currentChapter = 0;

    public Scorekeeper()
    {
        Debug.Log(this.GetType() + " ctor");
        initialize();
    }

	public void collideChapter(int index, float time)
	{
		Debug.Log(this.GetType() + " collideChapter(" + index + ", " + time + ")");
		if(index - _currentChapter == 1)
		{
			Debug.Log(this.GetType() + " collideChapter finished ch" + _currentChapter + ", started ch" + index);
			endChapter(_currentChapter, time);
			startChapter(index, time);
		}
		else if(index - _currentChapter == -1 || index - _currentChapter == 0)
		{
			Debug.Log(this.GetType() + " collideChapter went back from ch" + _currentChapter + " to ch" + index);
		}
		else
		{
			Debug.LogWarning(this.GetType() + " abnormal flow: previous chapter=" + _currentChapter + ", encountered chapter=" + index);
		}
	}

    public void startChapter(int index, float startTime)
    {
        Debug.Log(this.GetType() + " startChapter(" + index + ", " + startTime + ")");
        _currentChapter = index;
        _startTime = startTime;
    }

    private void endChapter(int index, float endTime)
    {
        Debug.Log(this.GetType() + " endChapter(" + index + ", " + endTime + ")");
        if (index != _currentChapter)
        {
            Debug.LogWarning(this.GetType() + " chapter indexes don't match: " + _currentChapter + " != " + index);
        }
        else
        {
            Debug.Log(this.GetType() + " endChapter");
            _chapters[_currentChapter].ownLastCompletionTime = endTime - _startTime;
            if (_chapters[_currentChapter].ownLastCompletionTime < _chapters[_currentChapter].ownBestCompletionTime)
            {
                Debug.Log(this.GetType() + " endChapter own record broken");
                // TODO put in MemoryManager
                _chapters[_currentChapter].ownBestCompletionTime = _chapters[_currentChapter].ownLastCompletionTime;
                if (_chapters[_currentChapter].ownLastCompletionTime < _chapters[_currentChapter].worldBestCompletionTime)
                {
                    Debug.Log(this.GetType() + " endChapter world record broken");
                    // TODO upload
                    _chapters[_currentChapter].worldBestCompletionTime = _chapters[_currentChapter].ownBestCompletionTime;
                }
            }
        }
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

		string result = "not completed";
        if(Mathf.Infinity != completionTime)
		{
			TimeSpan time = TimeSpan.FromSeconds(completionTime);
			result = time.ToString();
			// result = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
		}
		return result;
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
        ownLastCompletionTime = Mathf.Infinity;
        ownBestCompletionTime = Mathf.Infinity;
        worldBestCompletionTime = Mathf.Infinity;
    }
}


