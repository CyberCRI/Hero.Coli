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
                Vector3 push = this.transform.rotation * new Vector3(0, 0, 1);
                body.AddForce(push * force);
            }
        }
    }
}
