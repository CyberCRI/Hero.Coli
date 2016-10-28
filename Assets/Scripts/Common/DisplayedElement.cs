using UnityEngine;

public class DisplayedElement : ExternalOnPressButton
{

    protected static int _idCounter = 0;

    public UIAtlas _atlas;
    public int _id;
    public UISprite _sprite;

    public int getID()
    {
        return _id;
    }

    public static DisplayedElement Create(
      Transform parentTransform
      , Vector3 localPosition
      , string spriteName
      , Object prefab
      )
    {
        string nullSpriteName = (spriteName != null) ? "" : "(null)";

        // Debug.Log("DisplayedElement Create("
        // + "parentTransform=" + parentTransform
        // + ", localPosition=" + localPosition
        // + ", spriteName=" + spriteName + nullSpriteName
        // + ", prefab=" + prefab
        // + ")"
        // );

        GameObject newElement = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
        // Debug.Log("DisplayedElement Create instantiated");

        newElement.transform.parent = parentTransform;
        newElement.transform.localPosition = localPosition;
        newElement.transform.localScale = Vector3.one;
        // Debug.Log("DisplayedElement Create GetComponent<DisplayedElement>()");
        DisplayedElement script = newElement.GetComponent<DisplayedElement>();

        if (null == script)
        {
            //TODO remove hack
            Debug.LogWarning("DisplayedElement Create (null == script) with name=" + newElement.name);
            InventoryDevice inventoryDevice = newElement.GetComponent<InventoryDevice>();
            if (inventoryDevice != null)
            {
                // Debug.Log("DisplayedElement Create found inventory device");
                script = inventoryDevice.inventoriedDisplayedDevice;
                if (script != null)
                {
                    // Debug.Log("DisplayedElement Create found inventory device script");
                }
                else
                {
                    Debug.LogWarning("DisplayedElement Create failed to find inventory device script");
                }
            }
        }

        script._id = ++_idCounter;
        // Debug.Log("DisplayedElement Create script._id = " + script._id);
        script.setSprite(spriteName);
        // Debug.Log("DisplayedElement Create ends");

        return script;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    public void Redraw(Vector3 newLocalPosition)
    {
        gameObject.transform.localPosition = newLocalPosition;
    }

    protected void setSprite(string spriteName)
    {
        // Debug.Log(this.GetType() + " setSprite(" + spriteName + ")");
        _sprite.spriteName = spriteName;
    }

    public override void OnPress(bool isPressed)
    {
        // Debug.Log(this.GetType() + " OnPress " + _id);
    }
}
