using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableDeviceRef4Bricks : PickableDevice {

  private LinkedList<BioBrick> _allBioBricks = new LinkedList<BioBrick>();

  public string promoterName;
  public string rbsName;
  public string geneName;
  public string terminatorName = "DTER";

  public string expressionModuleName;

  public string deviceName;

  protected override Device produceDevice()
  {
    LinkedList<BioBrick> deviceBricks = new LinkedList<BioBrick>();
    string[] bricks = new string[]{promoterName, rbsName, geneName, terminatorName};

    foreach(string brickName in bricks)
    {
      BioBrick brick = AvailableBioBricksManager.get().getBioBrickFromAll(brickName);
      if(brick != null) {
        Logger.Log("PickableDeviceRef4Bricks::produceDevice successfully added brick "+brick, Logger.Level.ERROR);
        deviceBricks.AddLast(brick);
      } else {
        Logger.Log("PickableDeviceRef4Bricks::produceDevice failed to add brick with name "+brickName+"!", Logger.Level.ERROR);
      }
    }
    Debug.LogError("will create ExpressionModule 'deviceModule'");
    ExpressionModule deviceModule = new ExpressionModule(deviceName, deviceBricks);
    LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
    deviceModules.AddLast(deviceModule);
        Debug.LogError("will create Device 'result'");
    Device result = Device.buildDevice(deviceName, deviceModules);
    Debug.LogError("PickableDeviceRef4Bricks::produceDevice result device="+result);
    return result;
  }
}
