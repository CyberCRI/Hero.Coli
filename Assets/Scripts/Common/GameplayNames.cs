using System;
using System.Collections;
using System.Collections.Generic;

public class GameplayNames
{
  private static Dictionary<string, string> brickNames = new Dictionary<string, string>()
  {
    //PROMOTERS
    {"PRCONS3", "Constitutive promoter 3"},
    {"PRLACI1", "pL lacI repressible"},
    {"PRTETR1", "p(tetR) multi"},
    //GENES
    {"FLUO1", "Green - GFP"},
    {"FLUO2", "Red - mCherry"},
    {"MOV4", "Flagella master regulator"},
    {"ANTIBIO1", "Ampicillin resistance cassette"},
    {"REPR1", "lacI (IPTG)"},
    {"REPR2", "tetR (aTc)"},
    //TERMINATORS
    {"DTER", "Double terminator"}
  };

  private static Dictionary<string, string> proteinNames = new Dictionary<string, string>()
  {
    {"FLUO1", "Green - GFP"},
    {"FLUO2", "Red - mCherry"},
    {"MOV4", "Flagella master regulator"},
    {"ANTIBIO1", "Ampicillin resistance cassette"},
    {"REPR1", "lacI (IPTG)"},
    {"REPR2", "tetR (aTc)"}
  };

  private static Dictionary<string, string> deviceNames = new Dictionary<string, string>()
  {
    {"Motility", "Flagella master regulator"},
    {"Fluorescence", "Flagella master regulator"},
    {"Resistance", "Flagella master regulator"},
    {"Repression", "Flagella master regulator"}
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
}


