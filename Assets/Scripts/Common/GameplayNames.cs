using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplayNames
{
    public const string biobrickPrefix = "BRICK.";
    public const string moleculePrefix = "MOL.";
    public const string devicePrefix = "DEVICE.";
    private const string shortMoleculePrefix = "BRICK.ICONLABEL.";

    public static string getMoleculeRealName(string code)
    {
        return Localization.Localize(moleculePrefix + code);
    }

    public static string getDeviceRealName(string code)
    {
        return Localization.Localize(devicePrefix + code);
    }

    public static string getShortName(string code)
    {
        return Localization.Localize(shortMoleculePrefix + code);
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
        // Debug.Log("GameplayNames generateRealNameFromBricks(" + Logger.ToString<BioBrick>(bricks) + ")");
        string prefix = "";
        string suffix = "";
        string deviceName = "";

        foreach (BioBrick brick in bricks)
        {
            switch (brick.getType())
            {
                case BioBrick.Type.PROMOTER:
                    if (brick.getName() != "PRCONS")
                    {
                        prefix = Localization.Localize("DEVICE.PREREG") + " ";
                    }
                    break;
                case BioBrick.Type.RBS:
                    if (brick.getName() == "RBS1")
                    {
                        suffix = " " + Localization.Localize("DEVICE.SUFMED");
                    }
                    else if (brick.getName() == "RBS2")
                    {
                        suffix = " " + Localization.Localize("DEVICE.SUFLOW");
                    }
                    else if (brick.getName() == "RBS3")
                    {
                        suffix = " " + Localization.Localize("DEVICE.SUFBASE");
                    }
                    break;
                case BioBrick.Type.GENE:
                    deviceName = getShortName(brick.getName());
                    break;
            }
        }
        // if (!string.IsNullOrEmpty(prefix))
        // {
        //     deviceName = Char.ToLowerInvariant(deviceName[0]) + deviceName.Substring(1);
        // }
        // Debug.Log("GameplayNames generateRealNameFromBricks result=" + prefix + deviceName + suffix);
        return prefix + deviceName + suffix;
    }
}


