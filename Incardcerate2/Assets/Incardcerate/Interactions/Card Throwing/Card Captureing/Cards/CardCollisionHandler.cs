
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles collisions for a Card and emits a UnityEvent. Optionally spawns a particle effect.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CardCollisionHandler : MonoBehaviour
{
    [Header("Collision Events")]
    [Tooltip("Called when this card collides with something.")]
    public UnityEvent<GameObject> OnHit;

    [Header("Particle Settings")]
    [Tooltip("Optional particle system to spawn on collision.")]
    [SerializeField] private ParticleSystem defaultHitEffect;

    [Tooltip("Use world position for hit particles.")]
    [SerializeField] private bool spawnParticlesAtContact = true;

    [Tooltip("Automatically look for an ActiveCardHandler in scene.")]
    [SerializeField] private ActiveCardHandler activeCardHandler;

    private void Start()
    {
        if (activeCardHandler == null)
        {
            activeCardHandler = FindObjectOfType<ActiveCardHandler>();
            if (activeCardHandler == null)
            {
                Debug.LogWarning("No ActiveCardHandler found.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        Debug.Log($"Card collided with: {other.name}");

        // Trigger UnityEvent
        OnHit?.Invoke(other);

        if (activeCardHandler != null)
        {
            Vector3 impactPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;
            activeCardHandler.HandleCollisionWithImpact(other, impactPoint, gameObject);
        }

        // Optional: spawn particles
        if (defaultHitEffect != null)
        {
            Vector3 spawnPosition = spawnParticlesAtContact && collision.contacts.Length > 0
                ? collision.contacts[0].point
                : transform.position;

            ParticleSystem effect = Instantiate(defaultHitEffect, spawnPosition, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration + 0.5f);
        }
    }
}
