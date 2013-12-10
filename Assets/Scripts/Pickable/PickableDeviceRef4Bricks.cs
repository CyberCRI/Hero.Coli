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
    _allBioBricks = AvailableBioBricksManager.get ().getAllBioBricks();
    LinkedList<BioBrick> deviceBricks = new LinkedList<BioBrick>();
    string[] bricks = new string[]{promoterName, rbsName, geneName, terminatorName};

    foreach(string brickName in bricks)
    {
      BioBrick brick = LinkedListExtensions.Find<BioBrick>(_allBioBricks, b => (b.getName() == brickName));
      if(brick != null) {
        Logger.Log("DeviceLoader::loadDevicesFromFile successfully added brick "+brick, Logger.Level.TRACE);
        deviceBricks.AddLast(brick);
      } else {
        Logger.Log("DeviceLoader::loadDevicesFromFile failed to add brick!", Logger.Level.WARN);
      }
    }
    ExpressionModule deviceModule = new ExpressionModule(deviceName, deviceBricks);
    LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
    deviceModules.AddLast(deviceModule);
    return Device.buildDevice(deviceName, deviceModules);
  }
}
