
using UnityEngine;

public class ZonaPerigoLarva : MonoBehaviour
{
    [Header("Configuração do perigo")]
    [SerializeField] private int dano = 1;

    [Tooltip("Tempo mínimo entre danos causados por esta zona.")]
    [SerializeField] private float intervaloEntreDanos = 1.5f;

    private float proximoDanoPermitido = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou é o jogador.
        if (!other.CompareTag("Player"))
            return;

        // Evita dano repetido muito rapidamente.
        if (Time.time < proximoDanoPermitido)
            return;

        PlayerHealth vida = other.GetComponent<PlayerHealth>();

        if (vida == null)
            vida = other.GetComponentInParent<PlayerHealth>();

        if (vida == null)
        {
            Debug.LogWarning(
                "ZonaPerigoLarva: PlayerHealth não encontrado no jogador."
            );
            return;
        }

        // Aplica o dano usando o sistema de vida existente.
        vida.TakeDamage(dano);

        proximoDanoPermitido =
            Time.time + intervaloEntreDanos;
    }
}
