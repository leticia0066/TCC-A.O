
using UnityEngine;

public class BolaFogoEsqueleto : MonoBehaviour
{
    public float velocidade = 6f;
    public float tempoVida = 4f;
    public int dano = 1;

    private Vector2 direcao;

    public void ConfigurarDirecao(Vector2 novaDirecao)
    {
        direcao = novaDirecao.normalized;
    }

    private void Update()
    {
        transform.Translate(
            direcao * velocidade * Time.deltaTime,
            Space.World
        );
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth jogador =
            other.GetComponentInParent<PlayerHealth>();

        if (jogador != null)
        {
            jogador.TakeDamage(dano);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
