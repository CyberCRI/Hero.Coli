using UnityEngine;

public class InstantDeathZone : MonoBehaviour
{
    [SerializeField]
    private CustomDataValue _deathCause = CustomDataValue.CRUSHED;

    void OnCollisionEnter(Collision collision)
    {
        if (Hero.playerTag == collision.gameObject.tag)
        {
            Hero.get().kill(new CustomData(CustomDataTag.SOURCE, _deathCause.ToString()));
        }
    }
}