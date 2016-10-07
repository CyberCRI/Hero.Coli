using UnityEngine;
using System.Collections;

public class NanoBotCutSceneHandler : MonoBehaviour {

    private float _scaleDownSpeed;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "NPC")
        {
            StartCoroutine(CollisionNPC(col));
        }
    }

    IEnumerator CollisionNPC(Collider col)
    {
        /*while (this.transform.localScale.x > 0.1f)
        {
            this.transform.localScale =new Vector3 (this.transform.localScale.x - Time.deltaTime * _scaleDownSpeed, this.transform.localScale.y - Time.deltaTime * _scaleDownSpeed, this.transform.localScale.z - Time.deltaTime * _scaleDownSpeed);
            yield return null;
        }*/

        yield return new WaitForSeconds(3f);
        col.transform.GetChild(3).gameObject.SetActive(true);
        Destroy(this.gameObject);
        yield return null;
    }
}
