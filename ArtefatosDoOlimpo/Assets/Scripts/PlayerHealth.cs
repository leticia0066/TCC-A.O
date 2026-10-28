
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

    private Rigidbody2D rb;

    private bool isDead = false;
    private bool gameOverStarted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (heartUI == null)
        {
            heartUI = FindFirstObjectByType<HeartUI>();
        }
    }

    private void Start()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        maxLives = Mathf.Max(1, maxLives);

        currentHealth = maxHealth;
        lives = Mathf.Clamp(lives, 1, maxLives);

        AtualizarInterface();
    }

    public void TakeDamage(int damage)
    {
        // Não recebe dano durante a morte ou o Game Over.
        if (isDead || gameOverStarted || damage <= 0)
        {
            return;
        }

        // Reduz a vida sem permitir valores negativos.
        currentHealth = Mathf.Clamp(
            currentHealth - damage,
            0,
            maxHealth
        );

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarDano();
        }

        AtualizarInterface();

        // Se ainda tem vida, continua jogando.
        if (currentHealth > 0)
        {
            return;
        }

        // A vida chegou a zero.
        isDead = true;

        // Perde uma tentativa.
        lives = Mathf.Max(0, lives - 1);

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

    private void AtualizarInterface()
    {
        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHealth);
        }
        else
        {
            Debug.LogWarning(
                "PlayerHealth: HeartUI não foi encontrado. " +
                "Confira o campo Heart UI no Inspector."
            );
        }
    }

    private IEnumerator RespawnToCheckpoint()
    {
        PararMovimento();

        // Aguarda antes de retornar ao checkpoint.
        yield return new WaitForSeconds(respawnDelay);

        if (GameManager.instance != null)
        {
            transform.position =
                GameManager.instance.GetSpawnPosition();
        }
        else
        {
            Debug.LogError(
                "PlayerHealth: GameManager.instance não foi encontrado!"
            );
        }

        // Restaura toda a vida.
        currentHealth = maxHealth;

        // Atualiza os três corações.
        AtualizarInterface();

        PararMovimento();

        // Pequena espera antes de permitir receber dano novamente.
        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

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
            Debug.LogError(
                "PlayerHealth: GameOverManager.Instance não foi encontrado! " +
                "Confira o GameOverManager e a referência do painel."
            );

            // Evita manter o jogo pausado sem um painel.
            gameOverStarted = false;
            isDead = false;
        }
    }

    private void PararMovimento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void ResetarVidas()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;

        maxHealth = Mathf.Max(1, maxHealth);
        maxLives = Mathf.Max(1, maxLives);

        currentHealth = maxHealth;
        lives = maxLives;

        isDead = false;
        gameOverStarted = false;

        PararMovimento();
        AtualizarInterface();
    }
}