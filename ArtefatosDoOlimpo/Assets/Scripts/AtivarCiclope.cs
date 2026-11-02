
using UnityEngine;

public class AtivarCiclope : MonoBehaviour
{
    public CiclopeBoss ciclope;
    private bool ativado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            ativado = true;

            if (ciclope != null)
            {
                ciclope.AtivarBoss();
                Debug.Log("BATALHA DO CICLOPE INICIADA!");
            }
            else
            {
                Debug.LogWarning("Ciclope não foi conectado ao gatilho!");
            }
        }
    }
}
