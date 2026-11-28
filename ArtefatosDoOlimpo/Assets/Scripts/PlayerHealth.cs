
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Tentativas")]
    public int maxLives = 3;
    public int lives;

    [Header("Interface")]
    public HeartUI heartUI;

    [Header("Respawn")]
    public float respawnDelay = 0.5f;
    public float gameOverDelay = 0.5f;

    [Header("Invulnerabilidade")]
    public float tempoInvulnerabilidade = 1.5f;

    private Rigidbody2D rb;
    private PlayerController controle;
    private BloquearTeleportePlayer bloqueador;

    private bool estaMorrendo;
    private bool invulneravel;
    private bool gameOverAtivado;

    private Coroutine rotinaInvulnerabilidade;

    public bool EstaInvulneravel => invulneravel;
    public bool EstaMorrendo => estaMorrendo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controle = GetComponent<PlayerController>();
        bloqueador = GetComponent<BloquearTeleportePlayer>();

        EncontrarHeartUI();
    }

    private void Start()
    {
        bool estadoRestaurado = false;

        if (GameManager.instance != null)
        {
            estadoRestaurado =
                GameManager.instance.TentarCarregarEstadoJogador(
                    out int vidaSalva,
                    out int tentativasSalvas
                );

            if (estadoRestaurado)
            {
                currentHealth = Mathf.Clamp(
                    vidaSalva, 0, maxHealth
                );

                lives = Mathf.Clamp(
                    tentativasSalvas, 0, maxLives
                );
            }
        }

        if (!estadoRestaurado)
        {
            currentHealth = maxHealth;

            if (lives <= 0)
                lives = maxLives;

            if (GameManager.instance != null)
            {
                if (GameManager.instance.TentarConsumirVidasSalvas(
                    out int vidasSalvas))
                {
                    lives = Mathf.Clamp(vidasSalvas, 0, maxLives);
                }
            }
        }

        AtualizarInterface();
        SalvarEstadoAtual();

        Debug.Log("PLAYER INICIADO");
        Debug.Log("VIDA: " + currentHealth + "/" + maxHealth);
        Debug.Log("TENTATIVAS: " + lives + "/" + maxLives);
    }

    private void OnDestroy()
    {
        SalvarEstadoAtual();
    }

    private void EncontrarHeartUI()
    {
        if (heartUI == null)
        {
            heartUI = FindFirstObjectByType<HeartUI>(
                FindObjectsInactive.Include
            );
        }
    }

    private void SalvarEstadoAtual()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SalvarEstadoJogador(
                currentHealth,
                lives
            );
        }
    }

    public void TakeDamage(int damage)
    {
        if (estaMorrendo || gameOverAtivado || invulneravel)
            return;

        if (damage <= 0)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("DANO RECEBIDO: " + damage);
        Debug.Log("VIDA ATUAL: " + currentHealth);

        AtualizarInterface();
        SalvarEstadoAtual();

        if (controle != null)
            controle.TremerAoReceberDano();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            StartCoroutine(Morrer());
        }
        else
        {
            AtivarInvulnerabilidade();
        }
    }

    private IEnumerator Morrer()
    {
        if (estaMorrendo || gameOverAtivado)
            yield break;

        estaMorrendo = true;
        invulneravel = true;

        lives = Mathf.Max(0, lives - 1);

        Debug.Log("JOGADOR MORREU!");
        Debug.Log("TENTATIVAS RESTANTES: " + lives);

        SalvarEstadoAtual();
        PararMovimento();

        if (lives > 0)
        {
            yield return new WaitForSecondsRealtime(respawnDelay);

            Respawnar();
        }
        else
        {
            gameOverAtivado = true;

            yield return new WaitForSecondsRealtime(gameOverDelay);

            AbrirGameOver();
        }
    }

    private void Respawnar()
    {
        if (currentHealth > 0)
        {
            Debug.LogWarning(
                "RESPAWN BLOQUEADO! VIDA ATUAL: " + currentHealth
            );

            estaMorrendo = false;
            return;
        }

        Debug.Log("RESPAWN AUTORIZADO: jogador morreu.");

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager não encontrado!");
            estaMorrendo = false;
            return;
        }

        Vector3 destino = GameManager.instance.GetSpawnPosition();

        PararMovimento();

        if (bloqueador != null)
        {
            bloqueador.AutorizarTeleporte(destino);
        }
        else
        {
            transform.position = destino;

            if (rb != null)
            {
                rb.position = new Vector2(destino.x, destino.y);
            }
        }

        Physics2D.SyncTransforms();

        currentHealth = maxHealth;

        AtualizarInterface();

        estaMorrendo = false;

        SalvarEstadoAtual();
        AtivarInvulnerabilidade();

        Debug.Log("RESPAWN CONCLUÍDO!");
        Debug.Log("VIDA RESTAURADA: " + currentHealth);
        Debug.Log("TENTATIVAS RESTANTES: " + lives);
    }

    private void PararMovimento()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void AtivarInvulnerabilidade()
    {
        invulneravel = true;

        if (rotinaInvulnerabilidade != null)
            StopCoroutine(rotinaInvulnerabilidade);

        rotinaInvulnerabilidade =
            StartCoroutine(TempoDeInvulnerabilidade());
    }

    private IEnumerator TempoDeInvulnerabilidade()
    {
        yield return new WaitForSeconds(tempoInvulnerabilidade);

        invulneravel = false;
        rotinaInvulnerabilidade = null;

        Debug.Log("INVULNERABILIDADE TERMINOU");
    }

    private void AbrirGameOver()
    {
        Debug.Log("GAME OVER");

        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameOverManager não encontrado!");
        }
    }

    private void AtualizarInterface()
    {
        EncontrarHeartUI();

        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHealth);
        }
        else
        {
            Debug.LogError("HeartUI não encontrado!");
        }
    }

    public void RecuperarVida(int quantidade)
    {
        if (estaMorrendo || gameOverAtivado)
            return;

        if (quantidade <= 0)
            return;

        currentHealth += quantidade;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        AtualizarInterface();
        SalvarEstadoAtual();
    }

    public void Curar(int quantidade)
    {
        RecuperarVida(quantidade);
    }
}
