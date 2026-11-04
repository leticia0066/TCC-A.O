
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Identificação")]
    public string idMoeda;

    private Vector3 initialPosition;
    private bool collected = false;

    private void Start()
    {
        initialPosition = transform.position;

        if (ContadorMoedas.Instance == null)
        {
            Debug.LogError(
                "Coin: ContadorMoedas não encontrado na cena!"
            );
            return;
        }

        // Não mostra novamente uma moeda já coletada.
        if (ContadorMoedas.Instance.MoedaFoiColetada(idMoeda))
        {
            collected = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    private void Collect()
    {
        if (ContadorMoedas.Instance == null)
            return;

        bool registrada =
            ContadorMoedas.Instance.RegistrarMoeda(idMoeda);

        if (!registrada)
            return;

        collected = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.TocarMoeda();
        }

        gameObject.SetActive(false);

        Debug.Log("Moeda coletada: " + idMoeda);
    }

    public void Respawn()
    {
        if (ContadorMoedas.Instance != null &&
            ContadorMoedas.Instance.MoedaFoiColetada(idMoeda))
        {
            collected = true;
            gameObject.SetActive(false);
            return;
        }

        transform.position = initialPosition;

        collected = false;

        gameObject.SetActive(true);
    }
}
