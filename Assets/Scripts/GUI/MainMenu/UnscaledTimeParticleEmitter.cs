using UnityEngine;

public class UnscaledTimeParticleEmitter : MonoBehaviour
{
    [SerializeField]
    private ParticleEmitter _emitter;

    void Update ()
    {
        _emitter.Simulate(Time.unscaledDeltaTime);
    }
}
