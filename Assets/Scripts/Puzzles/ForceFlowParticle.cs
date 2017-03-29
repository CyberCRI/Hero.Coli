using UnityEngine;

public class ForceFlowParticle : MonoBehaviour
{
    [SerializeField]
    private float force;

    void OnParticleCollision(GameObject obj)
    {
        Character character = obj.GetComponent<Character>();
        if (character)
        {
            Rigidbody body = character.GetComponent<Rigidbody>();
            if (body)
            {
                Vector3 push = this.transform.rotation * Vector3.forward;
                body.AddForce(push * force);
            }
        }
    }
}
