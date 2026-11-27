
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HazardZone : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 1;

    [Header("Intervalo entre danos")]
    public float intervaloEntreDanos = 1f;

    private PlayerHealth playerHealth;
    private float proximoDano;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError(
                "PlayerHazard: PlayerHealth não encontrado no Player!"
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AplicarDano(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        AplicarDano(collision);
    }

    private void AplicarDano(Collider2D collision)
    {
        if (!collision.CompareTag("Hazard"))
            return;

        if (playerHealth == null)
            return;

        if (playerHealth.EstaMorrendo)
            return;

        if (playerHealth.EstaInvulneravel)
            return;

        if (Time.time < proximoDano)
            return;

        proximoDano = Time.time + intervaloEntreDanos;

        playerHealth.TakeDamage(dano);
    }
}
