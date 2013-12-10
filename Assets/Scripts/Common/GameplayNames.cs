using System;
using System.Collections;
using System.Collections.Generic;

public class GameplayNames
{
  private static Dictionary<string, string> brickNames = new Dictionary<string, string>()
  {
    //PROMOTERS
    {"PRCONS", "Constitutive promoter"},
    {"PRLACI1", "placI repressible"},
    {"PRTETR1", "p(tetR) multi"},
    //GENES
    {"FLUO1", "Green - GFP"},
    {"FLUO2", "Red - mCherry"},
    {"MOV", "Flagella master regulator"},
    {"ANTIBIO1", "Ampicillin resistance cassette"},
    {"REPR1", "lacI"},
    {"REPR2", "tetR"},
    //TERMINATORS
    {"DTER", "Double terminator"}
  };

  private static Dictionary<string, string> proteinNames = new Dictionary<string, string>()
  {
    {"FLUO1", "Green - GFP"},
    {"FLUO2", "Red - mCherry"},
    {"MOV", "Flagella master regulator"},
    {"ANTIBIO1", "Ampicillin resistance cassette"},
    {"REPR1", "lacI (IPTG)"},
    {"REPR2", "tetR (aTc)"}
  };

  private static Dictionary<string, string> deviceNames = new Dictionary<string, string>()
  {
    {"FLUO1", "Green fluorescence"},
    {"FLUO2", "Red fluorescence"},
    {"MOV", "Hyperflagellation"},
    {"ANTIBIO", "Ampicillin resistance"}
  };

  public static string getStringFromDico(string code, Dictionary<string, string> dico)
  {
    try {
      return dico[code];
    }
    catch (NullReferenceException exc) {
      Logger.Log("GameplayNames::getStringFromDico unknown code \""+code+"\"\n"+exc, Logger.Level.WARN);
      return code;
    }
    catch (Exception exc) {
      Logger.Log("GameplayNames::getStringFromDico failed, got exc="+exc, Logger.Level.WARN);
      return code;
    }
  }

  public static string getBrickRealName(string code)
  {
    return getStringFromDico(code, brickNames);
  }

  public static string getProteinRealName(string code)
  {
    return getStringFromDico(code, proteinNames);
  }

  public static string getDeviceRealName(string code)
  {
    return getStringFromDico(code, deviceNames);
  }

  public static string generateRealNameFromBricks(Device device)
  {
    string prefix = "";
    string suffix = "";
    string deviceName = "";

    LinkedList<BioBrick> bricks = device.getExpressionModules().First.Value.getBioBricks();

    foreach (BioBrick brick in bricks)
    {
      switch(brick.getType())
      {
        case BioBrick.Type.PROMOTER:
          if(brick.getName() != "PRCONS")
          {
            prefix = "Regulated ";
          }
          break;
        case BioBrick.Type.RBS:
          if(brick.getName() == "")
          {
            suffix = " (med)";
          }
          else if(brick.getName() == "")
          {
            suffix = " (low)";
          }
          break;
        case BioBrick.Type.GENE:
          deviceName = getDeviceRealName(brick.getName());
          break;
      }
    }
    if(!string.IsNullOrEmpty(prefix))
    //if(prefix == "")
    {
      deviceName = Char.ToLowerInvariant(deviceName[0]) + deviceName.Substring(1);
    }
    return prefix+deviceName+suffix;
  }
}


