using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedElement : MonoBehaviour {


  protected static int  _idCounter = 0;

  protected string      _currentSpriteName;
  public UIAtlas        _atlas;
  public int            _id;
  public UISprite       _sprite;

  public int getID() {
    return _id;
  }

  public static DisplayedElement Create(
    Transform parentTransform
    ,Vector3 localPosition
    ,string spriteName
    ,Object prefab
    )
  {
    string nullSpriteName = (spriteName!=null)?"":"(null)";

    Logger.Log("DisplayedElement::Create("
    + "parentTransform="+parentTransform
    + ", localPosition="+localPosition
    + ", spriteName="+spriteName+nullSpriteName
    + ", prefab="+prefab
	+ ")"
    , Logger.Level.WARN
    );

    GameObject newElement = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
    Logger.Log("DisplayedElement::Create instantiated", Logger.Level.TRACE);

    newElement.transform.parent = parentTransform;
    newElement.transform.localPosition = localPosition;
    newElement.transform.localScale = new Vector3(1f, 1f, 0);
    Logger.Log("DisplayedElement::Create GetComponent<DisplayedElement>()", Logger.Level.TRACE);
    DisplayedElement script = newElement.GetComponent<DisplayedElement>();
    
    if(null == script)
    {
      //TODO remove hack
      Debug.LogWarning("(null == script) 1 with name="+newElement.name);
      InventoryDevice inventoryDevice = newElement.GetComponent<InventoryDevice>();
      if(inventoryDevice != null)
      {
        Debug.LogError("WIN?");
        script = inventoryDevice.inventoriedDisplayedDevice;
        if(script != null)
        {
            Debug.LogError("WIN!");
        }
        else
        {
            Debug.LogError("LATE FAIL!");
        }
      }
      else
      {
          Debug.LogError("EARLY FAIL?");
          EquipmentDevice equipmentDevice = newElement.GetComponent<EquipmentDevice>();
          if(equipmentDevice != null)
          {
              Debug.LogError("WIN?");
              script = equipmentDevice.equipedDisplayedDevice;
              if(script != null)
              {
                  Debug.LogError("WIN!");
              }
              else
              {
                  Debug.LogError("LATE FAIL!");
              }
          }
          else
          {
              Debug.LogError("EARLY FAIL!");
          }
      }
    }

    script._id = ++_idCounter;
    Logger.Log("DisplayedElement::Create script._id = "+script._id, Logger.Level.TRACE);
    script._currentSpriteName = spriteName;
    script.setSprite(script._currentSpriteName);
    Logger.Log("DisplayedElement::Create script._currentSpriteName = "+script._currentSpriteName, Logger.Level.TRACE);
    Logger.Log("DisplayedDevice::Create ends", Logger.Level.TRACE);

    return script;
 }

 public void Remove() {
   Destroy(gameObject);
 }

  public void Redraw(Vector3 newLocalPosition) {
    gameObject.transform.localPosition = newLocalPosition;
  }

  protected void setSprite(string spriteName) {
    Logger.Log("DisplayedElement::setSprite("+spriteName+")", Logger.Level.TRACE);
    _sprite.spriteName = spriteName;
  }

  protected virtual void OnPress(bool isPressed)
  {
   Logger.Log("DisplayedElement::OnPress "+_id, Logger.Level.INFO);
  }
}
