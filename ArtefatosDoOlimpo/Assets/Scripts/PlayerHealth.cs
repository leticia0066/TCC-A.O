
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Vidas")]
    public int lives = 3;

    [Header("UI")]
    public HeartUI heartUI;

    private Rigidbody2D rb;
    private Animator anim;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (heartUI == null)
            heartUI = FindFirstObjectByType<HeartUI>();

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarDano();
        }

        UpdateUI();

        if (currentHealth > 0)
        {
            StartCoroutine(RespawnAfterHit());
            return;
        }

        lives--;

        if (lives > 0)
        {
            StartCoroutine(RespawnToCheckpoint());
        }
        else
        {
            StartCoroutine(RespawnToStart());
        }
    }

    void UpdateUI()
    {
        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHealth);
        }
    }

    IEnumerator RespawnAfterHit()
    {
        isDead = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.1f);

        if (GameManager.instance != null)
        {
            transform.position =
                GameManager.instance.GetSpawnPosition();
        }

        RespawnCoins();

        yield return new WaitForSeconds(0.1f);

        isDead = false;
    }

    IEnumerator RespawnToCheckpoint()
    {
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Die");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        if (GameManager.instance != null)
        {
            transform.position =
                GameManager.instance.GetSpawnPosition();
        }

        RespawnCoins();

        currentHealth = maxHealth;

        UpdateUI();

        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

    IEnumerator RespawnToStart()
    {
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Die");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.8f);

        lives = 3;
        currentHealth = maxHealth;

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetToStart();

            transform.position =
                GameManager.instance.GetSpawnPosition();
        }

        RespawnCoins();

        UpdateUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarRetornoInicio();
        }

        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

    void RespawnCoins()
    {
        CoinRespawner respawner =
            FindFirstObjectByType<CoinRespawner>();

        if (respawner != null)
        {
            respawner.RespawnAllCoins();
        }
        else
        {
            Debug.LogWarning(
                "CoinRespawner não encontrado na cena!"
            );
        }
    }
}
