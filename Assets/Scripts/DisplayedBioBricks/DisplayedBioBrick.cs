using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayedBioBrick : DisplayedElement {

  public static string                              prefabURI               = "GUI/screen3/BioBricks/DisplayedBioBrickPrefab";
  public static UnityEngine.Object                  prefab                  = null;
  public static string                              assemblyZonePanelName   = "AssemblyZonePanel";


  public static Dictionary<BioBrick.Type, string>   spriteNamesDico = new Dictionary<BioBrick.Type, string>() {
    {BioBrick.Type.GENE,        "gene"},
    {BioBrick.Type.PROMOTER,    "promoter"},
    {BioBrick.Type.RBS,         "RBS"},
    {BioBrick.Type.TERMINATOR,  "terminator"},
    {BioBrick.Type.UNKNOWN,     "unknown"}
  };

  public UILabel                    _label;
  public BioBrick                   _biobrick;
  public LastHoveredInfoManager     _lastHoveredInfoManager;


 public static DisplayedBioBrick Create(
   Transform parentTransform
   ,Vector3 localPosition
   ,string spriteName
   ,BioBrick biobrick
   ,Object externalPrefab = null
   )
 {
    string usedSpriteName = ((spriteName!=null)&&(spriteName!=""))?spriteName:getSpriteName(biobrick);
    string nullSpriteName = ((spriteName!=null)&&(spriteName!=""))?"":"(null)=>"+usedSpriteName;

    if(prefab == null) prefab = Resources.Load(prefabURI);
    Object prefabToUse = (externalPrefab==null)?prefab:externalPrefab;

    Logger.Log("DisplayedBioBrick::Create(parentTransform="+parentTransform
      + ", localPosition="+localPosition
      + ", spriteName="+spriteName+nullSpriteName
      + ", biobrick="+biobrick
      , Logger.Level.DEBUG
      );

    DisplayedBioBrick result = (DisplayedBioBrick)DisplayedElement.Create(
      parentTransform
      ,localPosition
      ,usedSpriteName
      ,prefabToUse
      );

    Initialize(result, biobrick);

    return result;
 }

  protected static void Initialize(
    DisplayedBioBrick biobrickScript
    ,BioBrick biobrick
  ) {
      Logger.Log("DisplayedBioBrick::Initialize("+biobrickScript+", "+biobrick+") starts", Logger.Level.TRACE);
      biobrickScript._biobrick = biobrick;
      biobrickScript._label.text = biobrick.getName();
      Logger.Log("DisplayedBioBrick::Initialize ends with biobrickScript._lastHoveredInfoManager="+biobrickScript._lastHoveredInfoManager, Logger.Level.TRACE);
  }

  public static string getSpriteName(BioBrick brick) {
    return spriteNamesDico[brick.getType()];
  }
 
  protected string getDebugInfos() {
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+", label="+_label.text+" time="+Time.realtimeSinceStartup;
  }

  protected override void OnPress(bool isPressed) {
    Logger.Log("DisplayedBioBrick::OnPress _id="+_id+", isPressed="+isPressed, Logger.Level.INFO);
  }

  protected void OnHover(bool isOver) {
    Logger.Log("DisplayedBioBrick::OnHover("+isOver+")", Logger.Level.TRACE);
    if(_lastHoveredInfoManager == null) {
      _lastHoveredInfoManager = GameObject.Find ("LastHoveredInfo").GetComponent<LastHoveredInfoManager>();
    }
    if (isOver) {
      _lastHoveredInfoManager.setHoveredBioBrick<BioBrick>(_biobrick);
    } else {
      _lastHoveredInfoManager.setHoveredDefault();
    }
  }
}
