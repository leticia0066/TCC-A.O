using UnityEngine;

public class SelecaoFases : MonoBehaviour
{
    [Header("Transição de cena")]
    public SceneTransition sceneTransition;

    [Header("Som dos botões")]
    public MainMenu mainMenu;

    public static string NomeFaseSelecionada = "Fase1_Aress";

    private void Awake()
    {
        if (mainMenu == null)
            mainMenu = FindFirstObjectByType<MainMenu>();
    }

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
        TocarSomBotao();

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
        TocarSomBotao();

        if (sceneTransition == null)
        {
            Debug.LogError(
                "SelecaoFases: o campo Scene Transition está vazio!"
            );

            return;
        }

        Debug.Log(
            "Fase selecionada: " + NomeFaseSelecionada +
            ". Abrindo CutsceneIntro."
        );

        sceneTransition.ChangeScene("CutsceneIntro");
    }

    private void TocarSomBotao()
    {
        if (mainMenu != null)
        {
            mainMenu.TocarSomBotao();
        }
        else
        {
            Debug.LogWarning(
                "SelecaoFases: não foi encontrado um MainMenu " +
                "para reproduzir o som dos botões."
            );
        }
    }
}