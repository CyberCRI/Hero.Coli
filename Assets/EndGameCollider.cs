using UnityEngine;
using System.Collections;

public class EndGameCollider : MonoBehaviour {
	
	public GameObject infoPanel, hero;
	public GameStateController gameStateController;
	private bool alreadyDisplayed;
	// Use this for initialization
	void Start () {
		alreadyDisplayed = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other) {
		Logger.Log("EndGameCollider::OnTriggerEnter("+other.ToString()+")"+alreadyDisplayed.ToString());
		if(alreadyDisplayed == false) {
			if(other == hero.GetComponent<Collider>()) {
		        
				gameStateController.StateChange(GameState.End);
				gameStateController.dePauseForbidden = true;
				alreadyDisplayed = true;
				StartCoroutine(WaitFade(2000f));
				
			}
		}
	}
	
	IEnumerator WaitFade(float waitTime)
	{
	    // do stuff before waitTime
	 	
	    yield return new WaitForSeconds(waitTime);
		
	 	infoPanel.SetActive(true);
	    // do stuff after waitTime
	}
}
