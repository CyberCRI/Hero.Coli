using UnityEngine;
using System;

public class Scorekeeper
{
    private const int _chapterCount = 9 + 1;
    private const int _columnCount = 4;
    private const string _keyStem = "TUTORIAL.FINALSCOREBOARD.";
    private const string _chapterSuffix = "CHAPTER";
    private const string _columnSuffix = "COLUMN";
    private const string _titleSuffix = "TITLE";
    private const string _hoursSuffix = "H";
    private const string _minutesSuffix = "MIN";
    private const string _secondsSuffix = "S";
    private const string _millisecondsSuffix = "MS";
    private const string _chapterKey = _keyStem + _chapterSuffix;
    private ChapterCompletion[] _chapters = new ChapterCompletion[_chapterCount];
    private float _startTime;
    private int _currentChapter = 0, _furthestChapter = -1;
    private string _chapterString, _hours, _minutes, _seconds, _milliseconds;

    public Scorekeeper()
    {
        // Debug.Log(this.GetType() + " ctor");
        initialize();
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

            // computation of completion time
            _chapters[_currentChapter].ownLastCompletionTime = endTime - _startTime;
            Debug.Log(this.GetType() + " endChapter logged " + _chapters[_currentChapter].ownLastCompletionTime + " for " + _currentChapter);

            // are this chapter's records broken?
            if (_chapters[_currentChapter].ownLastCompletionTime < _chapters[_currentChapter].ownBestCompletionTime)
            {
                Debug.Log(this.GetType() + " endChapter own chapter completion record broken");
                // TODO put in MemoryManager
                _chapters[_currentChapter].ownBestCompletionTime = _chapters[_currentChapter].ownLastCompletionTime;
                CustomData chapterData = new CustomData(CustomDataTag.CHAPTER, index.ToString());
                RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWOWNRECORD, chapterData);

                if (_chapters[_currentChapter].ownLastCompletionTime < _chapters[_currentChapter].worldBestCompletionTime)
                {
                    Debug.Log(this.GetType() + " endChapter world chapter completion record broken");
                    // TODO upload
                    _chapters[_currentChapter].worldBestCompletionTime = _chapters[_currentChapter].ownBestCompletionTime;
                    RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWWORLDRECORD, chapterData);
                }
            }

            // total completion time
            if (index == _chapterCount - 2)
            {
                Debug.Log(this.GetType() + " final chapter finished");
                
                // computation of completion time
                float totalTime = 0;
                for (int chapterIndex = 0; chapterIndex < _chapterCount-1; chapterIndex++)
                {
                    totalTime += _chapters[chapterIndex].ownLastCompletionTime;
                }

                // are this game's records broken?
                if (_chapters[_chapterCount-1].ownLastCompletionTime < _chapters[_chapterCount-1].ownBestCompletionTime)
                {
                    Debug.Log(this.GetType() + " endChapter own total completion record broken");
                    // TODO put in MemoryManager
                    _chapters[_chapterCount-1].ownBestCompletionTime = _chapters[_chapterCount-1].ownLastCompletionTime;
                    CustomData chapterData = new CustomData(CustomDataTag.CHAPTER, index.ToString());
                    RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWOWNRECORD, chapterData);

                    if (_chapters[_chapterCount-1].ownLastCompletionTime < _chapters[_chapterCount-1].worldBestCompletionTime)
                    {
                        Debug.Log(this.GetType() + " endChapter world total completion record broken");
                        // TODO upload
                        _chapters[_chapterCount-1].worldBestCompletionTime = _chapters[_chapterCount-1].ownBestCompletionTime;
                        RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWWORLDRECORD, chapterData);
                    }
                }
            }
        }
    }

    private void updateCompletion(int index)
    {
        string completionType = index == _chapterCount - 1 ? "chapter" : "total";
        if (_chapters[index].ownLastCompletionTime < _chapters[index].ownBestCompletionTime)
        {
            Debug.Log(this.GetType() + " updateCompletion own " + completionType + " completion record broken");
            // TODO put in MemoryManager

            // update of own completion of chapter / game
            _chapters[index].ownBestCompletionTime = _chapters[index].ownLastCompletionTime;

            // RedMetrics
            CustomData customData = getCustomData(index);            
            RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWOWNRECORD, customData);

            if (_chapters[index].ownLastCompletionTime < _chapters[index].worldBestCompletionTime)
            {
                Debug.Log(this.GetType() + " updateCompletion world " + completionType + " completion record broken");
                // TODO upload
                // update of world completion of chapter / game
                _chapters[index].worldBestCompletionTime = _chapters[index].ownBestCompletionTime;
                
                // RedMetrics
                RedMetricsManager.get().sendRichEvent(TrackingEvent.NEWWORLDRECORD, customData);
            }
        }
    }

    private CustomData getCustomData(int index)
    {
        Debug.Log(this.GetType() + " getCustomData(" + index + ")");
        CustomData data;
        bool isChapterCompletion = index == _chapterCount - 1;

        if (isChapterCompletion)
        {
            data = new CustomData(CustomDataTag.CHAPTER, index.ToString());
        }
        else
        {
            data = new CustomData(CustomDataTag.TOTAL, _chapters[index].ownLastCompletionTime.ToString());
        } 
        Debug.Log(this.GetType() + " getCustomData(" + index + ") returns " + data);
        return data;
    }

    private void initialize()
    {
        Debug.Log(this.GetType() + " initialize");
        for (int index = 0; index < _chapterCount; index++)
        {
            _chapters[index] = new ChapterCompletion();
        }
    }

    private string[] getCompletionAbstract()
    {
        Debug.Log(this.GetType() + " getCompletionAbstract");
        string[] result = new string[_columnCount];

        _chapterString = Localization.Localize(_chapterKey);
        _hours = Localization.Localize(_keyStem + _hoursSuffix);
        _minutes = Localization.Localize(_keyStem + _minutesSuffix);
        _seconds = Localization.Localize(_keyStem + _secondsSuffix);
        _milliseconds = Localization.Localize(_keyStem + _millisecondsSuffix);
        // Debug.Log("h='" + _hours + "'");
        // Debug.Log("min='" + _minutes + "'");
        // Debug.Log("s='" + _seconds + "'");
        // Debug.Log("ms='" + _milliseconds + "'");

        for (int index = 0; index <= _chapterCount; index++)
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
                if (index != _chapterCount)
                {
                    result[0] += "\n" + _chapterString + " " + index;
                }
                else
                {
                    result[0] += "\nTOTAL";
                }
                result[1] += "\n" + formatTime(_chapters[index - 1].ownLastCompletionTime);
                result[2] += "\n" + formatTime(_chapters[index - 1].ownBestCompletionTime);
                result[3] += "\n" + formatTime(_chapters[index - 1].worldBestCompletionTime);
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
                result += time.Hours + _hours;
            }
            if (0 != time.Minutes || 0 != time.Hours)
            {
                result += time.Minutes + _minutes;
            }
            result += time.Seconds + _seconds + time.Milliseconds + _milliseconds;
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


