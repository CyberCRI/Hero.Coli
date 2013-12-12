using UnityEngine;
using System.Collections.Generic;

public class Equipment : DeviceContainer
{
  //////////////////////////////// singleton fields & methods ////////////////////////////////
  public static string gameObjectName = "DeviceEquipment";
  private static Equipment _instance;
  public static Equipment get() {
    if(_instance == null) {
      Logger.Log("Equipment::get was badly initialized", Logger.Level.WARN);
      _instance = GameObject.Find(gameObjectName).GetComponent<Equipment>();
    }
    return _instance;
  }
  void Awake()
  {
    Logger.Log("Equipment::Awake", Logger.Level.DEBUG);
    _instance = this;
  }
  ////////////////////////////////////////////////////////////////////////////////////////////

  private ReactionEngine _reactionEngine;
  public int _celliaMediumID = 1;

	public Equipment() {
		//by default, nothing's equiped
		_devices = new List<Device>();
	}

  private void addToReactionEngine(Device device) {
    Logger.Log("Equipment::addToReactionEngine reactions from device "+device.getName()+" ("+device.ToString ()+")", Logger.Level.TRACE);

    LinkedList<IReaction> reactions = device.getReactions();
    Logger.Log("Equipment::addToReactionEngine reactions="+Logger.ToString<IReaction>(reactions)+" from "+device, Logger.Level.INFO);

    foreach (IReaction reaction in reactions) {
      Logger.Log("Equipment::addToReactionEngine adding reaction="+reaction, Logger.Level.TRACE);
      _reactionEngine.addReactionToMedium(_celliaMediumID, reaction);
    }
  }

  public override AddingResult askAddDevice(Device device) {
    if(device == null)
    {
      Logger.Log("Equipment::askAddDevice device == null", Logger.Level.WARN);
      return AddingResult.FAILURE_DEFAULT;
    }
    Device copy = Device.buildDevice(device);
    if(copy == null)
    {
      Logger.Log("Equipment::askAddDevice device == null", Logger.Level.WARN);
      return AddingResult.FAILURE_DEFAULT;
    }
    if(_devices.Exists(d => d.getName() == copy.getName()))
    {
      Logger.Log("Equipment::askAddDevice device already present", Logger.Level.INFO);
      return AddingResult.FAILURE_SAME_DEVICE;
    }
    _devices.Add(copy);
    safeGetDisplayer().addEquipedDevice(copy);
    addToReactionEngine(copy);
    return AddingResult.SUCCESS;
  }

  //TODO
  private void removeFromReactionEngine(Device device) {
    Logger.Log("Equipment::removeFromReactionEngine reactions from device "+device, Logger.Level.INFO);

    LinkedList<IReaction> reactions = device.getReactions();
    //Logger.Log("Equipment::removeFromReactionEngine device implies reactions="+Logger.ToString<IReaction>(reactions), Logger.Level.TRACE);

    //LinkedList<Medium> mediums = _reactionEngine.getMediumList();
    //Medium celliaMedium = ReactionEngine.getMediumFromId(_celliaMediumID, mediums);

    //LinkedList<IReaction> celliaReactions = celliaMedium.getReactions();
    //Logger.Log("Equipment::removeFromReactionEngine initialCelliaReactions="+Logger.ToString<IReaction>(celliaReactions)
    //  , Logger.Level.TRACE);

    foreach (IReaction reaction in reactions) {
      //Logger.Log("Equipment::removeFromReactionEngine removing reaction="+reaction, Logger.Level.TRACE);
      _reactionEngine.removeReaction(_celliaMediumID, reaction, false);
    }

    //celliaReactions = celliaMedium.getReactions();
    //Logger.Log("Equipment::removeFromReactionEngine finalCelliaReactions="+Logger.ToString<IReaction>(celliaReactions)
    //  , Logger.Level.TRACE);
  }

  public override void removeDevice(Device device) {
    _devices.RemoveAll(d => d.getName() == device.getName());
    safeGetDisplayer().removeEquipedDevice(device);
    removeFromReactionEngine(device);
  }

  public override void editDevice(Device device) {
    //TODO
    Debug.Log("Equipment::editeDevice NOT IMPLEMENTED");
  }

  new void Start()
  {
    base.Start();
    Logger.Log("Equipment::Start()", Logger.Level.DEBUG);
    _reactionEngine = ReactionEngine.get();
  }
}

