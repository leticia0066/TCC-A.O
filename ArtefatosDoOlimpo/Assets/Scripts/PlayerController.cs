
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 6f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("Petrificacao")]
    public bool petrificado;

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;

    private bool isGrounded;
    private bool wasGrounded;
    private bool hasCheckedGround;
    private bool jumpRequested;

    private SpriteRenderer[] sprites;
    private Color[] coresOriginais;

    private RigidbodyConstraints2D restricoesOriginais;
    private Coroutine rotinaPetrificacao;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        coresOriginais = new Color[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            coresOriginais[i] = sprites[i].color;
        }

        if (rb == null)
        {
            Debug.LogError("ERRO: O Player não possui Rigidbody2D!");
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.WakeUp();
        }

        if (anim != null)
        {
            // A física controla o movimento, não a animação.
            anim.applyRootMotion = false;
        }
        else
        {
            Debug.LogError("ERRO: Animator não encontrado!");
        }

        if (groundCheck == null)
            Debug.LogError("ERRO: GroundCheck não foi colocado!");

        if (attackPoint == null)
            Debug.LogError("ERRO: AttackPoint não foi colocado!");
    }

    private void Update()
    {
        if (petrificado)
        {
            moveInput = 0f;
            jumpRequested = false;

            AtualizarAnimacoes(false, false);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PararPassos();

            return;
        }

        // Captura o movimento.
        moveInput = Input.GetAxisRaw("Horizontal");

        CheckGround();

        // Atualiza as animações.
        AtualizarAnimacoes(
            Mathf.Abs(moveInput) > 0.01f && isGrounded,
            !isGrounded
        );

        // Som de aterrissagem.
        if (hasCheckedGround && !wasGrounded && isGrounded)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.TocarAterrissagem();
        }

        wasGrounded = isGrounded;
        hasCheckedGround = true;

        // Guarda o comando de pulo para aplicar na física.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        HandleFootsteps();

        if (Input.GetKeyDown(KeyCode.Z))
            Attack();

        Flip();
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        if (petrificado)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Garante que o Rigidbody continue acordado.
        if (!rb.simulated)
            return;

        rb.WakeUp();

        // Movimento horizontal independente da animação.
        float velocidadeHorizontal = moveInput * speed;

        rb.linearVelocity = new Vector2(
            velocidadeHorizontal,
            rb.linearVelocity.y
        );

        // Executa o pulo na física.
        if (jumpRequested)
        {
            jumpRequested = false;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            if (AudioManager.Instance != null)
                AudioManager.Instance.TocarPulo();
        }
    }

    private void AtualizarAnimacoes(bool andando, bool pulando)
    {
        if (anim == null)
            return;

        anim.SetBool("IsWalking", andando);
        anim.SetBool("IsJumping", pulando);
    }

    private void HandleFootsteps()
    {
        bool moving = Mathf.Abs(moveInput) > 0.01f;

        if (isGrounded && moving && !petrificado)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.IniciarPassos();
        }
        else
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PararPassos();
        }
    }

    private void CheckGround()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        Collider2D ground = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        isGrounded = ground != null;
    }

    private void Flip()
    {
        if (Mathf.Abs(moveInput) < 0.01f || petrificado)
            return;

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput);

        transform.localScale = scale;
    }

    private void Attack()
    {
        if (petrificado)
            return;

        if (anim != null)
            anim.SetTrigger("Attack");

        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarAtaque();

        if (attackPoint == null)
        {
            Debug.LogError("Ataque cancelado: AttackPoint está vazio!");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        Debug.Log(
            "Ataque executado! Objetos encontrados: "
            + hitEnemies.Length
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.transform.root == transform.root)
                continue;

            CiclopeBoss ciclope =
                enemy.GetComponentInParent<CiclopeBoss>();

            if (ciclope != null)
            {
                ciclope.TakeDamage(1);
                Debug.Log("CICLOPE ATINGIDO!");
                continue;
            }

            EsqueletoMiniboss esqueleto =
                enemy.GetComponentInParent<EsqueletoMiniboss>();

            if (esqueleto != null)
            {
                esqueleto.TakeDamage(1);

                Debug.Log(
                    "Esqueleto atingido! Vida restante: "
                    + esqueleto.vidaAtual
                );

                continue;
            }

            CobraMiniboss cobra =
                enemy.GetComponentInParent<CobraMiniboss>();

            if (cobra != null)
            {
                cobra.TakeDamage(1);

                Debug.Log(
                    "COBRA ATINGIDA! Vida restante: "
                    + cobra.vidaAtual
                );

                continue;
            }

            enemy.SendMessageUpwards(
                "TakeDamage",
                1,
                SendMessageOptions.DontRequireReceiver
            );

            Debug.Log("Objeto atingido: " + enemy.name);
        }
    }

    // PETRIFICACAO
    public void Petrificar(float duracao)
    {
        if (petrificado)
            return;

        if (rotinaPetrificacao != null)
            StopCoroutine(rotinaPetrificacao);

        rotinaPetrificacao = StartCoroutine(
            ExecutarPetrificacao(duracao)
        );
    }

    private IEnumerator ExecutarPetrificacao(float duracao)
    {
        petrificado = true;

        moveInput = 0f;
        jumpRequested = false;

        if (rb != null)
        {
            restricoesOriginais = rb.constraints;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Aparencia de pedra.
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
            {
                Color cor = coresOriginais[i];

                sprites[i].color = new Color(
                    cor.r * 0.45f,
                    cor.g * 0.50f,
                    cor.b * 0.55f,
                    cor.a
                );
            }
        }

        AtualizarAnimacoes(false, false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PararPassos();

        Debug.Log("JOGADOR PETRIFICADO!");

        yield return new WaitForSecondsRealtime(duracao);

        // Recupera as cores.
        RestaurarCores();

        if (rb != null)
        {
            rb.constraints = restricoesOriginais;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.WakeUp();
        }

        petrificado = false;
        rotinaPetrificacao = null;

        Debug.Log("PETRIFICACAO TERMINOU! Jogador liberado.");
    }

    private void RestaurarCores()
    {
        if (sprites == null || coresOriginais == null)
            return;

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
                sprites[i].color = coresOriginais[i];
        }
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PararPassos();

        if (petrificado)
        {
            if (rotinaPetrificacao != null)
                StopCoroutine(rotinaPetrificacao);

            RestaurarCores();

            if (rb != null)
            {
                rb.constraints = restricoesOriginais;
                rb.linearVelocity = Vector2.zero;
                rb.WakeUp();
            }

            petrificado = false;
            rotinaPetrificacao = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRange
            );
        }
    }
}
