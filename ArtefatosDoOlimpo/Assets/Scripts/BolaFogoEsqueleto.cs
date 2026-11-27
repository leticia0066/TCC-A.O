
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

    private bool jaAtingiu;

    public void ConfigurarDirecao(Vector2 novaDirecao, Transform novoDono)
    {
        direcao = novaDirecao.normalized;
        dono = novoDono;
    }

    private void Start()
    {
        Destroy(gameObject, tempoVida);

        Debug.Log("BOLA DE FOGO CRIADA!");
    }

    private void Update()
    {
        transform.Translate(
            direcao * velocidade * Time.deltaTime,
            Space.World
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaAtingiu)
            return;

        Debug.Log("BOLA DE FOGO COLIDIU COM: " + other.name);

        if (dono != null &&
            (other.transform == dono ||
             other.transform.IsChildOf(dono)))
        {
            return;
        }

        PlayerHealth player =
            other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            jaAtingiu = true;

            Debug.Log("PLAYER ENCONTRADO! APLICANDO DANO.");

            player.TakeDamage(dano);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground") ||
            other.CompareTag("Platform"))
        {
            Destroy(gameObject);
        }
    }
}
