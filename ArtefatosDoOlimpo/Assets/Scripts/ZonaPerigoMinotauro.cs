
using UnityEngine;

public class ZonaPerigoMinotauro : MonoBehaviour
{
    [Header("Chefao")]
    public Minotauro minotauro;

    private bool jogadorEntrou;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jogadorEntrou)
            return;

        bool eJogador =
            other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player");

        if (!eJogador)
            return;

        if (minotauro == null)
        {
            Debug.LogError(
                "Zona de perigo: Minotauro não foi configurado!"
            );
            return;
        }

        jogadorEntrou = true;

        minotauro.AtivarBoss();

        Debug.Log("JOGADOR ENTROU NA ZONA DE PERIGO!");
    }
}
