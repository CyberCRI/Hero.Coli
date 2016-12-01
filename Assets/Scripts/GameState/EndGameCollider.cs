using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
    private bool _alreadyDisplayed = false;

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log(this.GetType() + " OnTriggerEnter(" + other.ToString () + ")" + _alreadyDisplayed.ToString ());
        if (!_alreadyDisplayed)
        {
            if (other.tag == Hero.playerTag)
            {
                _alreadyDisplayed = true;
                GameStateController.get().triggerEnd();
            }
        }
    }
}