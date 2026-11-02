
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelecaoFases : MonoBehaviour
{
    public static string NomeFaseSelecionada = "Fase1_Aress";

    public void JogarFase1()
{
    Debug.Log("TESTE: O BOTÃO DA FASE 1 FOI ACIONADO!");

    NomeFaseSelecionada = "Fase1_Aress";
    AbrirCutscene();
}
    public void JogarFase2()
    {
        NomeFaseSelecionada = "Fase2_Atena";
        AbrirCutscene();
    }

    public void JogarFase3()
    {
        NomeFaseSelecionada = "Fase3_Zeus";
        AbrirCutscene();
    }

    public void Voltar()
    {
        TocarSomBotao();
        TrocarCena("TelaInicial");
    }

    private void AbrirCutscene()
    {
        Debug.Log(
            "Fase selecionada: " + NomeFaseSelecionada
        );

        TocarSomBotao();
        TrocarCena("CutsceneIntro");
    }

    private void TrocarCena(string nomeCena)
    {
        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena(nomeCena);
        }
        else
        {
            Debug.LogWarning(
                "Controlador de transições não encontrado. " +
                "Carregando a cena diretamente."
            );

            SceneManager.LoadScene(nomeCena);
        }
    }

    private void TocarSomBotao()
    {
        if (MainMenu.Instance != null)
        {
            MainMenu.Instance.TocarSomBotao();
        }
    }
}
