
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

    [Tooltip("Marque se o sprite original olha para a direita.")]
    public bool spriteOlhaParaDireita = true;

    [Header("Dano no jogador")]
    public int dano = 1;
    public float intervaloEntreDanos = 1f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somAtaque;
    public AudioClip somMorte;

    [Header("Animacao de morte")]
    [Tooltip("Nome do Trigger de morte no Animator.")]
    public string parametroMorte = "Die";

    [Tooltip("Tempo para a animacao de morte antes das piscadas.")]
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

        if (distancia > 0.05f)
        {
            direcao.Normalize();

            foreach (SpriteRenderer sr in sprites)
            {
                if (sr == null)
                    continue;

                sr.flipX = spriteOlhaParaDireita
                    ? direcao.x < 0
                    : direcao.x > 0;
            }
        }

        movimento = distancia > distanciaAtaque
            ? direcao * velocidade
            : Vector2.zero;
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

        // Som quando o esqueleto acerta o jogador.
        if (audioSource != null && somAtaque != null)
            audioSource.PlayOneShot(somAtaque);
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

        // Para o movimento e impede novos danos.
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

        // Toca o som de morte.
        if (audioSource != null && somMorte != null)
            audioSource.PlayOneShot(somMorte);

        // Mantem o Animator ativo durante a animacao de morte.
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

        // A animacao acabou: agora comeca o efeito de desaparecer.
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
                Mathf.Sin(tempo * velocidadeTremida) * intensidadeTremida;

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
