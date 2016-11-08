using UnityEngine;

public class RepulsingCollider : MonoBehaviour
{
    [SerializeField]
    private Vector3 _force;
    [SerializeField]
    private bool _useImpulse, _useRelativeVelocity, _usePositionsVector;
    [SerializeField]
    private float _factor;
    [SerializeField]
    private string[] _tags;

    // private void collide(Collision collision, string caller)
    private void collide(Collision collision)
    {
        // Debug.Log(this.GetType() + " OnCollisionEnter " + collision.gameObject.name);
        // string debug = this.GetType() + " " + caller + " " + collision.gameObject.name;
        if (contains(_tags, collision.collider.tag) && null != collision.collider.attachedRigidbody)
        {
            // debug += "\nnull != collision.collider.attachedRigidbody";
            Vector3 force = _force;
            if (_useImpulse)
            {
                force = collision.impulse;
            }
            else if (_useRelativeVelocity)
            {
                force = collision.relativeVelocity;
            }
            else if (_usePositionsVector)
            {
                force = this.transform.position - collision.transform.position;
            }
            else
            {
                force = _force;
            }
            collision.collider.attachedRigidbody.AddForce(force * _factor);
            // debug += ("\nforce = " + force);
            // debug += ("\nfactor = " + _factor);
        }
        // Debug.Log(debug);
    }

    private static bool contains<T>(T[] array, T target)
    {
        foreach (T t in array)
        {
            if (t.Equals(target))
            {
                return true;
            }
        }
        return false;
    }

    void OnCollisionStay(Collision collision)
    {
        // collide(collision, "OnCollisionStay");
        collide(collision);
    }

    void OnCollisionEnter(Collision collision)
    {
        // collide(collision, "OnCollisionEnter");
        collide(collision);
    }
}