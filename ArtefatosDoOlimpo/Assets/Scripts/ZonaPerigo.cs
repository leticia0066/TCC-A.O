using UnityEngine;

public class ZonaPerigo : MonoBehaviour
{
    public GameObject textoPerigo;

    public Minotauro minotauro;

    private bool ativado = false;

    private void OnTriggerEnter2D(
        Collider2D collision
    )
    {
        if (ativado)
            return;

        if (!collision.CompareTag("Player"))
            return;

        ativado = true;

        if (textoPerigo != null)
        {
            textoPerigo.SetActive(true);

            Destroy(
                textoPerigo,
                3f
            );
        }

        if (minotauro != null)
        {
            minotauro.AtivarBoss();
        }
    }
}