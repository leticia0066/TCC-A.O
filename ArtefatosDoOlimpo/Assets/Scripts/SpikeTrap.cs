
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Dano")]
    [Min(1)]
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Procura o PlayerHealth no objeto atingido
        // ou em um dos objetos pais.
        PlayerHealth player =
            collision.GetComponentInParent<PlayerHealth>();

        if (player == null)
            return;

        Debug.Log("Espinhos atingiram o jogador!");

        player.TakeDamage(damage);
    }
}
