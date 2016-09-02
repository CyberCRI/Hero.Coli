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
  public static UnityEngine.Object                  genericPrefab           = null;


  public static Dictionary<BioBrick.Type, string>   spriteNamesDico = new Dictionary<BioBrick.Type, string>() {
    {BioBrick.Type.GENE,        "gene"},
    {BioBrick.Type.PROMOTER,    "promoter"},
    {BioBrick.Type.RBS,         "RBS"},
    {BioBrick.Type.TERMINATOR,  "terminator"},
    {BioBrick.Type.UNKNOWN,     "unknown"}
  };

  public BioBrick                   _biobrick;

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

    if(genericPrefab == null) genericPrefab = Resources.Load(prefabURI);
    Object prefabToUse = (externalPrefab==null)?genericPrefab:externalPrefab;

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

  }

  public static string getSpriteName(BioBrick brick) {
    return spriteNamesDico[brick.getType()];
  }

  protected string getDebugInfos() {
    return "Displayed biobrick id="+_id+", inner biobrick="+_biobrick+" time="+Time.realtimeSinceStartup;
  }

  public override void OnPress(bool isPressed) {
    Logger.Log("GenericDisplayedBioBrick::OnPress _id="+_id+", isPressed="+isPressed, Logger.Level.INFO);
  }

  void OnHover(bool isOver)
  {
    Logger.Log("DisplayedDevice::OnHover("+isOver+")", Logger.Level.DEBUG);
    TooltipManager.displayTooltip(isOver, _biobrick, transform.position);
  }
}
