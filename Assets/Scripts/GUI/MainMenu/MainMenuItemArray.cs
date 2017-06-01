using UnityEngine;
using System.Collections.Generic;

public class MainMenuItemArray : MonoBehaviour
{
    public MainMenuItem[] _items;
    public int itemToActivateFirst;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void hideIndexes(List<int> indexesToHide)
    {
        // Debug.Log(this.GetType() + " hideIndexes indexesToHide=" + Logger.ToString<int>(indexesToHide));
        indexesToHide.RemoveAll((int i) => (i < 0 || i >= _items.Length));
        MainMenuItem[] savedItems = new MainMenuItem[_items.Length - indexesToHide.Count];
        int j = 0;
        for (int i = 0; i < _items.Length; i++)
        {
            if (!indexesToHide.Contains(i))
            {
                savedItems[j] = _items[i];
                j++;
            }
            // else
            // {
            //     Debug.Log(this.GetType() + " hideIndexes hides " + _items[i].name);
            // }
        }

        // Debug.Log(this.GetType() + " hideIndexes relativeOffset");

        for (int i = savedItems.Length - 1; i >= 0; i--)
        {
            savedItems[i].GetComponent<UIAnchor>().relativeOffset.y = _items[i].GetComponent<UIAnchor>().relativeOffset.y;
        }

        // Debug.Log(this.GetType() + " hideIndexes SetActive");

        foreach (int i in indexesToHide)
        {
            _items[i].gameObject.SetActive(false);
        }

        _items = new MainMenuItem[savedItems.Length];

        // Debug.Log(this.GetType() + " hideIndexes _items");

        for (int i = 0; i < _items.Length; i++)
        {
            _items[i] = savedItems[i];
        }
    }

    protected T getItemOfType<T>()
    {
        foreach (MainMenuItem item in _items)
        {
            if (item.gameObject.GetComponent<T>() != null)
            {
                return item.gameObject.GetComponent<T>();
            }
        }
        return default(T);
    }

    public override string ToString()
    {
        return "MainMenuItemArray[" + MainMenuItem.ToString(_items) + "]";
    }
}
