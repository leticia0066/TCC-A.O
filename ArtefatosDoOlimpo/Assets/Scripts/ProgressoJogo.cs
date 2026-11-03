
using UnityEngine;

public static class ProgressoJogo
{
    // A Fase 1 começa sempre liberada.
    public static bool Fase1Liberada => true;

    // A Fase 2 só libera depois de concluir a Fase 1.
    public static bool Fase2Liberada =>
        PlayerPrefs.GetInt("Fase1Concluida", 0) == 1;

    // A Fase 3 só libera depois de concluir a Fase 2.
    public static bool Fase3Liberada =>
        PlayerPrefs.GetInt("Fase2Concluida", 0) == 1;

    public static bool TodasAsFasesConcluidas =>
        PlayerPrefs.GetInt("Fase3Concluida", 0) == 1;

    public static void ConcluirFase(int numeroFase)
    {
        PlayerPrefs.SetInt("Fase" + numeroFase + "Concluida", 1);
        PlayerPrefs.Save();

        Debug.Log("Fase " + numeroFase + " concluída!");
    }
}
