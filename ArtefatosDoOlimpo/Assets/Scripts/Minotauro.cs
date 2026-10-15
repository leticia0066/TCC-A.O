using UnityEngine;

public class Minotauro : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pontoA;
    public Transform pontoB;
    public float velocidade = 2f;

    [Header("Ataque")]
    public float distanciaDeteccao = 6f;
    public float tempoEntreAtaques = 2f;

    public Transform attackPoint;
    public GameObject machadoPrefab;

    [Header("Vida")]
    public int vida = 3;

    private Transform player;
    private Animator anim;

    private bool indoParaB = true;
    private bool morto;

    private float proximoAtaque;

    void Start()
    {
        anim = GetComponent<Animator>();

        GameObject p =
            GameObject.FindGameObjectWithTag("Player");

        if (p != null)
        {
            player = p.transform;
        }
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
            Atacar();
        }
        else
        {
            Patrulhar();
        }
    }

    void Patrulhar()
    {
        Transform destino =
            indoParaB ? pontoB : pontoA;

        transform.position =
            Vector2.MoveTowards(
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

    void Atacar()
    {
        Virar(player.position.x);

        if (Time.time >= proximoAtaque)
        {
            proximoAtaque =
                Time.time + tempoEntreAtaques;

            anim.SetTrigger("Attack");

            Invoke(
                nameof(LancarMachado),
                0.4f
            );
        }
    }

    void LancarMachado()
    {
        GameObject novoMachado =
            Instantiate(
                machadoPrefab,
                attackPoint.position,
                Quaternion.identity
            );

        Vector2 direcao =
            (
                player.position -
                attackPoint.position
            ).normalized;

        novoMachado
            .GetComponent<Machado>()
            .SetDirection(direcao);
    }

    void Virar(float alvoX)
    {
        Vector3 escala =
            transform.localScale;

        escala.x =
            Mathf.Abs(escala.x)
            *
            Mathf.Sign(
                alvoX
                -
                transform.position.x
            );

        transform.localScale = escala;
    }

    public void TakeDamage(int dano)
    {
        if (morto)
            return;

        vida -= dano;

        if (vida <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        morto = true;

        anim.SetTrigger("Die");

        Destroy(gameObject, 1.5f);
    }
}