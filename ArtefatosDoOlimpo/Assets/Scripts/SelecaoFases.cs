
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelecaoFases : MonoBehaviour
{
    public static string NomeFaseSelecionada = "Fase1_Aress";

    [Header("Botões das fases")]
    public Button botaoFase1;
    public Button botaoFase2;
    public Button botaoFase3;

    [Header("Cores dos botões")]
    public Color corLiberada = Color.white;
    public Color corBloqueada = Color.gray;

    private void Start()
    {
        AtualizarBotoes();
    }

    private void AtualizarBotoes()
    {
        ConfigurarBotao(
            botaoFase1,
            ProgressoJogo.Fase1Liberada
        );

        ConfigurarBotao(
            botaoFase2,
            ProgressoJogo.Fase2Liberada
        );

        ConfigurarBotao(
            botaoFase3,
            ProgressoJogo.Fase3Liberada
        );
    }

    private void ConfigurarBotao(Button botao, bool liberado)
    {
        if (botao == null)
        {
            Debug.LogWarning("Um botão de fase não foi atribuído!");
            return;
        }

        botao.interactable = liberado;

        Image imagem = botao.GetComponent<Image>();

        if (imagem != null)
        {
            imagem.color = liberado ? corLiberada : corBloqueada;
        }
    }

    public void JogarFase1()
    {
        if (!ProgressoJogo.Fase1Liberada) return;

        NomeFaseSelecionada = "Fase1_Aress";
        AbrirCutscene();
    }

    public void JogarFase2()
    {
        if (!ProgressoJogo.Fase2Liberada) return;

        NomeFaseSelecionada = "Fase2_Atena";
        AbrirCutscene();
    }

    public void JogarFase3()
    {
        if (!ProgressoJogo.Fase3Liberada) return;

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
        Debug.Log("Fase selecionada: " + NomeFaseSelecionada);

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
