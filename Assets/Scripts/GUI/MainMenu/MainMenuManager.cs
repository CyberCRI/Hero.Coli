using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    public MainMenuItem[] _items;

    //_currentIndex == -1 means nothing is selected
    private int _currentIndex = -1;

    private void deselect() {
        if(_currentIndex >= 0 && _currentIndex < _items.Length) {
            _items[_currentIndex].deselect();
            Debug.LogWarning("deselected item "+_currentIndex);
        } else {
            Debug.LogWarning("couldn't deselect item "+_currentIndex);
        }
        _currentIndex = -1;
    }

    private bool selectItem(string itemName) {
        for( int index = 0; index < _items.Length; index++) {
            if(itemName == _items[index].name) {
                deselect();
                _items[index].select();
                _currentIndex = index;
                Debug.LogWarning("selected item "+index+" via its name '"+itemName+"'");
                return true;
            }
        }
        return false;
    }

    private bool selectItem(int index) {
        int normalizedIndex = index % _items.Length;
        if(null != _items[normalizedIndex]) {
            deselect();
            _items[normalizedIndex].select();
            _currentIndex = normalizedIndex;
            Debug.LogWarning("selected item "+normalizedIndex);
            return true;
        }
        return false;
    }
    
    private bool selectNext() {
        Debug.LogWarning("selectNext");
        return selectItem(_currentIndex+1);
    }

    private bool selectPrevious() {
        Debug.LogWarning("selectPrevious");
        return selectItem(_currentIndex-1);
    }

	// Use this for initialization
	void Start () {
        selectItem(0);
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyUp(KeyCode.UpArrow)) {
            selectPrevious();
        } else if(Input.GetKeyUp(KeyCode.DownArrow)) {
            selectNext();
        }
	}
}
