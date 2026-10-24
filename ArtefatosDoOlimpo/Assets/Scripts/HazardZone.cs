
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHazard : MonoBehaviour
{
    [Header("Checkpoint")]
    [Tooltip("Posição inicial ou posição do último checkpoint")]
    public Vector3 respawnPoint;

    [Header("Configuração do Perigo")]
    [Tooltip("Tempo mínimo para impedir respawns repetidos")]
    public float respawnCooldown = 0.2f;

    private Rigidbody2D rb;
    private bool respawning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // A posição inicial funciona como checkpoint padrão.
        respawnPoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        VerificarPerigo(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Também verifica se o Player continuar dentro do perigo.
        VerificarPerigo(collision);
    }

    private void VerificarPerigo(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        // Impede que o mesmo contato provoque vários respawns.
        if (respawning)
            return;

        respawning = true;

        // Retorna o Player ao checkpoint salvo.
        transform.position = respawnPoint;

        // Interrompe o movimento físico.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        Debug.Log("Player tocou em um perigo e voltou ao checkpoint.");

        StartCoroutine(LiberarRespawn());
    }

    private IEnumerator LiberarRespawn()
    {
        // Usa tempo real para funcionar mesmo com o jogo pausado.
        yield return new WaitForSecondsRealtime(
            Mathf.Max(0.05f, respawnCooldown)
        );

        respawning = false;
    }

    public void SetNewCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;

        Debug.Log("Novo checkpoint salvo: " + newPosition);
    }
}
