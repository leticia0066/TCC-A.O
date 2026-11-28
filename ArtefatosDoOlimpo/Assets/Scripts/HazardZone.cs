
using UnityEngine;

public class HazardZone : MonoBehaviour
{
    [Header("Dano")]
    public int dano = 6;

    [Header("Intervalo entre danos")]
    public float intervaloEntreDanos = 1f;

    private float proximoDano;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VerificarPerigo(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        VerificarPerigo(collision);
    }

    private void VerificarPerigo(Collider2D collision)
    {
        PlayerHealth playerHealth =
            collision.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                collision.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
            return;

        if (playerHealth.EstaMorrendo ||
            playerHealth.EstaInvulneravel)
            return;

        if (Time.time < proximoDano)
            return;

        proximoDano = Time.time + intervaloEntreDanos;

        Debug.Log("PLAYER ENTROU EM UMA ZONA PERIGOSA!");

        playerHealth.TakeDamage(dano);
    }
}
