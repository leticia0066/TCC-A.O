
using System.Collections;
using UnityEngine;
using DragonBones;

public class MedusaController : MonoBehaviour
{
    [Header("Jogador")]
    public UnityEngine.Transform player;

    [Header("Alerta de perigo")]
    public float warningDistance = 9f;

    [Header("Zona de perigo")]
    public bool jogadorNaZona;

    [Header("Ataque")]
    public float attackRange = 6f;
    public float attackCooldown = 2.5f;
    public float projectileSpeed = 6f;
    public int projectileDamage = 3;

    [Header("Feitico")]
    public GameObject projectilePrefab;
    public UnityEngine.Transform firePoint;

    [Header("Petrificacao")]
    public float duracaoPetrificacao = 5f;
    private bool petrificacaoAtivada;

    [Header("Animacoes DragonBones")]
    public UnityArmatureComponent armature;

    public string animacaoAndando = "andando";
    public string animacaoAtaque = "ataque";

    public float duracaoAnimacaoAtaque = 0.8f;

    private bool warningPlayed;
    private float nextAttackTime;

    private bool atacando;
    private Coroutine rotinaAtaque;

    private void Start()
    {
        FindPlayer();

        if (projectilePrefab == null)
            Debug.LogError("Medusa: configure o Projectile Prefab!");

        if (firePoint == null)
            Debug.LogWarning("Medusa: configure o FirePoint nos olhos!");

        if (armature == null)
            armature = GetComponentInChildren<UnityArmatureComponent>(true);

        if (armature == null)
            Debug.LogError("Medusa: UnityArmatureComponent nao encontrado!");
        else
            TocarAnimacaoAndando();
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= warningDistance && !warningPlayed)
        {
            warningPlayed = true;

            if (AudioManager.Instance != null)
                AudioManager.Instance.TocarAvisoMedusa();
        }

        if (distance > warningDistance + 1f)
            warningPlayed = false;

        if (jogadorNaZona &&
            distance <= attackRange &&
            Time.time >= nextAttackTime)
        {
            AttackPlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UnityEngine.Transform jogadorEncontrado =
            other.CompareTag("Player")
            ? other.transform
            : other.GetComponentInParent<PlayerHealth>()?.transform;

        if (jogadorEncontrado == null)
            return;

        player = jogadorEncontrado;
        jogadorNaZona = true;

        Debug.Log("Jogador entrou na zona de perigo da Medusa!");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UnityEngine.Transform jogadorEncontrado =
            other.CompareTag("Player")
            ? other.transform
            : other.GetComponentInParent<PlayerHealth>()?.transform;

        if (jogadorEncontrado == null || jogadorEncontrado != player)
            return;

        jogadorNaZona = false;

        Debug.Log("Jogador saiu da zona de perigo da Medusa!");
    }

    // CHAMADA PELO EnemyHealth QUANDO A VIDA CHEGAR A 70%.
    public void AtivarPetrificacao()
    {
        if (petrificacaoAtivada)
            return;

        if (player == null)
            FindPlayer();

        if (player == null)
        {
            Debug.LogWarning("Medusa: jogador nao encontrado para petrificar.");
            return;
        }

        PlayerController controle =
            player.GetComponent<PlayerController>();

        if (controle == null)
            controle = player.GetComponentInParent<PlayerController>();

        if (controle == null)
        {
            Debug.LogError("Medusa: PlayerController nao encontrado no jogador!");
            return;
        }

        petrificacaoAtivada = true;

        controle.Petrificar(duracaoPetrificacao);

        Debug.Log("MEDUSA ATIVOU A PETRIFICACAO!");
    }

    private void AttackPlayer()
    {
        if (projectilePrefab == null ||
            player == null ||
            atacando)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Vector3 spawnPosition = firePoint != null
            ? firePoint.position
            : transform.position;

        Vector2 direction =
            ((Vector2)player.position -
            (Vector2)spawnPosition).normalized;

        if (armature != null)
        {
            if (rotinaAtaque != null)
                StopCoroutine(rotinaAtaque);

            rotinaAtaque = StartCoroutine(ExecutarAnimacaoAtaque());
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.identity
        );

        MedusaProjectile projectileScript =
            projectile.GetComponent<MedusaProjectile>();

        if (projectileScript == null)
        {
            Debug.LogError("Prefab precisa ter MedusaProjectile!");
            Destroy(projectile);
            return;
        }

        projectileScript.Initialize(
            direction,
            projectileSpeed,
            projectileDamage,
            transform
        );

        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarAtaqueMedusa();

        Debug.Log("Medusa lancou um feitico pelos olhos!");
    }

    private IEnumerator ExecutarAnimacaoAtaque()
    {
        atacando = true;

        armature.animation.Play(animacaoAtaque, 1);

        yield return new WaitForSeconds(duracaoAnimacaoAtaque);

        TocarAnimacaoAndando();

        atacando = false;
        rotinaAtaque = null;
    }

    private void TocarAnimacaoAndando()
    {
        if (armature == null)
            return;

        armature.animation.Play(animacaoAndando, 0);
    }

    private void OnDisable()
    {
        if (rotinaAtaque != null)
        {
            StopCoroutine(rotinaAtaque);
            rotinaAtaque = null;
        }

        atacando = false;
    }
}
