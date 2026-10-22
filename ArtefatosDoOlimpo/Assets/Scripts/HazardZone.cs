using UnityEngine;

public class PlayerHazard : MonoBehaviour
{
    [Header("Configurações de Checkpoint")]
    [Tooltip("Posição para onde o player irá ao tocar no perigo")]
    public Vector3 respawnPoint;

    private Rigidbody2D rb;

    private void Start()
    {
        // Salva o Rigidbody2D do Player
        rb = GetComponent<Rigidbody2D>();

        // Salva a posição inicial como o primeiro checkpoint
        respawnPoint = transform.position;
    }

    // Este método é executado QUANDO O PLAYER entra em um colisor do tipo Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto onde o player entrou tem a Tag "Hazard" (sua água)
        if (collision.CompareTag("Hazard"))
        {
            Respawn();
        }
    }

    // Também adicionamos o OnTriggerStay2D caso o player comece o jogo já dentro da água
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Hazard"))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        // Move o player para a posição do checkpoint salvador
        transform.position = respawnPoint;

        // Zera as forças e aceleração do Rigidbody para o player não continuar se movendo após o respawn
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Unity 6 / 2023+
            rb.angularVelocity = 0f;
        }

        Debug.Log("O Player tocou na água e retornou ao Checkpoint!");
    }

    // Função pública para atualizar o checkpoint futuramente (ex: ao tocar numa bandeira)
    public void SetNewCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
        Debug.Log("Novo Checkpoint salvo em: " + newPosition);
    }
}