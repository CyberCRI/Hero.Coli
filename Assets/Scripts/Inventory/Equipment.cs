using UnityEngine;
using System.Collections.Generic;

public class Equipment : DeviceContainer
{
  public ReactionEngine _reactionEngine;
  public int _celliaMediumID = 1;

	public Equipment() {
		//by default, nothing's equiped
		_devices = new List<Device>();
	}

  private void addToReactionEngine(Device device) {
    Logger.Log("Equipment::addToReactionEngine reactions from device "+device.getName()+" ("+device.ToString ()+")", Logger.Level.INFO);

    LinkedList<IReaction> reactions = device.getReactions();
    Logger.Log("Equipment::addToReactionEngine reactions="+reactions, Logger.Level.TRACE);

    foreach (IReaction reaction in reactions) {
      Logger.Log("Equipment::addToReactionEngine adding reaction="+reaction.ToString(), Logger.Level.TRACE);
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
    Logger.Log("Equipment::removeFromReactionEngine reactions="+reactions, Logger.Level.TRACE);

    foreach (IReaction reaction in reactions) {
      Logger.Log("Equipment::removeFromReactionEngine removing reactions="+reaction, Logger.Level.TRACE);
      _reactionEngine.removeReactionFromMediumByName(_celliaMediumID, reaction.getName());
    }
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
}

