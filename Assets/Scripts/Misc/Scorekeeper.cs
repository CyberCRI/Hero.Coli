// #define WORLDRECORDS

using UnityEngine;
using System;

public class Scorekeeper : ILocalizable
{
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
    private int _currentChapterIndex = 0, _furthestChapterIndex = -1;
    private string _chapterString, _totalString, _hoursString, _minutesString, _secondsString, /*_millisecondsString, */_notCompletedString;
    private static MapChapterUnlocker _unlocker;
    private UILabel[] _columnLabels;
    private bool _isFurthestChapterInitialized = false;

    public Scorekeeper()
    {
        _unlocker = null;
        _completionsCount = 0;
        // Debug.Log(this.GetType() + " ctor");
    }

    // maxChapterIndex chapters + 1 total
    private static int _completionsCount;
    public static int completionsCount
    {
        get
        {
            if (0 == _completionsCount)
            {
                if (null != _unlocker)
                {
                    // count of completions = completion of each chapter + total completion
                    _completionsCount = _unlocker.maxChapterIndex + 2;
                }
                else
                {
                    Debug.LogWarning("Scorekeeper completionsCount_get null == _unlocker");
                }
            }
            return _completionsCount;
        }
    }
    private int __lastChapterIndex;
    private int _lastChapterIndex
    {
        get
        {
            if (0 == __lastChapterIndex)
            {
                __lastChapterIndex = completionsCount - 2;
            }
            return __lastChapterIndex;
        }
    }
    private int __totalCompletionIndex;
    private int _totalCompletionIndex
    {
        get
        {
            if (0 == __totalCompletionIndex)
            {
                __totalCompletionIndex = completionsCount - 1;
            }
            return __totalCompletionIndex;
        }
    }

    private int furthestChapter
    {
        get
        {
            initializeIfNecessary();
            return _furthestChapterIndex;
        }
        set
        {
            if (value < 0)
            {
                Debug.LogWarning(this.GetType() + " furthestChapter.set tried to erroneously set to " + value + " < 0");
            }
            else if (value > _unlocker.maxChapterIndex)
            {
                Debug.LogWarning(this.GetType() + " furthestChapter.set tried to erroneously set to " + value + " > MapChapterUnlocker.maxChapterIndex = " + _unlocker.maxChapterIndex);
            }
            else if (value == _furthestChapterIndex)
            {
                // Debug.Log(this.GetType() + " furthestChapter.set tried to set again to " + value);
            }
            else
            {
                // Debug.Log(this.GetType() + " furthestChapter.set set to " + value + ", was " + _furthestChapter);
                if (_isFurthestChapterInitialized)
                {
                    MemoryManager.get().configuration.furthestChapter = value;

                    // RedMetrics
                    CustomData customData = getCustomData(value);
                    RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWFURTHEST, customData);
                }

                _furthestChapterIndex = value;
                _unlocker.setFurthestChapter(value);
            }
        }

        // Debug.Log(this.GetType() + " furthestChapter.set set to " + _furthestChapter);
    }

    private void initializeIfNecessary()
    {
        if (!_isFurthestChapterInitialized)
        {
            furthestChapter = MemoryManager.get().configuration.furthestChapter;
            if (MemoryManager.get().configuration.furthestChapter != _furthestChapterIndex)
            {
                Debug.LogWarning(this.GetType() + " furthestChapter.get initialization failed with stored value=" + MemoryManager.get().configuration.furthestChapter);
                furthestChapter = 0;
            }
            _isFurthestChapterInitialized = true;
            // Debug.Log(this.GetType() + " initializeIfNecessary " + _furthestChapter);
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
        // Debug.Log(this.GetType() + " unlockAll");
        furthestChapter = _unlocker.maxChapterIndex;
    }

    public void collideChapter(int index, float time)
    {
        // Debug.Log(this.GetType() + " collideChapter(" + index + ", " + time + ")");
        if (index - _currentChapterIndex == 1)
        {
            // Debug.Log(this.GetType() + " collideChapter finished ch" + _currentChapter + ", started ch" + index);
            endChapter(_currentChapterIndex, time);
            startChapter(index, time);
        }
        else if (index - _currentChapterIndex == -1 || index - _currentChapterIndex == 0)
        {
            // Debug.Log(this.GetType() + " collideChapter went back from ch" + _currentChapter + " to ch" + index);
        }
        else
        {
            Debug.LogWarning(this.GetType() + " abnormal flow: previous chapter=" + _currentChapterIndex + ", encountered chapter=" + index);
        }
    }

    public void startChapter(int index, float startTime)
    {
        // Debug.Log(this.GetType() + " startChapter(" + index + ", " + startTime + ")");
        _currentChapterIndex = index;
        _startTime = startTime;
    }

    private void endChapter(int index, float endTime)
    {
        // Debug.Log(this.GetType() + " endChapter(" + index + ", " + endTime + ")");
        if (index != _currentChapterIndex)
        {
            Debug.LogWarning(this.GetType() + " chapter indexes don't match: " + _currentChapterIndex + " != " + index);
        }
        else
        {
            // is new chapter unlocked?
            furthestChapter = index + 1;

            // computation of completion time
            chapters[_currentChapterIndex].ownLastCompletionTime = endTime - _startTime;
            // Debug.Log(this.GetType() + " endChapter logged " + chapters[_currentChapter].ownLastCompletionTime + " for " + _currentChapter);

            updateRecord(index);
        }
    }

    private bool isTotalCompletion(int index)
    {
        return index == _totalCompletionIndex;
    }

    private void updateRecord(int index)
    {
        // chapter or total completion time update?

        if (index == _lastChapterIndex)
        {
            updateRecord(_totalCompletionIndex);
        }
        else if (isTotalCompletion(index))
        {
            // computation of total completion time
            float totalTime = 0;
            for (int chapterIndex = 0; chapterIndex < _totalCompletionIndex; chapterIndex++)
            {
                totalTime += chapters[chapterIndex].ownLastCompletionTime;
            }
            chapters[_totalCompletionIndex].ownLastCompletionTime = totalTime;
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
            CustomData customData = getCustomData(index);
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

    private CustomData getCustomData(int index)
    {
        // Debug.Log(this.GetType() + " getCustomData(" + index + ")");
        CustomData data;

        if (isTotalCompletion(index))
        {
            data = new CustomData(CustomDataTag.TOTAL, _lastChapterIndex.ToString());
        }
        else
        {
            data = new CustomData(CustomDataTag.CHAPTER, index.ToString());
        }

        data.merge(new CustomData(CustomDataTag.DURATION, secondsToString(chapters[index].ownLastCompletionTime)));

        // Debug.Log(this.GetType() + " getCustomData(" + index + ") returns " + data);
        return data;
    }

    private string[] getCompletionAbstract()
    {
        // Debug.Log(this.GetType() + " getCompletionAbstract with completionsCount=" + completionsCount);

        string[] result = new string[_columnCount];

        _chapterString = Localization.Localize(_chapterKey);
        _totalString = Localization.Localize(_totalKey);
        _hoursString = Localization.Localize(_keyStem + _hoursSuffix);
        _minutesString = Localization.Localize(_keyStem + _minutesSuffix);
        _secondsString = Localization.Localize(_keyStem + _secondsSuffix);
        // _millisecondsString = Localization.Localize(_keyStem + _millisecondsSuffix);
        _notCompletedString = Localization.Localize(_notCompletedKey);

        // Debug.Log("_chapterString='" + _chapterString + "'");
        // Debug.Log("_totalString='" + _totalString + "'");
        // Debug.Log("h='" + _hoursString + "'");
        // Debug.Log("min='" + _minutesString + "'");
        // Debug.Log("s='" + _secondsString + "'");
        // Debug.Log("ms='" + _millisecondsString + "'");
        // Debug.Log("_notCompletedString='" + _notCompletedString + "'");

        // column titles
        for (int column = 0; column < _columnCount; column++)
        {
            result[column] += Localization.Localize(_keyStem + _columnSuffix + (column + 1) + _titleSuffix);
        }

        for (int index = 0; index < completionsCount; index++)
        {
            if (index != completionsCount - 1)
            {
                result[0] += "\n" + _chapterString + " " + (index + 1);
            }
            else
            {
                result[0] += "\n" + _totalString;
            }
            result[1] += "\n" + secondsToString(chapters[index].ownLastCompletionTime);
            result[2] += "\n" + secondsToString(chapters[index].ownBestCompletionTime);
#if WORLDRECORDS
            result[3] += "\n" + secondsToString(chapters[index].worldBestCompletionTime);
#endif
        }
        return result;
    }

    public void finish(float endTime, UILabel[] columnLabels)
    {
        // Debug.Log(this.GetType() + " finish");

        // end last chapter
        endChapter(_currentChapterIndex, endTime);

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
                // line count = all completions + 1 title line
                _columnLabels[column].maxLineCount = completionsCount + 1;
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
            // result += time.Seconds + _secondsString + time.Milliseconds + _millisecondsString;
            result += time.Seconds + _secondsString;
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


