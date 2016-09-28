using UnityEngine;

public class PickablePlasmid : MonoBehaviour {
    private static bool _alreadyPicked = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == Hero.playerTag)
        {
            if (!_alreadyPicked)
            {
                ModalManager.setModal("T1_PLASMID");
            }
            _alreadyPicked = true;
            ((LimitedBiobricksCraftZoneManager)LimitedBiobricksCraftZoneManager.get()).addSlot();
            Destroy(this.gameObject);
        }
    }

    public static void clear()
    {
        _alreadyPicked = false;
    }
}