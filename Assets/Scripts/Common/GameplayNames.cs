using System;
using System.Collections;
using System.Collections.Generic;

public class GameplayNames
{
  private static Dictionary<string, string> brickNames = new Dictionary<string, string>()
  {
    //PROMOTERS
    {"PRCONS", "Constitutive promoter"},
    {"PRLACI", "pLac"},//"placI repressible"},
    {"PRTETR", "pTet"},//"p(tetR) multi"},
    //RBS
    {"RBS1", "Medium RBS"},
    {"RBS2", "Low RBS"},
    //GENES
    {"FLUO1", "Green - GFP"},
    {"FLUO2", "Red - mCherry"},
    {"MOV", "Flagella master regulator"},// FlhDC master operon
    {"ANTIBIO1", "Ampicillin resistance cassette"},
    {"REPR1", "lacI gene"},
    {"REPR2", "tetR gene"},
    //TERMINATORS
    {"DTER", "Double terminator"}
  };

  private static Dictionary<string, string> proteinNames = new Dictionary<string, string>()
  {
    //PROMOTER ACTIVATORS (ANTI-REPRESSORS)
    {"IPTG", "GFP"},             //inhibits lacI => activates PRLACI
    {"aTc", "aTc tetracycline"}, //inhibits tetR => activates PRTETR

    //PRODUCED BY GENES
    {"FLUO1", "GFP"},
    {"FLUO2", "Red - mCherry"},
    {"MOV", "FlhDC"},
    {"ANTIBIO", "ß-lactamase"},
    {"REPR1", "lacI"},
    {"REPR2", "tetR"}
  };

  private static Dictionary<string, string> deviceNames = new Dictionary<string, string>()
  {
    {"FLUO1", "Green fluorescence"},
    {"FLUO2", "Red fluorescence"},
    {"MOV", "Hyperflagellation"},
    {"ANTIBIO", "Ampicillin resistance"},

    //MISSING
    {"REPR1", "LacI inhibition"},
    {"REPR2", "TetR inhibition"}
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
    {
      deviceName = Char.ToLowerInvariant(deviceName[0]) + deviceName.Substring(1);
    }
    return prefix+deviceName+suffix;
  }
}


