
using UnityEngine;

public class ArtefatoColetavel : MonoBehaviour
{
    public enum TipoArtefato
    {
        Espada,
        Escudo,
        Raio
    }

    [Header("Artefato")]
    public TipoArtefato tipoArtefato;

    [Header("Som da coleta")]
    public AudioClip somColeta;

    [Range(0f, 1f)]
    public float volumeSom = 0.8f;

    private bool coletado = false;

    private void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager não encontrado!");
            return;
        }

        switch (tipoArtefato)
        {
            case TipoArtefato.Espada:
                coletado = GameManager.instance.espadaColetada;
                break;

            case TipoArtefato.Escudo:
                coletado = GameManager.instance.escudoColetado;
                break;

            case TipoArtefato.Raio:
                coletado = GameManager.instance.raioColetado;
                break;
        }

        if (coletado)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (coletado)
            return;

        PlayerHealth jogador =
            collision.GetComponentInParent<PlayerHealth>();

        if (jogador == null)
            return;

        if (GameManager.instance == null)
            return;

        coletado = true;

        switch (tipoArtefato)
        {
            case TipoArtefato.Espada:
                GameManager.instance.ColetarEspada();
                break;

            case TipoArtefato.Escudo:
                GameManager.instance.ColetarEscudo();
                break;

            case TipoArtefato.Raio:
                GameManager.instance.ColetarRaio();
                break;
        }

        if (somColeta != null)
        {
            AudioSource.PlayClipAtPoint(
                somColeta,
                transform.position,
                volumeSom
            );
        }

        ArtefatosUI interfaceArtefatos =
            FindFirstObjectByType<ArtefatosUI>();

        if (interfaceArtefatos != null)
        {
            interfaceArtefatos.AtualizarInterface();
        }

        Debug.Log("Artefato coletado: " + tipoArtefato);

        Destroy(gameObject);
    }
}
