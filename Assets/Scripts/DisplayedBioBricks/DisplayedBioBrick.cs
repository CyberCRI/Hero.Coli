using UnityEngine;
using System; //for Action<>
using System.Collections;
using System.Collections.Generic;

public class DisplayedBioBrick : DisplayedElement {
  public UILabel _label;
  public BioBrick _biobrick;
  public static CraftZoneManager _craftZoneManager;
  public static string prefabURI = "GUI/screen3/BioBricks/CraftZoneDisplayedBioBrickPrefab";
  public static UnityEngine.Object prefab = null;
  public static string assemblyZonePanelName = "AssemblyZonePanel";

 public static DisplayedBioBrick Create(
   Transform parentTransform,
   Vector3 localPosition,
   string spriteName,
   BioBrick biobrick
   )
 {
    string nullSpriteName = (spriteName!=null)?"":"(null)";
    _craftZoneManager = _craftZoneManager!=null?_craftZoneManager:GameObject.Find (assemblyZonePanelName).GetComponent<CraftZoneManager>();

    Debug.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
    + ", localPosition="+localPosition
    + ", spriteName="+spriteName+nullSpriteName
    + ", biobrick="+biobrick
    + ", craftZoneManager="+_craftZoneManager);

    if(prefab == null) prefab = Resources.Load(prefabURI);

    DisplayedBioBrick result = (DisplayedBioBrick)DisplayedElement.Create(
      parentTransform
      ,localPosition
      ,spriteName
      ,prefab
      //,GetInitializer(biobrick, "testString")
      );

    Initialize(result, biobrick);

    return result;
 }

  private static void Initialize(
    DisplayedBioBrick biobrickScript
    ,BioBrick biobrick
  ) {
      Debug.Log("DisplayedBioBrick::Initialize("+biobrickScript+", "+biobrick+") starts");
      biobrickScript._biobrick = biobrick;
      Debug.Log("DisplayedBioBrick::Create biobrick "+biobrickScript._biobrick);
      biobrickScript._label.text = biobrick.getName();
      Debug.Log("DisplayedBioBrick::Initialize ends");
  }
 
 // Use this for initialization
 void Start () {
 }
 
 // Update is called once per frame
 void Update () {
 
 }
 
 protected string getDebugInfos() {
   return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+", label="+_label.text+" time="+Time.realtimeSinceStartup;
 }

 protected override void OnPress(bool isPressed) {
    Logger.Log("DisplayedBioBrick::OnPress _id="+_id);
  }
}
