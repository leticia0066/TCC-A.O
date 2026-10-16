using UnityEngine;

public class Esqueleto : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pontoA;
    public Transform pontoB;
    public float velocidade = 2f;

    [Header("Vida")]
    public int vida = 3;

    [Header("Dano")]
    public int dano = 1;
    public float tempoEntreDanos = 1f;

    private bool indoParaB = true;
    private float ultimoDano;

    void Update()
    {
        Patrulhar();
    }

    void Patrulhar()
    {
        if (pontoA == null || pontoB == null)
            return;

        Transform destino =
            indoParaB ? pontoB : pontoA;

        transform.position = Vector2.MoveTowards(
            transform.position,
            destino.position,
            velocidade * Time.deltaTime
        );

        if (
            Vector2.Distance(
                transform.position,
                destino.position
            ) < 0.1f
        )
        {
            indoParaB = !indoParaB;
        }

        Virar(destino.position.x);
    }

    void Virar(float alvoX)
    {
        Vector3 escala = transform.localScale;

        escala.x =
            Mathf.Abs(escala.x)
            *
            Mathf.Sign(alvoX - transform.position.x);

        transform.localScale = escala;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (Time.time < ultimoDano + tempoEntreDanos)
            return;

        ultimoDano = Time.time;

        PlayerHealth player =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (player != null)
        {
            player.TakeDamage(dano);
        }
    }

    public void TakeDamage(int quantidade)
    {
        vida -= quantidade;

        if (vida <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (pontoA != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(
                pontoA.position,
                0.2f
            );
        }

        if (pontoB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(
                pontoB.position,
                0.2f
            );
        }
    }
}