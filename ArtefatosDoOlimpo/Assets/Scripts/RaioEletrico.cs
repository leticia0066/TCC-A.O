
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class RaioEletrico : MonoBehaviour
{
    [Header("Dano")]
    [Min(1)]
    public int damage = 1;

    [Header("Movimento")]
    public float velocidade = 8f;

    [Header("Duracao")]
    public float tempoDeVida = 5f;

    [Header("Aparencia")]
    public bool inverterHorizontal = false;

    private Rigidbody2D rb;
    private bool atingiuJogador;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        Collider2D colisor = GetComponent<Collider2D>();
        colisor.isTrigger = true;

        SpriteRenderer sprite =
            GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            sprite.flipX = inverterHorizontal;
        }
    }

    private void Start()
    {
        Destroy(gameObject, tempoDeVida);
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        
        Vector2 novaPosicao =
            rb.position +
            Vector2.left * velocidade * Time.fixedDeltaTime;

        rb.MovePosition(novaPosicao);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (atingiuJogador)
            return;

        PlayerHealth player =
            collision.GetComponentInParent<PlayerHealth>();

        if (player == null)
            return;

        atingiuJogador = true;

        Debug.Log("O raio de Zeus atingiu o jogador!");

        player.TakeDamage(damage);

        Destroy(gameObject);
    }
}
