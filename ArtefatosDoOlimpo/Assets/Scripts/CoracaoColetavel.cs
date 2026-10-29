using UnityEngine;

public class CoracaoColetavel : MonoBehaviour
{
    [Header("Configuração da cura")]
    [Min(1)]
    public int quantidadeDeCura = 2;

    private bool coletado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (coletado)
            return;

        // Procura o PlayerHealth no objeto atingido ou em seus pais.
        PlayerHealth jogador = collision.GetComponentInParent<PlayerHealth>();

        if (jogador == null)
            return;

        // Não coleta o coração se a vida já estiver completa.
        if (jogador.currentHealth >= jogador.maxHealth)
            return;

        coletado = true;

        // Recupera a vida do jogador.
        jogador.RecuperarVida(quantidadeDeCura);

        Debug.Log("Coração coletado! Vida recuperada.");

        // Remove o coração do cenário.
        Destroy(gameObject);
    }
}