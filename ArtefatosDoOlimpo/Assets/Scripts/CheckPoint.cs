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

        GameManager.instance.SetCheckpoint(
            transform.position
        );

        activated = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarCheckpoint();
        }
    }
}