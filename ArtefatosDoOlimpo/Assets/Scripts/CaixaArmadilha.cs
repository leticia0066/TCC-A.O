
using UnityEngine;

public class CaixaArmadilha : MonoBehaviour
{
    [Header("Configuração da queda")]
    public float gravidadeQueda = 3f;

    [Header("Comportamento")]
    public bool destruirDepoisDeAtingirJogador = true;

    private Rigidbody2D rb;

    private bool ativada = false;
    private bool atingiuJogador = false;
    private bool atingiuChao = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

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
        if (ativada)
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

            Debug.Log("JOGADOR ATINGIDO PELA CAIXA!");

            
            jogador.TakeDamage(jogador.currentHealth);

            if (destruirDepoisDeAtingirJogador)
            {
                Destroy(gameObject, 0.15f);
            }

            return;
        }

        
        if (ativada && !atingiuChao)
        {
            atingiuChao = true;

            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;

            // Impede que continue se movimentando.
            rb.bodyType = RigidbodyType2D.Kinematic;

            Debug.Log("A CAIXA ATINGIU O CHÃO!");
        }
    }
}
