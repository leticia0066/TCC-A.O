
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string cenaCheckpoint;

    public float posicaoX;
    public float posicaoY;
    public float posicaoZ;

    public int vidas;

    public bool espadaColetada;
    public bool escudoColetado;
    public bool raioColetado;

    public List<string> moedasColetadas = new List<string>();
}

public static class SaveSystem
{
    private static string CaminhoArquivo =>
        Path.Combine(Application.persistentDataPath, "save_artefatos_olimpo.json");

    public static bool TemSave()
    {
        return File.Exists(CaminhoArquivo);
    }

    public static void Salvar(SaveData dados)
    {
        try
        {
            string json = JsonUtility.ToJson(dados, true);
            File.WriteAllText(CaminhoArquivo, json);

            Debug.Log("JOGO SALVO COM SUCESSO!");
            Debug.Log("Arquivo: " + CaminhoArquivo);
        }
        catch (Exception erro)
        {
            Debug.LogError("Erro ao salvar o jogo: " + erro.Message);
        }
    }

    public static SaveData Carregar()
    {
        if (!TemSave())
        {
            Debug.LogWarning("Nenhum salvamento encontrado.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(CaminhoArquivo);
            SaveData dados = JsonUtility.FromJson<SaveData>(json);

            if (dados == null || string.IsNullOrEmpty(dados.cenaCheckpoint))
            {
                Debug.LogError("O arquivo de salvamento está inválido.");
                return null;
            }

            Debug.Log("Salvamento carregado!");
            return dados;
        }
        catch (Exception erro)
        {
            Debug.LogError("Erro ao carregar o jogo: " + erro.Message);
            return null;
        }
    }

    public static void ApagarSave()
    {
        if (File.Exists(CaminhoArquivo))
        {
            File.Delete(CaminhoArquivo);
        }

        Debug.Log("Salvamento apagado. Novo jogo disponível.");
    }
}
