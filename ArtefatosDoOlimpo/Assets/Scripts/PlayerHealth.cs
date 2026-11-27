
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Vidas")]
    public int maxLives = 2;
    public int lives;

    [Header("Interface")]
    public HeartUI heartUI;

    [Header("Respawn")]
    public float respawnDelay = 0.5f;
    public float gameOverDelay = 0.5f;

    [Header("Invulnerabilidade")]
    public float tempoInvulnerabilidade = 1f;

    private Rigidbody2D rb;
    private PlayerController controle;

    private bool estaMorrendo;
    private bool invulneravel;
    private bool gameOverAtivado;

    private Coroutine rotinaInvulnerabilidade;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controle = GetComponent<PlayerController>();
        EncontrarHeartUI();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (lives <= 0)
            lives = maxLives;

        if (GameManager.instance != null)
        {
            if (GameManager.instance.TentarConsumirVidasSalvas(out int vidasSalvas))
                lives = Mathf.Clamp(vidasSalvas, 0, maxLives);
        }

        AtualizarInterface();

        Debug.Log("JOGADOR INICIADO!");
        Debug.Log("VIDAS: " + lives);
        Debug.Log("VIDA: " + currentHealth);
    }

    private void EncontrarHeartUI()
    {
        if (heartUI == null)
            heartUI = FindFirstObjectByType<HeartUI>(FindObjectsInactive.Include);
    }

    public void TakeDamage(int damage)
    {
        if (estaMorrendo || invulneravel || gameOverAtivado)
            return;

        if (damage <= 0)
            return;

        Debug.Log("DANO RECEBIDO: " + damage);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("VIDA ATUAL: " + currentHealth);
        Debug.Log("VIDAS RESTANTES: " + lives);

        if (controle != null)
            controle.TremerAoReceberDano();

        AtualizarInterface();

        if (currentHealth <= 0)
        {
            StartCoroutine(Morrer());
        }
        else
        {
            AtivarInvulnerabilidade();
        }
    }

    private void AtivarInvulnerabilidade()
    {
        invulneravel = true;

        if (rotinaInvulnerabilidade != null)
            StopCoroutine(rotinaInvulnerabilidade);

        rotinaInvulnerabilidade = StartCoroutine(TempoDeInvulnerabilidade());
    }

    private IEnumerator Morrer()
    {
        if (estaMorrendo || gameOverAtivado)
            yield break;

        estaMorrendo = true;

        lives--;
        lives = Mathf.Clamp(lives, 0, maxLives);

        Debug.Log("MORTE!");
        Debug.Log("VIDAS RESTANTES: " + lives);

        if (lives > 0)
        {
            yield return new WaitForSeconds(respawnDelay);
            Respawnar();
        }
        else
        {
            gameOverAtivado = true;

            yield return new WaitForSeconds(gameOverDelay);

            GameOver();
        }
    }

    private void Respawnar()
    {
        Debug.LogError("RESPAWNAR FOI CHAMADO!");

        if (GameManager.instance != null)
            transform.position = GameManager.instance.GetSpawnPosition();
        else
            Debug.LogError("GameManager não encontrado!");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        currentHealth = maxHealth;

        AtualizarInterface();

        estaMorrendo = false;

        AtivarInvulnerabilidade();

        Debug.Log("Jogador voltou ao checkpoint!");
    }

    private IEnumerator TempoDeInvulnerabilidade()
    {
        yield return new WaitForSeconds(tempoInvulnerabilidade);

        invulneravel = false;
        rotinaInvulnerabilidade = null;

        Debug.Log("INVULNERABILIDADE TERMINOU!");
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER!");

        if (GameOverManager.Instance != null)
            GameOverManager.Instance.GameOver();
        else
            Debug.LogError("GameOverManager não encontrado!");
    }

    private void AtualizarInterface()
    {
        EncontrarHeartUI();

        if (heartUI != null)
            heartUI.UpdateHearts(currentHealth);
        else
            Debug.LogError("PlayerHealth: HeartUI não encontrado!");
    }

    public void RecuperarVida(int quantidade)
    {
        if (estaMorrendo || gameOverAtivado)
            return;

        currentHealth += quantidade;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        AtualizarInterface();
    }

    public void Curar(int quantidade)
    {
        RecuperarVida(quantidade);
    }
}
