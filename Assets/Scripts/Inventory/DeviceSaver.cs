using UnityEngine;
using System.Collections;
using System.Xml;
using System;
using System.Collections.Generic;
using System.IO;

public class DeviceSaver {

  public DeviceSaver() {
    // Debug.Log(this.GetType() + " DeviceSaver()");
  }

  //optionally saves details about biobricks
  public void saveDevicesToFile(List<Device> devices, string filePath, bool exhaustive = true)
  {
    // Debug.Log(this.GetType() + " saveDevicesToFile("+Logger.ToString<Device>(devices)+", "+filePath+")");

    XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

    using(XmlWriter writer = XmlWriter.Create(filePath, settings))
    {
      writer.WriteStartDocument();
      writer.WriteStartElement(BioBricksXMLTags.DEVICES);

      foreach (Device device in devices)
      {
        writer.WriteStartElement(BioBricksXMLTags.DEVICE);
        writer.WriteAttributeString(BioBricksXMLTags.ID, device.getInternalName());
        foreach (BioBrick brick in device.getExpressionModules().First.Value.getBioBricks())
        {
          writer.WriteStartElement(BioBricksXMLTags.BIOBRICK);
          writer.WriteAttributeString(BioBricksXMLTags.ID, brick.getName());

          if (exhaustive) { //saves details about biobricks
            writer.WriteAttributeString(BioBricksXMLTags.SIZE, brick.getLength().ToString());
            BioBrick.Type bioBrickType = brick.getType();
            writer.WriteAttributeString(BioBricksXMLTags.TYPE, bioBrickType.ToString().ToLower());
            switch(bioBrickType) {
              case BioBrick.Type.PROMOTER:
                writer.WriteElementString(BioBricksXMLTags.BETA, ((PromoterBrick)brick).getBeta().ToString());
                writer.WriteElementString(BioBricksXMLTags.FORMULA, ((PromoterBrick)brick).getFormula());
                break;
              case BioBrick.Type.RBS:
                writer.WriteElementString(BioBricksXMLTags.RBSFACTOR, ((RBSBrick)brick).getRBSFactor().ToString());
                break;
              case BioBrick.Type.GENE:
                writer.WriteElementString(BioBricksXMLTags.PROTEIN, ((GeneBrick)brick).getProteinName());
                break;
              case BioBrick.Type.TERMINATOR:
                writer.WriteElementString(BioBricksXMLTags.TERMINATORFACTOR, ((TerminatorBrick)brick).getTerminatorFactor().ToString());
                break;
              default:
                Debug.LogWarning(this.GetType() + " saveDevicesToFile unknown biobrick type "+ bioBrickType);
                break;
            }
          }

          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
      writer.WriteEndDocument();
    }

    // Debug.Log(this.GetType() + " DeviceSaver.saveDevicesToFile done");
  }
}
