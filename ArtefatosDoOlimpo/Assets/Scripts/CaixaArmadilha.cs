
using UnityEngine;

public class CaixaArmadilha : MonoBehaviour
{
    [Header("Configuração da queda")]
    public float gravidadeQueda = 3f;

    [Header("Dano")]
    public int dano = 1;

    [Header("Comportamento")]
    public bool destruirDepoisDeAtingirJogador = false;

    private Rigidbody2D rb;
    private Collider2D[] colliders;

    private bool ativada = false;
    private bool atingiuChao = false;
    private bool atingiuJogador = false;

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

        // A caixa começa parada no alto.
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativada || atingiuChao)
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
        // Se ainda está caindo, pode causar dano ao jogador.
        if (!atingiuChao && ativada)
        {
            PlayerHealth jogador =
                collision.collider.GetComponentInParent<PlayerHealth>();

            if (jogador != null)
            {
                if (!atingiuJogador)
                {
                    atingiuJogador = true;

                    Debug.Log("JOGADOR ATINGIDO PELA CAIXA!");

                    jogador.TakeDamage(dano);
                }

                // A caixa para, mas continua com seus colisores ativos.
                PararCaixa();

                if (destruirDepoisDeAtingirJogador)
                {
                    DestruirCaixa();
                }

                return;
            }
        }

        // Quando a caixa toca o chão ou uma plataforma,
        // ela para e permanece sólida.
        if (ativada && !atingiuChao)
        {
            atingiuChao = true;

            PararCaixa();

            Debug.Log("A CAIXA ATINGIU O CHÃO E VIROU UMA PLATAFORMA!");
        }
    }

    private void PararCaixa()
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void DestruirCaixa()
    {
        Destroy(gameObject, 0.15f);
    }
}
