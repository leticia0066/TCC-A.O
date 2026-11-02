
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHazard : MonoBehaviour
{
    [Header("Checkpoint")]
    [Tooltip("Posição inicial usada caso o GameManager não esteja disponível.")]
    public Vector3 respawnPoint;

    [Header("Configuração do Perigo")]
    [Tooltip("Tempo mínimo para impedir respawns repetidos.")]
    public float respawnCooldown = 0.2f;

    private Rigidbody2D rb;
    private bool respawning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Guarda a posição inicial como alternativa.
        respawnPoint = transform.position;
    }

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
        if (!collision.CompareTag("Hazard"))
            return;

        Respawn();
    }

    public void Respawn()
    {
        if (respawning)
            return;

        respawning = true;

        Vector3 destino;

        // Busca a posição do último checkpoint no GameManager.
        if (GameManager.instance != null)
        {
            destino = GameManager.instance.GetSpawnPosition();

            Debug.Log("Retornando ao checkpoint do GameManager: " + destino);
        }
        else
        {
            // Alternativa caso o GameManager não exista.
            destino = respawnPoint;

            Debug.LogWarning(
                "GameManager não encontrado! Retornando à posição inicial."
            );
        }

        // Teletransporta o jogador.
        transform.position = destino;

        // Interrompe o movimento físico.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log("Jogador retornou ao checkpoint após tocar no perigo.");

        StartCoroutine(LiberarRespawn());
    }

    private IEnumerator LiberarRespawn()
    {
        yield return new WaitForSecondsRealtime(
            Mathf.Max(0.05f, respawnCooldown)
        );

        respawning = false;
    }

    public void SetNewCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;

        Debug.Log("Posição alternativa atualizada: " + newPosition);
    }
}
