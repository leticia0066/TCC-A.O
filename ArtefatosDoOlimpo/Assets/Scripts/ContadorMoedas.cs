
using System;
using System.Collections.Generic;
using UnityEngine;

public class ContadorMoedas : MonoBehaviour
{
    public static ContadorMoedas Instance;

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

        QuantidadeAtual = 0;
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

        AoAtualizarMoedas?.Invoke(QuantidadeAtual);

        Debug.Log("Moeda coletada! Total: " + QuantidadeAtual);

        return true;
    }

    public bool MoedaFoiColetada(string id)
    {
        return !string.IsNullOrEmpty(id) &&
               moedasColetadas.Contains(id);
    }

    public List<string> ObterMoedasColetadas()
    {
        return new List<string>(moedasColetadas);
    }

    public void RestaurarMoedas(List<string> lista)
    {
        moedasColetadas.Clear();

        if (lista != null)
        {
            foreach (string id in lista)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    moedasColetadas.Add(id);
                }
            }
        }

        QuantidadeAtual = moedasColetadas.Count;

        AoAtualizarMoedas?.Invoke(QuantidadeAtual);

        Debug.Log("Moedas restauradas: " + QuantidadeAtual);
    }

    public void ResetarMoedas()
    {
        moedasColetadas.Clear();

        QuantidadeAtual = 0;

        AoAtualizarMoedas?.Invoke(QuantidadeAtual);

        Debug.Log("Contador de moedas reiniciado!");
    }
}
