// #define WORLDRECORDS

using UnityEngine;
using System;

public class Scorekeeper : ILocalizable
{
    // all chapters + total time
    public const int completionsCount = Checkpoint.chapterCount + 1;
#if WORLDRECORDS
    private const int _columnCount = 4;
#else 
    private const int _columnCount = 3;
#endif
    private const string _keyStem = "TUTORIAL.FINALSCOREBOARD.";
    private const string _chapterSuffix = "CHAPTER";
    private const string _totalSuffix = "TOTAL";
    private const string _columnSuffix = "COLUMN";
    private const string _titleSuffix = "TITLE";
    private const string _hoursSuffix = "H";
    private const string _minutesSuffix = "MIN";
    private const string _secondsSuffix = "S";
    private const string _millisecondsSuffix = "MS";
    private const string _notCompletedSuffix = "NOTCOMPLETED";
    private const string _chapterKey = _keyStem + _chapterSuffix;
    private const string _totalKey = _keyStem + _totalSuffix;
    private const string _notCompletedKey = _keyStem + _notCompletedSuffix;
    private ChapterCompletion[] _chapters = null;
    private float _startTime;
    private int _currentChapter = 0, _furthestChapter = -1;
    private string _chapterString, _totalString, _hoursString, _minutesString, _secondsString, _millisecondsString, _notCompletedString;
    private MapChapterUnlocker _unlocker;
    private UILabel[] _columnLabels;

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
                furthestChapter = MemoryManager.get().configuration.furthestChapter;
                // Debug.Log(this.GetType() + " furthestChapter.get " + _furthestChapter);
            }
            return _furthestChapter;
        }
        set
        {
            int checkedValue = value;
            if (value < 0)
            {
                Debug.LogWarning(this.GetType() + " furthestChapter.set tried to set to " + value + " < 0");
                checkedValue = 0;
            }
            else if (value > _unlocker.maxChapterIndex)
            {
                Debug.LogWarning(this.GetType() + " furthestChapter.set tried to set to " + value + " > MapChapterUnlocker.maxChapterIndex = " + _unlocker.maxChapterIndex);
                checkedValue = _unlocker.maxChapterIndex;
            }

            // Debug.Log(this.GetType() + " furthestChapter.set set to " + value + ", was " + _furthestChapter);

            _furthestChapter = checkedValue;
            MemoryManager.get().configuration.furthestChapter = checkedValue;
            _unlocker.setFurthestChapter(checkedValue);

            // RedMetrics
            CustomData customData = getCustomData(checkedValue, true);
            RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWFURTHEST, customData);

            // Debug.Log(this.GetType() + " furthestChapter.set set to " + _furthestChapter);
        }
    }

    private ChapterCompletion[] chapters
    {
        get
        {
            if (null == _chapters)
            {
                // Debug.Log(this.GetType() + " get chapters initializes");
                _chapters = new ChapterCompletion[completionsCount];
                float[] bestTimes = MemoryManager.get().configuration.bestTimes;
                // Debug.Log(this.GetType() + " get chapters initializes got configuration.bestTimes=" + Logger.ToString<float>(bestTimes));
                for (int index = 0; index < completionsCount; index++)
                {
                    // Debug.Log(this.GetType() + " get chapters initializes index = " + index);
                    _chapters[index] = new ChapterCompletion();
                    _chapters[index].ownBestCompletionTime = bestTimes[index];
                }
            }
            return _chapters;
        }
    }

    // dev method
    public void unlockAll()
    {
        Debug.Log(this.GetType() + " unlockAll");
        furthestChapter = _unlocker.maxChapterIndex;
    }

    public void collideChapter(int index, float time)
    {
        // Debug.Log(this.GetType() + " collideChapter(" + index + ", " + time + ")");
        if (index - _currentChapter == 1)
        {
            // Debug.Log(this.GetType() + " collideChapter finished ch" + _currentChapter + ", started ch" + index);
            endChapter(_currentChapter, time);
            startChapter(index, time);
        }
        else if (index - _currentChapter == -1 || index - _currentChapter == 0)
        {
            // Debug.Log(this.GetType() + " collideChapter went back from ch" + _currentChapter + " to ch" + index);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " abnormal flow: previous chapter=" + _currentChapter + ", encountered chapter=" + index);
        }
    }

    public void startChapter(int index, float startTime)
    {
        // Debug.Log(this.GetType() + " startChapter(" + index + ", " + startTime + ")");
        _currentChapter = index;
        _startTime = startTime;
    }

    private void endChapter(int index, float endTime)
    {
        // Debug.Log(this.GetType() + " endChapter(" + index + ", " + endTime + ")");
        if (index != _currentChapter)
        {
            Debug.LogWarning(this.GetType() + " chapter indexes don't match: " + _currentChapter + " != " + index);
        }
        else
        {
            // Debug.Log(this.GetType() + " endChapter logged ");

            // is new chapter unlocked?
            if (index + 1 > furthestChapter)
            {
                // Debug.Log(this.GetType() + " endChapter unlocked chapter index = " + (index + 1));
                furthestChapter = index + 1;
            }
            // else
            // {
            //     // Debug.Log(this.GetType() + " endChapter reached chapter index =" + index + 1 + " <= furthestChapter index = " + furthestChapter);
            // }

            // Debug.Log(this.GetType() + " endChapter(" + index + ", " + endTime + ")");

            // computation of completion time
            chapters[_currentChapter].ownLastCompletionTime = endTime - _startTime;
            // Debug.Log(this.GetType() + " endChapter logged " + chapters[_currentChapter].ownLastCompletionTime + " for " + _currentChapter);

            // chapter or total completion time update?
            bool isChapterCompletion = (index != completionsCount - 2);
            // string completionType = isChapterCompletion ? "chapter" : "total";
            // Debug.Log(this.GetType() + " endChapter(" + index + ") completion = " + completionType);

            if (!isChapterCompletion)
            {
                // computation of total completion time
                float totalTime = 0;
                for (int chapterIndex = 0; chapterIndex < completionsCount - 2; chapterIndex++)
                {
                    totalTime += chapters[chapterIndex].ownLastCompletionTime;
                }
                chapters[index].ownLastCompletionTime = totalTime;
                // Debug.Log(this.GetType() + " endChapter logged total completion time " + chapters[_currentChapter].ownLastCompletionTime + " for " + _currentChapter);
            }

            // are records broken?
            if (chapters[index].ownLastCompletionTime < chapters[index].ownBestCompletionTime)
            {
                // Debug.Log(this.GetType() + " endChapter own " + completionType + " completion record broken");

                // update of own completion of chapter / game
                chapters[index].ownBestCompletionTime = chapters[index].ownLastCompletionTime;
                // update of saved record
                MemoryManager.get().configuration.setBestTime(index, chapters[index].ownBestCompletionTime);

                // RedMetrics
                CustomData customData = getCustomData(index, isChapterCompletion);
                RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWOWNRECORD, customData);
#if WORLDRECORDS
                if (chapters[index].ownLastCompletionTime < chapters[index].worldBestCompletionTime)
                {
                    // Debug.Log(this.GetType() + " endChapter world " + completionType + " completion record broken");

                    // update of world completion of chapter / game
                    chapters[index].worldBestCompletionTime = chapters[index].ownBestCompletionTime;
                    // TODO upload

                    // RedMetrics
                    RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWWORLDRECORD, customData);
                }
#endif
            }
        }
    }

    private CustomData getCustomData(int index, bool isChapterCompletion)
    {
        // Debug.Log(this.GetType() + " getCustomData(" + index + ", " + isChapterCompletion + ")");
        CustomData data;

        if (isChapterCompletion)
        {
            data = new CustomData(CustomDataTag.CHAPTER, index.ToString());
        }
        else
        {
            data = new CustomData(CustomDataTag.TOTAL, chapters[index].ownLastCompletionTime.ToString());
        }

        data.merge(new CustomData(CustomDataTag.DURATION, secondsToString(chapters[index].ownLastCompletionTime)));

        // Debug.Log(this.GetType() + " getCustomData(" + index + ") returns " + data);
        return data;
    }

    private string[] getCompletionAbstract()
    {
        // Debug.Log(this.GetType() + " getCompletionAbstract");
        string[] result = new string[_columnCount];

        _chapterString = Localization.Localize(_chapterKey);
        _totalString = Localization.Localize(_totalKey);
        _hoursString = Localization.Localize(_keyStem + _hoursSuffix);
        _minutesString = Localization.Localize(_keyStem + _minutesSuffix);
        _secondsString = Localization.Localize(_keyStem + _secondsSuffix);
        _millisecondsString = Localization.Localize(_keyStem + _millisecondsSuffix);
        _notCompletedString = Localization.Localize(_notCompletedKey);

        // Debug.Log("_chapterString='" + _chapterString + "'");
        // Debug.Log("_totalString='" + _totalString + "'");
        // Debug.Log("h='" + _hoursString + "'");
        // Debug.Log("min='" + _minutesString + "'");
        // Debug.Log("s='" + _secondsString + "'");
        // Debug.Log("ms='" + _millisecondsString + "'");
        // Debug.Log("_notCompletedString='" + _notCompletedString + "'");

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
                result[1] += "\n" + secondsToString(chapters[index - 1].ownLastCompletionTime);
                result[2] += "\n" + secondsToString(chapters[index - 1].ownBestCompletionTime);
#if WORLDRECORDS
                result[3] += "\n" + secondsToString(chapters[index - 1].worldBestCompletionTime);
#endif
            }
        }
        return result;
    }

    public void finish(float endTime, UILabel[] columnLabels)
    {
        // Debug.Log(this.GetType() + " finish");

        // end last chapter
        endChapter(_currentChapter, endTime);

#if !WORLDRECORDS
        columnLabels[3].gameObject.SetActive(false);
        float deltaX = columnLabels[1].transform.localPosition.x - columnLabels[0].transform.localPosition.x;
        columnLabels[0].transform.localPosition = new Vector3(-deltaX, columnLabels[0].transform.localPosition.y, columnLabels[0].transform.localPosition.z);
        columnLabels[1].transform.localPosition = new Vector3(0, columnLabels[1].transform.localPosition.y, columnLabels[1].transform.localPosition.z);
        columnLabels[2].transform.localPosition = new Vector3(deltaX, columnLabels[2].transform.localPosition.y, columnLabels[2].transform.localPosition.z);
#endif
        _columnLabels = columnLabels;
        fillInScoreboard();
    }

    private void fillInScoreboard()
    {
        // Debug.Log(this.GetType() + " fillInScoreboard");
        if (null != _columnLabels)
        {
            // Debug.Log(this.GetType() + " fillInScoreboard null != _columnLabels");
            string[] results = getCompletionAbstract();
            for (int column = 0; column < _columnCount; column++)
            {
                _columnLabels[column].text = results[column];
            }
        }
    }

    private string secondsToString(float completionTimeInSeconds)
    {
        // Debug.Log(this.GetType() + " formatTime(" + completionTime + ")");

        string result = _notCompletedString;
        if (Mathf.Infinity != completionTimeInSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(completionTimeInSeconds);
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

    public void setMapChapterUnlocker(MapChapterUnlocker unlocker)
    {
        // Debug.Log(this.GetType() + " setMapChapterUnlocker");
        _unlocker = unlocker;
        _unlocker.setFurthestChapter(furthestChapter);
    }

    public void onLanguageChanged()
    {
        // Debug.Log(this.GetType() + " onLanguageChanged");
        fillInScoreboard();
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


