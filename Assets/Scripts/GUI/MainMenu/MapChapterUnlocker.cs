using UnityEngine;

public class MapChapterUnlocker : MonoBehaviour
{
    [SerializeField]
    private MainMenuItemArray _chaptersArray;
    private CheckpointMainMenuItem[] _savedItems; // contains all saved chapters; does not contain the back button
    private MainMenuItem _backButton;
    private bool _updateOnDisable = false;
    private int _index;
    private int _maxChapterIndex;
    public int maxChapterIndex
    {
        get
        {
            initializeIfNecessary();
            return _maxChapterIndex;
        }
    }

    private MainMenuItemArray chaptersArray
    {
        get
        {
            initializeIfNecessary();
            return _chaptersArray;
        }
    }

    private CheckpointMainMenuItem[] savedItems
    {
        get
        {
            initializeIfNecessary();
            return _savedItems;
        }
    }

    private void initializeIfNecessary()
    {
        if (null == _savedItems)
        {
            // Debug.Log(this.GetType() + " initializeIfNecessary initializes");

            // _chaptersArray contains chapters and the backbutton
            _maxChapterIndex = _chaptersArray._items.Length - 2;

            // save all but the backbutton in this array
            _savedItems = new CheckpointMainMenuItem[_maxChapterIndex + 1];
            // Debug.Log(this.GetType() + " initializeIfNecessary initializes with"
            // + " _chaptersArray._items.Length=" + _chaptersArray._items.Length
            // + " _maxChapterIndex=" + _maxChapterIndex
            // + " _savedItems.Length=" + _savedItems.Length);

            for (int i = 0; i < _savedItems.Length; i++)
            {
                // Debug.Log(this.GetType() + " initializeIfNecessary saves item " + i + " = " + _chaptersArray._items[i].name);
                _savedItems[i] = _chaptersArray._items[i] as CheckpointMainMenuItem;
                // string debugString = (null == _savedItems[i]) ? "null" : _savedItems[i].name;
                // Debug.Log(this.GetType() + " initializeIfNecessary saved " + debugString);
            }
            _backButton = _chaptersArray._items[_chaptersArray._items.Length - 1];
        }
    }

    public void setFurthestChapter(int index)
    {
        // Debug.Log(this.GetType() + " setFurthestChapter(" + index + ")");

        _index = index;
        _chaptersArray.itemToActivateFirst = index;
        // Debug.Log(this.GetType() + " setFurthestChapter _chaptersArray.itemToActivateFirst = " + index);

        // if (!this.isActiveAndEnabled)
        {
            // Debug.Log(this.GetType() + " setFurthestChapter isActiveAndEnabled");

            // all unlocked chapters + back button
            chaptersArray._items = new MainMenuItem[index + 2];

            // activation of unlocked chapters
            // for (int i = 0; i < chaptersArray._items.Length - 1; i++)
            for (int i = 0; i <= index; i++)
            {
                // Debug.Log(this.GetType() + " setFurthestChapter unlocks item " + i + " = " + savedItems[i].name);
                chaptersArray._items[i] = savedItems[i];
                savedItems[i].activate(true);
            }

            // special treatment for back button
            // Debug.Log(this.GetType() + " setFurthestChapter special treatment for back button " + _backButton.name);
            chaptersArray._items[chaptersArray._items.Length - 1] = _backButton;

            // inactivation of next chapters
            for (int i = chaptersArray._items.Length + 1; i < savedItems.Length; i++)
            {
                // Debug.Log(this.GetType() + " setFurthestChapter deactivates item " + i + " = " + savedItems[i].name);
                savedItems[i].activate(false);
            }

            // Debug.Log(this.GetType() + " setFurthestChapter = " + index);
        }
        // else
        // {
        //     // Don't process if the map is already displayed
        //     // May otherwise interfere with arrow key navigation of main menu items
        //     // Debug.Log(this.GetType() + " setFurthestChapter !isActiveAndEnabled: can't process");
        //     _updateOnDisable = true;
        // }
    }

    void OnDisable()
    {
        if (_updateOnDisable)
        {
            // Debug.Log(this.GetType() + " OnDisable _updateOnDisable = true");
            _updateOnDisable = false;
            setFurthestChapter(_index);
        }
    }

}