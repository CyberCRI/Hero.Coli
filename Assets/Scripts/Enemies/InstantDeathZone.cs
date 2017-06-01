using UnityEngine;

public class InstantDeathZone : MonoBehaviour
{
    private const CustomDataValue _deathCause = CustomDataValue.CRUSHED;

    void OnCollisionEnter(Collision collision)
    {
        if (Character.playerTag == collision.gameObject.tag)
        {
            Character.get().kill(_deathCause);
        }
    }
}