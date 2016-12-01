using UnityEngine;

public class BasicMine : ResettableMine
{
    public void detonate()
    {
        // Debug.Log(this.GetType() + " " + name + " detonates ");
        MineManager.get().detonate(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        detonate();
        if (collision.gameObject.name == Hero.gameObjectName)
        {
            Hero.get().kill(new CustomData(CustomDataTag.SOURCE, CustomDataValue.MINE.ToString()));
        }
        else if (collision.gameObject.tag == "NPC")
        {
            collision.gameObject.GetComponent<DeathDummy>().startDeath();
        }
        this.gameObject.SetActive(false);
    }
}
