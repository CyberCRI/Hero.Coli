using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*!
\brief This class enables the generic display of BioBricks
\details The texture used is by default set according to the BioBrick type

\author Raphael GOUJET
*/

public class GenericDisplayedBioBrick : DisplayedElement {

  public static string                              prefabURI               = "GUI/screen1/Devices/TinyBioBrickIconPrefab";
  public static UnityEngine.Object                  prefab                  = null;
  //public static string                              assemblyZonePanelName   = "AssemblyZonePanel";


  public static Dictionary<BioBrick.Type, string>   spriteNamesDico = new Dictionary<BioBrick.Type, string>() {
    {BioBrick.Type.GENE,        "gene"},
    {BioBrick.Type.PROMOTER,    "promoter"},
    {BioBrick.Type.RBS,         "RBS"},
    {BioBrick.Type.TERMINATOR,  "terminator"},
    {BioBrick.Type.UNKNOWN,     "unknown"}
  };

  //public UILabel                    _label;
  public BioBrick                   _biobrick;
  //public LastHoveredInfoManager     _lastHoveredInfoManager;


 public static GenericDisplayedBioBrick Create(
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

    Logger.Log("GenericDisplayedBioBrick::Create(parentTransform="+parentTransform
      + ", localPosition="+localPosition
      + ", spriteName="+spriteName+nullSpriteName
      + ", biobrick="+biobrick
      , Logger.Level.TRACE
      );

    GenericDisplayedBioBrick result = (GenericDisplayedBioBrick)DisplayedElement.Create(
      parentTransform
      ,localPosition
      ,usedSpriteName
      ,prefabToUse
      );

    Initialize(result, biobrick);

    return result;
 }

  protected static void Initialize(
    GenericDisplayedBioBrick biobrickScript
    ,BioBrick biobrick
  ) {
      Logger.Log("GenericDisplayedBioBrick::Initialize("+biobrickScript+", "+biobrick+") starts", Logger.Level.TRACE);
      biobrickScript._biobrick = biobrick;
      //biobrickScript._label.text = biobrick.getName();
      //Logger.Log("GenericDisplayedBioBrick::Initialize ends with biobrickScript._lastHoveredInfoManager="+biobrickScript._lastHoveredInfoManager, Logger.Level.TRACE);
  }

  public static string getSpriteName(BioBrick brick) {
    return spriteNamesDico[brick.getType()];
  }

  protected string getDebugInfos() {
    //return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+", label="+_label.text+" time="+Time.realtimeSinceStartup;
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+" time="+Time.realtimeSinceStartup;
  }

  protected override void OnPress(bool isPressed) {
    Logger.Log("GenericDisplayedBioBrick::OnPress _id="+_id+", isPressed="+isPressed, Logger.Level.INFO);
  }

  void OnHover(bool isOver)
  {
    Logger.Log("DisplayedDevice::OnHover("+isOver+")", Logger.Level.TEMP);
    TooltipManager.tooltip(isOver, _biobrick, new Vector3(0, 0, 0));
  }
}
