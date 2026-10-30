using UnityEngine;

public class CoracaoColetavel : MonoBehaviour
{
    [Header("Configuração da cura")]
    [Min(1)]
    public int quantidadeDeCura = 2;

    [Header("Som da coleta")]
    public AudioClip somColeta;

    [Range(0f, 1f)]
    public float volumeSom = 1f;

    private bool coletado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (coletado)
            return;

        // Procura o PlayerHealth no objeto atingido ou em seus pais.
        PlayerHealth jogador = collision.GetComponentInParent<PlayerHealth>();

        if (jogador == null)
            return;

        // Não coleta se a vida já estiver completa.
        if (jogador.currentHealth >= jogador.maxHealth)
            return;

        // Marca como coletado para impedir coleta duplicada.
        coletado = true;

        // Recupera a vida do jogador.
        jogador.RecuperarVida(quantidadeDeCura);

        // Reproduz o som sem precisar de um AudioSource no coração.
        if (somColeta != null)
        {
            AudioSource.PlayClipAtPoint(
                somColeta,
                Camera.main != null
                    ? Camera.main.transform.position
                    : transform.position,
                volumeSom
            );
        }
        else
        {
            Debug.LogWarning(
                "CoracaoColetavel: nenhum som de coleta foi configurado.",
                this
            );
        }

        Debug.Log("Coração coletado! Vida recuperada.");

        // Remove o coração do cenário.
        Destroy(gameObject);
    }
}