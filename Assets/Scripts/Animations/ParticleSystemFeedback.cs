﻿using UnityEngine;

public class ParticleSystemFeedback : MonoBehaviour {

    public ParticleSystem system;

    public void burst(Vector3 position)
    {
        system.transform.position = position;
        system.Emit(10);
    }
}
