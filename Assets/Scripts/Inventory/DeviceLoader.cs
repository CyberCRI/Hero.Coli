using System.Xml;
using System;
using System.Collections.Generic;

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
        Logger.Log("DeviceLoader::DeviceLoader(" + Logger.ToString<BioBrick>(availableBioBricks) + ")", Logger.Level.TRACE);
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
        Logger.Log("DeviceLoader::loadDevicesFromFile brick name " + brickName, Logger.Level.TRACE);
        //"warn" parameter is true to indicate that there is no such BioBrick
        //as the one mentioned in the xml file of the device
        brick = LinkedListExtensions.Find<BioBrick>(_availableBioBricks
                                                    , b => (b.getName() == brickName)
                                                    , true
                                                    , " DeviceLoader::loadDevicesFromFile(" + filePath + ")"
                                                        );

        if (brick == null)
        {
            brick = LinkedListExtensions.Find<BioBrick>(_allBioBricks
                                                        , b => (b.getName() == brickName)
                                                        , true
                                                        , " DeviceLoader::loadDevicesFromFile(" + filePath + ")"
                                                            );
            if (brick != null)
            {
                Logger.Log("DeviceLoader::loadDevicesFromFile successfully added brick " + brick, Logger.Level.TRACE);
                AvailableBioBricksManager.get().addAvailableBioBrick(brick);
            }
        }
        if (brick != null)
        {
            Logger.Log("DeviceLoader::loadDevicesFromFile successfully added brick " + brick, Logger.Level.TRACE);
            deviceBricks.AddLast(brick);
            return true;
        }
        else
        {
            Logger.Log("DeviceLoader::loadDevicesFromFile failed to add brick with name " + brickName + "!", Logger.Level.WARN);
            return false;
        }
    }

    public LinkedList<Device> loadDevicesFromFile(string filePath)
    {
        Logger.Log("DeviceLoader::loadDevicesFromFile(" + filePath + ")", Logger.Level.INFO);

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
                Logger.Log("DeviceLoader::loadDevicesFromFile bad xml, missing field \"id\"\n" + exc, Logger.Level.WARN);
                continue;
            }
            catch (Exception exc)
            {
                Logger.Log("DeviceLoader::loadDevicesFromFile failed, got exc=" + exc, Logger.Level.WARN);
                continue;
            }

            Logger.Log("DeviceLoader::loadDevicesFromFile got id=" + deviceName
              , Logger.Level.TRACE);


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
                            Logger.Log("DeviceLoader::loadDevicesFromFile unknown attr " + attr.Name, Logger.Level.WARN);
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
                    ExpressionModule deviceModule = new ExpressionModule(deviceName, deviceBricks);
                    LinkedList<ExpressionModule> deviceModules = new LinkedList<ExpressionModule>();
                    deviceModules.AddLast(deviceModule);
                    device = Device.buildDevice(deviceName, deviceModules);
                    if (device != null)
                    {
                        resultDevices.AddLast(device);
                    }
                }
            }
            else
            {
                Logger.Log("DeviceLoader::loadDevicesFromFile Error : missing attribute id in Device node", Logger.Level.WARN);
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
        Logger.Log("DeviceLoader::logCurrentBioBrick type=" + type, Logger.Level.TRACE);
    }

    private bool checkString(string toCheck)
    {
        return !String.IsNullOrEmpty(toCheck);
    }

}
