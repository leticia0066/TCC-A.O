
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Minotauro : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaParada = 1f;

    [Header("Ataque")]
    public float distanciaAtaque = 5f;
    public float tempoEntreAtaques = 2f;
    public float tempoParaLancarMachado = 0.35f;

    [Header("Machado")]
    public Transform attackPoint;
    public GameObject machadoPrefab;
    public bool spriteOlhaParaDireita = true;

    [Header("Animacoes")]
    public string estadoAndar = "Andar";
    public string parametroAtaque = "Attack";
    public string parametroMorte = "Die";

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip somAtaque;
    public AudioClip somMorte;

    [Header("Vida")]
    public int vidaMaxima = 20;
    public Slider barraVida;
    public Image fillImage;

    [Header("Animacao de morte")]
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

    private SpriteRenderer spritePrincipal;
    private SpriteRenderer[] sprites;
    private Collider2D[] colisores;

    private bool bossAtivado;
    private bool morto;
    private bool atacando;
    private bool olhandoParaDireita;

    private float proximoAtaque;
    private float movimentoHorizontal;

    private Vector3 escalaInicial;
    private Vector3 posicaoOriginalAttackPoint;

    private Coroutine rotinaAtaque;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponentInChildren<Animator>();

        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        colisores = GetComponentsInChildren<Collider2D>();

        spritePrincipal = GetComponentInChildren<SpriteRenderer>(true);

        escalaInicial = transform.localScale;

        olhandoParaDireita = spriteOlhaParaDireita;

        if (attackPoint != null)
            posicaoOriginalAttackPoint = attackPoint.localPosition;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        vidaAtual = vidaMaxima;

        if (rb == null)
        {
            Debug.LogError("MINOTAURO: Rigidbody2D não encontrado!");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
        }

        if (barraVida != null)
        {
            barraVida.minValue = 0;
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaAtual;
        }

        AtualizarCorBarra();

        EncontrarPlayer();

        if (attackPoint == null)
            Debug.LogError("MINOTAURO: AttackPoint não configurado!");

        if (machadoPrefab == null)
            Debug.LogError("MINOTAURO: Machado Prefab não configurado!");

        if (anim == null)
            Debug.LogError("MINOTAURO: Animator não encontrado!");
    }

    private void Update()
    {
        if (morto)
            return;

        if (player == null)
            EncontrarPlayer();

        if (!bossAtivado || player == null)
        {
            movimentoHorizontal = 0f;
            AtualizarAnimacaoAndar(false);
            return;
        }

        VerificarDistanciaEAtacar();
    }

    private void FixedUpdate()
    {
        if (rb == null || morto)
            return;

        if (atacando || !bossAtivado)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );

            return;
        }

        rb.linearVelocity = new Vector2(
            movimentoHorizontal * velocidade,
            rb.linearVelocity.y
        );
    }

    private void EncontrarPlayer()
    {
        GameObject objetoPlayer =
            GameObject.FindGameObjectWithTag("Player");

        if (objetoPlayer != null)
            player = objetoPlayer.transform;
    }

    public void AtivarBoss()
    {
        if (morto)
            return;

        bossAtivado = true;

        Debug.Log("MINOTAURO ATIVADO!");
    }

    private void VerificarDistanciaEAtacar()
    {
        float diferencaX =
            player.position.x - transform.position.x;

        float distanciaHorizontal = Mathf.Abs(diferencaX);

        VirarParaJogador();

        if (atacando)
        {
            movimentoHorizontal = 0f;
            AtualizarAnimacaoAndar(false);
            return;
        }

        if (distanciaHorizontal <= distanciaAtaque)
        {
            movimentoHorizontal = 0f;
            AtualizarAnimacaoAndar(false);

            if (Time.time >= proximoAtaque)
            {
                Debug.Log("MINOTAURO VAI ATACAR!");

                rotinaAtaque = StartCoroutine(ExecutarAtaque());
            }

            return;
        }

        if (distanciaHorizontal > distanciaParada)
        {
            movimentoHorizontal = Mathf.Sign(diferencaX);
            AtualizarAnimacaoAndar(true);
        }
        else
        {
            movimentoHorizontal = 0f;
            AtualizarAnimacaoAndar(false);
        }
    }

    private void VirarParaJogador()
    {
        if (player == null)
            return;

        float diferenca =
            player.position.x - transform.position.x;

        if (Mathf.Abs(diferenca) < 0.01f)
            return;

        olhandoParaDireita = diferenca > 0f;

        if (spritePrincipal != null)
        {
            spritePrincipal.flipX =
                olhandoParaDireita != spriteOlhaParaDireita;
        }

        if (attackPoint != null)
        {
            float direcao = olhandoParaDireita ? 1f : -1f;

            attackPoint.localPosition = new Vector3(
                Mathf.Abs(posicaoOriginalAttackPoint.x) * direcao,
                posicaoOriginalAttackPoint.y,
                posicaoOriginalAttackPoint.z
            );
        }
    }

    private IEnumerator ExecutarAtaque()
    {
        atacando = true;
        movimentoHorizontal = 0f;

        proximoAtaque = Time.time + tempoEntreAtaques;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }

        if (anim != null)
        {
            anim.speed = 1f;

            if (PossuiParametro(
                parametroAtaque,
                AnimatorControllerParameterType.Trigger))
            {
                anim.SetTrigger(parametroAtaque);
            }
            else
            {
                Debug.LogError(
                    "Animator sem Trigger: " + parametroAtaque
                );
            }
        }

        if (audioSource != null && somAtaque != null)
            audioSource.PlayOneShot(somAtaque);

        Debug.Log("MINOTAURO INICIOU O ATAQUE!");

        yield return new WaitForSeconds(tempoParaLancarMachado);

        if (!morto)
            LancarMachado();

        yield return new WaitForSeconds(
            Mathf.Max(0f, tempoEntreAtaques - tempoParaLancarMachado)
        );

        if (!morto)
        {
            atacando = false;

            if (anim != null)
            {
                anim.speed = 1f;

                if (anim.HasState(
                    0,
                    Animator.StringToHash(estadoAndar)))
                {
                    anim.Play(estadoAndar, 0, 0f);
                }
            }

            rotinaAtaque = null;
        }
    }

    private void LancarMachado()
    {
        if (morto || player == null)
            return;

        if (machadoPrefab == null || attackPoint == null)
        {
            Debug.LogError("Não foi possível lançar o machado!");
            return;
        }

        GameObject machado = Instantiate(
            machadoPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        Vector2 direcao = (
            (Vector2)player.position -
            (Vector2)attackPoint.position
        ).normalized;

        Machado scriptMachado = machado.GetComponent<Machado>();

        if (scriptMachado != null)
        {
            scriptMachado.SetDirection(direcao);

            Debug.Log("MINOTAURO LANÇOU O MACHADO!");
        }
        else
        {
            Debug.LogError("O prefab não possui o script Machado!");
            Destroy(machado);
        }
    }

    private void AtualizarAnimacaoAndar(bool andando)
    {
        if (anim == null || morto || atacando)
            return;

        if (andando)
        {
            anim.speed = 1f;

            if (anim.HasState(
                0,
                Animator.StringToHash(estadoAndar)))
            {
                AnimatorStateInfo estado = anim.GetCurrentAnimatorStateInfo(0);

                if (!estado.IsName(estadoAndar))
                    anim.Play(estadoAndar);
            }
        }
        else
        {
            // Não existe animação Idle.
            // Pausamos a animação de caminhada enquanto o Minotauro está parado.
            anim.speed = 0f;
        }
    }

    private bool PossuiParametro(
        string nome,
        AnimatorControllerParameterType tipo)
    {
        if (anim == null)
            return false;

        foreach (AnimatorControllerParameter parametro in anim.parameters)
        {
            if (parametro.name == nome && parametro.type == tipo)
                return true;
        }

        return false;
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
        if (rb == null)
            yield break;

        float direcao =
            player != null && player.position.x > transform.position.x
            ? -1f
            : 1f;

        rb.linearVelocity = new Vector2(
            direcao * 3f,
            rb.linearVelocity.y
        );

        yield return new WaitForSeconds(0.05f);

        if (!morto && rb != null && !atacando)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    private IEnumerator Morrer()
    {
        morto = true;
        bossAtivado = false;
        atacando = false;
        movimentoHorizontal = 0f;

        if (rotinaAtaque != null)
            StopCoroutine(rotinaAtaque);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        foreach (Collider2D colisor in colisores)
        {
            if (colisor != null)
                colisor.enabled = false;
        }

        if (barraVida != null)
            barraVida.gameObject.SetActive(false);

        if (audioSource != null && somMorte != null)
            audioSource.PlayOneShot(somMorte);

        if (anim != null)
        {
            anim.speed = 1f;

            if (PossuiParametro(
                parametroMorte,
                AnimatorControllerParameterType.Trigger))
            {
                anim.SetTrigger(parametroMorte);
            }
        }

        if (tempoAnimacaoMorte > 0f)
        {
            yield return new WaitForSecondsRealtime(
                tempoAnimacaoMorte
            );
        }

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

            float escala = Mathf.Lerp(
                1f,
                0f,
                tempo / duracaoPiscadas
            );

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaAtaque
        );
    }
}
