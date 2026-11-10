
using System.Collections;
using UnityEngine;
using DragonBones;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CobraMiniboss : MonoBehaviour
{
    [Header("Vida da cobra")]
    public int vidaMaxima = 3;
    public int vidaAtual;

    [Header("Perseguir o jogador")]
    public float velocidade = 4f;
    public float distanciaParada = 0.8f;
    public string tagPlayer = "Player";

    [Tooltip("Marque se a cobra olha para a direita na imagem original.")]
    public bool spriteOlhaParaDireita = true;

    [Header("Dano no jogador")]
    public int dano = 1;
    public float intervaloEntreDanos = 1f;

    [Header("DragonBones")]
    public UnityArmatureComponent armature;
    public string animacaoAndando = "andando";

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somAtaque;
    public AudioClip somMorte;

    [Header("Efeito de morte")]
    public float duracaoPiscadas = 1.2f;
    public float intervaloPiscada = 0.08f;
    public float intensidadeTremida = 0.06f;
    public float velocidadeTremida = 30f;

    private UnityEngine.Transform jogador;
    private Rigidbody2D rb;
    private Collider2D[] colisores;
    private Renderer[] renderizadores;

    private float proximoDano;
    private bool morta;
    private Vector2 movimento;
    private Vector3 escalaInicial;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        colisores = GetComponentsInChildren<Collider2D>();
        renderizadores = GetComponentsInChildren<Renderer>();

        escalaInicial = transform.localScale;
        vidaAtual = vidaMaxima;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (armature == null)
            armature = GetComponentInChildren<UnityArmatureComponent>(true);
    }

    private void Start()
    {
        GameObject objetoPlayer =
            GameObject.FindGameObjectWithTag(tagPlayer);

        if (objetoPlayer != null)
            jogador = objetoPlayer.transform;

        if (jogador == null)
            Debug.LogError("Cobra: jogador não encontrado! Confira a tag Player.");

        if (armature == null)
        {
            Debug.LogError(
                "Cobra: UnityArmatureComponent não encontrado!"
            );
        }
        else
        {
            TocarAnimacaoAndando();
        }
    }

    private void Update()
    {
        if (morta || jogador == null)
        {
            movimento = Vector2.zero;
            return;
        }

        Vector2 direcao = jogador.position - transform.position;
        float distancia = direcao.magnitude;

        if (distancia > 0.05f)
        {
            VirarParaJogador(direcao.x);
            direcao.Normalize();
        }

        if (distancia > distanciaParada)
        {
            movimento = direcao * velocidade;
        }
        else
        {
            movimento = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (morta || rb == null || !rb.simulated)
            return;

        rb.MovePosition(
            rb.position + movimento * Time.fixedDeltaTime
        );
    }

    private void VirarParaJogador(float direcaoX)
    {
        if (Mathf.Abs(direcaoX) < 0.01f)
            return;

        Vector3 escala = escalaInicial;

        bool olhandoParaDireita = direcaoX > 0;

        if (spriteOlhaParaDireita)
        {
            escala.x = Mathf.Abs(escalaInicial.x) *
                (olhandoParaDireita ? 1f : -1f);
        }
        else
        {
            escala.x = Mathf.Abs(escalaInicial.x) *
                (olhandoParaDireita ? -1f : 1f);
        }

        transform.localScale = escala;
    }

    private void TocarAnimacaoAndando()
    {
        if (armature == null)
            return;

        armature.animation.Play(animacaoAndando, 0);
    }

    private void OnCollisionStay2D(Collision2D colisao)
    {
        if (morta || !colisao.gameObject.CompareTag(tagPlayer))
            return;

        if (Time.time < proximoDano)
            return;

        PlayerHealth vidaPlayer =
            colisao.gameObject.GetComponentInParent<PlayerHealth>();

        if (vidaPlayer == null)
            return;

        vidaPlayer.TakeDamage(dano);
        proximoDano = Time.time + intervaloEntreDanos;

        if (audioSource != null && somAtaque != null)
            audioSource.PlayOneShot(somAtaque);

        Debug.Log("Cobra atacou o jogador!");
    }

    public void TakeDamage(int quantidade)
    {
        if (morta || quantidade <= 0)
            return;

        vidaAtual = Mathf.Max(0, vidaAtual - quantidade);

        Debug.Log("Cobra recebeu dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
            StartCoroutine(Morrer());
    }

    private IEnumerator Morrer()
    {
        morta = true;
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

        if (armature != null)
            armature.animation.Stop();

        Vector3 posicaoInicial = transform.position;

        float tempo = 0f;
        float tempoPiscada = 0f;
        bool visivel = true;

        while (tempo < duracaoPiscadas)
        {
            tempo += Time.unscaledDeltaTime;
            tempoPiscada += Time.unscaledDeltaTime;

            float tremorX =
                Mathf.Sin(tempo * velocidadeTremida) *
                intensidadeTremida;

            float tremorY =
                Mathf.Cos(tempo * velocidadeTremida * 1.3f) *
                intensidadeTremida * 0.5f;

            transform.position = posicaoInicial +
                new Vector3(tremorX, tremorY, 0f);

            float escala =
                Mathf.Lerp(1f, 0f, tempo / duracaoPiscadas);

            transform.localScale = escalaInicial * escala;

            if (tempoPiscada >= intervaloPiscada)
            {
                visivel = !visivel;
                tempoPiscada = 0f;

                foreach (Renderer renderizador in renderizadores)
                {
                    if (renderizador != null)
                        renderizador.enabled = visivel;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    public bool EstaMorta()
    {
        return morta;
    }
}
