using UnityEngine;

public abstract class BacteriumSelector : MonoBehaviour
{
    protected abstract bool isBacteriumSelected();
    protected abstract void getRidOf();

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            // Debug.Log(this.GetType() + " OnCollisionEnter hit player");
            if (!isBacteriumSelected())
            {
                getRidOf();
            }
        }
    }
}