using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractor : MonoBehaviour
{
    public Transform attractor;
    public float attractionForce = 0.5f;

    private new ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }

    void LateUpdate()
    {
        int count = particleSystem.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + particles[i].position;
            //Vector3 vel = particles[i].velocity;
            float t = (particles[i].startLifetime - particles[i].remainingLifetime) / particles[i].startLifetime;

            // interpolate towards final position
            particles[i].position = Vector3.Lerp(pos, attractor.position, t*t) - transform.position;
        }


        particleSystem.SetParticles(particles, count);
    }

}
