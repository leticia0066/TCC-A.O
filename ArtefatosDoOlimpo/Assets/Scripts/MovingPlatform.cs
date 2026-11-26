
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Pontos de movimento")]
    public Transform pointA;
    public Transform pointB;

    [Header("Configurações")]
    public float speed = 2f;

    private Transform target;

    private Rigidbody2D rb;
    private Collider2D plataformaCollider;

    private Rigidbody2D jogadorRb;
    private Collider2D jogadorCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        plataformaCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        target = pointB;
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null)
            return;

        if (target == null)
            return;

        Vector2 posicaoAtual = rb.position;

        Vector2 proximaPosicao = Vector2.MoveTowards(
            posicaoAtual,
            target.position,
            speed * Time.fixedDeltaTime
        );

        // Calcula quanto a plataforma vai se deslocar.
        Vector2 deslocamento = proximaPosicao - posicaoAtual;

        // Move a plataforma pela física.
        rb.MovePosition(proximaPosicao);

        // Transporta o jogador quando ele está em cima.
        if (jogadorRb != null && jogadorCollider != null)
        {
            bool estaEmCima =
                jogadorCollider.bounds.min.y >=
                plataformaCollider.bounds.max.y - 0.15f;

            bool naoEstaPulando =
                jogadorRb.linearVelocity.y <= 0.1f;

            if (estaEmCima && naoEstaPulando)
            {
                jogadorRb.position += deslocamento;
            }
        }

        // Verifica se chegou ao destino.
        if (Vector2.Distance(proximaPosicao, target.position) < 0.05f)
        {
            target = target == pointA ? pointB : pointA;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController jogador =
            collision.collider.GetComponentInParent<PlayerController>();

        if (jogador == null)
            return;

        Rigidbody2D rbJogador =
            jogador.GetComponent<Rigidbody2D>();

        if (rbJogador == null)
            return;

        jogadorRb = rbJogador;
        jogadorCollider = collision.collider;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController jogador =
            collision.collider.GetComponentInParent<PlayerController>();

        if (jogador == null)
            return;

        jogadorRb = null;
        jogadorCollider = null;
    }
}
