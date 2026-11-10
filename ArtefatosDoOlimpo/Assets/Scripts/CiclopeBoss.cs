
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CiclopeBoss : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public float distanciaParada = 2f;

    [Header("Ataque com taco")]
    public int danoAtaque = 1;
    public float distanciaAtaque = 2.5f;
    public float tempoEntreAtaques = 2f;
    public float tempoParaGolpe = 0.25f;
    public float duracaoAtaque = 0.7f;

    [Header("Ponto de ataque")]
    public Transform attackPoint;
    public float raioAtaque = 0.8f;
    public LayerMask camadaJogador;

    [Header("Vida")]
    public int vidaMaxima = 30;
    public Slider barraVida;
    public Image fillImage;

    [Header("Animacao")]
    public string parametroAtaque = "Attack";
    public string parametroMorte = "Die";

    [Header("Animacao de morte")]
    public float tempoAnimacaoMorte = 1.2f;

    [Header("Efeito de morte")]
    public float duracaoPiscadas = 1.2f;
    public float intervaloPiscada = 0.08f;
    public float intensidadeTremida = 0.04f;
    public float velocidadeTremida = 25f;

    private int vidaAtual;
    private Transform jogador;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer[] sprites;
    private Collider2D[] colisores;
    private SpriteRenderer spritePrincipal;

    private bool bossAtivado;
    private bool morto;
    private bool atacando;

    private float proximoAtaque;
    private Vector3 escalaInicial;
    private Vector3 posicaoOriginalAttackPoint;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        sprites = GetComponentsInChildren<SpriteRenderer>();
        colisores = GetComponentsInChildren<Collider2D>();

        spritePrincipal = GetComponentInChildren<SpriteRenderer>();

        escalaInicial = transform.localScale;

        if (attackPoint != null)
            posicaoOriginalAttackPoint = attackPoint.localPosition;
    }

    private void Start()
    {
        vidaAtual = vidaMaxima;

        GameObject objetoJogador =
            GameObject.FindGameObjectWithTag("Player");

        if (objetoJogador != null)
            jogador = objetoJogador.transform;

        if (barraVida != null)
        {
            barraVida.minValue = 0;
            barraVida.maxValue = vidaMaxima;
            barraVida.value = vidaAtual;
        }

        AtualizarCorBarra();
    }

    private void Update()
    {
        if (morto || !bossAtivado || jogador == null)
            return;

        float distanciaHorizontal = Mathf.Abs(
            jogador.position.x - transform.position.x
        );

        VirarParaJogador();

        if (atacando)
            return;

        if (distanciaHorizontal <= distanciaAtaque)
        {
            PararMovimento();

            if (Time.time >= proximoAtaque)
                StartCoroutine(ExecutarAtaque());

            return;
        }

        MoverParaJogador();
    }

    public void AtivarBoss()
    {
        if (!morto)
        {
            bossAtivado = true;
            Debug.Log("CICLOPE ATIVADO!");
        }
    }

    private void MoverParaJogador()
    {
        if (rb == null)
            return;

        float direcao = Mathf.Sign(
            jogador.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(
            direcao * velocidade,
            rb.linearVelocity.y
        );
    }

    private void PararMovimento()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    private void VirarParaJogador()
    {
        if (jogador == null)
            return;

        float diferenca =
            jogador.position.x - transform.position.x;

        if (Mathf.Abs(diferenca) < 0.01f)
            return;

        bool olhandoParaEsquerda = diferenca < 0;

        if (spritePrincipal != null)
            spritePrincipal.flipX = olhandoParaEsquerda;

        if (attackPoint != null)
        {
            float direcao = olhandoParaEsquerda ? -1f : 1f;

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
        proximoAtaque = Time.time + tempoEntreAtaques;

        PararMovimento();

        if (anim != null)
            anim.SetTrigger(parametroAtaque);

        yield return new WaitForSeconds(tempoParaGolpe);

        if (!morto)
            AplicarDano();

        float restante = Mathf.Max(
            0f,
            duracaoAtaque - tempoParaGolpe
        );

        yield return new WaitForSeconds(restante);

        if (!morto)
            atacando = false;
    }

    private void AplicarDano()
    {
        if (attackPoint == null)
            return;

        Collider2D[] atingidos = Physics2D.OverlapCircleAll(
            attackPoint.position,
            raioAtaque,
            camadaJogador
        );

        foreach (Collider2D alvo in atingidos)
        {
            PlayerHealth vida =
                alvo.GetComponentInParent<PlayerHealth>();

            if (vida != null)
                vida.TakeDamage(danoAtaque);
        }
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
        }
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

    private IEnumerator Morrer()
    {
        morto = true;
        bossAtivado = false;
        atacando = false;

        // Cancela o movimento e impede novos ataques.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // Desativa todos os colisores.
        foreach (Collider2D colisor in colisores)
        {
            if (colisor != null)
                colisor.enabled = false;
        }

        // Esconde a barra de vida.
        if (barraVida != null)
            barraVida.gameObject.SetActive(false);

        // Dispara a animacao de morte.
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
        {
            yield return new WaitForSecondsRealtime(
                tempoAnimacaoMorte
            );
        }

        // Desativa o Animator antes do efeito de desaparecimento.
        if (anim != null)
            anim.enabled = false;

        Vector3 posicaoInicial = transform.position;

        float tempo = 0f;
        float tempoPiscada = 0f;

        bool visivel = true;

        // Tremida, piscadas e diminuicao de tamanho.
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

        // Garante que o Ciclope desapareca.
        Destroy(gameObject);
    }

    public bool EstaMorto()
    {
        return morto;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            raioAtaque
        );
    }
}
