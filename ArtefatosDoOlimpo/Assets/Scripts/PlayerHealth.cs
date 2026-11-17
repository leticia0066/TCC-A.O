
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Vidas")]
    public int maxLives = 3;
    public int lives = 3;

    [Header("Interface")]
    public HeartUI heartUI;

    [Header("Tempo")]
    public float respawnDelay = 0.5f;
    public float gameOverDelay = 0.5f;

    [Header("Efeito ao receber dano")]
    public float duracaoPiscar = 0.45f;
    public float intervaloPiscar = 0.07f;

    private Rigidbody2D rb;
    private SpriteRenderer[] spriteRenderers;
    private bool[] estadosOriginais;

    private Coroutine efeitoDanoCoroutine;

    private bool isDead = false;
    private bool gameOverStarted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (heartUI == null)
        {
            heartUI = FindFirstObjectByType<HeartUI>();
        }

        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>(true);

        estadosOriginais = new bool[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            estadosOriginais[i] = spriteRenderers[i].enabled;
        }
    }

    private void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        maxLives = Mathf.Max(1, maxLives);

        currentHealth = maxHealth;

        // Se veio de um salvamento, recupera as vidas do checkpoint.
        if (GameManager.instance != null &&
            GameManager.instance.TentarConsumirVidasSalvas(
                out int vidasSalvas))
        {
            lives = Mathf.Clamp(vidasSalvas, 1, maxLives);

            Debug.Log("Vidas restauradas do checkpoint: " + lives);
        }
        else
        {
            lives = Mathf.Clamp(lives, 1, maxLives);
        }

        AtualizarInterface();

        Debug.Log("PlayerHealth iniciado. Vida: " +
                  currentHealth + "/" + maxHealth +
                  " | Vidas: " + lives);
    }

    // RECEBER DANO
    public void TakeDamage(int damage)
    {
        if (isDead || gameOverStarted || damage <= 0)
            return;

        IniciarEfeitoDano();

        int vidaAnterior = currentHealth;

        currentHealth = Mathf.Clamp(
            currentHealth - damage,
            0,
            maxHealth
        );

        Debug.Log("DANO RECEBIDO! Vida: " +
                  vidaAnterior + " -> " + currentHealth);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarDano();
        }

        AtualizarInterface();

        if (currentHealth > 0)
            return;

        isDead = true;
        lives = Mathf.Max(0, lives - 1);

        Debug.Log("Jogador morreu! Vidas restantes: " + lives);

        if (lives > 0)
        {
            StartCoroutine(RespawnToCheckpoint());
        }
        else
        {
            gameOverStarted = true;
            StartCoroutine(GameOver());
        }
    }

    // EFEITO DE PISCAR
    private void IniciarEfeitoDano()
    {
        if (spriteRenderers == null ||
            spriteRenderers.Length == 0)
        {
            return;
        }

        if (efeitoDanoCoroutine != null)
        {
            StopCoroutine(efeitoDanoCoroutine);
            RestaurarSprites();
        }

        efeitoDanoCoroutine =
            StartCoroutine(PiscarPersonagem());
    }

    private IEnumerator PiscarPersonagem()
    {
        float tempo = 0f;
        bool mostrar = false;

        intervaloPiscar = Mathf.Max(0.02f, intervaloPiscar);

        while (tempo < duracaoPiscar)
        {
            mostrar = !mostrar;

            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                if (spriteRenderers[i] != null)
                {
                    spriteRenderers[i].enabled =
                        estadosOriginais[i] && mostrar;
                }
            }

            yield return new WaitForSeconds(intervaloPiscar);

            tempo += intervaloPiscar;
        }

        RestaurarSprites();

        efeitoDanoCoroutine = null;
    }

    // RESTAURAR APARÊNCIA
    private void RestaurarSprites()
    {
        if (spriteRenderers == null ||
            estadosOriginais == null)
        {
            return;
        }

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
            {
                spriteRenderers[i].enabled =
                    estadosOriginais[i];
            }
        }
    }

    // RECUPERAR VIDA
    public void RecuperarVida(int quantidade)
    {
        if (isDead || gameOverStarted || quantidade <= 0)
            return;

        if (currentHealth >= maxHealth)
            return;

        int vidaAnterior = currentHealth;

        currentHealth = Mathf.Clamp(
            currentHealth + quantidade,
            0,
            maxHealth
        );

        if (currentHealth > vidaAnterior)
        {
            AtualizarInterface();

            Debug.Log("VIDA RECUPERADA! Vida: " +
                      vidaAnterior + " -> " + currentHealth);
        }
    }

    // ATUALIZAR CORAÇÕES
    private void AtualizarInterface()
    {
        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHealth);
        }
        else
        {
            Debug.LogWarning(
                "PlayerHealth: HeartUI não foi encontrado."
            );
        }
    }

    // VOLTAR AO CHECKPOINT
    private IEnumerator RespawnToCheckpoint()
    {
        PararMovimento();

        yield return new WaitForSeconds(respawnDelay);

        if (GameManager.instance != null)
        {
            transform.position =
                GameManager.instance.GetSpawnPosition();
        }
        else
        {
            Debug.LogError("GameManager não encontrado!");
        }

        currentHealth = maxHealth;
        AtualizarInterface();

        PararMovimento();

        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

    // GAME OVER
    private IEnumerator GameOver()
    {
        PararMovimento();

        yield return new WaitForSeconds(gameOverDelay);

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver();
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError("GameOverManager não encontrado!");

            gameOverStarted = false;
            isDead = false;
        }
    }

    // PARAR MOVIMENTO
    private void PararMovimento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // REINICIAR VIDAS
    public void ResetarVidas()
    {
        StopAllCoroutines();

        efeitoDanoCoroutine = null;
        RestaurarSprites();

        Time.timeScale = 1f;

        maxHealth = Mathf.Max(1, maxHealth);
        maxLives = Mathf.Max(1, maxLives);

        currentHealth = maxHealth;
        lives = maxLives;

        isDead = false;
        gameOverStarted = false;

        PararMovimento();
        AtualizarInterface();

        Debug.Log("Vidas reiniciadas. Vida: " +
                  currentHealth + "/" + maxHealth +
                  " | Vidas restantes: " + lives);
    }
}
