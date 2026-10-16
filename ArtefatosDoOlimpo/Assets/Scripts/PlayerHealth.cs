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
            return;
        }

        lives--;

        if (lives > 0)
        {
            StartCoroutine(RespawnToCheckpoint());
        }
        else
        {
            StartCoroutine(GameOver());
        }
    }

    void UpdateUI()
    {
        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHealth);
        }
    }

    IEnumerator RespawnToCheckpoint()
    {
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Die");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        transform.position =
            GameManager.instance.GetSpawnPosition();

        currentHealth = maxHealth;

        UpdateUI();

        yield return new WaitForSeconds(0.2f);

        isDead = false;
    }

    IEnumerator GameOver()
    {
        isDead = true;

        if (anim != null)
            anim.SetTrigger("Die");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        Time.timeScale = 0f;

        GameOverManager.Instance.ShowGameOver();
    }
}