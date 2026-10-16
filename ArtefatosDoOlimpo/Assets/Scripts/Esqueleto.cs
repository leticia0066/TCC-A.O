
using System.Collections;
using UnityEngine;

public class Esqueleto : MonoBehaviour
{
    [Header("Pontos da patrulha")]
    public Transform pontoA;
    public Transform pontoB;

    [Header("Movimento")]
    public float velocidade = 2f;

    [Header("Teletransporte")]
    public float tempoDesaparecido = 0.4f;

    [Header("Vida")]
    public int vida = 3;

    [Header("Dano ao jogador")]
    public int dano = 1;
    public float tempoEntreDanos = 1f;

    private SpriteRenderer[] sprites;
    private Collider2D[] colisores;
    private Rigidbody2D rb;

    private float ultimoDano = -999f;
    private bool morto = false;
    private bool teleportando = false;

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        colisores = GetComponentsInChildren<Collider2D>(true);
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (pontoA == null || pontoB == null)
        {
            Debug.LogError("Esqueleto: arraste os pontos A e B no Inspector.");
            enabled = false;
            return;
        }

        // Começa no ponto A.
        transform.position = pontoA.position;
    }

    void Update()
    {
        if (morto || teleportando)
            return;

        // Anda sempre na mesma direção visual: de A para B.
        transform.position = Vector2.MoveTowards(
            transform.position,
            pontoB.position,
            velocidade * Time.deltaTime
        );

        // Ao chegar em B, desaparece e volta para A.
        if (Vector2.Distance(transform.position, pontoB.position) < 0.05f)
        {
            StartCoroutine(VoltarParaA());
        }
    }

    IEnumerator VoltarParaA()
    {
        teleportando = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = pontoB.position;
        }

        // Desaparece e deixa de causar dano durante a troca.
        foreach (SpriteRenderer sr in sprites)
        {
            if (sr != null)
                sr.enabled = false;
        }

        foreach (Collider2D col in colisores)
        {
            if (col != null)
                col.enabled = false;
        }

        yield return new WaitForSeconds(tempoDesaparecido);

        // Reaparece no ponto A e começa a andar para B novamente.
        transform.position = pontoA.position;

        if (rb != null)
            rb.position = pontoA.position;

        foreach (SpriteRenderer sr in sprites)
        {
            if (sr != null)
                sr.enabled = true;
        }

        foreach (Collider2D col in colisores)
        {
            if (col != null)
                col.enabled = true;
        }

        teleportando = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (morto || teleportando)
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < ultimoDano + tempoEntreDanos)
            return;

        PlayerHealth player =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (player == null)
            player = collision.gameObject.GetComponentInParent<PlayerHealth>();

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
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
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
