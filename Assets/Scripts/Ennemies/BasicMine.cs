using UnityEngine;

public class BasicMine : ResettableMine
{
    private bool _detonated = false;

    private bool _isReseting = false;

    private bool _first = true;

    public void detonate(bool reseting)
    {
        Debug.Log("self " + this.gameObject.name + " detonates");
        _isReseting = reseting;
        MineManager.get().detonate(this);
        _detonated = true;
    }

    public bool isDetonated() { return _detonated; }

    void OnCollisionEnter(Collision collision)
    {
		Debug.Log("colliding with " + collision.gameObject.name);
        if (collision.gameObject.name == Hero.gameObjectName)
        {
            detonate(true);
            collision.gameObject.GetComponent<Hero>().getLifeManager().setSuddenDeath(true);
        }
        else if (collision.gameObject.tag == "NPC")
        {
            detonate(false);
        }
    }
}
