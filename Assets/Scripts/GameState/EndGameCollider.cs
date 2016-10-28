using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
  
    [HideInInspector]
    public GameObject endInfoPanel, hero;
    [HideInInspector]
    public EndMainMenuButton endMainMenuButton;
    private bool _alreadyDisplayed;
    // Use this for initialization
    void Start ()
    {
        _alreadyDisplayed = false;
    }

    void OnTriggerEnter (Collider other)
    {
        //Debug.Log(this.GetType() + " EndGameCollider::OnTriggerEnter(" + other.ToString () + ")" + _alreadyDisplayed.ToString ());
        if (!_alreadyDisplayed) {
            if (other == hero.GetComponent<Collider> ()) {
                triggerEnd();
            }
        }
    }

    public void displayEndMessage()
    {
        //Debug.Log(this.GetType() + " EndGameCollider:displayEndMessage");
        ModalManager.setModal (endInfoPanel, true, endMainMenuButton.gameObject, endMainMenuButton.GetType ().AssemblyQualifiedName);
    }

    // can be called externally to trigger the end of the level
    public void triggerEnd()
    {
        _alreadyDisplayed = true;
        GameStateController.get ().triggerEnd (this);
    }
}