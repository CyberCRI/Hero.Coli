using UnityEngine;
using System.Collections.Generic;

public class Equipment : DeviceContainer
{
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
      Device copy = Device.buildDevice(device);
      _devices.Add(copy);
      _displayer.addEquipedDevice(copy);
      addToReactionEngine(copy);
      return AddingResult.SUCCESS;
  }

  //TODO
  private void removeFromReactionEngine(Device device) {
    Logger.Log("Equipment::removeFromReactionEngine reactions from device "+device, Logger.Level.INFO);

    LinkedList<IReaction> reactions = device.getReactions();
    //Logger.Log("Equipment::removeFromReactionEngine device implies reactions="+Logger.ToString<IReaction>(reactions), Logger.Level.TRACE);

    LinkedList<Medium> mediums = _reactionEngine.getMediumList();
    Medium celliaMedium = ReactionEngine.getMediumFromId(_celliaMediumID, mediums);

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
    _devices.Remove(device);
    _displayer.removeEquipedDevice(device);
    removeFromReactionEngine(device);
  }

  public override void editDevice(Device device) {
    //TODO
    Debug.Log("Equipment::editeDevice NOT IMPLEMENTED");
  }

  void Start()
  {
    _reactionEngine = ReactionEngine.get();
  }
}

