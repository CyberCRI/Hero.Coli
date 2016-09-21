using UnityEngine;

public class BasicMine : ResettableMine
{
    public void detonate()
    {
        MineManager.get().detonate(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        detonate();
        if (collision.gameObject.name == Hero.gameObjectName)
        {
            Hero.get().getLifeManager().setSuddenDeath(true);
        }
        else if (collision.gameObject.tag == "NPC")
        {
            collision.gameObject.GetComponent<DeathDummy>().startDeath();
        }
    }
}
