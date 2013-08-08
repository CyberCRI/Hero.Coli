using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedBioBrick : MonoBehaviour {

  private static int _idCounter = 0;

  public string _currentSpriteName;
  public UIAtlas _atlas;
  public int _biobrickID;
  public UISprite _sprite;
  public UILabel _label;
  public BioBrick _biobrick;
  public CraftZoneManager _craftZoneManager;
  public static string prefabURI = "GUI/screen3/BioBricks/CraftZoneDisplayedBioBrickPrefab";
  public static Object prefab = null;
 
  public int getID() {
    return _biobrickID;
  }

 public static DisplayedBioBrick Create(
   Transform parentTransform, 
   Vector3 localPosition,
   BioBrick biobrick,
   CraftZoneManager craftZoneManager,
   string spriteName
   )
 {
    string nullSpriteName = (spriteName!=null)?"":"(null)";

    Debug.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
    + ", localPosition="+localPosition
    + ", biobrick="+biobrick
    + ", craftZoneManager="+craftZoneManager
    + ", spriteName="+spriteName+nullSpriteName);

    if(prefab == null) prefab = Resources.Load(prefabURI);
    GameObject newBioBrick = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
    Debug.Log("DisplayedBioBrick::Create instantiation done");

    newBioBrick.transform.parent = parentTransform;
    newBioBrick.transform.localPosition = localPosition;
    newBioBrick.transform.localScale = new Vector3(1f, 1f, 0);
    
    DisplayedBioBrick biobrickScript = newBioBrick.GetComponent<DisplayedBioBrick>();
    biobrickScript._biobrickID = ++_idCounter;
    Debug.Log("DisplayedBioBrick::Create biobrickScript._biobrickID = "+biobrickScript._biobrickID);

    biobrickScript._biobrick = biobrick;
    Debug.Log("DisplayedBioBrick::Create biobrick "+biobrickScript._biobrick);
    biobrickScript._craftZoneManager = craftZoneManager;
    biobrickScript._currentSpriteName = spriteName;
    biobrickScript.setSprite(biobrickScript._currentSpriteName);
    biobrickScript._label.text = biobrick.getName();
    Debug.Log("DisplayedBioBrick::Create ends");

    return biobrickScript;
 }
 
 public void Remove() {
   Destroy(gameObject);
 }
 
 public void Redraw(Vector3 newLocalPosition) {
   gameObject.transform.localPosition = newLocalPosition;
 }
 
 private void setSprite(string spriteName) {
   Debug.Log("setSprite("+spriteName+")");
   _sprite.spriteName = spriteName;
 }
 
 // Use this for initialization
 void Start () {
 }
 
 // Update is called once per frame
 void Update () {
 
 }
 
 protected string getDebugInfos() {
   return "biobrick "+_biobrickID+", inner biobrick "+_biobrick+" time="+Time.realtimeSinceStartup;
 }
 
  private void OnPress(bool isPressed) {
    Logger.Log("DisplayedBioBrick::OnPress _biobrickID="+_biobrickID);
  }
}
