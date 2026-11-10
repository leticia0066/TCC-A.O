
using System.Collections;
using UnityEngine;
using DragonBones;

public class MedusaController : MonoBehaviour
{
    [Header("Jogador")]
    public UnityEngine.Transform player;

    [Header("Alerta de perigo")]
    public float warningDistance = 9f;

    [Header("Ataque")]
    public float attackRange = 6f;
    public float attackCooldown = 2.5f;
    public float projectileSpeed = 6f;
    public int projectileDamage = 3;

    [Header("Feitico")]
    public GameObject projectilePrefab;
    public UnityEngine.Transform firePoint;

    [Header("Animacoes DragonBones")]
    public UnityArmatureComponent armature;

    public string animacaoAndando = "andando";
    public string animacaoAtaque = "ataque";

    [Tooltip("Tempo que a animacao de ataque fica tocando.")]
    public float duracaoAnimacaoAtaque = 0.8f;

    private bool warningPlayed;
    private float nextAttackTime;

    private bool atacando;
    private Coroutine rotinaAtaque;

    private void Start()
    {
        FindPlayer();

        if (projectilePrefab == null)
        {
            Debug.LogError(
                "Medusa: arraste o Prefab do feitico para Projectile Prefab!"
            );
        }

        if (armature == null)
        {
            armature = GetComponentInChildren<UnityArmatureComponent>(true);
        }

        if (armature == null)
        {
            Debug.LogError(
                "Medusa: UnityArmatureComponent nao encontrado!"
            );
        }
        else
        {
            TocarAnimacaoAndando();
        }
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

        if (distance <= attackRange && Time.time >= nextAttackTime)
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

    private void AttackPlayer()
    {
        if (projectilePrefab == null || player == null || atacando)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Vector2 direction =
            ((Vector2)player.position -
            (Vector2)transform.position).normalized;

        if (armature != null)
        {
            if (rotinaAtaque != null)
                StopCoroutine(rotinaAtaque);

            rotinaAtaque = StartCoroutine(ExecutarAnimacaoAtaque());
        }

        Vector3 spawnPosition;

        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }
        else
        {
            spawnPosition = transform.position +
                (Vector3)(direction * 1.2f);
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
            Debug.LogError(
                "O Prefab precisa ter o script MedusaProjectile!"
            );

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

        Debug.Log("Medusa lancou um feitico contra o jogador!");
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
