using UnityEngine;
using System.Collections;

public class WorldEquipButton : MonoBehaviour {
	
	private Inventory _inventory;
	public UpdateInventoryAnimation updateAnim;

  private void OnPress(bool isPressed) {
    if(isPressed && _inventory.GetDeviceCount() != 0) {
      Logger.Log("WorldEquipButton::OnPress()", Logger.Level.INFO);
      GUITransitioner.get().SwitchScreen(GUITransitioner.GameScreen.screen1, GUITransitioner.GameScreen.screen2);
    }

		if(updateAnim.isPlaying == true)
		{
			updateAnim.reset();
		}
  }

  void Start()
  {
    _inventory = Inventory.get();
  }



}