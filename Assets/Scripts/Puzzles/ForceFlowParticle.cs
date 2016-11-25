using UnityEngine;

public class ForceFlowParticle : MonoBehaviour
{
    [SerializeField]
    private float force;

    void OnParticleCollision(GameObject obj)
    {
        Hero hero = obj.GetComponent<Hero>();
        if (hero)
        {
            Rigidbody body = hero.GetComponent<Rigidbody>();
            if (body)
            {
                Vector3 push = this.transform.rotation * Vector3.forward;
                body.AddForce(push * force);
            }
        }
    }
}
