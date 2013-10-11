using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DisplayedElement : MonoBehaviour {


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

    Debug.Log("DisplayedElement::Create("
    + "parentTransform="+parentTransform
    + ", localPosition="+localPosition
    + ", spriteName="+spriteName+nullSpriteName
    + ", prefab="+prefab
      );

    GameObject newElement = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
    Debug.Log("DisplayedElement::Create instantiated");

    newElement.transform.parent = parentTransform;
    newElement.transform.localPosition = localPosition;
    newElement.transform.localScale = new Vector3(1f, 1f, 0);
    Debug.Log("DisplayedElement::Create GetComponent<DisplayedElement>()");
    DisplayedElement script = newElement.GetComponent<DisplayedElement>();
    script._id = ++_idCounter;
    Debug.Log("DisplayedElement::Create script._id = "+script._id);
    script._currentSpriteName = spriteName;
    script.setSprite(script._currentSpriteName);
    Debug.Log("DisplayedElement::Create script._currentSpriteName = "+script._currentSpriteName);
    Debug.Log("DisplayedDevice::Create ends");

    return script;
 }

 public void Remove() {
   Destroy(gameObject);
 }

 public void Redraw(Vector3 newLocalPosition) {
   gameObject.transform.localPosition = newLocalPosition;
 }

 protected void setSprite(string spriteName) {
   Logger.Log("setSprite("+spriteName+")", Logger.Level.TRACE);
   _sprite.spriteName = spriteName;
 }
 
 protected abstract void OnPress(bool isPressed);
}
