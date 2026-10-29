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

        Debug.Log("PlayerHealth iniciado. Vida: " +
                  currentHealth + "/" + maxHealth);
    }

    // RECEBER DANO
    public void TakeDamage(int damage)
    {
        if (isDead || gameOverStarted || damage <= 0)
            return;

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

    // RECUPERAR VIDA AO COLETAR UM CORAÇÃO
    public void RecuperarVida(int quantidade)
    {
        if (isDead || gameOverStarted || quantidade <= 0)
            return;

        // Não permite recuperar vida além do máximo.
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

    // ATUALIZAR A INTERFACE DOS CORAÇÕES
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

    // RETORNAR AO CHECKPOINT APÓS MORRER
    private IEnumerator RespawnToCheckpoint()
    {
        PararMovimento();

        yield return new WaitForSeconds(respawnDelay);

        if (GameManager.instance != null)
        {
            transform.position = GameManager.instance.GetSpawnPosition();
        }
        else
        {
            Debug.LogError(
                "PlayerHealth: GameManager.instance não foi encontrado!"
            );
        }

        // Recupera a vida após retornar ao checkpoint.
        currentHealth = maxHealth;
        AtualizarInterface();

        PararMovimento();

        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

    // GAME OVER QUANDO TODAS AS VIDAS ACABAM
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

            gameOverStarted = false;
            isDead = false;
        }
    }

    // PARAR O MOVIMENTO DO JOGADOR
    private void PararMovimento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // REINICIAR VIDA E QUANTIDADE DE VIDAS
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

        Debug.Log("Vidas reiniciadas. Vida: " +
                  currentHealth + "/" + maxHealth +
                  " | Vidas restantes: " + lives);
    }
}