using System.Xml;
using System;
using System.Collections.Generic;
using UnityEngine;

//TODO refactor with FileLoader
public class DeviceLoader
{
    private LinkedList<BioBrick> _availableBioBricks;
    private LinkedList<BioBrick> _allBioBricks;

    private string deviceName;
    private Device device;
    private BioBrick brick;
    private LinkedList<BioBrick> deviceBricks = new LinkedList<BioBrick>();
    private BioBrickLoader bLoader = new BioBrickLoader();

    private void reinitVars()
    {
        deviceName = null;
        device = null;
        brick = null;
        deviceBricks.Clear();
    }

    public DeviceLoader(LinkedList<BioBrick> availableBioBricks, LinkedList<BioBrick> allBioBricks = null)
    {
        // Debug.Log(this.GetType() + " DeviceLoader(" + Logger.ToString<BioBrick>(availableBioBricks) + ")");
        _availableBioBricks = new LinkedList<BioBrick>();
        _availableBioBricks.AppendRange(availableBioBricks);

        _allBioBricks = new LinkedList<BioBrick>();
        _allBioBricks.AppendRange(allBioBricks);
    }

    // Searches for a brick from its name in available BioBricks
    //  If it succeeds, adds brick to deviceBricks and returns true
    //  If it fails, searches for it in all the known BioBricks
    //      If it succeeds, adds brick to available BioBricks list and does previous success treatment
    //      If it fails, returns false
    private bool processBrick(string brickName, string filePath = "")
    {
        // Debug.Log(this.GetType() + " loadDevicesFromFile brick name " + brickName);
        //"warn" parameter is true to indicate that there is no such BioBrick
        //as the one mentioned in the xml file of the device
        brick = LinkedListExtensions.Find<BioBrick>(_availableBioBricks
                                                    , b => (b.getName() == brickName)
                                                    , true
                                                    , this.GetType() + " loadDevicesFromFile(" + filePath + ")"
                                                        );

        if (brick == null)
        {
            brick = LinkedListExtensions.Find<BioBrick>(_allBioBricks
                                                        , b => (b.getName() == brickName)
                                                        , true
                                                        , "this.GetType() +   loadDevicesFromFile(" + filePath + ")"
                                                            );
            if (brick != null)
            {
                // Debug.Log(this.GetType() + " loadDevicesFromFile successfully added brick " + brick);
                AvailableBioBricksManager.get().addAvailableBioBrick(brick);
            }
        }
        if (brick != null)
        {
            // Debug.Log(this.GetType() + " loadDevicesFromFile successfully added brick " + brick);
            deviceBricks.AddLast(brick);
            return true;
        }
        else
        {
            Debug.LogWarning(this.GetType() + " loadDevicesFromFile failed to add brick with name " + brickName + "!");
            return false;
        }
    }

    public LinkedList<Device> loadDevicesFromFile(string filePath)
    {
        // Debug.Log(this.GetType() + " loadDevicesFromFile(" + filePath + ")");

        LinkedList<Device> resultDevices = new LinkedList<Device>();

        XmlDocument xmlDoc = Tools.getXmlDocument(filePath);

        XmlNodeList deviceList = xmlDoc.GetElementsByTagName(BioBricksXMLTags.DEVICE);

        reinitVars();

        foreach (XmlNode deviceNode in deviceList)
        {
            try
            {
                deviceName = deviceNode.Attributes[BioBricksXMLTags.ID].Value;
            }
            catch (NullReferenceException exc)
            {
                Debug.LogWarning(this.GetType() + " loadDevicesFromFile bad xml, missing field \"id\"\n" + exc);
                continue;
            }
            catch (Exception exc)
            {
                Debug.LogWarning(this.GetType() + " loadDevicesFromFile failed, got exc=" + exc);
                continue;
            }

            // Debug.Log(this.GetType() + " loadDevicesFromFile got id=" + deviceName);


            bool processSuccess = true;

            if (checkString(deviceName))
            {
                // processes longer grammars that use explicit brick nodes names
                if (0 != deviceNode.ChildNodes.Count)
                {
                    foreach (XmlNode attr in deviceNode)
                    {
                        if (attr.Name == BioBricksXMLTags.BIOBRICK)
                        {
                            //find brick in existing bricks
                            string brickName = attr.Attributes[BioBricksXMLTags.ID].Value;
                            if (!processBrick(brickName, filePath))
                            {
                                // processes even longer grammar that uses explicit brick description
                                // because the brick wasn't found neither in 'available' nor 'all' biobricks lists
                                brick = bLoader.loadBioBrick(attr);
                                if (null != brick)
                                {
                                    // Debug.Log(this.GetType() + " addAvailableBioBrick " + brick.getInternalName());
                                    AvailableBioBricksManager.get().addAvailableBioBrick(brick);
                                    deviceBricks.AddLast(brick);
                                }
                                else
                                {
                                    processSuccess = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning(this.GetType() + " loadDevicesFromFile unknown attr " + attr.Name);
                        }
                    }
                }
                else
                {
                    // processes shortened grammar that uses only device name
                    foreach (string brickName in deviceName.Split(':'))
                    {
                        if (!processBrick(brickName, filePath))
                        {
                            processSuccess = false;
                            break;
                        }
                    }
                }

                if (processSuccess)
                {
                    // Debug.Log(this.GetType() + " process succeeded");
                    ExpressionModule deviceModule = new ExpressionModule(deviceName, deviceBricks);
                    LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
                    deviceModules.AddLast(deviceModule);
                    device = Device.buildDevice(deviceName, deviceModules);
                    if (device != null)
                    {
                        // Debug.Log(this.GetType() + " added " + device.getInternalName());
                        resultDevices.AddLast(device);
                    }
                    else
                    {
                        Debug.LogWarning(this.GetType() + " device is null");
                    }
                }
                else
                {
                    Debug.LogWarning(this.GetType() + " process failed");
                }
            }
            else
            {
                Debug.LogWarning(this.GetType() + " loadDevicesFromFile Error : missing attribute id in Device node");
            }
            reinitVars();
        }
        return resultDevices;
    }

    private static float parseFloat(string real)
    {
        return float.Parse(real.Replace(",", "."));
    }

    private static int parseInt(string integer)
    {
        return int.Parse(integer);
    }

    private static void logUnknownAttr(XmlNode attr, string type)
    {

    }

    private static void logCurrentBioBrick(string type)
    {
        // Debug.Log(this.GetType() + " logCurrentBioBrick type=" + type);
    }

    private bool checkString(string toCheck)
    {
        return !String.IsNullOrEmpty(toCheck);
    }

}
