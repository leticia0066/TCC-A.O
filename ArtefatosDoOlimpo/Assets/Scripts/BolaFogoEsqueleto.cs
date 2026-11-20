
using UnityEngine;

public class BolaFogoEsqueleto : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 6f;
    public float tempoVida = 4f;

    [Header("Dano")]
    public int dano = 1;

    private Vector2 direcao;
    private Transform dono;
    private bool configurada;

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    public void ConfigurarDirecao(Vector2 novaDirecao, Transform objetoDono)
    {
        direcao = novaDirecao.normalized;
        dono = objetoDono;
        configurada = true;

        // Ignora colisões com todos os colisores do esqueleto.
        if (dono != null)
        {
            Collider2D[] colisoresDono =
                dono.GetComponentsInChildren<Collider2D>();

            Collider2D[] colisoresBola =
                GetComponentsInChildren<Collider2D>();

            foreach (Collider2D colisorDono in colisoresDono)
            {
                foreach (Collider2D colisorBola in colisoresBola)
                {
                    if (colisorDono != null && colisorBola != null)
                    {
                        Physics2D.IgnoreCollision(
                            colisorBola,
                            colisorDono,
                            true
                        );
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (!configurada)
            return;

        transform.Translate(
            direcao * velocidade * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Não deixa a bola atingir o próprio esqueleto.
        if (dono != null &&
            (other.transform == dono ||
             other.transform.IsChildOf(dono)))
        {
            return;
        }

        // Verifica se atingiu o jogador.
        PlayerHealth jogador =
            other.GetComponentInParent<PlayerHealth>();

        if (jogador != null)
        {
            jogador.TakeDamage(dano);

            Debug.Log("BOLA DE FOGO: jogador atingido!");

            Destroy(gameObject);
            return;
        }

        // Destrói ao atingir obstáculos sólidos.
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
