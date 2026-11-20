
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EsqueletoMiniboss : MonoBehaviour
{
    [Header("Vida do miniboss")]
    public int vidaMaxima = 3;
    public int vidaAtual;

    [Header("Perseguir o jogador")]
    public float velocidade = 4f;
    public float distanciaAtaque = 0.8f;
    public string tagPlayer = "Player";
    public bool spriteOlhaParaDireita = true;

    [Header("Ataque com fogo")]
    public GameObject prefabBolaFogo;
    public Transform pontoDisparo;
    public float distanciaDisparo = 5f;
    public float intervaloDisparo = 2f;

    [Header("Dano no jogador")]
    public int dano = 1;
    public float intervaloEntreDanos = 1f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somAtaque;
    public AudioClip somMorte;

    [Header("Animacao de morte")]
    public string parametroMorte = "Die";
    public float tempoAnimacaoMorte = 0.5f;

    [Header("Efeito de morte")]
    public float duracaoPiscadas = 1.2f;
    public float intervaloPiscada = 0.08f;
    public float intensidadeTremida = 0.06f;
    public float velocidadeTremida = 30f;

    private Transform jogador;
    private Rigidbody2D rb;
    private SpriteRenderer[] sprites;
    private Animator animator;
    private Collider2D[] colisores;

    private float proximoDano;
    private float proximoDisparo;

    private bool morto;
    private Vector2 movimento;
    private Vector3 escalaInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        sprites = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        colisores = GetComponentsInChildren<Collider2D>();

        escalaInicial = transform.localScale;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        vidaAtual = vidaMaxima;
    }

    private void Start()
    {
        GameObject objetoPlayer =
            GameObject.FindGameObjectWithTag(tagPlayer);

        if (objetoPlayer != null)
            jogador = objetoPlayer.transform;

        if (sprites.Length == 0)
            Debug.LogError("O Esqueleto precisa de um SpriteRenderer!");

        if (jogador == null)
            Debug.LogError("Jogador nao encontrado! Confira a tag Player.");

        if (prefabBolaFogo == null)
            Debug.LogError("Prefab da bola de fogo nao foi atribuido!");

        if (pontoDisparo == null)
            Debug.LogError("PontoDisparo nao foi atribuido!");

        proximoDisparo = Time.time + 1f;
    }

    private void Update()
    {
        if (morto || jogador == null)
        {
            movimento = Vector2.zero;
            return;
        }

        Vector2 direcao = jogador.position - transform.position;
        float distancia = direcao.magnitude;

        // Vira o esqueleto na direcao do jogador.
        if (Mathf.Abs(direcao.x) > 0.05f)
        {
            foreach (SpriteRenderer sr in sprites)
            {
                if (sr == null)
                    continue;

                sr.flipX = spriteOlhaParaDireita
                    ? direcao.x < 0
                    : direcao.x > 0;
            }
        }

        // Ataca com bola de fogo quando o jogador esta no alcance.
        if (prefabBolaFogo != null &&
            pontoDisparo != null &&
            distancia <= distanciaDisparo &&
            distancia > distanciaAtaque)
        {
            movimento = Vector2.zero;

            if (Time.time >= proximoDisparo)
            {
                AtirarBolaFogo(direcao);
                proximoDisparo = Time.time + intervaloDisparo;
            }
        }
        else
        {
            // Persegue o jogador quando esta longe.
            movimento = distancia > distanciaAtaque
                ? direcao.normalized * velocidade
                : Vector2.zero;
        }
    }

    private void AtirarBolaFogo(Vector2 direcao)
    {
        if (prefabBolaFogo == null || pontoDisparo == null)
            return;

        GameObject bola = Instantiate(
            prefabBolaFogo,
            pontoDisparo.position,
            Quaternion.identity
        );

        BolaFogoEsqueleto fogo =
            bola.GetComponent<BolaFogoEsqueleto>();

        if (fogo != null)
        {
            // Informa a direcao e o dono do disparo.
            fogo.ConfigurarDirecao(direcao, transform);
        }
        else
        {
            Debug.LogError(
                "O prefab precisa ter o script BolaFogoEsqueleto!"
            );
        }

        if (audioSource != null && somAtaque != null)
            audioSource.PlayOneShot(somAtaque);

        Debug.Log("ESQUELETO DISPAROU BOLA DE FOGO!");
    }

    private void FixedUpdate()
    {
        if (morto || rb == null || !rb.simulated)
            return;

        rb.MovePosition(
            rb.position + movimento * Time.fixedDeltaTime
        );
    }

    private void OnCollisionStay2D(Collision2D colisao)
    {
        if (morto || !colisao.gameObject.CompareTag(tagPlayer))
            return;

        if (Time.time < proximoDano)
            return;

        PlayerHealth vidaPlayer =
            colisao.gameObject.GetComponentInParent<PlayerHealth>();

        if (vidaPlayer == null)
            return;

        vidaPlayer.TakeDamage(dano);
        proximoDano = Time.time + intervaloEntreDanos;
    }

    public void TakeDamage(int quantidade)
    {
        if (morto || quantidade <= 0)
            return;

        vidaAtual = Mathf.Max(0, vidaAtual - quantidade);

        if (vidaAtual <= 0)
            StartCoroutine(Morrer());
    }

    private IEnumerator Morrer()
    {
        morto = true;
        movimento = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        foreach (Collider2D colisor in colisores)
        {
            if (colisor != null)
                colisor.enabled = false;
        }

        if (audioSource != null && somMorte != null)
            audioSource.PlayOneShot(somMorte);

        bool temAnimacaoMorte = false;

        if (animator != null && animator.isActiveAndEnabled)
        {
            foreach (AnimatorControllerParameter parametro
                     in animator.parameters)
            {
                if (parametro.name == parametroMorte &&
                    parametro.type == AnimatorControllerParameterType.Trigger)
                {
                    temAnimacaoMorte = true;
                    break;
                }
            }

            if (temAnimacaoMorte)
                animator.SetTrigger(parametroMorte);
        }

        if (tempoAnimacaoMorte > 0f)
            yield return new WaitForSecondsRealtime(tempoAnimacaoMorte);

        if (animator != null)
            animator.enabled = false;

        Vector3 posicaoInicial = transform.position;

        float tempo = 0f;
        float tempoPiscada = 0f;
        bool visivel = true;

        while (tempo < duracaoPiscadas)
        {
            tempo += Time.unscaledDeltaTime;
            tempoPiscada += Time.unscaledDeltaTime;

            float tremorX =
                Mathf.Sin(tempo * velocidadeTremida)
                * intensidadeTremida;

            float tremorY =
                Mathf.Cos(tempo * velocidadeTremida * 1.3f)
                * intensidadeTremida * 0.5f;

            transform.position = posicaoInicial +
                new Vector3(tremorX, tremorY, 0f);

            float escala =
                Mathf.Lerp(1f, 0f, tempo / duracaoPiscadas);

            transform.localScale = escalaInicial * escala;

            if (tempoPiscada >= intervaloPiscada)
            {
                visivel = !visivel;
                tempoPiscada = 0f;

                foreach (SpriteRenderer sr in sprites)
                {
                    if (sr != null)
                        sr.enabled = visivel;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public bool EstaMorto()
    {
        return morto;
    }
}
