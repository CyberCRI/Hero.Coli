using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
  
    public GameObject endInfoPanel, hero;
    public EndMainMenuButton endMainMenuButton;
    private bool _alreadyDisplayed;
    // Use this for initialization
    void Start ()
    {
        _alreadyDisplayed = false;
    }

    void OnTriggerEnter (Collider other)
    {
        //Logger.Log ("EndGameCollider::OnTriggerEnter(" + other.ToString () + ")" + _alreadyDisplayed.ToString (), Logger.Level.INFO);
        if (!_alreadyDisplayed) {
            if (other == hero.GetComponent<Collider> ()) {
                triggerEnd();
            }
        }
    }

    public void displayEndMessage()
    {
        //Logger.Log("EndGameCollider:displayEndMessage", Logger.Level.INFO);
        ModalManager.setModal (endInfoPanel, true, endMainMenuButton.gameObject, endMainMenuButton.GetType ().AssemblyQualifiedName);
    }

    // can be called externally to trigger the end of the level
    public void triggerEnd()
    {
        _alreadyDisplayed = true;
        GameStateController.get ().triggerEnd (this);
    }
}