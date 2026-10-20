using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    public int vidaMaxima = 15;
    private int vidaAtual;

    public Slider barraVida;

    private Transform player;
    private Animator anim;

    private bool indoParaB = true;
    private bool morto;

    private float proximoAtaque;

    void Start()
    {
        anim = GetComponent<Animator>();

        vidaAtual = vidaMaxima;

        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaMaxima;
        }

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
            transform.localScale.x > 0
            ? Vector2.right
            : Vector2.left;

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
                alvoX -
                transform.position.x
            );

        transform.localScale = escala;
    }

    public void TakeDamage(int dano)
    {
        if (morto)
            return;

        vidaAtual -= dano;

        if (barraVida != null)
        {
            barraVida.value = vidaAtual;
        }

        StartCoroutine(Recuo());

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    IEnumerator Recuo()
    {
        Vector3 posOriginal =
            transform.position;

        Vector3 direcao =
            player.position.x > transform.position.x
            ? Vector3.left
            : Vector3.right;

        transform.position += direcao * 0.15f;

        yield return new WaitForSeconds(0.05f);

        transform.position =
            posOriginal;
    }

    void Morrer()
    {
        morto = true;

        anim.SetTrigger("Die");

        Destroy(gameObject, 2f);
    }
}