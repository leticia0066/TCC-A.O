
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (activated)
            return;

        if (CheckpointManager.Instance == null)
        {
            Debug.LogWarning(
                "CheckpointManager não encontrado na cena!"
            );
            return;
        }

        CheckpointManager.Instance.SetCheckpoint(transform.position);

        activated = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarCheckpoint();
        }
    }
}
