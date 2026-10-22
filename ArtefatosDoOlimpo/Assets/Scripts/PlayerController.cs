
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

    private Rigidbody2D rb;
    private Animator anim;

    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private bool hasCheckedGround;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("ERRO: O Player não possui Rigidbody2D!");
        }

        if (groundCheck == null)
        {
            Debug.LogError(
                "ERRO: GroundCheck não foi colocado no PlayerController!"
            );
        }

        if (attackPoint == null)
        {
            Debug.LogError(
                "ERRO: AttackPoint não foi colocado no PlayerController!"
            );
        }
    }

    void Update()
    {
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
            {
                AudioManager.Instance.TocarAterrissagem();
            }
        }

        wasGrounded = isGrounded;
        hasCheckedGround = true;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        HandleFootsteps();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }

        Flip();
    }

    void FixedUpdate()
    {
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
        if (rb == null)
            return;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarPulo();
        }
    }

    void HandleFootsteps()
    {
        bool moving = Mathf.Abs(moveInput) > 0.01f;

        if (isGrounded && moving)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.IniciarPassos();
            }
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PararPassos();
            }
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
        if (moveInput == 0)
            return;

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput);

        transform.localScale = scale;
    }

    void Attack()
    {
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarAtaque();
        }

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

            // Procura o miniboss no collider atingido
            // ou em algum objeto pai.
            EsqueletoMiniboss esqueleto =
                enemy.GetComponentInParent<EsqueletoMiniboss>();

            if (esqueleto != null)
            {
                esqueleto.TakeDamage(1);

                Debug.Log("Esqueleto atingido! Vida restante: "
                    + esqueleto.vidaAtual);

                continue;
            }

            // Mantém compatibilidade com outros inimigos
            // que já utilizam um método chamado TakeDamage.
            enemy.SendMessageUpwards(
                "TakeDamage",
                1,
                SendMessageOptions.DontRequireReceiver
            );

            Debug.Log("Objeto atingido: " + enemy.name);
        }
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PararPassos();
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
