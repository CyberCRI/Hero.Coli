using UnityEngine;
using System;

public class Scorekeeper
{
    // all chapters + total time
    public const int completionsCount = Checkpoint.chapterCount + 1;
    private const int _columnCount = 4;
    private const string _keyStem = "TUTORIAL.FINALSCOREBOARD.";
    private const string _chapterSuffix = "CHAPTER";
    private const string _totalSuffix = "TOTAL";
    private const string _columnSuffix = "COLUMN";
    private const string _titleSuffix = "TITLE";
    private const string _hoursSuffix = "H";
    private const string _minutesSuffix = "MIN";
    private const string _secondsSuffix = "S";
    private const string _millisecondsSuffix = "MS";
    private const string _chapterKey = _keyStem + _chapterSuffix;
    private const string _totalKey = _keyStem + _totalSuffix;
    private ChapterCompletion[] _chapters = null;
    private float _startTime;
    private int _currentChapter = 0, _furthestChapter = -1;
    private string _chapterString, _totalString, _hoursString, _minutesString, _secondsString, _millisecondsString;

    public Scorekeeper()
    {
        // Debug.Log(this.GetType() + " ctor");
    }

    private int furthestChapter
    {
        get
        {
            if (_furthestChapter < 0)
            {
                _furthestChapter = MemoryManager.get().configuration.furthestChapter;
            }
            return _furthestChapter;
        }
        set
        {
            _furthestChapter = value;
        }
    }

    private ChapterCompletion[] chapters
    {
        get
        {
            if (null == _chapters)
            {
                Debug.Log(this.GetType() + " get chapters initializes");
                _chapters = new ChapterCompletion[completionsCount];
                float[] bestTimes = MemoryManager.get().configuration.bestTimes;
                for (int index = 0; index < completionsCount; index++)
                {
                    Debug.Log(this.GetType() + " get chapters initializes index = " + index);
                    _chapters[index].ownBestCompletionTime = bestTimes[index];
                }
            }
            return _chapters;
        }
    }

    public void collideChapter(int index, float time)
    {
        Debug.Log(this.GetType() + " collideChapter(" + index + ", " + time + ")");
        if (index - _currentChapter == 1)
        {
            Debug.Log(this.GetType() + " collideChapter finished ch" + _currentChapter + ", started ch" + index);
            endChapter(_currentChapter, time);
            startChapter(index, time);
        }
        else if (index - _currentChapter == -1 || index - _currentChapter == 0)
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
            Debug.Log(this.GetType() + " endChapter logged ");

            // is new chapter unlocked?
            if (index > furthestChapter)
            {
                furthestChapter = index;
                MemoryManager.get().configuration.furthestChapter = index;
                RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWFURTHEST);
            }

            updateCompletion(index, endTime);
        }
    }

    private void updateCompletion(int index, float endTime)
    {
        Debug.Log(this.GetType() + " updateCompletion(" + index + ", " + endTime + ")");

        // computation of completion time
        chapters[_currentChapter].ownLastCompletionTime = endTime - _startTime;
        Debug.Log(this.GetType() + " updateCompletion logged " + chapters[_currentChapter].ownLastCompletionTime + " for " + _currentChapter);

        // chapter or total completion time update?
        bool isChapterCompletion = (index != completionsCount - 2);
        string completionType = isChapterCompletion ? "chapter" : "total";
        Debug.Log(this.GetType() + " updateCompletion(" + index + ") completion = " + completionType);

        if (!isChapterCompletion)
        {
            // computation of total completion time
            float totalTime = 0;
            for (int chapterIndex = 0; chapterIndex < completionsCount - 2; chapterIndex++)
            {
                totalTime += chapters[chapterIndex].ownLastCompletionTime;
            }
            chapters[index].ownLastCompletionTime = totalTime;
            Debug.Log(this.GetType() + " updateCompletion logged total completion time " + chapters[_currentChapter].ownLastCompletionTime + " for " + _currentChapter);
        }

        // are records broken?
        if (chapters[index].ownLastCompletionTime < chapters[index].ownBestCompletionTime)
        {
            Debug.Log(this.GetType() + " updateCompletion own " + completionType + " completion record broken");
            // TODO put in MemoryManager

            // update of own completion of chapter / game
            chapters[index].ownBestCompletionTime = chapters[index].ownLastCompletionTime;

            // RedMetrics
            CustomData customData = getCustomData(index, isChapterCompletion);
            RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWOWNRECORD, customData);

            if (chapters[index].ownLastCompletionTime < chapters[index].worldBestCompletionTime)
            {
                Debug.Log(this.GetType() + " updateCompletion world " + completionType + " completion record broken");
                // TODO upload
                // update of world completion of chapter / game
                chapters[index].worldBestCompletionTime = chapters[index].ownBestCompletionTime;

                // RedMetrics
                RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWWORLDRECORD, customData);
            }
        }
    }

    private CustomData getCustomData(int index, bool isChapterCompletion)
    {
        Debug.Log(this.GetType() + " getCustomData(" + index + ", " + isChapterCompletion + ")");
        CustomData data;

        if (isChapterCompletion)
        {
            data = new CustomData(CustomDataTag.CHAPTER, index.ToString());
        }
        else
        {
            data = new CustomData(CustomDataTag.TOTAL, chapters[index].ownLastCompletionTime.ToString());
        }
        Debug.Log(this.GetType() + " getCustomData(" + index + ") returns " + data);
        return data;
    }

    private string[] getCompletionAbstract()
    {
        Debug.Log(this.GetType() + " getCompletionAbstract");
        string[] result = new string[_columnCount];

        _chapterString = Localization.Localize(_chapterKey);
        _totalString = Localization.Localize(_totalKey);
        _hoursString = Localization.Localize(_keyStem + _hoursSuffix);
        _minutesString = Localization.Localize(_keyStem + _minutesSuffix);
        _secondsString = Localization.Localize(_keyStem + _secondsSuffix);
        _millisecondsString = Localization.Localize(_keyStem + _millisecondsSuffix);
        // Debug.Log("h='" + _hours + "'");
        // Debug.Log("min='" + _minutes + "'");
        // Debug.Log("s='" + _seconds + "'");
        // Debug.Log("ms='" + _milliseconds + "'");

        for (int index = 0; index <= completionsCount; index++)
        {
            if (index == 0)
            {
                for (int column = 0; column < _columnCount; column++)
                {
                    result[column] += Localization.Localize(_keyStem + _columnSuffix + (column + 1) + _titleSuffix);
                }
            }
            else
            {
                if (index != completionsCount)
                {
                    result[0] += "\n" + _chapterString + " " + index;
                }
                else
                {
                    result[0] += "\n" + _totalString;
                }
                result[1] += "\n" + formatTime(chapters[index - 1].ownLastCompletionTime);
                result[2] += "\n" + formatTime(chapters[index - 1].ownBestCompletionTime);
                result[3] += "\n" + formatTime(chapters[index - 1].worldBestCompletionTime);
            }
        }
        return result;
    }

    public void finish(float endTime, UILabel[] columnLabels)
    {
        Debug.Log(this.GetType() + " fillInColumns");

        // end last chapter
        endChapter(_currentChapter, endTime);

        if (_columnCount != columnLabels.Length)
        {
            Debug.LogWarning(this.GetType() + "incorrect column count");
        }
        else
        {
            string[] results = getCompletionAbstract();
            for (int column = 0; column < _columnCount; column++)
            {
                columnLabels[column].text = results[column];
            }
        }

    }

    private string formatTime(float completionTime)
    {
        Debug.Log(this.GetType() + " formatTime(" + completionTime + ")");

        string result = "not completed";
        if (Mathf.Infinity != completionTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(completionTime);
            // result = time.ToString();
            // result = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            result = "";
            if (0 != time.Hours)
            {
                result += time.Hours + _hoursString;
            }
            if (0 != time.Minutes || 0 != time.Hours)
            {
                result += time.Minutes + _minutesString;
            }
            result += time.Seconds + _secondsString + time.Milliseconds + _millisecondsString;
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
        // Debug.Log(this.GetType() + " ctor");
        ownLastCompletionTime = Mathf.Infinity;
        ownBestCompletionTime = Mathf.Infinity;
        worldBestCompletionTime = Mathf.Infinity;
    }
}


