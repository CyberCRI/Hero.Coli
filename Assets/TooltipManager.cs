using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour {

  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "TooltipManager";
  private static TooltipManager _instance;
  public static TooltipManager get() {
    if(_instance == null) {
      Logger.Log("TooltipManager::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<TooltipManager>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("TooltipManager::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  public UISprite tooltipSprite;
  private static IDictionary<BioBrick.Type, IDictionary<string, string>> BioBrickTextureNames = new Dictionary<BioBrick.Type, IDictionary<string, string>>()
  {
    {BioBrick.Type.PROMOTER, new Dictionary<string, string>(){
        {"pCons", "pCons"},
        {"pLac", "pLac"},
        {"pTet", "pTet"}
      }
    },
    {BioBrick.Type.RBS, new Dictionary<string, string>(){
        {"RBS low", "rbs_l"},
        {"RBS very low", "rbs_vl"}
      }
    },
    {BioBrick.Type.GENE, new Dictionary<string, string>(){
        {"Z", "ampR"},
        //{"ampR", "ampR"},
        {"GFP", "gfp"},
        {"X", "hyperflagellation"}
        //{"FLhDC", "hyperflagellation"},
      }
    },
    {BioBrick.Type.TERMINATOR, new Dictionary<string, string>(){
        {"Terminator", "terminator"}
      }
    }
  };

  private static IDictionary<string, string> DeviceTextureNames = new Dictionary<string, string>()
  {
    {"Ampicillin resistance (med)", "ampR_m"},
    {"Green fluorescence (med)", "gfp_m"},
    {"Hyperflagellation (low)", "speed_l"},
    {"Hyperflagellation (med)", "speed_m"}
  };

  public static void tooltip(Device device, Vector3 pos)
  {
    Logger.Log("TooltipManager::tooltip device", Logger.Level.TEMP);
    string spriteName;
    if(DeviceTextureNames.TryGetValue(device.getName(), out spriteName))
    {
      setSpriteNameAndSetActive(spriteName);
    }
    else
    {
      Logger.Log("TooltipManager::tooltip couldn't find texture for device name "+device.getName(), Logger.Level.WARN);
    }
  }

  public static void tooltip()
  {
    Logger.Log("TooltipManager::tooltip()", Logger.Level.TEMP);
    _instance.tooltipSprite.gameObject.SetActive(false);
  }

  public static void tooltip(BioBrick brick, Vector3 pos)
  {
    Logger.Log("TooltipManager::tooltip brick", Logger.Level.TEMP);
    IDictionary<string, string> dico;
    string spriteName;
    if(BioBrickTextureNames.TryGetValue(brick.getType(), out dico))
    {
      if(dico.TryGetValue(brick.getName(), out spriteName))
      {
        setSpriteNameAndSetActive(spriteName);
      }
      else
      {
        Logger.Log("TooltipManager::tooltip couldn't find texture for brick name "+brick.getName(), Logger.Level.WARN);
      }
    }
    else
    {
      Logger.Log("TooltipManager::tooltip couldn't find dico for brick type "+brick.getType(), Logger.Level.WARN);
    }

  }

  private static void setSpriteNameAndSetActive(string spriteName)
  {
    if(null == spriteName) return;
    _instance.tooltipSprite.spriteName = spriteName;
    _instance.tooltipSprite.gameObject.SetActive(true);
  }

}
