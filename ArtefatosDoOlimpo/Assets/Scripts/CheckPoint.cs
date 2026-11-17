
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (activated)
            return;

        if (GameManager.instance == null)
        {
            Debug.LogError("Checkpoint: GameManager não encontrado!");
            return;
        }

        PlayerHealth jogador =
            collision.GetComponentInParent<PlayerHealth>();

        if (jogador == null)
        {
            Debug.LogError("Checkpoint: PlayerHealth não encontrado no jogador!");
            return;
        }

        GameManager.instance.SetCheckpoint(
            transform.position,
            jogador
        );

        activated = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarCheckpoint();
        }

        Debug.Log("CHECKPOINT ATIVADO E PROGRESSO SALVO!");
    }
}
