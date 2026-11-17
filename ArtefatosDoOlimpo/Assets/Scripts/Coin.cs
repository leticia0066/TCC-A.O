
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    [Header("Identificação")]
    public string idMoeda;

    private Vector3 initialPosition;
    private bool collected = false;

    private string ObterIdCompleto()
    {
        return SceneManager.GetActiveScene().name + "::" + idMoeda;
    }

    private void Start()
    {
        initialPosition = transform.position;

        if (ContadorMoedas.Instance == null)
        {
            Debug.LogError("Coin: ContadorMoedas não encontrado!");
            return;
        }

        if (ContadorMoedas.Instance.MoedaFoiColetada(
            ObterIdCompleto()))
        {
            collected = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        Collect();
    }

    private void Collect()
    {
        if (ContadorMoedas.Instance == null)
            return;

        bool registrada = ContadorMoedas.Instance.RegistrarMoeda(
            ObterIdCompleto()
        );

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
            ContadorMoedas.Instance.MoedaFoiColetada(
                ObterIdCompleto()))
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
