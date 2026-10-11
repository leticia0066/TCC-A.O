
using UnityEngine;

public class MedusaController : MonoBehaviour
{
    [Header("Jogador")]
    public Transform player;

    [Header("Alerta de perigo")]
    public float warningDistance = 9f;

    [Header("Ataque")]
    public float attackRange = 6f;
    public float attackCooldown = 2.5f;
    public float projectileSpeed = 6f;
    public int projectileDamage = 3;

    [Header("Feitico")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    private bool warningPlayed;
    private float nextAttackTime;

    private void Start()
    {
        FindPlayer();

        if (projectilePrefab == null)
            Debug.LogError("Medusa: arraste o Prefab do feitico para Projectile Prefab!");
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
            AttackPlayer();
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
        if (projectilePrefab == null || player == null)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Vector2 direction =
            ((Vector2)player.position -
            (Vector2)transform.position).normalized;

        Vector3 spawnPosition;

        if (firePoint != null)
            spawnPosition = firePoint.position;
        else
            spawnPosition = transform.position +
                (Vector3)(direction * 1.2f);

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

        // Novo: som de lancamento do feitico.
        if (AudioManager.Instance != null)
            AudioManager.Instance.TocarAtaqueMedusa();

        Debug.Log("Medusa lancou um feitico contra o jogador!");
    }
}
