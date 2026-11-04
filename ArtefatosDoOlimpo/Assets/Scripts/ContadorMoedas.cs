
using System;
using System.Collections.Generic;
using UnityEngine;

public class ContadorMoedas : MonoBehaviour
{
    public static ContadorMoedas Instance;

    private const string CHAVE_MOEDAS = "TotalMoedas";
    private const string CHAVE_COLETADAS = "MoedasColetadas";

    private HashSet<string> moedasColetadas = new HashSet<string>();

    public int QuantidadeAtual { get; private set; }

    public event Action<int> AoAtualizarMoedas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CarregarMoedas();
    }

    private void CarregarMoedas()
    {
        moedasColetadas.Clear();

        string dados = PlayerPrefs.GetString(CHAVE_COLETADAS, "");

        if (!string.IsNullOrEmpty(dados))
        {
            string[] ids = dados.Split('|');

            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                    moedasColetadas.Add(id);
            }
        }

        QuantidadeAtual = PlayerPrefs.GetInt(
            CHAVE_MOEDAS,
            moedasColetadas.Count
        );

        AoAtualizarMoedas?.Invoke(QuantidadeAtual);

        Debug.Log("Moedas carregadas: " + QuantidadeAtual);
    }

    public bool RegistrarMoeda(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogError("Moeda sem ID configurado!");
            return false;
        }

        if (moedasColetadas.Contains(id))
        {
            Debug.Log("Esta moeda já foi coletada: " + id);
            return false;
        }

        moedasColetadas.Add(id);
        QuantidadeAtual++;

        SalvarMoedas();

        AoAtualizarMoedas?.Invoke(QuantidadeAtual);

        Debug.Log("Moeda coletada! Total: " + QuantidadeAtual);

        return true;
    }

    public bool MoedaFoiColetada(string id)
    {
        return !string.IsNullOrEmpty(id) &&
               moedasColetadas.Contains(id);
    }

    private void SalvarMoedas()
    {
        PlayerPrefs.SetInt(CHAVE_MOEDAS, QuantidadeAtual);

        PlayerPrefs.SetString(
            CHAVE_COLETADAS,
            string.Join("|", moedasColetadas)
        );

        PlayerPrefs.Save();
    }

    public void ResetarMoedas()
    {
        moedasColetadas.Clear();
        QuantidadeAtual = 0;

        PlayerPrefs.DeleteKey(CHAVE_MOEDAS);
        PlayerPrefs.DeleteKey(CHAVE_COLETADAS);
        PlayerPrefs.Save();

        AoAtualizarMoedas?.Invoke(QuantidadeAtual);

        Debug.Log("Contador de moedas reiniciado!");
    }
}
