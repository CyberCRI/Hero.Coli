using UnityEngine;

public class UnscaledTimeParticleSystem : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _system;

    void Update ()
    {
        _system.Simulate(Time.unscaledDeltaTime, true, false);
    }
}
