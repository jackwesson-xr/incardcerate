using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class InkAttractor : MonoBehaviour
{
    public Transform target;
    public float attractionStrength = 5f;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        int numParticlesAlive = ps.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 dir = (target.position - particles[i].position).normalized;
            particles[i].velocity += dir * attractionStrength * Time.deltaTime;
        }

        ps.SetParticles(particles, numParticlesAlive);
    }
}
