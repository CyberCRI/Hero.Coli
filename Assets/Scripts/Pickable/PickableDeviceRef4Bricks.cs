using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableDeviceRef4Bricks : PickableDevice {

  public string promoterName;
  public string rbsName;
  public string geneName;
  public string terminatorName = "DTER";

  public string expressionModuleName;

  public string deviceName;

  protected override DNABit produceDNABit()
  {
    LinkedList<BioBrick> deviceBricks = new LinkedList<BioBrick>();
    string[] bricks = new string[]{promoterName, rbsName, geneName, terminatorName};

    foreach(string brickName in bricks)
    {
      BioBrick brick = AvailableBioBricksManager.get().getBioBrickFromAll(brickName);
      if(brick != null) {
        deviceBricks.AddLast(brick);
      } else {
        Debug.LogWarning(this.GetType() + " produceDNABit failed to add brick with name "+brickName+"!");
      }
    }

        if(ExpressionModule.isBioBricksSequenceValid(deviceBricks))
        {
            ExpressionModule deviceModule = new ExpressionModule(deviceName, deviceBricks);
            LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
            deviceModules.AddLast(deviceModule);
            Device result = Device.buildDevice(deviceName, deviceModules);
            return result;
        } else {
            Debug.LogWarning(this.GetType() + " produceDNABit failed to produce DNABit - BioBrick sequence is incorrect: list="+Logger.ToString<BioBrick>(deviceBricks));
            return null;
        }
  }
}
