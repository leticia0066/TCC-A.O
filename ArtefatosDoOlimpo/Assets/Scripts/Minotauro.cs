using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minotauro : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;

    [Header("Ataque")]
    public float distanciaAtaque = 5f;
    public float tempoEntreAtaques = 2f;

    public Transform attackPoint;
    public GameObject machadoPrefab;

    [Header("Vida")]
    public int vidaMaxima = 20;

    public Slider barraVida;

    private int vidaAtual;

    private Transform player;
    private Animator anim;

    private bool bossAtivado = false;
    private bool morto = false;

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

        if (!bossAtivado)
            return;

        if (player == null)
            return;

        SeguirPlayer();
    }

    public void AtivarBoss()
    {
        bossAtivado = true;
    }

    void SeguirPlayer()
    {
        float distancia =
            Vector2.Distance(
                transform.position,
                player.position
            );

        Virar(player.position.x);

        if (distancia > 2f)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    velocidade * Time.deltaTime
                );
        }

        if (
            distancia <= distanciaAtaque
            && Time.time >= proximoAtaque
        )
        {
            proximoAtaque =
                Time.time + tempoEntreAtaques;

            anim.SetTrigger("Attack");

            Invoke(
                nameof(LancarMachado),
                0.35f
            );
        }
    }

    void LancarMachado()
    {
        if (machadoPrefab == null)
            return;

        GameObject machado =
            Instantiate(
                machadoPrefab,
                attackPoint.position,
                Quaternion.identity
            );

        Vector2 direcao =
            transform.localScale.x > 0
            ? Vector2.right
            : Vector2.left;

        machado
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

        transform.position = posOriginal;
    }

    void Morrer()
    {
        morto = true;

        anim.SetTrigger("Die");

        Destroy(
            gameObject,
            2f
        );
    }
}