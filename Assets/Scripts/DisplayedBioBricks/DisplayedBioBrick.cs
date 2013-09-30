using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedBioBrick : DisplayedElement {

  //public static CraftZoneManager                    craftZoneManager;
  public static string                              prefabURI               = "GUI/screen3/BioBricks/DisplayedBioBrickPrefab";
  public static UnityEngine.Object                  prefab                  = null;
  public static string                              assemblyZonePanelName   = "AssemblyZonePanel";
  public static LastHoveredInfoManager              lastHoveredInfoManager  = null;

  public static Dictionary<BioBrick.Type, string>   spriteNamesDico = new Dictionary<BioBrick.Type, string>() {
    {BioBrick.Type.GENE,        "gene"},
    {BioBrick.Type.PROMOTER,    "promoter"},
    {BioBrick.Type.RBS,         "RBS"},
    {BioBrick.Type.TERMINATOR,  "terminator"},
    {BioBrick.Type.UNKNOWN,     "unknown"}
  };



  public UILabel                    _label;
  public BioBrick                   _biobrick;

 public static DisplayedBioBrick Create(
   Transform parentTransform
   ,Vector3 localPosition
   ,string spriteName
   ,BioBrick biobrick
   ,Object externalPrefab = null
   )
 {
    string nullSpriteName = (spriteName!=null)?"":"(null)";

    if(prefab == null) prefab = Resources.Load(prefabURI);
    Object prefabToUse = (externalPrefab==null)?prefab:externalPrefab;

    if(lastHoveredInfoManager==null) lastHoveredInfoManager = GameObject.Find("LastHoveredInfo").GetComponent<LastHoveredInfoManager>();

    Debug.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
      + ", localPosition="+localPosition
      + ", spriteName="+spriteName+nullSpriteName
      + ", biobrick="+biobrick
      );

    DisplayedBioBrick result = (DisplayedBioBrick)DisplayedElement.Create(
      parentTransform
      ,localPosition
      ,spriteName
      ,prefabToUse
      );

    Initialize(result, biobrick);

    return result;
 }

  protected static void Initialize(
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

  public static string getSpriteName<T>(T brick) where T:BioBrick {
    return spriteNamesDico[brick.getType()];
  }
 
  protected string getDebugInfos() {
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+", label="+_label.text+" time="+Time.realtimeSinceStartup;
  }

  protected override void OnPress(bool isPressed) {
    Logger.Log("DisplayedBioBrick::OnPress _id="+_id);
  }

  protected void OnHover(bool isOver) {
    if (isOver) {
      Logger.Log("DisplayedBioBrick::OnHover("+isOver+")", Logger.Level.DEBUG);
      lastHoveredInfoManager.setHoveredBioBrick<BioBrick>(_biobrick);
    }
  }
}
