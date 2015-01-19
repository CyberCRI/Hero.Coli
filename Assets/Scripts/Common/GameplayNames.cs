using System;
using System.Collections;
using System.Collections.Generic;

public class GameplayNames
{
  public static string getBrickRealName(string code)
  {
    return Localization.Localize("BRICK."+code);
  }

  public static string getMoleculeRealName(string code)
  {
    return Localization.Localize("MOL."+code);
  }

  public static string getDeviceRealName(string code)
  {
    return Localization.Localize("DEVICE."+code);
  }

    //TODO check use
  public static string generateRealNameFromBricks(Device device)
  {
    LinkedList<BioBrick> bricks = device.getExpressionModules().First.Value.getBioBricks();
    return generateRealNameFromBricks(bricks);
  }

    //TODO check use
  public static string generateRealNameFromBricks(LinkedList<BioBrick> bricks)
  {
    string prefix = "";
    string suffix = "";
    string deviceName = "";

    foreach (BioBrick brick in bricks)
    {
      switch(brick.getType())
      {
        case BioBrick.Type.PROMOTER:
          if(brick.getName() != "PRCONS")
          {
            prefix = Localization.Localize("DEVICE.PREREG")+" ";
          }
          break;
        case BioBrick.Type.RBS:
          if(brick.getName() == "RBS1")
          {
            suffix = " "+Localization.Localize("DEVICE.SUFMED");
          }
          else if(brick.getName() == "RBS2")
          {
            suffix = " "+Localization.Localize("DEVICE.SUFLOW");
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


