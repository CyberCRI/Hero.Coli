using UnityEngine;

public class InstantDeathZone : MonoBehaviour
{
    [SerializeField]
    private CustomDataValue _deathCause = CustomDataValue.CRUSHED;

    void OnCollisionEnter(Collision collision)
    {
        if (Character.playerTag == collision.gameObject.tag)
        {
            Character.get().kill(new CustomData(CustomDataTag.SOURCE, _deathCause.ToString()));
        }
    }
}