
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

    [Header("UI")]
    public HeartUI heartUI;

    [Header("Tempo")]
    public float respawnDelay = 0.5f;
    public float gameOverDelay = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;
    private bool gameOverStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (heartUI == null)
            heartUI = FindFirstObjectByType<HeartUI>();
    }

    void Start()
    {
        // Garante valores iniciais válidos.
        maxHealth = Mathf.Max(1, maxHealth);
        maxLives = Mathf.Max(1, maxLives);

        currentHealth = maxHealth;
        lives = Mathf.Clamp(lives, 1, maxLives);

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        // Impede dano repetido durante morte ou Game Over.
        if (isDead || gameOverStarted || damage <= 0)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarDano();

        UpdateUI();

        // Ainda tem corações: continua jogando.
        if (currentHealth > 0)
            return;

        // A vida chegou a zero: começa a morrer.
        isDead = true;

        // Só perde uma vida quando todos os corações acabam.
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

    void UpdateUI()
    {
        if (heartUI != null)
            heartUI.UpdateHearts(currentHealth);
    }

    IEnumerator RespawnToCheckpoint()
    {
        PararMovimento();

        // Não usa SetTrigger("Die"), pois esse parâmetro
        // não existe no Animator atual do personagem.

        yield return new WaitForSeconds(respawnDelay);

        if (GameManager.instance != null)
        {
            transform.position =
                GameManager.instance.GetSpawnPosition();
        }
        else
        {
            Debug.LogError(
                "GameManager não encontrado! Confira se existe na cena."
            );
        }

        // Recupera os corações, mas NÃO recupera a vida perdida.
        currentHealth = maxHealth;
        UpdateUI();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

    IEnumerator GameOver()
    {
        PararMovimento();

        // Não teleporta o jogador para o checkpoint.
        yield return new WaitForSeconds(gameOverDelay);

        if (GameOverManager.Instance != null)
        {
            // Primeiro mostra o painel.
            GameOverManager.Instance.ShowGameOver();

            // Depois pausa o jogo.
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogError(
                "GameOverManager.Instance está vazio! " +
                "Confira o GameOverManager e o painel na cena."
            );

            // Evita deixar o jogo travado sem painel.
            isDead = false;
            gameOverStarted = false;
        }
    }

    void PararMovimento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    // Permite recuperar o jogador caso você precise reiniciar
    // os atributos por um botão ou outro sistema.
    public void ResetarVidas()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;

        currentHealth = maxHealth;
        lives = maxLives;

        isDead = false;
        gameOverStarted = false;

        UpdateUI();
    }
}
