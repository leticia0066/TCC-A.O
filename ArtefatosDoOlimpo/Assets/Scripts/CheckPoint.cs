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

        // Verifica se o GameManager existe.
        if (GameManager.instance == null)
        {
            Debug.LogError(
                "Checkpoint: GameManager.instance está vazio! " +
                "Verifique se o GameManager foi criado na cena inicial."
            );

            return;
        }

        GameManager.instance.SetCheckpoint(transform.position);

        activated = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarCheckpoint();
        }

        Debug.Log("CHECKPOINT ATIVADO!");
    }
}