using System.Collections;
using UnityEngine;

public class Esqueleto : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadePerseguicao = 4f;

    [Header("Detecção")]
    public float distanciaDeteccao = 8f;

    [Header("Vida")]
    public int vida = 1;

    [Header("Dano")]
    public int dano = 1;
    public float tempoEntreDanos = 1f;

    private Transform player;

    private float ultimoDano;

    private bool morto;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;

    void Start()
    {
        GameObject p =
            GameObject.FindGameObjectWithTag("Player");

        if (p != null)
        {
            player = p.transform;
        }

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        rb =
            GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (morto)
            return;

        if (player == null)
            return;

        float distancia =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (distancia <= distanciaDeteccao)
        {
            SeguirPlayer();
        }
    }

    void SeguirPlayer()
    {
        transform.position =
            Vector2.MoveTowards(
                transform.position,
                player.position,
                velocidadePerseguicao *
                Time.deltaTime
            );

        Virar(player.position.x);
    }

    void Virar(float alvoX)
    {
        Vector3 escala =
            transform.localScale;

        escala.x =
            Mathf.Abs(escala.x)
            *
            Mathf.Sign(
                alvoX -
                transform.position.x
            );

        transform.localScale = escala;
    }

    private void OnTriggerStay2D(
        Collider2D collision
    )
    {
        if (!collision.CompareTag("Player"))
            return;

        if (
            Time.time <
            ultimoDano + tempoEntreDanos
        )
            return;

        ultimoDano = Time.time;

        PlayerHealth playerHealth =
            collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(dano);
        }
    }

    public void TakeDamage(int danoRecebido)
    {
        if (morto)
            return;

        Debug.Log("ESQUELETO TOMOU DANO");

        vida -= danoRecebido;

        if (vida <= 0)
        {
            StartCoroutine(Morrer());
        }
    }

    IEnumerator Morrer()
    {
        morto = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        for (int i = 0; i < 6; i++)
        {
            spriteRenderer.enabled = false;

            yield return new WaitForSeconds(
                0.08f
            );

            spriteRenderer.enabled = true;

            yield return new WaitForSeconds(
                0.08f
            );
        }

        Destroy(gameObject);
    }
}