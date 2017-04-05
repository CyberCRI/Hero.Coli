using UnityEngine;

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

	protected T GetItemOfType<T>()
	{
		foreach (var item in _items) {
			if (item.gameObject.GetComponent<T> () != null)
				return item.gameObject.GetComponent<T>();
		}
		return default (T);
	}

    public override string ToString()
    {
        return "MainMenuItemArray[" + MainMenuItem.ToString(_items) + "]";
    }
}
