using UnityEngine;
using System.Collections;

public class EndGameCollider : MonoBehaviour
{
  
    public GameObject endInfoPanel, hero;
    public EndRestartButton endInfoPanelRestartButton;
    public GameStateController gameStateController;
    private bool alreadyDisplayed;
    // Use this for initialization
    void Start ()
    {
        alreadyDisplayed = false;
    }
  
    // Update is called once per frame
    void Update ()
    {
    }

    void OnTriggerEnter (Collider other)
    {
        Logger.Log ("EndGameCollider::OnTriggerEnter(" + other.ToString () + ")" + alreadyDisplayed.ToString (), Logger.Level.INFO);
        if (alreadyDisplayed == false) {
            if (other == hero.GetComponent<Collider> ()) {
                alreadyDisplayed = true;
                gameStateController.triggerEnd(this);
            }
        }
    }

    public void displayEndMessage()
    {
        Logger.Log("EndGameCollider:displayEndMessage", Logger.Level.INFO);
        ModalManager.setModal (endInfoPanel, true, endInfoPanelRestartButton.gameObject, endInfoPanelRestartButton.GetType ().AssemblyQualifiedName);
    }
}