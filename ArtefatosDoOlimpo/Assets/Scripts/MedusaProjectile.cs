
using UnityEngine;

public class MedusaProjectile : MonoBehaviour
{
    [Header("Tempo de vida do feitico")]
    public float lifetime = 5f;

    private Rigidbody2D rb;
    private Collider2D projectileCollider;

    private Vector2 direction;
    private float speed;
    private int damage;

    private bool initialized;
    private bool hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        if (rb == null)
        {
            Debug.LogError(
                "Feitico da Medusa precisa de Rigidbody2D!"
            );
        }

        if (projectileCollider == null)
        {
            Debug.LogError(
                "Feitico da Medusa precisa de Collider2D!"
            );
        }
    }

    public void Initialize(
        Vector2 shootDirection,
        float projectileSpeed,
        int projectileDamage,
        Transform owner)
    {
        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;

        // Impede o feitico de colidir com a propria Medusa.
        if (owner != null && projectileCollider != null)
        {
            Collider2D[] ownerColliders =
                owner.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D ownerCollider in ownerColliders)
            {
                if (ownerCollider != null)
                {
                    Physics2D.IgnoreCollision(
                        projectileCollider,
                        ownerCollider,
                        true
                    );
                }
            }
        }

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.collisionDetectionMode =
                CollisionDetectionMode2D.Continuous;

            rb.linearVelocity = direction * speed;
        }

        initialized = true;

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        if (!initialized || rb == null || hit)
            return;

        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hit)
            return;

        // Ignora objetos que nao sejam o jogador.
        PlayerHealth playerHealth =
            other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                other.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            hit = true;

            Debug.Log("Feitico acertou o jogador! Dano: " + damage);

            playerHealth.TakeDamage(damage);

            Destroy(gameObject);
            return;
        }

        // Nao desaparece ao atravessar outros triggers.
        if (other.isTrigger)
            return;

        // Desaparece ao atingir uma parede ou obstaculo solido.
        hit = true;
        Destroy(gameObject);
    }
}
