
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

    private SpriteRenderer[] sprites;
    private Color[] coresOriginais;
    private RigidbodyConstraints2D restricoesOriginais;
    private Coroutine rotinaPetrificacao;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        sprites = GetComponentsInChildren<SpriteRenderer>(true);
        coresOriginais = new Color[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            coresOriginais[i] = sprites[i].color;
        }

        if (rb == null)
            Debug.LogError("ERRO: O Player não possui Rigidbody2D!");

        if (groundCheck == null)
            Debug.LogError("ERRO: GroundCheck não foi colocado!");

        if (attackPoint == null)
            Debug.LogError("ERRO: AttackPoint não foi colocado!");
    }

    void Update()
    {
        if (petrificado)
        {
            moveInput = 0f;

            if (anim != null)
            {
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsJumping", false);
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PararPassos();

            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        CheckGround();

        if (anim != null)
        {
            anim.SetBool("IsWalking", Mathf.Abs(moveInput) > 0.01f);
            anim.SetBool("IsJumping", !isGrounded);
        }

        if (hasCheckedGround && !wasGrounded && isGrounded)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.TocarAterrissagem();
        }

        wasGrounded = isGrounded;
        hasCheckedGround = true;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        HandleFootsteps();

        if (Input.GetKeyDown(KeyCode.Z))
            Attack();

        Flip();
    }

    void FixedUpdate()
    {
        if (petrificado)
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            return;
        }

        Move();
    }

    void Move()
    {
        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(
            moveInput * speed,
            rb.linearVelocity.y
        );
    }

    void Jump()
    {
        if (rb == null || petrificado)
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarPulo();
    }

    void HandleFootsteps()
    {
        bool moving = Mathf.Abs(moveInput) > 0.01f;

        if (isGrounded && moving)
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

    void CheckGround()
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

    void Flip()
    {
        if (moveInput == 0 || petrificado)
            return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput);
        transform.localScale = scale;
    }

    void Attack()
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

        Debug.Log("Ataque executado! Objetos encontrados: "
            + hitEnemies.Length);

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

                Debug.Log("Esqueleto atingido! Vida restante: "
                    + esqueleto.vidaAtual);

                continue;
            }

            CobraMiniboss cobra =
                enemy.GetComponentInParent<CobraMiniboss>();

            if (cobra != null)
            {
                cobra.TakeDamage(1);

                Debug.Log("COBRA ATINGIDA! Vida restante: "
                    + cobra.vidaAtual);

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

    // ATIVAR PETRIFICACAO
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

        if (rb != null)
        {
            restricoesOriginais = rb.constraints;

            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // Deixa o personagem com aparencia de pedra.
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

        if (anim != null)
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsJumping", false);
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PararPassos();

        Debug.Log("JOGADOR PETRIFICADO!");

        yield return new WaitForSecondsRealtime(duracao);

        // Recupera a cor original.
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null)
                sprites[i].color = coresOriginais[i];
        }

        if (rb != null)
        {
            rb.constraints = restricoesOriginais;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        petrificado = false;
        rotinaPetrificacao = null;

        Debug.Log("PETRIFICACAO TERMINOU! Jogador liberado.");
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PararPassos();

        if (petrificado)
        {
            if (rotinaPetrificacao != null)
                StopCoroutine(rotinaPetrificacao);

            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null)
                    sprites[i].color = coresOriginais[i];
            }

            if (rb != null)
                rb.constraints = restricoesOriginais;

            petrificado = false;
            rotinaPetrificacao = null;
        }
    }

    void OnDrawGizmosSelected()
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
