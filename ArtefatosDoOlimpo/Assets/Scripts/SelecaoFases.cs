using UnityEngine;

public class SelecaoFases : MonoBehaviour
{
    [Header("Transição de cena")]
    public SceneTransition sceneTransition;

    // Guarda a fase escolhida enquanto a cutscene acontece.
    public static string NomeFaseSelecionada = "Fase1_Aress";

    public void JogarFase1()
    {
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
        if (sceneTransition == null)
        {
            Debug.LogError(
                "SelecaoFases: o campo Scene Transition está vazio!"
            );
            return;
        }

        sceneTransition.ChangeScene("TelaInicial");
    }

    private void AbrirCutscene()
    {
        if (sceneTransition == null)
        {
            Debug.LogError(
                "SelecaoFases: arraste o objeto com SceneTransition " +
                "para o campo Scene Transition no Inspector."
            );
            return;
        }

        Debug.Log(
            "Fase selecionada: " + NomeFaseSelecionada +
            ". Abrindo CutsceneIntro."
        );

        sceneTransition.ChangeScene("CutsceneIntro");
    }
}