
using UnityEngine;

public class CaixaArmadilha : MonoBehaviour
{
    [Header("Configuração da queda")]
    public float gravidadeQueda = 3f;

    [Header("Comportamento")]
    public bool destruirDepoisDeAtingirJogador = true;

    private Rigidbody2D rb;
    private Collider2D[] colliders;

    private bool ativada = false;
    private bool atingiuJogador = false;
    private bool atingiuChao = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();

        if (rb == null)
        {
            Debug.LogError("CaixaArmadilha precisa de Rigidbody2D!");
            enabled = false;
            return;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativada || atingiuJogador)
            return;

        PlayerHealth jogador =
            other.GetComponentInParent<PlayerHealth>();

        if (jogador == null)
            return;

        ativada = true;
        rb.gravityScale = gravidadeQueda;

        Debug.Log("ARMADILHA ATIVADA! A CAIXA ESTÁ CAINDO!");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth jogador =
            collision.collider.GetComponentInParent<PlayerHealth>();

        if (jogador != null)
        {
            if (atingiuJogador)
                return;

            atingiuJogador = true;

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Desativa as colisões da caixa para não interferir
            // no jogador durante o respawn.
            foreach (Collider2D col in colliders)
            {
                if (col != null)
                    col.enabled = false;
            }

            Debug.Log("JOGADOR ATINGIDO PELA CAIXA!");

            jogador.TakeDamage(jogador.currentHealth);

            if (destruirDepoisDeAtingirJogador)
                Destroy(gameObject, 0.15f);

            return;
        }

        if (ativada && !atingiuChao)
        {
            atingiuChao = true;

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;

            Debug.Log("A CAIXA ATINGIU O CHÃO!");
        }
    }
}
