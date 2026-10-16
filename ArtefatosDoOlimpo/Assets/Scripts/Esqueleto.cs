
using UnityEngine;

public class Esqueleto : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pontoA;
    public Transform pontoB;
    public float velocidade = 2f;
    public float distanciaParaVirar = 0.08f;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    // Marque se a imagem original olha para a direita.
    public bool spriteOlhaParaDireita = true;

    [Header("Vida")]
    public int vida = 3;

    [Header("Dano")]
    public int dano = 1;
    public float tempoEntreDanos = 1f;

    private Transform destino;
    private float ultimoDano = -999f;
    private bool morto = false;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        if (pontoA == null || pontoB == null)
        {
            Debug.LogError(
                "Esqueleto: arraste os pontos A e B no Inspector."
            );
            enabled = false;
            return;
        }

        // Começa indo para B.
        destino = pontoB;
        AtualizarDirecao();
    }

    void Update()
    {
        if (morto || destino == null)
            return;

        Vector3 posicaoAnterior = transform.position;

        transform.position = Vector2.MoveTowards(
            transform.position,
            destino.position,
            velocidade * Time.deltaTime
        );

        // Usa o movimento real para orientar a imagem.
        float movimentoX = transform.position.x - posicaoAnterior.x;

        if (Mathf.Abs(movimentoX) > 0.0001f)
        {
            VirarPara(movimentoX);
        }

        // Ao chegar, troca o destino.
        if (Vector2.Distance(
            transform.position,
            destino.position
        ) <= distanciaParaVirar)
        {
            destino = destino == pontoB ? pontoA : pontoB;

            // Vira imediatamente para o próximo trajeto.
            float direcaoX = destino.position.x - transform.position.x;

            if (Mathf.Abs(direcaoX) > 0.0001f)
                VirarPara(direcaoX);
        }
    }

    void AtualizarDirecao()
    {
        if (destino == null)
            return;

        float direcaoX = destino.position.x - transform.position.x;

        if (Mathf.Abs(direcaoX) > 0.0001f)
            VirarPara(direcaoX);
    }

    void VirarPara(float direcaoX)
    {
        if (spriteRenderer == null)
            return;

        bool movimentoParaEsquerda = direcaoX < 0f;

        spriteRenderer.flipX = spriteOlhaParaDireita
            ? movimentoParaEsquerda
            : !movimentoParaEsquerda;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (morto)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < ultimoDano + tempoEntreDanos)
            return;

        PlayerHealth player =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (player == null)
        {
            player =
                collision.gameObject.GetComponentInParent<PlayerHealth>();
        }

        if (player != null)
        {
            ultimoDano = Time.time;
            player.TakeDamage(dano);
        }
    }

    public void TakeDamage(int quantidade)
    {
        if (morto || quantidade <= 0)
            return;

        vida -= quantidade;

        if (vida <= 0)
        {
            morto = true;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (pontoA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pontoA.position, 0.2f);
        }

        if (pontoB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pontoB.position, 0.2f);
        }

        if (pontoA != null && pontoB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pontoA.position, pontoB.position);
        }
    }
}
