
using System.Collections;
using UnityEngine;

public class GerenciadorRaios : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject prefabRaio;
    public Transform pontoSaida;

    [Header("Intervalo dos ataques")]
    public float intervaloEntreSequencias = 4f;

    [Header("Quantidade de raios")]
    [Min(1)]
    public int quantidadeRaios = 2;

    public float intervaloEntreRaios = 0.7f;

    [Header("Inicializacao")]
    public bool iniciarAutomaticamente = true;

    private Coroutine ataqueCoroutine;

    private void Start()
    {
        if (prefabRaio == null || pontoSaida == null)
        {
            Debug.LogError(
                "GerenciadorRaios: confira o Prefab do raio " +
                "e o Ponto de Saida no Inspector!"
            );

            enabled = false;
            return;
        }

        if (iniciarAutomaticamente)
        {
            IniciarAtaques();
        }
    }

    public void IniciarAtaques()
    {
        if (ataqueCoroutine != null)
            return;

        ataqueCoroutine = StartCoroutine(
            CicloDeAtaques()
        );
    }

    public void PararAtaques()
    {
        if (ataqueCoroutine == null)
            return;

        StopCoroutine(ataqueCoroutine);
        ataqueCoroutine = null;
    }

    private IEnumerator CicloDeAtaques()
    {
        while (true)
        {
            
            for (int i = 0; i < quantidadeRaios; i++)
            {
                CriarRaio();

                if (i < quantidadeRaios - 1)
                {
                    yield return new WaitForSeconds(
                        intervaloEntreRaios
                    );
                }
            }

            
            yield return new WaitForSeconds(
                intervaloEntreSequencias
            );
        }
    }

    private void CriarRaio()
    {
        if (prefabRaio == null || pontoSaida == null)
            return;

        Instantiate(
            prefabRaio,
            pontoSaida.position,
            Quaternion.identity
        );
    }

    private void OnDisable()
    {
        if (ataqueCoroutine != null)
        {
            StopCoroutine(ataqueCoroutine);
            ataqueCoroutine = null;
        }
    }
}
