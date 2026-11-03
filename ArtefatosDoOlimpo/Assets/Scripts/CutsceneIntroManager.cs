
using System.Collections;
using TMPro;
using UnityEngine;

public class CutsceneIntroManager : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelHistoria;
    public GameObject painelTutorial;

    [Header("Textos")]
    public TMP_Text textoHistoria;
    public TMP_Text textoPular;
    public TMP_Text textoTutorial;

    [Header("Configuração da digitação")]
    [Min(0.001f)]
    public float velocidadeDigitacao = 0.09f;

    [Header("Som da digitação")]
    public AudioSource audioSource;
    public AudioClip somDigitacao;

    [Range(0f, 1f)]
    public float volumeDigitacao = 0.35f;

    [Min(1)]
    public int letrasEntreSons = 4;

    private string historiaCompleta;
    private Coroutine digitacaoCoroutine;

    private bool digitando;
    private bool historiaTerminada;
    private bool tutorialAtivo;
    private bool iniciandoFase;

    private int contadorLetras;

    private void Start()
    {
        Time.timeScale = 1f;

        if (!ReferenciasValidas())
        {
            enabled = false;
            return;
        }

        // Escolhe a história conforme a fase selecionada.
        historiaCompleta = EscolherHistoria();

        painelHistoria.SetActive(true);
        painelTutorial.SetActive(false);

        textoHistoria.text = "";
        textoPular.text = "PRESSIONE ESPAÇO PARA PULAR";

        textoTutorial.text =
            "COMO JOGAR\n\n" +
            "A / D — Mover para os lados\n" +
            "ESPAÇO — Pular\n" +
            "Z — Atirar\n" +
            "P — Abrir configurações\n\n" +
            "PRESSIONE ESPAÇO PARA COMEÇAR";

        contadorLetras = 0;

        digitacaoCoroutine = StartCoroutine(DigitarHistoria());
    }

    private string EscolherHistoria()
    {
        string fase = SelecaoFases.NomeFaseSelecionada;

        switch (fase)
        {
            case "Fase1_Aress":

                return
                    "CAPÍTULO I — A PROVAÇÃO DE ARES\n\n" +

                    "O Olimpo está ameaçado. Os antigos artefatos, " +
                    "que mantinham o equilíbrio entre os deuses, " +
                    "foram perdidos pelo mundo antigo.\n\n" +

                    "Sua primeira jornada o leva ao domínio de Ares, " +
                    "o deus da guerra. Em seu templo, a lendária " +
                    "Espada do Olimpo permanece protegida por " +
                    "criaturas e perigos.\n\n" +

                    "Ares não entregará seu poder facilmente. " +
                    "Somente aqueles que demonstrarem coragem " +
                    "e determinação poderão conquistar a espada.\n\n" +

                    "Sua jornada começa agora.";

            case "Fase2_Atena":

                return
                    "CAPÍTULO II — O JULGAMENTO DE ATENA\n\n" +

                    "A Espada do Olimpo foi recuperada, mas a " +
                    "jornada está longe de terminar.\n\n" +

                    "O próximo destino é o templo de Atena, " +
                    "deusa da sabedoria e da estratégia. " +
                    "Ali está escondido o Escudo do Olimpo.\n\n" +

                    "Diferente da provação de Ares, este desafio " +
                    "exigirá atenção, inteligência e equilíbrio. " +
                    "Os perigos do templo testarão cada decisão " +
                    "tomada pelo viajante.\n\n" +

                    "Atena observa cada passo. " +
                    "Será que você está preparado para seu julgamento?";

            case "Fase3_Zeus":

                return
                    "CAPÍTULO III — A IRA DE ZEUS\n\n" +

                    "A Espada e o Escudo já foram recuperados. " +
                    "Agora resta encontrar o último artefato.\n\n" +

                    "No alto do Olimpo, entre tempestades e trovões, " +
                    "está o domínio de Zeus, senhor dos deuses.\n\n" +

                    "O Raio do Olimpo guarda uma força extraordinária. " +
                    "Seu poder não pode cair em mãos erradas.\n\n" +

                    "Zeus será o último grande desafio desta jornada. " +
                    "Prepare-se para enfrentar a provação final " +
                    "e restaurar o equilíbrio do Olimpo.";

            default:

                Debug.LogError("História não encontrada para a fase: " + fase);

                return
                    "Uma nova jornada começa no mundo dos deuses.\n\n" +
                    "Prepare-se para enfrentar os desafios do Olimpo.";
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (tutorialAtivo)
        {
            ComecarFase();
            return;
        }

        if (digitando)
        {
            MostrarHistoriaCompleta();
            return;
        }

        if (historiaTerminada)
        {
            MostrarTutorial();
        }
    }

    private bool ReferenciasValidas()
    {
        if (painelHistoria == null ||
            painelTutorial == null ||
            textoHistoria == null ||
            textoPular == null ||
            textoTutorial == null)
        {
            Debug.LogError(
                "CutsceneIntroManager: preencha os campos da interface no Inspector."
            );

            return false;
        }

        string faseEscolhida = SelecaoFases.NomeFaseSelecionada;

        if (string.IsNullOrEmpty(faseEscolhida) ||
            !Application.CanStreamedLevelBeLoaded(faseEscolhida))
        {
            Debug.LogError(
                "CutsceneIntroManager: a fase selecionada '" +
                faseEscolhida +
                "' não foi encontrada no Build Profile."
            );

            return false;
        }

        if (audioSource == null)
        {
            Debug.LogWarning(
                "Audio Source não configurado. A cutscene continuará sem som."
            );
        }

        if (somDigitacao == null)
        {
            Debug.LogWarning("O som de digitação não foi atribuído.");
        }

        return true;
    }

    private IEnumerator DigitarHistoria()
    {
        digitando = true;
        historiaTerminada = false;
        contadorLetras = 0;

        textoHistoria.text = "";

        foreach (char letra in historiaCompleta)
        {
            textoHistoria.text += letra;

            if (!char.IsWhiteSpace(letra))
            {
                contadorLetras++;

                if (contadorLetras >= letrasEntreSons)
                {
                    TocarSomDigitacao();
                    contadorLetras = 0;
                }
            }

            yield return new WaitForSecondsRealtime(
                velocidadeDigitacao
            );
        }

        PararSomDigitacao();

        digitando = false;
        historiaTerminada = true;
        digitacaoCoroutine = null;

        textoPular.text = "PRESSIONE ESPAÇO PARA CONTINUAR";
    }

    private void TocarSomDigitacao()
    {
        if (audioSource == null || somDigitacao == null)
            return;

        if (audioSource.isPlaying)
            return;

        audioSource.PlayOneShot(
            somDigitacao,
            volumeDigitacao
        );
    }

    private void PararSomDigitacao()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void MostrarHistoriaCompleta()
    {
        if (digitacaoCoroutine != null)
        {
            StopCoroutine(digitacaoCoroutine);
            digitacaoCoroutine = null;
        }

        PararSomDigitacao();

        textoHistoria.text = historiaCompleta;

        digitando = false;
        historiaTerminada = true;

        textoPular.text = "PRESSIONE ESPAÇO PARA CONTINUAR";
    }

    private void MostrarTutorial()
    {
        PararSomDigitacao();

        painelHistoria.SetActive(false);
        painelTutorial.SetActive(true);

        tutorialAtivo = true;

        Debug.Log("Cutscene: tutorial dos controles exibido.");
    }

    private void ComecarFase()
    {
        if (iniciandoFase)
            return;

        iniciandoFase = true;

        PararSomDigitacao();

        Time.timeScale = 1f;

        string faseEscolhida = SelecaoFases.NomeFaseSelecionada;

        Debug.Log(
            "Cutscene concluída. Indo para: " + faseEscolhida
        );

        if (TransicaoCenas.Instance != null)
        {
            TransicaoCenas.Instance.TrocarCena(faseEscolhida);
        }
        else
        {
            Debug.LogError(
                "CutsceneIntroManager: Controlador TransicaoCenas não encontrado!"
            );

            iniciandoFase = false;
        }
    }

    private void OnDisable()
    {
        PararSomDigitacao();
    }
}
