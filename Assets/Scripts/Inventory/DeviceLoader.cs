using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

//TODO refactor with FileLoader
public class DeviceLoader {
  private LinkedList<BioBrick> _availableBioBricks;

  private string deviceName;
  private Device device;
  private BioBrick brick;
  private LinkedList<BioBrick> deviceBricks = new LinkedList<BioBrick>();

  private void reinitVars() {
    deviceName = null;
    device = null;
    brick = null;
    deviceBricks.Clear();
  }

  public DeviceLoader(LinkedList<BioBrick> availableBioBricks) {
    Logger.Log("DeviceLoader::DeviceLoader("+Logger.ToString<BioBrick>(availableBioBricks)+")", Logger.Level.TRACE);
    _availableBioBricks = new LinkedList<BioBrick>();
    _availableBioBricks.AppendRange(availableBioBricks);
  }


  public LinkedList<Device> loadDevicesFromFile(string filePath)
  {
    Logger.Log("DeviceLoader::loadBioBricksFromFile("+filePath+")", Logger.Level.INFO);

    LinkedList<Device> resultDevices = new LinkedList<Device>();

    XmlDocument xmlDoc = Tools.getXmlDocument(filePath);
    
    XmlNodeList deviceList = xmlDoc.GetElementsByTagName(BioBricksXMLTags.DEVICE);

    reinitVars();

    foreach (XmlNode deviceNode in deviceList)
      {
        try {
          deviceName = deviceNode.Attributes[BioBricksXMLTags.ID].Value;
        }
        catch (NullReferenceException exc) {
          Logger.Log("DeviceLoader::loadDevicesFromFile bad xml, missing field \"id\"\n"+exc, Logger.Level.WARN);
          continue;
        }
        catch (Exception exc) {
          Logger.Log("DeviceLoader::loadDevicesFromFile failed, got exc="+exc, Logger.Level.WARN);
          continue;
        }

        Logger.Log ("DeviceLoader::loadDevicesFromFile got id="+deviceName
          , Logger.Level.TRACE);

        if (checkString(deviceName)) {
          foreach (XmlNode attr in deviceNode) {
            if (attr.Name == BioBricksXMLTags.BIOBRICK) {
              //find brick in existing bricks
              string brickName = attr.Attributes[BioBricksXMLTags.ID].Value;
              Logger.Log("DeviceLoader::loadDevicesFromFile brick name "+brickName, Logger.Level.TRACE);
              brick = LinkedListExtensions.Find<BioBrick>(_availableBioBricks, b => (b.getName() == brickName));
              if(brick != null) {
                Logger.Log("DeviceLoader::loadDevicesFromFile successfully added brick "+brick, Logger.Level.TRACE);
                deviceBricks.AddLast(brick);
              } else {
                Logger.Log("DeviceLoader::loadDevicesFromFile failed to add brick with name "+brickName+"!", Logger.Level.WARN);
              }
            } else {
              Logger.Log("DeviceLoader::loadDevicesFromFile unknown attr "+attr.Name, Logger.Level.WARN);
            }
          }
          ExpressionModule deviceModule = new ExpressionModule(deviceName, deviceBricks);
          LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
          deviceModules.AddLast(deviceModule);
          device = Device.buildDevice(deviceName, deviceModules);
          if(device != null){
            resultDevices.AddLast(device);
          }
        } else {
          Logger.Log("DeviceLoader::loadDevicesFromFile Error : missing attribute id in Device node", Logger.Level.WARN);
        }
        reinitVars();
      }
    return resultDevices;
  }

  private static float parseFloat(string real) {
    return float.Parse(real.Replace (",", "."));
  }

  private static int parseInt(string integer) {
    return int.Parse(integer);
  }

  private static void logUnknownAttr(XmlNode attr, string type) {

  }

  private static void logCurrentBioBrick(string type){
    Logger.Log("DeviceLoader::loadBioBricksFromFile type="+type, Logger.Level.TRACE);
  }

  private bool checkString(string toCheck) {
    return !String.IsNullOrEmpty(toCheck);
  }

}
