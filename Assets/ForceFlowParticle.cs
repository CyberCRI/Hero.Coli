using UnityEngine;
using System.Collections;

public class ForceFlowParticle : MonoBehaviour {

  public float force;

	void OnParticleCollision(GameObject obj){
    Hero cellia = obj.GetComponent<Hero>();
    if(cellia){
     //cellia.subLife(0.1f);
      Rigidbody body = cellia.rigidbody;
     //if (col.attachedRigidbody){
      if (body){
        Vector3 push = this.transform.rotation * new Vector3(0,0,1);
        body.AddForce((push * force));
      }
    }
  }

}
