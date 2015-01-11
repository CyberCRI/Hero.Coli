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
        Logger.Log ("EndGameCollider::OnTriggerEnter(" + other.ToString () + ")" + alreadyDisplayed.ToString (), Logger.Level.WARN);
        if (alreadyDisplayed == false) {
            if (other == hero.GetComponent<Collider> ()) {
            
                gameStateController.changeState (GameState.End);

                ModalManager.setModal (
                    endInfoPanel, true, 
                    endInfoPanelRestartButton.gameObject, endInfoPanelRestartButton.GetType ().Name);

                alreadyDisplayed = true;
            }
        }
    }
}
