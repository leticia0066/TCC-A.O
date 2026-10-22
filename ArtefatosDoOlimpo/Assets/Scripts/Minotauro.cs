
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

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somAtaque;
    public AudioClip somMorte;

    [Header("Vida")]
    public int vidaMaxima = 20;
    public Slider barraVida;
    public Image fillImage;

    [Header("Animacao de morte")]
    public string parametroMorte = "Die";

    [Tooltip("Tempo para o Minotauro cair antes de comecar a piscar.")]
    public float tempoAnimacaoMorte = 1.2f;

    [Header("Efeito de morte")]
    public float duracaoPiscadas = 1.2f;
    public float intervaloPiscada = 0.08f;
    public float intensidadeTremida = 0.04f;
    public float velocidadeTremida = 25f;

    private int vidaAtual;
    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer[] sprites;
    private Collider2D[] colisores;

    private bool bossAtivado;
    private bool morto;
    private float proximoAtaque;
    private Vector3 escalaInicial;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        colisores = GetComponentsInChildren<Collider2D>();
        escalaInicial = transform.localScale;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        vidaAtual = vidaMaxima;

        if (barraVida != null)
        {
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaMaxima;
        }

        AtualizarCorBarra();

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    private void Update()
    {
        if (morto || !bossAtivado || player == null)
            return;

        SeguirPlayer();
    }

    public void AtivarBoss()
    {
        if (!morto)
            bossAtivado = true;
    }

    private void SeguirPlayer()
    {
        float distancia =
            Vector2.Distance(transform.position, player.position);

        Virar(player.position.x);

        if (distancia > 2f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                velocidade * Time.deltaTime
            );
        }

        if (distancia <= distanciaAtaque &&
            Time.time >= proximoAtaque)
        {
            proximoAtaque = Time.time + tempoEntreAtaques;

            if (anim != null)
                anim.SetTrigger("Attack");

            // Som do ataque.
            if (audioSource != null && somAtaque != null)
                audioSource.PlayOneShot(somAtaque);

            Invoke(nameof(LancarMachado), 0.35f);
        }
    }

    private void LancarMachado()
    {
        if (morto || machadoPrefab == null || attackPoint == null)
            return;

        GameObject machado = Instantiate(
            machadoPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        Vector2 direcao =
            transform.localScale.x >= 0
            ? Vector2.right
            : Vector2.left;

        Machado scriptMachado = machado.GetComponent<Machado>();

        if (scriptMachado != null)
            scriptMachado.SetDirection(direcao);
    }

    private void Virar(float alvoX)
    {
        float diferenca = alvoX - transform.position.x;

        if (Mathf.Abs(diferenca) < 0.01f)
            return;

        Vector3 escala = transform.localScale;

        escala.x = Mathf.Abs(escala.x) *
            Mathf.Sign(diferenca);

        transform.localScale = escala;
    }

    public void TakeDamage(int dano)
    {
        if (morto || dano <= 0)
            return;

        vidaAtual = Mathf.Max(0, vidaAtual - dano);

        if (barraVida != null)
            barraVida.value = vidaAtual;

        AtualizarCorBarra();

        if (vidaAtual <= 0)
        {
            StartCoroutine(Morrer());
            return;
        }

        if (player != null)
            StartCoroutine(Recuo());
    }

    private void AtualizarCorBarra()
    {
        if (fillImage == null || vidaMaxima <= 0)
            return;

        float porcentagem = (float)vidaAtual / vidaMaxima;

        if (porcentagem > 0.5f)
            fillImage.color = Color.green;
        else if (porcentagem > 0.25f)
            fillImage.color = Color.yellow;
        else
            fillImage.color = Color.red;
    }

    private IEnumerator Recuo()
    {
        Vector3 posOriginal = transform.position;

        Vector3 direcao =
            player != null && player.position.x > transform.position.x
            ? Vector3.left
            : Vector3.right;

        transform.position += direcao * 0.15f;

        yield return new WaitForSeconds(0.05f);

        if (!morto)
            transform.position = posOriginal;
    }

    private IEnumerator Morrer()
    {
        morto = true;
        bossAtivado = false;

        // Cancela qualquer ataque que ainda estivesse agendado.
        CancelInvoke();

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

        // Esconde a barra de vida.
        if (barraVida != null)
            barraVida.gameObject.SetActive(false);

        // Som de morte.
        if (audioSource != null && somMorte != null)
            audioSource.PlayOneShot(somMorte);

        // Dispara a animacao de cair para tras.
        bool temAnimacaoMorte = false;

        if (anim != null && anim.isActiveAndEnabled)
        {
            foreach (AnimatorControllerParameter parametro
                     in anim.parameters)
            {
                if (parametro.name == parametroMorte &&
                    parametro.type == AnimatorControllerParameterType.Trigger)
                {
                    temAnimacaoMorte = true;
                    break;
                }
            }

            if (temAnimacaoMorte)
                anim.SetTrigger(parametroMorte);
        }

        // Espera a animacao de morte terminar.
        if (tempoAnimacaoMorte > 0f)
            yield return new WaitForSecondsRealtime(tempoAnimacaoMorte);

        // So agora desativa a animacao e comeca a piscar.
        if (anim != null)
            anim.enabled = false;

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

            transform.position = posicaoInicial +
                new Vector3(tremorX, 0f, 0f);

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
