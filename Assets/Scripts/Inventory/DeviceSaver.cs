using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

public class DeviceSaver {

  public DeviceSaver() {
    Logger.Log("DeviceSaver::DeviceSaver()", Logger.Level.WARN);
  }


  public void saveDevicesToFile(LinkedList<Device> devices, string filePath)
  {
    Logger.Log("DeviceSaver::saveDevicesToFile("+Logger.ToString<Device>(devices)+", "+filePath+")", Logger.Level.WARN);

    MemoryStream ms = Tools.getEncodedFileContent("Assets/Data/raph/devices.xml");

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(ms);

    XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
    XmlWriter writer = XmlWriter.Create(filePath, settings);
    xmlDoc.Save(writer);

    Logger.Log("DeviceSaver.saveDevicesToFile done", Logger.Level.WARN);
  }
}
