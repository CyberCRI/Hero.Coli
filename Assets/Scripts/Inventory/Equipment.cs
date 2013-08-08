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
    Debug.Log("Equipment::addToReactionEngine reactions from device "+device.getName()+" ("+device.ToString ()+")");

    LinkedList<IReaction> reactions = device.getReactions();
    Debug.Log("Equipment::addToReactionEngine reactions="+reactions);

    foreach (IReaction reaction in reactions) {
      Debug.Log("Equipment::addToReactionEngine adding reaction="+reaction.ToString());
      _reactionEngine.addReactionToMedium(_celliaMediumID, reaction);
    }
  }

  public override void addDevice(Device device) {
      Device copy = Device.buildDevice(device);
      _devices.Add(copy);
      _displayer.addEquipedDevice(copy);
      addToReactionEngine(copy);
  }

  //TODO
  private void removeFromReactionEngine(Device device) {
    Debug.Log("Equipment::removeFromReactionEngine reactions from device "+device);

    LinkedList<IReaction> reactions = device.getReactions();
    Debug.Log("Equipment::removeFromReactionEngine reactions="+reactions);

    foreach (IReaction reaction in reactions) {
      Debug.Log("Equipment::removeFromReactionEngine removing reactions="+reaction);
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

